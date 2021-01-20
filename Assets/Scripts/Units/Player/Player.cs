// #undef DEBUG
#pragma warning disable CS0649

using UnityEngine;
using System;
using System.Diagnostics;
using CoronaGame.UI;

namespace CoronaGame.Units
{
    [Serializable]
    public sealed class Player : Unit, IDamageable
    {
        public static Player Instance { get; set; }

        [Range(0, 0.1f)]
        [SerializeField] private float movementSmoothing = 0.02f;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private LayerMask groundLayerMask;

        [Header("Ground Checks")]
        [SerializeField] private Transform gcLeft;
        [SerializeField] private Transform gcCenter;
        [SerializeField] private Transform gcRight;

        [Header("Gun")]
        [SerializeField] private Shotgun shotgun;

        public PlayerDeathEvent PlayerDeath { get; private set; }
        public bool HasGun { get; private set; }
        public bool IsDead { get; private set; }
        public bool FacingRight { get; private set; } = true;
        public int FaceMaskCount { get; set; }
        public int AmmoCount { get; set; }
        public Vector3 SpawnPoint { get; set; }
        public bool JumpOnCooldown => Time.time < this.nextJumpTime;
        public bool InputOnCooldown => Time.time < this.nextInputTime;
        
        private float nextInputTime;
        private float nextJumpTime;
        private Vector3 velocity = Vector3.zero;
        private PlayerInput inputs;
        private bool inAir;
        
        protected override void Awake()
        {
            if (Player.Instance == null)
            {
                Player.Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            base.Awake();
            this.PlayerDeath = new PlayerDeathEvent();
            
            PlayerAnimations.Init(this.animator);
        }

        /// <summary>
        /// Updates player's data to UI.
        /// </summary>
        private void Start()
        {
            GameManager.Instance.UpdatePlayerDataToUI();
        }

        /// <summary>
        /// Checks player inputs if game is running.
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.OnPause || !GameManager.Instance.ActiveGame) return;
            if (this.InputOnCooldown) return;
            this.inputs = GetPlayerInputs();
            Jump();
            Walk();
            Shoot();
        }

        /// <summary>
        /// Player physics movement.
        /// </summary>
        private void FixedUpdate()
        {
            if (this.InputOnCooldown || !GameManager.Instance.ActiveGame)
            {
                SlowDown();
                return;
            }
            WalkPhysics(this.inputs.Horizontal);
        }
        
        #region Movement
        /// <summary>
        /// 
        /// </summary>
        private void Walk()
        {
            if (this.inputs.ActiveMovement)
            {
                PlayerAnimations.PlayerWalking(this.HasGun, this.FaceMaskCount);
            }
            else
            {
                PlayerAnimations.PlayerStopped(this.HasGun, this.FaceMaskCount);
            }
        }

        /// <summary>
        /// Player perform a jump.
        /// </summary>
        private void Jump()
        {
            if (!CanJump()) return;
            if (this.inAir)
            {
                PlayerAnimations.PlayerLanded(this.HasGun, this.FaceMaskCount);
            }
            this.inAir = false;

            if (!this.inputs.Jump) return;
            this.rgbody.AddForce(new Vector2(0f, this.jumpSpeed));
            PlayerAnimations.PlayerJumped(this.HasGun, this.FaceMaskCount);
            this.inAir = true;
            nextJumpTime = Time.time + 0.2f;
        }

        /// <summary>
        /// Player walk physics. This methods moves player
        /// on x-axis according to player horizontal inputs.
        /// Flips players sprite if he turns around.
        /// Use in FixedUpdate.
        /// </summary>
        /// <param name="direction"></param>
        private void WalkPhysics(float direction)
        {
            Vector2 velocity1 = this.rgbody.velocity;
            Vector3 targetVelocity = new Vector2(direction * this.movementSpeed, velocity1.y);
            this.rgbody.velocity = Vector3.SmoothDamp(velocity1, targetVelocity, ref this.velocity, this.movementSmoothing);

            // Sprite flip
            if (direction > 0 && !this.FacingRight || direction < 0 && this.FacingRight)
            {
                FlipSprite();
            }
        }

        /// <summary>
        /// Player has no friction (because of wall sliding). Use
        /// this method to slow down player when movement inputs are
        /// inactive.
        /// </summary>
        private void SlowDown()
        {
            Vector2 velocity1 = this.rgbody.velocity;
            Vector3 targetVelocity = new Vector2(0, velocity1.y);
            this.rgbody.velocity = Vector3.SmoothDamp(velocity1,
                targetVelocity, ref this.velocity, this.movementSmoothing
            );
        }

        /// <summary>
        /// Player gets a boosted jump
        /// </summary>
        public void BounceUp()
        {
            if (this.IsDead) return;
            this.rgbody.velocity = Vector3.zero;
            this.rgbody.AddForce(new Vector2(0, 25), ForceMode2D.Impulse);
            this.inAir = true;
        }

        #endregion

        #region Game Events

        /// <summary>
        /// Player shoots with his shotgun.
        /// (If he has ammo).
        /// </summary>
        private void Shoot()
        {
            if (!this.inputs.Fire || !this.HasGun) return;
            if (this.AmmoCount > 0)
            {
                if (!shotgun.Shoot(this.FacingRight ? Vector2.right : Vector2.left)) return;
                this.AmmoCount--;
                GameManager.Instance.UpdatePlayerDataToUI();
            }
            else
            {
                ViewManager.Instance.FlashMessage("Out of Ammo!");
                AudioManager.Instance.Play("GunMisfire");
            }
        }

        /// <summary>
        /// Player takes specified amount of damage.
        /// If player haas face masks, face mask will take the
        /// hit and be removed from player inventory.
        /// If player health falls below 1, player dies.
        /// 
        /// Player hit audio clip is played in all cases.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            if (!GameManager.Instance.ActiveGame) return;
            if (this.FaceMaskCount > 0)
            {
                this.FaceMaskCount -= damage;
                GameManager.Instance.UpdatePlayerDataToUI();
            }
            else if ((this.health -= damage) <= 0)
            {
                this.Die();
            }
            AudioManager.Instance.Play("PlayerHit");
        }

        /// <summary>
        /// Player takes specified amount of damage.
        /// Damage direction is taken to equation.
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="direction"></param>
        public void TakeDamage(int damageAmount, Vector2 direction)
        {
            this.TakeDamage(damageAmount);
            this.rgbody.velocity = Vector2.zero;
            this.rgbody.AddForce(direction, ForceMode2D.Impulse);
            this.nextInputTime = Time.time + 0.4f;
        }

        /// <summary>
        /// Pick up shotgun. Player can shoot after
        /// this.
        /// </summary>
        public void PickupShotgun()
        {
            this.HasGun = true;
        }

        /// <summary>
        /// Resets player data
        /// </summary>
        public void Spawn()
        {
            this.transform.position = this.SpawnPoint;
            PlayerAnimations.PlayerWalking(this.HasGun, this.FaceMaskCount);
            this.IsDead = false;
        }

        /// <summary>
        /// Player's unit dies. Invokes
        /// PlayerDiedEvent.
        /// </summary>
        public override void Die()
        {
            this.PlayerDeath?.Invoke();
            PlayerAnimations.PlayerDied();
            this.IsDead = true;
        }

        /// <summary>
        /// Adds specified amount of face masks to player's
        /// inventory. Updates player's data panel in UI.
        /// </summary>
        /// <param name="count">face mask count</param>
        public void AddFacemask(int count)
        {
            this.FaceMaskCount += count;
            GameManager.Instance.UpdatePlayerDataToUI();
        }

        /// <summary>
        /// Adds specified amount of ammunition to player's
        /// inventory. Updates player's data panel in UI.
        /// </summary>
        /// <param name="count">Added ammo count</param>
        public void AddAmmo(int count)
        {
            this.AmmoCount += count;
            GameManager.Instance.UpdatePlayerDataToUI();
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Gets player's inputs and returns them in
        /// a <see cref="PlayerInput"/> struct
        /// </summary>
        /// <returns>Player's inputs wrapped to struct</returns>
        private PlayerInput GetPlayerInputs()
        {
            return new PlayerInput(Input.GetButtonDown("Jump"),
                Input.GetAxisRaw("Horizontal"),
                Input.GetButtonDown("Fire1")
            );
        }

        /// <summary>
        /// Creates a 2D raycast directly towards ground from
        /// start position. Returns the hit object.
        /// </summary>
        /// <param name="startPos">Raycast start position</param>
        /// <returns>Raycast2DHit struct</returns>
        private RaycastHit2D Raycast2DGround(Vector3 startPos)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, 0.3f, this.groundLayerMask);
            DebugGroundCheck(hit, startPos);
            return hit;
        }

        /// <summary>
        /// Performs ground check raycasts for each groundcheck
        /// transform. This way we get to know if player is touching
        /// ground and can perform a jump.
        /// </summary>
        /// <returns>true if player can jump</returns>
        private bool CanJump()
        {
            if (this.JumpOnCooldown) return false;
            if (Raycast2DGround(this.gcCenter.position)||
                Raycast2DGround(this.gcLeft.position)  ||
                Raycast2DGround(this.gcRight.position))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Flips player's unit's Sprite. Also moves
        /// gunpoint to new appropriate position.
        /// </summary>
        private void FlipSprite()
        {
            this.FacingRight = !this.FacingRight;
            this.spriteRenderer.flipX = !this.spriteRenderer.flipX;
            this.shotgun.TurnGunPoint();
        }
        #endregion
        
        [Conditional("DEBUG")]
        private void DebugGroundCheck(RaycastHit2D hit, Vector2 startPos)
        {
            Vector3 position = this.gcCenter.position;
            UnityEngine.Debug.DrawLine(startPos,
                startPos + new Vector2(0f, -0.3f),
                hit ? Color.green : Color.red
            );
        }
    }
}

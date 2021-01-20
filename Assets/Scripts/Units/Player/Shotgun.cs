#pragma warning disable CS0649

using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;
using CoronaGame.Units.Enemies;

namespace CoronaGame.Units
{
    
    public class Shotgun : MonoBehaviour
    {
        #region Unity Editor Fields
        [Header("Gun Properties")]
        [Min(0)]
        [SerializeField] private int damage;
        [Min(12)]
        [SerializeField] private int maxAmmo;
        [Min(0)]
        [SerializeField] private float cooldownTimer;
        [Min(5)]
        [SerializeField] private float range;
        [SerializeField] private LayerMask targetMask;

        [FormerlySerializedAs("tracerOffset_X")]
        [Range(2f, 12f)]
        [SerializeField] private float tracerOffsetX;

        [Header("Shell Object Pool")]
        [SerializeField] private ShellPool shellPool;
        [SerializeField] private GameObject gunPoint;

        [Header("Shot Effect Prefabs")]
        [SerializeField] private GameObject tracerSprite;
        [SerializeField] private GameObject tracerSpriteMask;
        [SerializeField] private GameObject hitAnimation;
        [SerializeField] private GameObject hitAniamtionBlood;
        #endregion

        private bool isOnCooldown;
        private CinemachineImpulseSource gunImpulseSource;
        private Light2D muzzleBlast;
        private float maskWidth;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            this.gunImpulseSource = this.gunPoint.GetComponent<CinemachineImpulseSource>();
            this.muzzleBlast = this.gunPoint.GetComponent<Light2D>();
            this.maskWidth = this.tracerSpriteMask.GetComponent<SpriteMask>().bounds.size.x;
        }

        /// <summary>
        /// Shoots at the direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>true if hit</returns>
        public bool Shoot(Vector2 direction)
        {
            if (!this.isOnCooldown)
            {
                this.ShootTheGun(direction);
                RaycastHit2D hit = Physics2D.Raycast(this.gunPoint.transform.position, direction, this.range, targetMask);
                if (hit)
                {
                    HandleHit(hit);
                }
                this.StartShootCooldown();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the gun visuals that are not dependent
        /// on hit.
        /// </summary>
        /// <param name="direction">Shoot direction</param>
        private void ShootTheGun(Vector2 direction)
        {
            // Throw the shell into air.
            this.shellPool.GetShell(-direction);

            // Shake the camera.
            this.gunImpulseSource.GenerateImpulse();

            // Gun tracer and muzzle light
            this.DrawMuzzleBlast();
            this.DrawTracer(direction);

            AudioManager.Instance.Play("Shoot");
        }

        /// <summary>
        /// Handle the bullet hitting something.
        /// </summary>
        /// <param name="hit"></param>
        private void HandleHit(RaycastHit2D hit)
        {
            bool hitOnFlesh = false;
            if (Unit.Is<Enemy>(hit.collider.gameObject, out var enemy))
            {
                hitOnFlesh = true;
                enemy.TakeDamage(this.damage, hit.normal);
                AudioManager.Instance.Play("EnemyHit");
            }
            SetTracerMask(hit.point, -hit.normal);
            DrawHitAnimation(hit, hitOnFlesh);
        }

        /// <summary>
        /// Draws muzzle blast at the gunpoint.
        /// Fades after short amount of time.
        /// </summary>
        private void DrawMuzzleBlast()
        {
            // Simple timer coroutine
            IEnumerator MuzzelBlastTimer()
            {
                this.muzzleBlast.enabled = true;
                yield return new WaitForSeconds(0.08f);
                this.muzzleBlast.enabled = false;
            }
            StartCoroutine(MuzzelBlastTimer());
        }

        /// <summary>
        /// Instantiates the hit animation prefab on the hit
        /// position. Animation game object gets destroyed after
        /// a short amount of time.
        /// </summary>
        /// <param name="hit">Shooting raycast hit</param>
        /// <param name="onFlesh">if hitted fleshy target</param>
        private void DrawHitAnimation(RaycastHit2D hit, bool onFlesh)
        {
            GameObject hitAnimationPrefab = onFlesh ? this.hitAniamtionBlood : this.hitAnimation;
            GameObject bulletHitSprite = Instantiate(hitAnimationPrefab, hit.point, Quaternion.identity);

            if (hit.normal.x > 0)
            {
                bulletHitSprite.GetComponent<SpriteRenderer>().flipX = true;
            }

            Destroy(bulletHitSprite, 0.2f);
        }

        /// <summary>
        /// Sets sprite mask for tracer sprite. Masks tracer
        /// after hit position. Mask gets destroyed after
        /// short amount time.
        /// </summary>
        /// <param name="hitPosition">world point where hit occured</param>
        /// <param name="direction">normalized direction</param>
        private void SetTracerMask(Vector2 hitPosition, Vector2 direction)
        {
            float tempMaskWidth = this.maskWidth * direction.x;
            Vector3 tracerPos = (Vector3)hitPosition + new Vector3(tempMaskWidth / 2f, 0f, 0f);
            GameObject tracerMask = Instantiate(this.tracerSpriteMask, tracerPos, Quaternion.identity);

            Destroy(tracerMask, 0.08f);
        }

        /// <summary>
        /// Calculates the correct position for tracer
        /// sprite from gunpoint position.
        /// </summary>
        /// <param name="direction">normalized direction</param>
        /// <returns>tracer position</returns>
        private Vector3 CalcTracerPosition(Vector2 direction)
        {
            var offsetVector = new Vector3(direction.x * this.tracerOffsetX, 0, 0);
            return this.gunPoint.transform.position + offsetVector;
        }

        /// <summary>
        /// Changes gunpoint position and rotation to
        /// match flipped player sprite.
        /// </summary>
        public void TurnGunPoint()
        {
            // Move gunpoint to new position.
            Vector3 gunPointPos = this.gunPoint.transform.localPosition;
            var newPos = new Vector3(gunPointPos.x * -1, gunPointPos.y, gunPointPos.z);
            this.gunPoint.transform.localPosition = newPos;

            // Rotate gunpoint transform to point to new facing direction.
            Vector3 angles = this.gunPoint.transform.eulerAngles;
            var newRotation = new Vector3(angles.x, angles.y, angles.z*-1);
            this.gunPoint.transform.eulerAngles = newRotation;
        }

        /// <summary>
        /// Instantiates tracer sprite and aligns it
        /// based on player faceing direction. Sprite
        /// gets destroyed after small amount of time.
        /// </summary>
        /// <param name="direction">Shooting direction</param>
        private void DrawTracer(Vector2 direction)
        {
            Vector3 instantiatePos = CalcTracerPosition(direction);
            GameObject tracer = Instantiate(this.tracerSprite, instantiatePos, Quaternion.identity);

            // If facing left, flip the sprite
            if (direction.x < 0)
            {
                tracer.GetComponent<SpriteRenderer>().flipX = true;
            }
            Destroy(tracer, 0.08f);
        }

        /// <summary>
        /// Suffer shooting cooldown duration
        /// based on cooldown timer field.
        /// </summary>
        private void StartShootCooldown()
        {
            IEnumerator ShootCooldown()
            {
                this.isOnCooldown = true;
                yield return new WaitForSeconds(this.cooldownTimer);
                this.isOnCooldown = false;
            }
            StartCoroutine(ShootCooldown());
        }
    }
}


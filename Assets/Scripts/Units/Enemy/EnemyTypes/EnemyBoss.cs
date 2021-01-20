#pragma warning disable CS0649

using UnityEngine;
using CoronaGame.UI;

namespace CoronaGame.Units.Enemies
{
    public class EnemyBoss : Enemy
    {
        [SerializeField] private Projectile projectile;
        [SerializeField] private float projectileTravelDuration;
        [SerializeField] private float shootRange;
        [SerializeField] private float shootCooldown;
        [SerializeField] private GameObject vaccinePrefab;

        public bool IsDead { get; private set; }
        private BossHealthBarView healthBarView;
        private bool active;

        // Cooldowns
        private float nextMoveTime;
        private float nextActionTime;
        private bool CanMove => Time.time >= this.nextMoveTime;
        private bool CanAct => Time.time >= this.nextActionTime;

        private int startHealth; // cache the inital health
        private static Vector2 DOWN_FORCE = new Vector2(0f, 0.2f);

        // Enrage
        private int enrageHealthLimit;
        private float enrageAttackTime;

        public override void TakeDamage(int amount, Vector2 direction)
        {
            base.TakeDamage(amount);
            StopAllCoroutines();
            StartCoroutine(this.Pushback(-direction, recover: direction.y > 0));

            if (this.health > enrageHealthLimit)
            {
                this.nextMoveTime = Time.time + 1f;
                if (direction.x != 0)
                    this.nextActionTime = Time.time + 0.8f;
            }
            
            animator.SetTrigger("Damaged");
            ViewManager.Instance.Get<BossHealthBarView>()
                .UpdateHealthBar((float)this.health / (float)startHealth);
        }

        protected override void HandleBeingJumpedOver(Player player)
        {
            if (player.IsDead) return;
            this.TakeDamage(1, EnemyBoss.DOWN_FORCE);
            player.BounceUp();
        }

        protected override void HandlePlayerCollision(Vector2 direction, Player player)
        {
            player.TakeDamage(1, new Vector2(direction.x * -50, 10));
            this.nextMoveTime = Time.time + 1f;
        }

        protected override void Awake()
        {
            base.Awake();
            this.startHealth = this.health; // cache the initial health
            this.healthBarView = ViewManager.Instance.Get<BossHealthBarView>();
            this.healthBarView.Open();
            this.UpdateHealthBar();

            AudioManager.Instance.Stop("Theme");
            AudioManager.Instance.Play("BossMusic");

            // Calculate enrage limits from serialized values
            this.enrageHealthLimit = (int)((float)this.health * 0.2);
            this.enrageAttackTime = this.shootCooldown * 0.5f;
        }
        private void UpdateHealthBar()
        {
            this.healthBarView.UpdateHealthBar((float)this.health / (float)startHealth);
        }

        void Update()
        {
            this.FollowPlayer();
            this.ShootAttack();
            if (this.health <= enrageHealthLimit)
            {
                this.shootCooldown = enrageAttackTime;
            } 
        }

        public override void Die()
        {
            base.Die();
            this.IsDead = true;
            Instantiate(this.vaccinePrefab, this.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.Euler(0f, 0f, -38.504f));
            AudioManager.Instance.Stop("BossMusic");
            AudioManager.Instance.Play("Victory");
            ViewManager.Instance.Close<BossHealthBarView>();
        }

        private void ShootAttack()
        {
            if (!GameManager.Instance.ActiveAI || !this.CanAct || !this.active) return;
            var target = Player.Instance.transform.position;
            this.StartRangedAttack(target);
        }

        private void FollowPlayer()
        {
            if (!GameManager.Instance.ActiveAI || !this.CanMove) return;
            if (!this.active) this.active = true;
            if (this.ScanForTargetInRange(10, out RaycastHit2D hit))
            {
                Vector2 target = hit.transform.position;
                this.MoveTowards(hit.point, NORMAL_DAMP, 5f);
            }
        }

        private void StartRangedAttack(Vector3 target)
        {
            this.nextActionTime = Time.time + this.shootCooldown;
            Vector3 shootOrigin = transform.position;
            var projectile = Instantiate(this.projectile, shootOrigin, Quaternion.identity);
            projectile.Damage = 1;
            projectile.Shooter = this;

            float projectileSpeed;
            var rng = Random.Range(0f, 1f);
            if (rng > 0.7)
                projectileSpeed = 0.5f;
            else
                projectileSpeed = this.projectileTravelDuration;

            Vector2 trajectoryV;
            float x_distance = Mathf.Abs(this.transform.position.x - target.x);
            if (x_distance < 2f)
            {
                // Target is so close, no target side bias
                trajectoryV = CalcTrajectoryV(shootOrigin, target, projectileSpeed);
            }
            else
            {
                // Use bias
                bool rightSide = this.transform.position.x > target.x ? true : false;
                trajectoryV = CalculateTrajectoryVelocity(shootOrigin, target, projectileSpeed, rightSide);
            }
            projectile.GetComponent<Rigidbody2D>().velocity = trajectoryV;
            AudioManager.Instance.Play("BossShoot");
        }

        private static Vector3 CalcTrajectoryV(Vector2 origin, Vector2 target, float duration)
        {
            float vx = (target.x - origin.x) / duration;
            float vy = (target.y - origin.y - 0.5f * Physics.gravity.y * duration * duration) / duration;
            return new Vector2(vx, vy);
        }

        private static Vector3 CalculateTrajectoryVelocity(Vector2 origin, Vector2 target, float duration, bool rightSide)
        {
            float bias = rightSide ? Random.Range(-1f, 3f) : Random.Range(1, -3f);
            float vx = (target.x + bias - origin.x) / duration;
            float vy = (target.y - origin.y - 0.5f * Physics.gravity.y * duration * duration) / duration;
            return new Vector2(vx, vy);
        }
    }
}


#pragma warning disable CS0649

using UnityEngine;

namespace CoronaGame.Units.Enemies
{
    public class EnemyPatroller : Enemy
    {
        private static Vector2 DOWN_FORCE = new Vector2(0f, 0.2f);

        [Header("Patrolling")]
        [SerializeField] private float waitAfterPatrol;
        [SerializeField] private PatrolRoute patrolRoute;

        private float nextPatrolTime;
        private bool CanPatrol => Time.time >= this.nextPatrolTime && GameManager.Instance.ActiveAI;
        private Vector3 targetPatrolPoint;

        private void OnEnable() => this.targetPatrolPoint = this.patrolRoute.GetNextPoint();
        private void Update() => this.Patrol();
        protected override void Awake()
        {
            base.Awake();
            this.patrolRoute.UseVectors();
        }

        /// <summary>
        /// Patroller must wait for next possible time
        /// to continue.
        /// </summary>
        /// <param name="duration">cooldown duration</param>
        private void SetPatrolCoodlown(float duration)
        {
            this.nextPatrolTime = Time.time + duration;
        }

        /// <summary>
        /// Patroller takes damage. If health falls below
        /// zero, the unit dies and get's destoyed.
        /// </summary>
        /// <param name="damageAmount">The amount of damage taken.</param>
        /// <param name="direction">The direction from where the damage came.</param>
        public override void TakeDamage(int damageAmount, Vector2 direction)
        {
            base.TakeDamage(damageAmount);
            StartCoroutine(this.Pushback(-direction, direction.y > 0));
            this.SetPatrolCoodlown(0.8f);
        }

        /// <summary>
        /// When patroller gets jumped, It bounces the player
        /// in air and gets damaged.
        /// </summary>
        /// <param name="player"></param>
        protected override void HandleBeingJumpedOver(Player player)
        {
            if (player.IsDead) return;
            this.TakeDamage(1, EnemyPatroller.DOWN_FORCE);
            player.BounceUp();
        }

        /// <summary>
        /// Player takes damage and gets pushed back.
        /// Patroller gets patrolling cooldown.
        /// </summary>
        /// <param name="direction">From which direction player collided.</param>
        /// <param name="player">Player object</param>
        protected override void HandlePlayerCollision(Vector2 direction, Player player)
        {
            player.TakeDamage(1, new Vector2(-direction.x * 50f, 10f));
            StartCoroutine(this.Pushback(direction / 2, true));
            this.SetPatrolCoodlown(1.5f);
        }

        /// <summary>
        /// Patroller enemy travels it's route.
        /// If waitAfterPatrol is set, patroller waits
        /// the specified amount of time after reaching each node.
        /// </summary>
        private void Patrol()
        {
            if (this.CanPatrol)
            {
                this.MoveTowards(this.targetPatrolPoint, Enemy.SLOW_DAMP, this.movementSpeed);
                if (Vector3.Distance(this.transform.position, this.targetPatrolPoint) < 0.01f)
                {
                    if (waitAfterPatrol > 0)
                    {
                        this.SetPatrolCoodlown(waitAfterPatrol);
                    }
                    this.targetPatrolPoint = this.patrolRoute.GetNextPoint();
                }
            }
        }
    }
}


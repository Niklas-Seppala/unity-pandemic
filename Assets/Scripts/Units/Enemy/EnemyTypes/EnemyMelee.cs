#pragma warning disable CS0649

using System.Collections;
using UnityEngine;

namespace CoronaGame.Units.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyMelee : Enemy
    {
        [Header("Melee")]
        [SerializeField] private float meleeRange;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float attackSpeed;

        private static Vector2 DOWN_FORCE = new Vector2(0f, 0.6f);
        private float nextAttackTime;
        private bool CanAttack => Time.time >= this.nextAttackTime;
        private MeleeAttack attackState;

        private void Update() => this.Attack();
        protected override void Awake()
        {
            base.Awake();
            this.attackState = new MeleeAttack();
        }

        /// <summary>
        /// Loses hp, suffers pushback
        /// </summary>
        /// <param name="amount">damage amount</param>
        /// <param name="direction">damage direction</param>
        public override void TakeDamage(int amount, Vector2 direction)
        {
            base.TakeDamage(amount);
            if (!this.attackState.Done)
            {
                this.attackState.Finish();
            }
            this.SetAttackCooldown(this.attackCooldown);
            StopAllCoroutines();
            StartCoroutine(this.Pushback(-direction, direction.y > 0));
        }

        /// <summary>
        /// Takes damage, bounces player in air.
        /// Suffers downward pushback.
        /// </summary>
        /// <param name="player"></param>
        protected override void HandleBeingJumpedOver(Player player)
        {
            if (player.IsDead) return;
            this.TakeDamage(1, EnemyMelee.DOWN_FORCE);
            player.BounceUp();
        }

        /// <summary>
        /// Sets cooldown for melee attack.
        /// </summary>
        /// <param name="duration">cooldown duration</param>
        private void SetAttackCooldown(float duration)
        {
            this.nextAttackTime = Time.time + duration;
        }

        /// <summary>
        /// Enemy melee unit perform melee attack, if possible.
        /// </summary>
        private void Attack()
        {
            if (CanAttack && GameManager.Instance.ActiveAI)
            {
                if (this.ScanForTargetInRange(this.meleeRange, out var hit))
                {
                    this.SetAttackCooldown(this.attackCooldown);
                    this.attackState.NewState(this.transform.position);
                    StartCoroutine(this.MeleeForward(hit.point));
                }
                this.VisualizeRange(this.meleeRange, Color.red);
            }
        }

        /// <summary>
        /// Sets this enemy on attack cooldown, player takes
        /// damage and get's pushed back. If this enemy was commiting
        /// melee attack, it proceeds to retreat phase.
        /// </summary>
        /// <param name="direction">The direction player collided.</param>
        /// <param name="player">Player object</param>
        protected override void HandlePlayerCollision(Vector2 direction, Player player)
        {
            this.SetAttackCooldown(this.attackCooldown);
            player.TakeDamage(1, new Vector2(-direction.x * 100, 12));
            StopAllCoroutines();

            if (this.attackState.Done) return;
            if (this.attackState.Forward)
            {
                StartCoroutine(MeleeRetreat());
            }
        }

        /// <summary>
        /// Moves enemy transform towards the target world
        /// position.
        /// </summary>
        /// <param name="target">Target World position</param>
        private IEnumerator MeleeForward(Vector3 target)
        {
            while (Vector3.Distance(target, this.transform.position) > 0.01f)
            {
                this.MoveTowards(target, NORMAL_DAMP, this.attackSpeed);
                yield return null;
            }
            this.transform.position = target;
            this.attackState.SwitchToRetreat();
            yield return MeleeRetreat();
        }

        /// <summary>
        /// Moves enemy object back to original position
        /// after attack.
        /// </summary>
        private IEnumerator MeleeRetreat()
        {
            while (Vector3.Distance(this.attackState.StartPosition, this.transform.position) > 0.01f)
            {
                this.MoveTowards(this.attackState.StartPosition, NORMAL_DAMP, this.attackSpeed);
                yield return null;
            }
            this.transform.position = this.attackState.StartPosition;
            this.attackState.Finish();
        }
    }
}

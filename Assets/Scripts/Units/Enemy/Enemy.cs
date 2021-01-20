#pragma warning disable CS0649

using UnityEngine;
using System.Diagnostics;
using System.Collections;

namespace CoronaGame.Units.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Enemy : Unit, IDamageable
    {
        [Header("AI")]
        [SerializeField] protected LayerMask aggroMask;

        [Header("Death")]
        [SerializeField] private GameObject deathEffect;

        private static readonly int damagedAnimation = Animator.StringToHash("Damaged");
        protected Vector2 moveVelocity = Vector3.zero;
        protected const float NORMAL_DAMP = 0.05f;
        protected const float SLOW_DAMP = 0.2f;

        /// <summary>
        /// Enemy moves smoothly towards target position
        /// </summary>
        /// <param name="target">target position</param>
        /// <param name="damp">damp amount</param>
        /// <param name="maxSpeed">max movement speed</param>
        protected void MoveTowards(Vector2 target, float damp, float maxSpeed)
        {
            this.transform.position = Vector2.SmoothDamp(this.transform.position,
                target, ref this.moveVelocity, damp, maxSpeed
            );
        }

        /// <summary>
        /// Enemy suffers pushback, aka. moves
        /// back specified distance.
        /// </summary>
        /// <param name="distance">pushback distance</param>
        /// <param name="recover">recover from pushback</param>
        protected IEnumerator Pushback(Vector3 distance, bool recover)
        {
            Vector3 position = this.transform.position;
            Vector3 start = position;
            Vector3 target = position + distance;
            
            // Get pushed back by some force.
            while(Vector3.Distance(this.transform.position, target) > 0.01f)
            {
                this.MoveTowards(target, NORMAL_DAMP, this.movementSpeed);
                yield return null;
            }
            // Snap to final position.
            this.transform.position = target;
            yield return null;

            if (!recover) yield break;

            // Recover from pushback to original position.
            while(Vector3.Distance(this.transform.position, start) > 0.01f)
            {
                this.MoveTowards(start, NORMAL_DAMP, this.movementSpeed);
                yield return null;
            }

            // Snap to position
            this.transform.position = start;
        }

        /// <summary>
        /// Enemy object takes damage according to
        /// damage source. Invokes <see cref="Unit.Die"/> - method
        /// if health level fell to zero.
        /// </summary>
        /// <param name="damageAmount">how much damage is taken</param>
        public virtual void TakeDamage(int damageAmount)
        {
            AudioManager.Instance.Play("EnemyHit");
            this.animator.SetTrigger(damagedAnimation);
            if ((this.health -= damageAmount) <= 0)
            {
                this.Die();
            }
        }

        /// <summary>
        /// When dying, this enemy leaves behind death
        /// particle effect.
        /// Destroys this game object.
        /// </summary>
        public override void Die()
        {
            Instantiate(deathEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

        public abstract void TakeDamage(int damageAmount, Vector2 direction);
        protected abstract void HandleBeingJumpedOver(Player player);
        protected abstract void HandlePlayerCollision(Vector2 direction, Player player);

        /// <summary>
        /// Scans for any targets specified in aggro layer mask
        /// in given range (x-axis).
        /// </summary>
        /// <param name="range">Scan range</param>
        /// <param name="hit">possible raycast2D hit</param>
        /// <returns>true if target was detected</returns>
        protected bool ScanForTargetInRange(float range, out RaycastHit2D hit)
        {
            Vector3 pos = this.transform.position;
            hit = Physics2D.Raycast(pos, Vector2.right, range, this.aggroMask);
            if (hit)
            {
                return true;
            }
            hit = Physics2D.Raycast(pos, Vector2.left, range, this.aggroMask);
            return hit;
        }

        /// <summary>
        /// Detects if the player collided with the enemy object.
        /// If so, checks if player made successful jump on top
        /// of the enemy, or if the collision was caused by touching
        /// the enemy.
        /// 
        /// Invokes appropriate abstract methods according to
        /// collision detection.
        /// </summary>
        /// <param name="other">Other object that collided</param>
        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (Unit.Is<Player>(other.gameObject, out var player))
            {
                Vector2 collisionNormal = other.contacts[0].normal;
                if (collisionNormal.y < 0)
                {
                    HandleBeingJumpedOver(player);
                }
                else
                {
                    HandlePlayerCollision(collisionNormal, player);
                }
            }
        }

        /// <summary>
        /// Visualizes enemy range by drawing rays
        /// on left and right side of the enemy.
        /// </summary>
        /// <param name="range">ray length</param>
        /// <param name="color">ray color</param>
        /// <param name="duration">ray duration</param>
        [Conditional("DEBUG")]
        protected void VisualizeRange(float range, Color color, float duration=0.2f)
        {
            Vector3 pos = this.transform.position;
            float xRight = pos.x + range;
            float xLeft = pos.x - range;
            UnityEngine.Debug.DrawLine(pos, new Vector3(xRight,pos.y, pos.z), color, duration);
            UnityEngine.Debug.DrawLine(pos, new Vector3(xLeft,pos.y, pos.z), color, duration);
        }
    }
}

#pragma warning disable CS0649

using UnityEngine;

namespace CoronaGame.Units.Enemies
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private GameObject hitEffect;
        public EnemyBoss Shooter { get; set; }

        public int Damage { get; set; }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (Unit.Is<Player>(other.gameObject, out var player))
            {
                if (!player.IsDead && !this.Shooter.IsDead)
                {
                    player.TakeDamage(this.Damage);
                }
            }
            AudioManager.Instance.Play("EnemyHit");
            Instantiate(hitEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}


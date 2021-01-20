#pragma warning disable CS0649

using UnityEngine;

namespace CoronaGame.Units.Enemies
{
    public class EnemyStatic : Enemy
    {
        private static Vector2 DOWN_FORCE = new Vector2(0f, 0.2f);

        /// <summary>
        /// Loses hp, suffers pushback.
        /// </summary>
        /// <param name="amount">damage amount</param>
        /// <param name="direction">damage direction</param>
        public override void TakeDamage(int amount, Vector2 direction)
        {
           base.TakeDamage(amount);
            StartCoroutine(this.Pushback(-direction, direction.y > 0));
        }

        /// <summary>
        /// Takes damage and boosts player in air.
        /// Suffers downward pushback.
        /// </summary>
        /// <param name="player">player reference</param>
        protected override void HandleBeingJumpedOver(Player player)
        {
            if (player.IsDead) return;
            this.TakeDamage(1, EnemyStatic.DOWN_FORCE);
            player.BounceUp();
        }

        /// <summary>
        /// Player takes damage and gets pushed back.
        /// </summary>
        /// <param name="direction">collision direction</param>
        /// <param name="player">player reference</param>
        protected override void HandlePlayerCollision(Vector2 direction, Player player)
        {
            player.TakeDamage(1, new Vector2(-direction.x * 50f, 10f));
        }
    }
}


using UnityEngine;

namespace CoronaGame.Units
{
    public interface IDamageable
    {
        /// <summary>
        /// Object takes damage.
        /// </summary>
        /// <param name="damageAmount">The amount of damage taken</param>
        void TakeDamage(int damageAmount);

        /// <summary>
        /// Object takes damage, direction is
        /// taken to equation.
        /// </summary>
        /// <param name="damageAmount">The amount of damage taken</param>
        /// <param name="direction">damage direction</param>
        void TakeDamage(int damageAmount, Vector2 direction);
    }
}


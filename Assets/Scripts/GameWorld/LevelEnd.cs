
using UnityEngine;
using CoronaGame.Units;

namespace CoronaGame
{
    public class LevelEnd : MonoBehaviour
    {
        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Unit.Is<Player>(other.gameObject, out var player))
            {
                GameManager.Instance.NextLevel();
            }
        }
    }
}


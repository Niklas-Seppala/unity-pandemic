#pragma warning disable CS0649

using UnityEngine;
using Cinemachine;
using CoronaGame.Units;

namespace CoronaGame
{
    /// <summary>
    /// Fall detector that follows the player and checks
    /// trigger collisions.
    /// </summary>
    public class FallDetectionCollider : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cam;
        private Player player;

        void Start()
        {
            this.player = GameManager.Instance.Player
                ?? GameObject.FindWithTag("Player").GetComponent<Player>();
        }

        /// <summary>
        /// Fall collider follows the player on x-axis.
        /// </summary>
        void Update()
        {
            if (this.player)
            {
                float playerX = player.transform.position.x;
                float myY = this.transform.position.y;
                this.transform.position = new Vector2(playerX, myY);
            }
        }

        /// <summary>
        /// Checks if player collided with Fall check.
        /// If so, stop the camera follow, and kill the player.
        /// </summary>
        /// <param name="other">collision</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Unit.Is<Player>(other.gameObject, out Player player))
            {
                if (player.IsDead) return;        // No actions needed if already dead
                this.cam.Follow = this.transform; // stop the camera
                player.Die();
            }
        }
    }

}

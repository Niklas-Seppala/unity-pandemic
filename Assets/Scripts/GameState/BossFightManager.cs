using UnityEngine;
using CoronaGame.Units;

namespace CoronaGame
{
    /// <summary>
    /// Class for handling things when player is fighting a
    /// boss.
    /// </summary>
    public class BossFightManager : MonoBehaviour
    {
        private int playerStartFacemaskCount;
        private int playerStartAmmoCount;
        private Player player;

        /// <summary>
        /// Reset player resources back to original state.
        /// </summary>
        public void RestartPlayerState()
        {
            this.player.AmmoCount = this.playerStartAmmoCount;
            this.player.FaceMaskCount = this.playerStartFacemaskCount;
            GameManager.Instance.UpdatePlayerDataToUI();
        }

        /// <summary>
        /// Cache player resource state.
        /// </summary>
        private void Start()
        {
            this.player = FindObjectOfType<Player>();
            this.playerStartAmmoCount = this.player.AmmoCount;
            this.playerStartFacemaskCount = this.player.FaceMaskCount;
            player.PlayerDeath.AddListener(() => {
                AudioManager.Instance.Stop("BossMusic");
            });
        }
    }
}


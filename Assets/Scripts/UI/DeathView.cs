#pragma warning disable CS0649

using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoronaGame.UI
{
    [Serializable]
    public class DeathView : UIView
    {
        [Header("Master")]
        [SerializeField] private ViewManager viewManager;
        [SerializeField] private GameManager gameManager;

        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button exitButton;

        /// <summary>
        /// Set eventhandlers
        /// </summary>
        private void Awake()
        {
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        /// <summary>
        /// Player has pressed the retry button. Respawn
        /// the player.
        /// </summary>
        private void OnRetryButtonClicked()
        {
            this.gameManager.RespawnPlayer();
        }

        /// <summary>
        /// Player has pressed the exit button. Navigate
        /// to menu scene.
        /// </summary>
        private void OnExitButtonClicked()
        {
            this.gameManager.ExitToMainMenu();
        }
    }
}

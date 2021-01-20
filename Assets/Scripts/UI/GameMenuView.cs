#pragma warning disable CS0649

using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoronaGame.UI
{
    [Serializable]
    public class GameMenuView : UIView
    {
        [Header("Master")]
        [SerializeField] private ViewManager viewManager;
        [SerializeField] private GameManager gameManager;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button exitButton;

        /// <summary>
        /// Set eventhandlers.
        /// </summary>
        private void Awake()
        {
            this.resumeButton.onClick.AddListener(OnResumeButtonClicked);
            this.settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            this.exitButton.onClick.AddListener(OnExitButtonClicked);
            
            // Disable saving/loading buttons for WebGL
            #if UNITY_WEBGL
            this.saveButton.interactable = false;
            this.loadButton.interactable = false;
            #else
            this.saveButton.onClick.AddListener(OnSaveButtonClicked);
            this.loadButton.onClick.AddListener(OnLoadButtonClicked);
            #endif
        }
        

        /// <summary>
        /// Player has pressed the exit button.
        /// Navigate to main menu, after unpausing the game.
        /// </summary>
        private void OnExitButtonClicked()
        {
            this.gameManager.UnpauseGame();
            this.gameManager.ExitToMainMenu();
        }

        /// <summary>
        /// Player has pressed the load button. Display
        /// available game saves.
        /// </summary>
        private void OnLoadButtonClicked()
        {
            this.viewManager.Open<LoadSaveView>();
        }

        /// <summary>
        /// Player has pressed the save button. Open the
        /// save dialog;
        /// </summary>
        private void OnSaveButtonClicked()
        {
            this.viewManager.Open<SaveGameView>();
        }

        /// <summary>
        /// Player has pressed the settings button. Open the
        /// settings dialog.
        /// </summary>
        private void OnSettingsButtonClicked()
        {
            this.viewManager.Open<SettingsView>();
        }

        /// <summary>
        /// Player has pressed the resume button. Unpause the game.
        /// </summary>
        private void OnResumeButtonClicked()
        {
            this.Close();
        }

        public override void Open()
        {
            base.Open();
            this.gameManager.PauseGame();
        }

        public override void Close()
        {
            base.Close();
            this.gameManager.UnpauseGame();
        }
    }
}

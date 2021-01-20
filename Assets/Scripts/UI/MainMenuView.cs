#pragma warning disable CS0649

using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoronaGame.UI
{
    [Serializable]
    public class MainMenuView : UIView
    {
        [Header("Master")]
        [SerializeField] private ViewManager viewManager;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button loadButton;

        private void Awake()
        {
            this.playButton.onClick.AddListener(OnPlayButtonClicked);
            this.exitButton.onClick.AddListener(OnExitButtonClicked);
            this.settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            // Disable saving/loading in WebGL
            #if UNITY_WEBGL
            this.loadButton.interactable = false;
            #else
            this.loadButton.onClick.AddListener(OnLoadButtonClicked);
            #endif
        }

        /// <summary>
        /// Player has pressed exit button. Exit application.
        /// </summary>
        private static void OnExitButtonClicked()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        /// <summary>
        /// Player has pressed the load button. Display
        /// available saves.
        /// </summary>
        private void OnLoadButtonClicked()
        {
            this.viewManager.Open<LoadSaveView>();
        }

        /// <summary>
        /// Player has pressed the settings button. Display
        /// settings dialog.
        /// </summary>
        private void OnSettingsButtonClicked()
        {
            this.viewManager.Open<SettingsView>();
        }

        /// <summary>
        /// Player has pressed the play button. Start new game
        /// from level 1.
        /// </summary>
        private static void OnPlayButtonClicked()
        {
            SceneController.Instance.ChangeScene(1);
        }
    }
}

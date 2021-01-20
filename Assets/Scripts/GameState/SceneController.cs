using System;
using UnityEngine;
using System.Collections;
using CoronaGame.UI;

namespace CoronaGame
{
    /// <summary>
    /// SceneController for changing to different scenes
    /// and handling operations during scene transitions.
    /// 
    /// Singleton pattern: use static Instance - object reference.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }
        
        private void Awake()
        {
            if (SceneController.Instance == null)
            {
                SceneController.Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }

        /// <summary>
        /// Changes scene to specified scene. Has "fade to black"
        /// scene transition animation. If custom view transition delegate
        /// is not provided, uses default for specified scene.
        /// </summary>
        /// <param name="scene">Scene</param>
        public void ChangeScene(int sceneIndex)
        {
            switch (sceneIndex)
            {
                case 0:
                    StartCoroutine(ChangeSceneCoroutine(ChangeToMainMenu, 0));
                    break;
                case 1: goto case 4;
                case 2: goto case 4;
                case 3: goto case 4;
                case 4:
                    StartCoroutine(ChangeSceneCoroutine(ChangeToGame, sceneIndex));
                    break;
            }
        }

        /// <summary>
        /// Changes to new scene using fade animation.
        /// transition delegate is invoked in between animations.
        /// </summary>
        /// <param name="transition">Scene transition delegate</param>
        private static IEnumerator ChangeSceneCoroutine(Action<int> transition, int levelIndex)
        {
            // Fade to black
            ViewManager.Instance.Open<ScreenFadeView>();
            yield return new WaitForSeconds(0.5f); // Wait animation duration.

            // Hide/Display different UI views
            transition.Invoke(levelIndex);

            // Fade to transparent.
            yield return new WaitForSeconds(0.5f);
            ViewManager.Instance.Close<ScreenFadeView>();
        }
        
        /// <summary>
        /// Closes other views and opens MainMenuView.
        /// </summary>
        private static void ChangeToMainMenu(int index=0)
        {
            ViewManager.Instance.CloseAllExcept(typeof(ScreenFadeView), typeof(MessageView));
            ViewManager.Instance.Open<MainMenuView>();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

            if (!AudioManager.Instance.IsPlaying("Theme"))
            {
                AudioManager.Instance.Stop("BossMusic");
                AudioManager.Instance.Stop("Victory");
                AudioManager.Instance.Play("Theme");
            }
        }

        /// <summary>
        /// Closes menu views and opens views related
        /// to game.
        /// </summary>
        private static void ChangeToGame(int levelIndex)
        {
            ViewManager.Instance.CloseAllExcept(typeof(ScreenFadeView), typeof(MessageView));
            ViewManager.Instance.Open<PlayerDataView>();
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex);
        }
    }
}

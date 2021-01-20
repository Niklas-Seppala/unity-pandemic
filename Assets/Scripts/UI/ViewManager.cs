#pragma warning disable CS0649

using System;
using System.Collections;
using UnityEngine;

namespace CoronaGame.UI
{
    public class ViewManager : MonoBehaviour
    {
        [Header("Master")]
        [SerializeField] private GameManager gameManager;

        [Header("UI views")]
        [SerializeField] private UIView[] views;

        public static ViewManager Instance { get; private set; }

        private UIView gameMenuView;
        private UIView loadView;
        private UIView newSaveView;
        private UIView saveGameView;
        private UIView creditsView;
        private UIView sceneTransitionView;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                this.creditsView = this.Get<CreditsView>();
                this.loadView = this.Get<LoadSaveView>();
                this.gameMenuView = this.Get<GameMenuView>();
                this.newSaveView = this.Get<NewSaveView>();
                this.saveGameView = this.Get<SaveGameView>();
                this.sceneTransitionView = this.Get<ScreenFadeView>();
            }
            else Destroy(this.gameObject);
        }

        /// <summary>
        /// UI inputs
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {   // Horrible abomination, but works :D
                if (this.creditsView.Active || this.sceneTransitionView.Active) return;
                if (this.gameManager.ActiveGame)
                {
                    if (this.newSaveView.Active) // Top "z-index view"
                    {
                        this.newSaveView.Close();
                        return;
                    }
                    if (this.saveGameView.Active)
                    {
                        this.saveGameView.Close();
                        return;
                    }
                    if (this.loadView.Active)
                    {
                        this.loadView.Close();
                        return;
                    }
                    if (this.gameMenuView.Active)
                    {
                        this.gameMenuView.Close();
                    }
                    else
                    {
                        this.gameMenuView.Open();
                    }
                }
            }
        }

        /// <summary>
        /// Updates player ammo count on player data view.
        /// </summary>
        /// <param name="count">ammo count</param>
        public void UpdateAmmoCount(int count)
        {
            this.Get<PlayerDataView>().AmmoText = count.ToString();
        }

        /// <summary>
        /// Updates player facemask count on player data view.
        /// </summary>
        /// <param name="count">facemaks count</param>
        public void UpdateFacemaskCount(int count)
        {
            this.Get<PlayerDataView>().FaceMaskText = count.ToString();
        }


        /// <summary>
        /// Shows message to player.
        /// </summary>
        /// <param name="message">message string</param>
        /// <param name="cooldown">time untill next message</param>
        /// <param name="duration">message duration</param>
        public void FlashMessage(string message, float cooldown=0.5f, float duration=1f)
        {
            this.Get<MessageView>().QueueMessage(message, cooldown, duration);
        }

        /// <summary>
        /// Gets the View.
        /// </summary>
        /// <typeparam name="T">Type of the view</typeparam>
        /// <returns>View</returns>
        public T Get<T>() where T : UIView
        {
            foreach (var view in this.views)
            {
                if (view is T) return view as T;
            }
            throw new Exception($"Type {typeof(T)} not found!");
        }

        /// <summary>
        /// Closes the specified view.
        /// </summary>
        /// <typeparam name="T">Type of the view</typeparam>
        public void Close<T>() where T : UIView
        {
            foreach (var view in this.views)
            {
                if (view is T)
                {
                    view.Close();
                    return;
                }
            }
            throw new Exception($"Type {typeof(T)} not found!");
        }

        /// <summary>
        /// Opens the specified view.
        /// </summary>
        /// <typeparam name="T">Type of the view.</typeparam>
        public void Open<T>() where T : UIView
        {
            foreach (var view in this.views)
            {
                if (view is T)
                {
                    view.Open();
                    return;
                }
            }
            throw new Exception($"Type {typeof(T)} not found!");
        }

        /// <summary>
        /// Closes all except specified types of views.
        /// </summary>
        /// <param name="exclude">types of views to exclude</param>
        public void CloseAllExcept(params Type[] exclude)
        {
            foreach (var view in this.views)
            {
                if (exclude != null && Array.Exists(exclude, t => view.GetType() == t))
                    continue;
                if (view.Active) view.Close();
            }
        }

        /// <summary>
        /// Screen fades to black and becomes clear again. Specify actions
        /// if you wish.
        /// </summary>
        /// <param name="beginAction">Invoked at the beginning</param>
        /// <param name="middleAction">Invoked at the middle of fade</param>
        /// <param name="callback">Invoked at the end</param>
        public void ScreenFade(Action beginAction=null, Action middleAction=null, Action callback=null)
            => StartCoroutine(FadeToCoroutine(beginAction, middleAction, callback));

        /// <summary>
        /// Fade coroutine. See above.
        /// </summary>
        private IEnumerator FadeToCoroutine(Action beginAction=null, Action middleAction=null, Action callback=null)
        {
            const float ANIM_DUR = 0.5f;

            beginAction?.Invoke();

            this.Open<ScreenFadeView>();
            yield return new WaitForSeconds(ANIM_DUR);

            middleAction?.Invoke();

            yield return new WaitForSeconds(ANIM_DUR);
            this.Close<ScreenFadeView>();

            callback?.Invoke();
        }
    }
}

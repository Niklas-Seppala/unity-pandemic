#pragma warning disable CS0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CoronaGame.Data;
using System;

namespace CoronaGame.UI
{
    public class LoadSaveView : UIView
    {
        [SerializeField] private SaveView savePrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private Transform saveList;

        private void Awake()
        {
            this.backButton.onClick.AddListener(() => this.Close());
        }

        /// <summary>
        /// Create the save buttons for each save fetched from the
        /// database.
        /// </summary>
        /// <param name="saves">collection of save models</param>
        public void CreateSaveViews(IReadOnlyCollection<SaveModel> saves)
        {
            // Destroy the old save buttons.
            var old = saveList.transform.GetComponentsInChildren<SaveView>();
            foreach (var save in old)
                Destroy(save.gameObject);
            
            // Add fresh save buttons.
            foreach (var save in saves)
            {
                var uiView = Instantiate(savePrefab);
                uiView.transform.SetParent(saveList, false);
                uiView.Save = save;

                var time = DateTimeOffset.FromUnixTimeSeconds(save.Timestamp).Date
                    .ToShortDateString();
                uiView.Text = $"{save.Name} {time}";
                uiView.SetText();
            }
        }

        public override void Open()
        {
            GameManager.Instance.SaveManager.ReloadSaves();
            this.CreateSaveViews(GameManager.Instance.SaveManager.Saves);
            base.Open();
        }
    }
}


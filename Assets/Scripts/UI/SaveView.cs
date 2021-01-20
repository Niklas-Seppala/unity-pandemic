#pragma warning disable CS0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using CoronaGame.Data;

namespace CoronaGame.UI
{
    public class SaveView : UIView
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;
        public string Text { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public SaveModel Save { get; set; }
        public int Id => this.Save.Id;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            button.onClick.AddListener(this.LoadSave);
        }

        /// <summary>
        /// Set the text to UI text field
        /// </summary>
        public void SetText() => this.text.text = Text;


        /// <summary>
        /// Load this save.
        /// </summary>
        private void LoadSave()
        {
            GameManager.Instance.LoadGame(this.Save);
            ViewManager.Instance.Close<GameMenuView>();
        }
    }
}


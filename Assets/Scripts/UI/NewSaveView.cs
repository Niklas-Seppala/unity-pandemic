#pragma warning disable CS0649

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CoronaGame.UI
{
    public class NewSaveView : UIView
    {
        [SerializeField] private TMP_InputField textInput;
        [SerializeField] private Button backButton;
        [SerializeField] private Button saveButton;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            saveButton.onClick.AddListener(SubmitSave);
            backButton.onClick.AddListener(() => this.Close());
        }

        /// <summary>
        /// Save the game
        /// </summary>
        private void SubmitSave()
        {
            GameManager.Instance.SaveGame(this.textInput.text);

            // Close this and previous view.
            this.Close();
            ViewManager.Instance.Close<SaveGameView>();
        }
        
        public override void Close()
        {
            base.Close();
            this.textInput.text = "";
        }
    }
}


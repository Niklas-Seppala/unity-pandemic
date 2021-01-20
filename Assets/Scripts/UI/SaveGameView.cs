#pragma warning disable CS0649

using UnityEngine;
using UnityEngine.UI;

namespace CoronaGame.UI
{
    public class SaveGameView : UIView
    {
        [SerializeField] Button backButton;
        [SerializeField] Button overwriteButton;
        [SerializeField] Button newSaveButton;

        /// <summary>
        /// Add click eventhandlers to buttons.
        /// </summary>
        void Awake()
        {
            this.backButton.onClick.AddListener(() => this.Close());
            this.overwriteButton.onClick.AddListener(OverWriteCurrenSave);
            this.newSaveButton.onClick.AddListener(CreateNewSave);
        }

        /// <summary>
        /// Player has clicked new save button. Open save creation
        /// dialog.
        /// </summary>
        private void CreateNewSave()
        {
            ViewManager.Instance.Open<NewSaveView>();
        }

        /// <summary>
        /// Player has clicked the overwrite button. Overwrite
        /// current save.
        /// </summary>
        private void OverWriteCurrenSave()
        {
            if (GameManager.Instance.CurrentSave != null)
            {
                GameManager.Instance.OverwriteCurrentSave();
                this.Close();
            }
            else
            {
                ViewManager.Instance.FlashMessage("No Save to Override!");
            }
        }
    }
}


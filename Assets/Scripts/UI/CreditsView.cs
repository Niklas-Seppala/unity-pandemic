using System.Collections;
using UnityEngine;

namespace CoronaGame.UI
{
    public class CreditsView : UIView
    {
        [SerializeField] float duration = 10f;

        public override void Open()
        {
            base.Open();
            StartCoroutine(CloseAfterDuration(this.duration));
        }

        private IEnumerator CloseAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            AudioManager.Instance.Stop("Victory");
            GameManager.Instance.ExitToMainMenu();
        }
    }
}


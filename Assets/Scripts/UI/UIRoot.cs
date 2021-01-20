using UnityEngine;

namespace CoronaGame.UI
{
    /// <summary>
    /// UI lives the whole the app life.
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        private static UIRoot Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                UIRoot.Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}


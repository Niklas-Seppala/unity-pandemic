#pragma warning disable CS0649
using UnityEngine;

namespace CoronaGame
{
    public class MainMenuBackground : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        /// <summary>
        /// Scroll the camera to the right in main menu view.
        /// </summary>
        private void Update()
        {
            Transform backgroundTransform = this.transform;
            backgroundTransform.position = (Vector2)backgroundTransform.position +
                 new Vector2(speed * Time.deltaTime, 0f);
        }
    }
}

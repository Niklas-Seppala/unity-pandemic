#pragma warning disable CS0649

using System.Collections;
using UnityEngine;

using CoronaGame.UI;

namespace CoronaGame
{
    public class InfoPoint : MonoBehaviour
    {
        [TextArea]
        [SerializeField] string message;
        [SerializeField] float messageDuration;
        
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            this.messageDuration = 3;
            ViewManager.Instance.FlashMessage(this.message, duration: messageDuration);
        }
    }
}


#pragma warning disable CS0649

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System;

namespace CoronaGame.UI
{
    [Serializable]
    public class MessageView : UIView
    {
        [Header("Master")]
        [SerializeField] private ViewManager viewManager;
        private TextMeshProUGUI tmp;
        private string Text
        {
            get => tmp.text;
            set => tmp.text = value;
        }
        private bool FlashActive { get; set; } = false;
        private Queue<string> Messages { get; set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            this.tmp = this.GetComponent<TextMeshProUGUI>();
            this.Messages = new Queue<string>();
        }

        /// <summary>
        /// Adds message to message queue. Displays
        /// the message on screen.
        /// </summary>
        /// <param name="message">message</param>
        public void QueueMessage(string message, float waitUntilNext=0.5f, float duration=1f)
        {
            // If queue already contains the same message, ignore it.
            if (this.Text == message || this.Messages.Contains(message))
                return;

            // add the message to queue
            this.Messages.Enqueue(message);

            // Start flashing the messages, if not already active
            if (!this.FlashActive)
                StartCoroutine(this.MessageFlashCoroutine(duration, waitUntilNext));
        }

        /// <summary>
        /// Displays all messages on the queue.
        /// </summary>
        /// <param name="duration">message duration</param>
        /// <param name="waitUntilNext">duration between messages</param>
        private IEnumerator MessageFlashCoroutine(float duration, float waitUntilNext)
        {
            this.FlashActive = true;
            while (this.Messages.Count > 0)
            {
                this.Text = this.Messages.Dequeue();
                yield return new WaitForSecondsRealtime(duration);

                this.Text = "";
                if (waitUntilNext > 0)
                yield return new WaitForSecondsRealtime(waitUntilNext);
            }
            // Clear the message field.
            this.FlashActive = false;
            this.Text = "";
        }
    }
}

#pragma warning disable CS0649

using System.Collections;
using UnityEngine;
using CoronaGame.Units;

namespace CoronaGame.Items
{
    /// <summary>
    /// Base class for all pickup items.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public abstract class WorldItem : MonoBehaviour, ICollectable
    {
        protected Player player;
        private Collider2D collider_2D;

        public abstract void Collect();

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="otherCollider">The other Collider2D involved in this collision.</param>
        private void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if (Unit.Is<Player>(otherCollider.gameObject, out var player))
            {
                AudioManager.Instance.Play("HitItem");
                this.player = player;
                StopAllCoroutines();
                this.CollectionAnimation();
            }
        }

        protected virtual void Awake()
        {
            collider_2D = this.GetComponent<Collider2D>();
        }

        /// <summary>
        /// Play the collect animation.
        /// </summary>
        private void CollectionAnimation()
        {
            this.collider_2D.enabled = false;
            StartCoroutine(Up());
        }

        /// <summary>
        /// Item flies in the air and then
        /// to player.
        /// </summary>
        private IEnumerator Up()
        {
            const float duration = 0.5f;
            float elapsed = 0f;

            Transform item = this.transform;
            Vector3 position = item.position;
            Vector2 startPos = position;
            Vector2 targetPos = (Vector2)position + new Vector2(0f, 3f);

            Vector3 startScale = item.localScale;
            Vector3 targetScale = startScale + new Vector3(0.3f, 0.3f, 0.3f);

            while (elapsed < duration)
            {
                this.transform.position = Vector2.Lerp(startPos, targetPos, elapsed / duration);
                this.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            yield return Down();
        }

        /// <summary>
        /// Item flies to player and gets collected.
        /// </summary>
        private IEnumerator Down()
        {
            const float duration = 0.2f;
            float elapsed = 0f;

            Transform item = this.transform;
            var startPos = (Vector2)item.position;
            var offset = new Vector3(0f, 0.5f, 0f);

            Vector3 startScale = item.localScale;
            var targetScale = new Vector3(0.5f, 0.5f, 0.5f);

            while (elapsed < duration)
            {
                this.transform.position = Vector2.Lerp(startPos, this.player.transform.position + offset, elapsed / duration);
                this.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Destroy(this.gameObject);
            this.Collect();
            AudioManager.Instance.Play("ItemCollected");
        }
    }
}


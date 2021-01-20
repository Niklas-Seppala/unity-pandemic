using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS0649

namespace CoronaGame.Units
{
    public class ParticleCollisions : MonoBehaviour
    {
        #region Unity Editor Fields
        [SerializeField]
        private GameObject[] bloodStains;
        #endregion

        private ParticleSystem particles;
        private List<ParticleCollisionEvent> collisionEvents;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            this.particles = this.GetComponent<ParticleSystem>();
            this.collisionEvents = new List<ParticleCollisionEvent>();
        }

        /// <summary>
        /// Sets splatter transforms rotation and position
        /// to provided values.
        /// </summary>
        /// <param name="splatter">Splatter sprite transform</param>
        /// <param name="rotation">desired rotation</param>
        /// <param name="position">desired position</param>
        private static void FixSplatterTransform(Transform splatter, Vector3 rotation, Vector3 position)
        {
            splatter.eulerAngles = rotation;
            splatter.position = position;
        }

        /// <summary>
        /// Handle enemy death effect particle collisions.
        /// Only include collisions with "Platform" tag
        /// </summary>
        /// <param name="other">other object in collision</param>
        private void OnParticleCollision(GameObject other)
        {
            if (!other.gameObject.CompareTag("Platform")) return;

            particles.GetCollisionEvents(other, collisionEvents);
            foreach (ParticleCollisionEvent t in collisionEvents)
            {
                // Randomize splatter sprite
                GameObject splatter = Instantiate(bloodStains[Random.Range(0, 3)]);
                Vector3 normal = t.normal.normalized;
                Vector3 collision = t.intersection;

                if (Mathf.Approximately(Mathf.Round(normal.x) , 0f))
                {
                    // Particle collided from above
                    FixSplatterTransform(splatter.transform,
                        Vector3.zero,
                        new Vector3(collision.x, collision.y - 0.15f, 0f)
                    );
                    
                }
                else if (normal.x > 0f)
                {
                    // Particle collided from right.
                    FixSplatterTransform(splatter.transform,
                        new Vector3(0f, 0f, -90f),
                        new Vector3(collision.x - 0.25f, collision.y, 0f)
                    );
                }
                else if ( normal.x < 0f)
                {   
                    // Collision collided from left.
                    FixSplatterTransform(splatter.transform,
                        new Vector3(0f, 0f, 90f),
                        new Vector3(collision.x + 0.25f, collision.y, 0f)
                    );
                }
                // Destroy splatter sprites after one second.
                Destroy(splatter, 1f);
            }
        }
    }
}

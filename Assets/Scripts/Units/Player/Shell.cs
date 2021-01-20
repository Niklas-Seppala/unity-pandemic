using System.Collections;
using UnityEngine;

namespace CoronaGame.Units
{
    public class Shell : MonoBehaviour
    {
        private float lifeSpan;
        public Vector2 LaunchDirection{ get; set; }
        private Rigidbody2D rgbody;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            this.rgbody = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Sets this shell game object to active.
        /// </summary>
        /// <param name="state">true for active,
        /// false for inactive</param>
        public Shell SetActive(bool state)
        {
            this.gameObject.SetActive(state);
            return this;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            Launch();
            StartCoroutine(ReturnToPool());
            IEnumerator ReturnToPool()
            {
                yield return new WaitForSeconds(this.lifeSpan);
                this.SetActive(false).transform.position = this.transform.parent.position;
            }
        }

        /// <summary>
        /// Initialises the shell object with provided
        /// parameters.
        /// </summary>
        /// <param name="lifeSpan">lifespan in seconds</param>
        /// <param name="launchDirection">normalized direction vector</param>
        /// <param name="parent">parent object transform</param>
        public Shell Init(float lifeSpan, Vector2 launchDirection, Transform parent)
        {
            this.lifeSpan = lifeSpan;
            this.LaunchDirection = launchDirection;
            this.transform.SetParent(parent);
            return this;
        }

        /// <summary>
        /// Launches the shell towards set LaunchDirection with constant
        /// force vector.
        /// </summary>
        private void Launch()
        {
            this.rgbody.AddForce(new Vector2(LaunchDirection.x * 100f, 400));
        }
    }
}


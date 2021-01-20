#pragma warning disable CS0649

using UnityEngine;

namespace CoronaGame
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField] private GameObject cam;

        [Range(0, 1)]
        [SerializeField] private float parallaxEffect;

        private float start;
        private float bgWidth;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            start = transform.position.x;
            bgWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            float temp = cam.transform.position.x * (1f - parallaxEffect);
            float dist = cam.transform.position.x * parallaxEffect;
            transform.position = new Vector3(start + dist, transform.position.y, transform.position.z);

            if (temp > start + bgWidth)
            {
                start += bgWidth;
            }
            else if (temp < start - bgWidth)
            {
                start -= bgWidth;
            }
        }
    }
}


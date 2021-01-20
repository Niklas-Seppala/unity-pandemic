#pragma warning disable CS0649

using System.Collections;
using UnityEngine;

namespace CoronaGame
{
    /// <summary>
    /// Simple component that floats the game object in
    /// the air
    /// </summary>
    public class Floater : MonoBehaviour
    {
        [SerializeField] private float range;

        private Vector3 up;
        private Vector3 down;
        private Vector3 middle;
        private Vector2 vel = Vector2.zero;

        private void Awake()
        {
            this.up = this.transform.position + new Vector3(0, range, 0);
            this.down = this.transform.position + new Vector3(0, -range, 0);
            StartCoroutine(FloatInAir());
        }

        /// <summary>
        /// Moves the game object in air between two points.
        /// </summary>
        private IEnumerator FloatInAir()
        {
            while (true)
            {
                while (Vector3.Distance(this.transform.position, up) > 0.01f)
                {
                    this.transform.position = Vector2.SmoothDamp(this.transform.position, up, ref vel, 0.7f, 0.5f);
                    yield return null;
                }
                while (Vector3.Distance(this.transform.position, down) > 0.01f)
                {
                    this.transform.position = Vector2.SmoothDamp(this.transform.position, down, ref vel, 0.7f, 0.2f);
                    yield return null;
                }
            }
        }
    }
}

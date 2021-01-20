#pragma warning disable CS0649

using UnityEngine;

namespace CoronaGame.Units
{
    public class ShellPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [Min(5)] [SerializeField] private int depth = 5;
        [SerializeField] private Shell shellPrefab;
        [Min(0.1f)] [SerializeField] private float shellLifeSpan;

        private bool Filled => this.count == this.depth;
        private Shell[] objectPool;
        private int count = 0;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// 
        /// Initialize object pool.
        /// </summary>
        private void Awake()
        {
            this.objectPool = new Shell[this.depth];
            shellPrefab.gameObject.SetActive(false);
        }

        /// <summary>
        /// Gets free shell object from the pool.
        /// </summary>
        /// <param name="direction"></param>
        public void GetShell(Vector2 direction)
        {
            if (!this.Filled)
            {
                // Instantiate shell prefab and use that first.
                this.objectPool[this.count++] = Instantiate(this.shellPrefab, this.transform.position, Quaternion.identity)
                    .Init(shellLifeSpan, direction, this.transform)
                    .SetActive(true);
                return;
            }

            // Take the first free shell from the pool and use that.
            for (int i = 0; i < this.depth; i++)
            {
                if (this.objectPool[i].gameObject.activeSelf) continue;
                this.objectPool[i].LaunchDirection = direction;
                this.objectPool[i].SetActive(true);
                return;
            }
        }
    }

}

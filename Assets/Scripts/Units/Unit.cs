using System;
using UnityEngine;

namespace CoronaGame.Units
{
    /// <summary>
    /// Abstract base class for game world units.
    /// </summary>
    [Serializable]
    public abstract class Unit : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] protected int health;
        public int Health {get => this.health; set => this.health = value; }

        [Header("Movement")]
        [SerializeField] protected float movementSpeed;

        protected Animator animator;
        protected Rigidbody2D rgbody;
        protected new Collider2D collider2D;
        protected SpriteRenderer spriteRenderer;

        /// <summary>
        /// Unit dies.
        /// </summary>
        public abstract void Die();

        /// <summary>
        /// Init Unity components.
        /// </summary>
        protected virtual void Awake()
        {
            this.collider2D = GetComponent<Collider2D>();
            this.animator = GetComponent<Animator>();
            this.rgbody = GetComponent<Rigidbody2D>();
            this.spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Checks if the Unity GameObject has
        /// Unit typed script component.
        /// </summary>
        /// <param name="obj">Unity game object</param>
        /// <param name="unit">Typed unit object</param>
        /// <typeparam name="T">Unit type</typeparam>
        /// <returns>true if game object has T - Type script</returns>
        public static bool Is<T>(GameObject obj, out T unit) where T : Unit
            => unit = obj.GetComponent<T>();
    }
}

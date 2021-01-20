using UnityEngine;

namespace CoronaGame.Units.Enemies
{
    public class MeleeAttack
    {
        /// <summary>
        /// Get the world position where attack was
        /// started.
        /// </summary>
        public Vector3 StartPosition { get; set; }

        public bool Forward { get; private set; }
        public bool Retreat { get; private set; }
        public bool Done => !this.Forward && !this.Retreat;

        /// <summary>
        /// Initializes new state.
        /// </summary>
        /// <param name="position"></param>
        public void NewState(Vector3 position)
        {
            this.StartPosition = position;
            this.Forward = true;
            this.Retreat = false;
        }

        /// <summary>
        /// Attack moves to retreat phase.
        /// </summary>
        public void SwitchToRetreat()
        {
            this.Retreat = true;
            this.Forward = false;
        }

        /// <summary>
        /// Attack done.
        /// </summary>
        public void Finish() => this.Retreat = this.Forward = false;

        /// <summary>
        /// Instantiates MeleeAttackState object.
        /// </summary>
        public MeleeAttack()
        {
            this.StartPosition = Vector3.zero;
            this.Forward = false;
            this.Retreat = false;
        }
    }
}

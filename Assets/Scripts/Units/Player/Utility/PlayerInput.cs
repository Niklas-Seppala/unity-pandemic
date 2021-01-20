
namespace CoronaGame
{
    /// <summary>
    /// Holds player's inputs for one frame.
    /// </summary>
    internal readonly struct PlayerInput
    {
        public bool Jump { get; }
        public float Horizontal { get; }
        public bool Fire { get; }
        public bool ActiveMovement => Horizontal != 0;

        public PlayerInput(bool jump, float xAxis, bool fire)
        {
            this.Jump = jump;
            this.Fire = fire;
            this.Horizontal = xAxis;
        }
    }
}


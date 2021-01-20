using UnityEngine;

namespace CoronaGame.Units
{
    /// <summary>
    /// Player Animation state machine.
    /// Only plays walking sounds.
    /// </summary>
    public class WalkingStateSound : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator,
            AnimatorStateInfo stateInfo, int layerIndex)
        {
            AudioManager.Instance.Play("RunGround");
        }

        public override void OnStateExit(Animator animator,
            AnimatorStateInfo stateInfo, int layerIndex)
        {
            AudioManager.Instance.Stop("RunGround");
        }
    }
}

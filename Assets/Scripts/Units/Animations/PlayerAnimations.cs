using UnityEngine;

namespace CoronaGame.Units
{
    /// <summary>
    /// Custom animator for player unit. Changes the
    /// animation states based on players movement and
    /// facemask count.
    /// </summary>
    public static class PlayerAnimations
    {
        private static Animator animator;
        private static bool jumping;

        /// <summary>
        /// Intializes the class for controlling player animation.
        /// </summary>
        /// <param name="pAnimator">Player Animator</param>
        public static void Init(Animator pAnimator)
        {
            animator = pAnimator;
            jumping = false;
        }

        public static void PlayerDied() => animator.Play("player_death");

        
        public static void PlayerStopped(bool playerHasGun, int maskCount)
        {
            if (jumping) return;
            if (maskCount > 0)
                animator.Play(playerHasGun ? "player_gun_facemask_idle" : "player_facemask_idle");
            else
                animator.Play(playerHasGun ? "player_gun_idle" : "player_idle");
        }

        public static void PlayerWalking(bool playerHasGun, int maskCount)
        {
            if (jumping) return;
            if (maskCount > 0)
                animator.Play(playerHasGun ? "player_gun_facemask_walk" : "player_facemask_walk");
            else
                animator.Play(playerHasGun ? "player_gun_walk" : "player_walk");
        }

        public static void PlayerLanded(bool playerHasGun, int maskCount)
        {
            jumping = false;
            if (maskCount > 0)
                animator.Play(playerHasGun ? "player_gun_facemask_idle" : "player_facemask_idle");
            else
                animator.Play(playerHasGun ? "player_gun_idle" : "player_idle");
        }

        public static void PlayerJumped(bool playerHasGun, int maskCount)
        {
            jumping = true;
            if (maskCount > 0)
                animator.Play(playerHasGun ? "player_gun_facemask_jump" : "player_facemask_jump");
            else
                animator.Play(playerHasGun ? "player_gun_jump" : "player_jump");
        }
    }
}


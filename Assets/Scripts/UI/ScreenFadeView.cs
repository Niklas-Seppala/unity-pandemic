using UnityEngine;
using System;

namespace CoronaGame.UI
{
    [Serializable]
    public class ScreenFadeView : UIView
    {
        private Animator animator;
        private void Awake() => this.animator = this.GetComponent<Animator>();
        private void OnEnable() => animator.Play("scene_fade");
    }
}

#pragma warning disable CS0649

using System;
using UnityEngine.UI;

namespace CoronaGame.UI
{
    [Serializable]
    public class BossHealthBarView : UIView
    {
        private Image bossHealth;
        private void Awake() => this.bossHealth = this.GetComponent<Image>();
        public void UpdateHealthBar(float healthPercent) => this.bossHealth.fillAmount = healthPercent;
        public override void Close()
        {
            base.Close();
            this.UpdateHealthBar(1); // fill the health bar back to full
        }
    }
}

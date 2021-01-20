using UnityEngine;

namespace CoronaGame.Items
{
    public class AmmoBoxItem : WorldItem
    {
        [SerializeField] private int count = 4;

        public override void Collect()
        {
            this.player.AddAmmo(this.count);
        }
    }
}


using CoronaGame.Units;
using UnityEngine;

namespace CoronaGame.Items
{
    public class ShotgunPickup : WorldItem
    {
        public override void Collect() => this.player.PickupShotgun();
    }
}


#pragma warning disable CS0649

using UnityEngine;

namespace CoronaGame.Items
{
    public class FacemaskItem : WorldItem
    {
        #region Unity Editor Fields
        [SerializeField]
        private int count = 1;
        #endregion
        
        public override void Collect()
        {
            this.player.AddFacemask(this.count);
        }
    }
}


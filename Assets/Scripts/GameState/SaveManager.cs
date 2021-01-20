using System.Collections.Generic;
using UnityEngine;
using CoronaGame.Data;

namespace CoronaGame
{
    /// <summary>
    /// Object that loads saves from database.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public IReadOnlyList<SaveModel> Saves { get; private set; }

        /// <summary>
        /// Loads the saves and stores them.
        /// </summary>
        public void ReloadSaves()
        {
            this.Saves = DbAccess.GetModels<SaveModel>();
            foreach (var save in this.Saves)
            {
                save.GatherRelatedData();
            }
        }
    }
}

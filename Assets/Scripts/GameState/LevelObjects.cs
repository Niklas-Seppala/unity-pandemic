#pragma warning disable CS0649

using UnityEngine;
using System;

namespace CoronaGame
{
    /// <summary>
    /// Default level items and enemies.
    /// </summary>
    [Serializable]
    public class LevelObjects
    {
        [SerializeField] private GameObject enemiesPrefab;
        public GameObject EnemyTemplate => this.enemiesPrefab;

        [SerializeField] private GameObject itemsPrefab;
        public GameObject ItemsTemplate => this.itemsPrefab;
    }
}

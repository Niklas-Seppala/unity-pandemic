#pragma warning disable CS0649

using UnityEngine;
using System.Collections.Generic;
using System;

namespace CoronaGame.Units.Enemies
{
    [Serializable]
    public struct PatrolRoute
    {
        [SerializeField] private Transform[] points;
        
        private Vector2[] vectors;
        public IReadOnlyList<Transform> Points => this.points;
        public int CurrentIndex { get; private set; }
        private bool useVectors;

        public void UseVectors()
        {
            this.useVectors = true;
            this.vectors = new Vector2[points.Length];
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = points[i].position;
            }
        }

        public Vector2 GetNextPoint()
        {
            this.CurrentIndex = this.CurrentIndex < this.points.Length-1 ? this.CurrentIndex + 1 : 0;
            return useVectors ? this.vectors[this.CurrentIndex] : (Vector2)this.points[this.CurrentIndex].position;
        }
    }
}

#pragma warning disable CS0649

using UnityEngine.Audio;
using UnityEngine;
using System;

namespace CoronaGame
{
    /// <summary>
    /// Class for audio clips. Serialized to
    /// Unity editor.
    /// </summary>
    [Serializable]
    public class Audio
    {
        public AudioClip clip;
        public bool loop;
        [Range(0f, 1f)] public float volume;
        [Range(1f, 3f)] public float pitch;
        
        [SerializeField] private string name;
        public string Name { get => name; set => name = value; }
        
        public AudioMixerGroup audioGroup;

        [HideInInspector] public AudioSource Source { get; set; }
    }
}



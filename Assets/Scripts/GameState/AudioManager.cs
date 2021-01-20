#pragma warning disable CS0649

using UnityEngine;
using System;
using Debug = UnityEngine.Debug;

namespace CoronaGame
{
    /// <summary>
    /// AudioManager that plays audio clips.
    /// Uses singleton pattern: use static Instance - object reference.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private Audio[] sounds;
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (AudioManager.Instance == null)
            {
                AudioManager.Instance = this;
                InitAudioClips();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Go trough all editor audio clips and create Unity AudioSource
        /// component for each clip.
        /// </summary>
        private void InitAudioClips()
        {
            foreach (Audio clip in sounds)
            {
                clip.Source = this.gameObject.AddComponent<AudioSource>();
                clip.Source.pitch = clip.pitch;
                clip.Source.volume = clip.volume;
                clip.Source.loop = clip.loop;
                clip.Source.clip = clip.clip;
                clip.Source.outputAudioMixerGroup = clip.audioGroup;
            }
        }

        /// <summary>
        /// Pause the clip if it was playing.
        /// </summary>
        /// <param name="clipName">Audio clip name</param>
        public void Pause(string clipName)
        {
            foreach (Audio clip in sounds)
            {
                if (string.Equals(clip.Name, clipName, StringComparison.OrdinalIgnoreCase))
                {
                    if (clip.Source.isPlaying)
                    {
                        clip.Source.Pause();
                    }
                    return;
                }
            }
            Debug.LogWarning($"Audio {clipName} not found!");
        }

        /// <summary>
        /// Stop the clip if it was playing.
        /// </summary>
        /// <param name="clipName">Audio clip name</param>
        public void Stop(string clipName)
        {
            foreach (Audio clip in sounds)
            {
                if (string.Equals(clip.Name, clipName, StringComparison.OrdinalIgnoreCase))
                {
                    if (clip.Source.isPlaying)
                    {
                        clip.Source.Stop();
                    }
                    return;
                }
            }
            Debug.LogWarning($"Audio {clipName} not found!");
        }

        /// <summary>
        /// Check if the clip is playing.
        /// </summary>
        /// <param name="clipName">Clip name</param>
        /// <returns>true if clip was playing</returns>
        public bool IsPlaying(string clipName)
        {
            foreach (Audio clip in sounds)
            {
                if (string.Equals(clip.Name, clipName, StringComparison.OrdinalIgnoreCase))
                {
                    return clip.Source.isPlaying;
                }
            }
            Debug.LogWarning($"Audio {clipName} not found!");
            return false;
        }

        /// <summary>
        /// Play the audio clip.
        /// </summary>
        /// <param name="clipName">Clip name</param>
        public void Play(string clipName)
        {
            foreach (Audio clip in sounds)
            {
                if (string.Equals(clip.Name, clipName, StringComparison.OrdinalIgnoreCase))
                {
                    clip.Source.Play();
                    return;
                }
            }
            Debug.LogWarning($"Audio {clipName} not found!");
        }
    }
}


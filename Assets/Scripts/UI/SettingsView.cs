#pragma warning disable CS0649

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Audio;

namespace CoronaGame.UI
{
    [Serializable]
    public abstract class UIView : MonoBehaviour
    {
        public virtual void Open() => this.gameObject?.SetActive(true);
        public virtual void Close() => this.gameObject?.SetActive(false);
        public virtual bool Active => this.gameObject.activeSelf;
    }

    [Serializable]
    public class SettingsView : UIView
    {
        [Header("Master")]
        [SerializeField] ViewManager viewManager;

        [Header("Inputs")]
        [SerializeField] Button backButton;
        [SerializeField] Button okButton;
        [SerializeField] Slider masterVolumeSlider;
        [SerializeField] Slider musicVolumeSlider;
        [SerializeField] TMP_Dropdown qualityDropdown;
        [SerializeField] AudioMixer audioMixer;

        /// <summary>
        /// Sets eventhandlers for UI settings inputs.
        /// </summary>
        private void Awake()
        {
            masterVolumeSlider.onValueChanged.AddListener(ChangeMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
            backButton.onClick.AddListener(OnBackClicked);
            qualityDropdown.onValueChanged.AddListener(OnChangeQuality);
        }

        private void OnBackClicked()
        {
            this.Close();
        }

        private void ChangeMusicVolume(float musicVolumeSlider)
        {
            audioMixer.SetFloat("VolumeMusic", musicVolumeSlider);
        }

        private void ChangeMasterVolume(float masterVolumeSlider)
        {
            audioMixer.SetFloat("VolumeMaster", masterVolumeSlider);
        }

        private void OnChangeQuality(int quality)
        {
             QualitySettings.SetQualityLevel(Math.Abs(quality - 3));
        }
    }
}
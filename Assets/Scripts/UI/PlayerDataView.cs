#pragma warning disable CS0649

using UnityEngine;
using TMPro;
using System;

namespace CoronaGame.UI
{
    [Serializable]
    public class PlayerDataView : UIView
    {
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI faceMaskText;

        public string AmmoText
        {
            get => ammoText.text;
            set => ammoText.text = value;
        }

        public string FaceMaskText
        {
            get => faceMaskText.text;
            set => faceMaskText.text = value;
        }
    }
}


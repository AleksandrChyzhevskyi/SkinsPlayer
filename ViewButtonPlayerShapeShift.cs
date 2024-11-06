using System;
using _Development.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.SkinsPlayer
{
    public class ViewButtonPlayerShapeShift : MonoBehaviour
    {
        public Image IconBoarder;
        public Image IconEffect;

        [SerializeField] private Button _button;

        private int _effectId;
        private SkinData _skinData;

        public event Action<int, SkinData> Clicked;

        private void OnEnable() => 
            _button.onClick.AddListener(OnClicked);

        private void OnDisable() => 
            _button.onClick.RemoveListener(OnClicked);

        private void OnClicked() => 
            Clicked?.Invoke(_effectId, _skinData);

        public void SetID(int Id) => 
            _effectId = Id;

        public void SetSkinData(SkinData skinData) => 
            _skinData = skinData;

        public void SetIcon(Sprite sprite) => 
            IconEffect.sprite = sprite;
    }
}
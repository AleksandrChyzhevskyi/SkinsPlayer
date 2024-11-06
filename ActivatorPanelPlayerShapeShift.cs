
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.LogicMono;
using UnityEngine;

namespace _Development.Scripts.SkinsPlayer
{
    public class ActivatorPanelPlayerShapeShift : MonoBehaviour
    {
        private ModelShapeShiftChoose _modelShapeShiftChoose;

        private void Start() =>
            GetElements();

        public void Show()
        {
            if (Character.Instance.CharacterData.Skins.DataFiles.Count > 1)
            {
                GeneralEvents.Instance.OnMovementFinished();
                _modelShapeShiftChoose.gameObject.SetActive(true);
            }
            else
                _modelShapeShiftChoose.ApplyEffect();
        }

        private void GetElements() => 
            _modelShapeShiftChoose = RPGBuilderEssentials.Instance.ModelShape;
    }
}
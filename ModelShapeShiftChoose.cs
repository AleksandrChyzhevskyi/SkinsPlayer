using System.Collections.Generic;
using System.Linq;
using _Development.Scripts.Boot;
using _Development.Scripts.Data;
using _Development.Scripts.SkinsPlayer.PlayerSkinsData;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.SkinsPlayer
{
    public class ModelShapeShiftChoose : MonoBehaviour
    {
        [SerializeField] private Button _prefabButton;
        [SerializeField] private Transform _transformParent;

        private Dictionary<int, ViewButtonPlayerShapeShift> _buttons;
        private List<StaticData.ShapeShiftingAbilityApply> _shapeShiftingAbilityApplies;
        private GameObject _effectModel;
        private SkinData _skinData;
        private CoroutineRunnerElements _coroutineRunner;

        private void OnEnable()
        {
            foreach (ViewButtonPlayerShapeShift buttonPlayerShapeShift in _buttons.Values)
                buttonPlayerShapeShift.Clicked += ApplyEffect;

            UpdateSkins(Character.Instance.CharacterData.Skins);
        }

        private void OnDisable()
        {
            foreach (ViewButtonPlayerShapeShift buttonPlayerShapeShift in _buttons.Values)
                buttonPlayerShapeShift.Clicked -= ApplyEffect;
        }

        public void Construct()
        {
            _buttons = new Dictionary<int, ViewButtonPlayerShapeShift>();
            _shapeShiftingAbilityApplies = Game.instance.GetStaticData().ShapeshiftingAbilities.ToList();
            _coroutineRunner = RPGBuilderEssentials.Instance.RunnerElements;

            CreateAllButtons();
            Subscribe();
        }

        private void OnDestroy() =>
            GeneralEvents.UpdatedSkins -= UpdateSkins;

        public void ApplyEffect(int iD = -1, SkinData skinData = null)
        {
            if (iD < 0)
            {
                PlayerSkinDataFile playerSkinDataFile = TryGetActiveEffect().FirstOrDefault();

                if (playerSkinDataFile != null)
                {
                    iD = playerSkinDataFile.EffectID;
                    _skinData = TryGetSkinData(playerSkinDataFile);
                }
                else
                {
                    iD = (int)AnimalsToEffectID.Bear;

                    if (Character.Instance.CharacterData.Skins.DataFiles.Count < 1)
                        RPGBuilderEssentials.Instance.InitializedPlayerSkinsDataService();

                    Analytics.instance.SendError("ApplyEffect", "Empty player skins");
                }
            }
            else
                _skinData = skinData;

            GameActionsManager.Instance.ApplyEffect(RPGCombatDATA.TARGET_TYPE.Caster,
                GameState.playerEntity, iD);

            _coroutineRunner.StartWaitFindElement(WaitModel);
        }

        private void Subscribe() =>
            GeneralEvents.UpdatedSkins += UpdateSkins;

        private void WaitModel()
        {
            if (_skinData != null)
            {
                _effectModel = GameState.playerEntity.ShapeshiftingGameobject;
                Renderer children = _effectModel.GetComponentInChildren<Renderer>();
                children.sharedMaterial = _skinData.Material;
                _skinData = null;
            }

            GeneralEvents.Instance.OnMovementStarted();
            gameObject.SetActive(false);
        }

        private void UpdateSkins(AllPlayerSkins newPlayerSkins)
        {
            foreach (PlayerSkinDataFile playerSkinDataFile in newPlayerSkins.DataFiles)
            {
                if (_buttons.ContainsKey(playerSkinDataFile.EffectID))
                    UpdateButton(_buttons[playerSkinDataFile.EffectID], playerSkinDataFile);
                else
                    CreateButton(playerSkinDataFile);
            }
        }

        private void CreateAllButtons()
        {
            foreach (PlayerSkinDataFile playerSkinDataFile in TryGetActiveEffect())
                CreateButton(playerSkinDataFile);
        }

        private List<PlayerSkinDataFile> TryGetActiveEffect()
        {
            List<PlayerSkinDataFile> playerSkinDataFiles = new List<PlayerSkinDataFile>();

            foreach (PlayerSkinDataFile playerSkinDataFile in Character.Instance.CharacterData.Skins.DataFiles)
            {
                foreach (SkinIDDataFile skinIDDataFile in playerSkinDataFile.Skinslist)
                {
                    if (skinIDDataFile.IsActive)
                        playerSkinDataFiles.Add(playerSkinDataFile);
                }
            }

            return playerSkinDataFiles;
        }

        private void CreateButton(PlayerSkinDataFile playerSkinDataFile)
        {
            Button buttonSkin = Instantiate(_prefabButton, _transformParent);

            ViewButtonPlayerShapeShift buttonPlayerShapeShift =
                buttonSkin.gameObject.GetComponent<ViewButtonPlayerShapeShift>();

            SkinData skinData = TryGetSkinData(playerSkinDataFile);

            if (skinData != null)
                SetElementsInButton(buttonPlayerShapeShift, skinData);
            else
                buttonPlayerShapeShift.SetIcon(
                    GameDatabase.Instance.GetEffects()[playerSkinDataFile.EffectID].entryIcon);

            buttonPlayerShapeShift.SetID(playerSkinDataFile.EffectID);

            _buttons.Add(playerSkinDataFile.EffectID, buttonPlayerShapeShift);
        }

        private SkinData TryGetSkinData(PlayerSkinDataFile playerSkinDataFile)
        {
            SkinID skinID = playerSkinDataFile.Skinslist.FirstOrDefault(skinIDDataFile => skinIDDataFile.IsActive)!.SkinIDData;

            foreach (StaticData.ShapeShiftingAbilityApply shapeShiftingAbilityApply in _shapeShiftingAbilityApplies)
            {
                if (shapeShiftingAbilityApply.ShapeshiftId == playerSkinDataFile.EffectID)
                {
                    foreach (SkinData skinData in shapeShiftingAbilityApply.SkinDates)
                    {
                        if (skinData.ID == skinID)
                            return skinData;
                    }
                }
            }

            return null;
        }

        private void UpdateButton(ViewButtonPlayerShapeShift buttonPlayerShapeShift,
            PlayerSkinDataFile playerSkinDataFile)
        {
            SkinData skinData = TryGetSkinData(playerSkinDataFile);

            if (skinData == null)
            {
                RPGEffect effect = GameDatabase.Instance.GetEffects()[playerSkinDataFile.EffectID];
                buttonPlayerShapeShift.SetIcon(effect.entryIcon);
                buttonPlayerShapeShift.SetSkinData(null);
                return;
            }

            SetElementsInButton(buttonPlayerShapeShift, skinData);
        }

        private void SetElementsInButton(ViewButtonPlayerShapeShift buttonPlayerShapeShift, SkinData skinData)
        {
            buttonPlayerShapeShift.SetIcon(skinData.ChooseIcon);
            buttonPlayerShapeShift.SetSkinData(skinData);
        }
    }
}
using System.IO;
using System.Linq;
using _Development.Scripts.Data;
using _Development.Scripts.SaveLoadDatesPlayer;
using _Development.Scripts.SaveLoadDatesPlayer.InterfaceSaveDatesPlayer;
using _Development.Scripts.SkinsPlayer.PlayerSkinsData;
using BLINK.RPGBuilder.Characters;

namespace _Development.Scripts.SkinsPlayer.Service
{
    public class PlayerSkinsDataService : Interface.IPlayerSkinsDataService
    {
        public ILoadSavePlayerSkins LoadSavePlayerSkins { get; }

        public PlayerSkinsDataService()
        {
            LoadSavePlayerSkins = new LoadSave();

            LoadPlayerSkins();
            Subscribe();
        }

        public void Unsubscribe() =>
            GeneralEvents.OpenedNewSkin -= OnOpenNewSkin;

        private void Subscribe() =>
            GeneralEvents.OpenedNewSkin += OnOpenNewSkin;

        private void LoadPlayerSkins()
        {
            if (File.Exists(PathSaveLoad.ForFindPathToPlayerSkinDataFile + ".txt") == false)
                LoadSavePlayerSkins.SaveDefaultEffect();

            LoadSavePlayerSkins.LoadPlayerSkins();
        }

        private void OnOpenNewSkin(RPGEffect effect, SkinID skinID)
        {
            AllPlayerSkins newSkins = GetAllPlayerSkins(effect, skinID);
            LoadSavePlayerSkins.SavePlayerSkins(newSkins);
            GeneralEvents.Instance.OnUpdatedNewSkin(newSkins);
        }

        private AllPlayerSkins GetAllPlayerSkins(RPGEffect effect, SkinID skinID)
        {
            ChangeStateInactive(effect);
            if (TryAddSkin(effect, skinID) == false)
                AddNewEffect(effect, skinID);

            return Character.Instance.CharacterData.Skins;
        }

        private void AddNewEffect(RPGEffect effect, SkinID skinID)
        {
            PlayerSkinDataFile skinDataFile = null;

            foreach (PlayerSkinDataFile playerSkinDataFile in Character.Instance.CharacterData.Skins.DataFiles)
            {
                if (playerSkinDataFile.EffectID != effect.ID)
                    skinDataFile = CreatePlayerSkinDataFile(effect, skinID);
            }

            if (skinDataFile != null)
                Character.Instance.CharacterData.Skins.DataFiles.Add(skinDataFile);
        }

        private bool TryAddSkin(RPGEffect effect, SkinID skinID)
        {
            SkinIDDataFile newSkinIDDataFile = null;

            foreach (PlayerSkinDataFile playerSkinDataFile in Character.Instance.CharacterData.Skins.DataFiles)
            {
                if (playerSkinDataFile.EffectID != effect.ID)
                    continue;

                foreach (var skinIDDataFile in playerSkinDataFile.Skinslist.ToList())
                {
                    if (skinIDDataFile.SkinIDData != skinID)
                        newSkinIDDataFile = CreateNewSkinIDDataFile(skinID);
                }
            }

            if (newSkinIDDataFile != null)
            {
                Character.Instance.CharacterData.Skins.DataFiles.FirstOrDefault(x => x.EffectID == effect.ID)!.Skinslist
                    .Add(newSkinIDDataFile);
                return true;
            }

            return false;
        }

        private void ChangeStateInactive(RPGEffect effect)
        {
            foreach (PlayerSkinDataFile playerSkinDataFile in Character.Instance.CharacterData.Skins.DataFiles)
            {
                if (playerSkinDataFile.EffectID != effect.ID)
                    continue;

                foreach (SkinIDDataFile skinIDDataFile in playerSkinDataFile.Skinslist.ToList())
                    skinIDDataFile.IsActive = false;
            }
        }

        private PlayerSkinDataFile CreatePlayerSkinDataFile(RPGEffect effect, SkinID skinID)
        {
            PlayerSkinDataFile skinDataFile = new PlayerSkinDataFile { EffectID = effect.ID };
            skinDataFile.Skinslist.Add(CreateNewSkinIDDataFile(skinID));
            return skinDataFile;
        }

        private SkinIDDataFile CreateNewSkinIDDataFile(SkinID skinID) =>
            new() { SkinIDData = skinID, IsActive = true };
    }
}
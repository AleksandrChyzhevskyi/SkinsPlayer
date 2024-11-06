using System;
using System.Collections.Generic;
using _Development.Scripts.Data;

namespace _Development.Scripts.SkinsPlayer.PlayerSkinsData
{
    [Serializable]
    public class PlayerSkinDataFile
    {
        public int EffectID;
        public List<SkinIDDataFile> Skinslist = new();
    }

    [Serializable]
    public class AllPlayerSkins
    {
        public List<PlayerSkinDataFile> DataFiles = new();
    }

    [Serializable]
    public class SkinIDDataFile
    {
        public SkinID SkinIDData;
        public bool IsActive;
    }
}
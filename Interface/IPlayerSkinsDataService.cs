using _Development.Scripts.SaveLoadDatesPlayer.InterfaceSaveDatesPlayer;

namespace _Development.Scripts.SkinsPlayer.Interface
{
    public interface IPlayerSkinsDataService
    {
        void Unsubscribe();
        ILoadSavePlayerSkins LoadSavePlayerSkins { get; }
    }
}
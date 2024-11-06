using _Development.Scripts.Data;
using UnityEngine;

namespace _Development.Scripts.SkinsPlayer
{
    public class PlayerNewSkinActivator : MonoBehaviour
    {
        [SerializeField] private RPGEffect _effect;

        public void UnlockSkin() =>
            GeneralEvents.Instance.OnOpenedNewSkin(_effect, SkinID.DefaultSkin);
    }
}
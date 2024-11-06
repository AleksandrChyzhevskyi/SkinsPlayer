using UnityEngine;

namespace _Development.Scripts.SkinsPlayer.VisualEffectAttack
{
    public class VisualEffectsRemovalController : MonoBehaviour
    {
        private GameObject _visualEffectsGameObject;

        private void OnEnable()
        {
            GeneralEvents.VisualEffectPrefabCreated += OnVisualEffectPrefabCreated;
            CombatEvents.PlayerDeathAccepted += DelAttack;
            CombatEvents.PlayerChangedSkin += DelAttack;
        }

        private void OnDestroy()
        {
            GeneralEvents.VisualEffectPrefabCreated -= OnVisualEffectPrefabCreated;
            CombatEvents.PlayerDeathAccepted -= DelAttack;
            CombatEvents.PlayerChangedSkin -= DelAttack;
        }

        private void OnVisualEffectPrefabCreated(GameObject visualEffects)
        {
            if(_visualEffectsGameObject != null)
                return;
            
            _visualEffectsGameObject = visualEffects;
        }

        public void DelAttack() => 
            Destroy(_visualEffectsGameObject);
    }
}
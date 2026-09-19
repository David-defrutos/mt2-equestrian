using System.Collections;

namespace mt2_equestrian.Plugin
{
    // Use card-trigger effects, avoiding the installed Trainworks trigger_effects finalizer.
    public sealed class CardEffectTavernAceUnplayed : CardEffectBase
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new PropDescriptions();

        public override bool TestEffect(CardEffectState effect, CardEffectParams parameters, ICoreGameManagers managers)
            => effect.GetParentCardState() != null;

        public override IEnumerator ApplyEffect(CardEffectState effect, CardEffectParams parameters,
            ICoreGameManagers managers, ISystemManagers systems)
        {
            var card = effect.GetParentCardState();
            if (card == null || managers.GetSaveManager().PreviewMode) yield break;
            // Same persistent card modifier used by the game's BuffCharacterDamage trigger.
            card.GetCardStateModifiers().IncrementAdditionalDamage(effect.GetParamInt());
            card.UpdateDamageText();
        }
    }
}

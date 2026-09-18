using System.Collections;
using HarmonyLib;

namespace mt2_equestrian.Plugin
{
    // Snapshot current values before changing either stat; healing still obeys Heartless.
    public sealed class CardEffectAlicornification : CardEffectBase
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new PropDescriptions();
        public override bool TestEffect(CardEffectState s, CardEffectParams p, ICoreGameManagers c) => p.targets.Count > 0;
        public override IEnumerator ApplyEffect(CardEffectState s, CardEffectParams p, ICoreGameManagers c, ISystemManagers m)
        {
            foreach (var target in p.targets)
            {
                int attack = target.GetAttackDamage();
                yield return target.ApplyHeal(target.GetMaxHP(), responsibleCard: p.playedCard);
                int health = target.GetHP();
                target.BuffDamage(attack);
                yield return target.BuffMaxHP(health, triggerOnHeal: false);
            }
        }
    }

    public sealed class CardEffectNonHeartless : CardEffectBase
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new PropDescriptions();
        public override bool TestEffectOnTarget(CardEffectState s, CardEffectParams p, CharacterState target, ICoreGameManagers c)
            => target != null && !target.HasStatusEffect("heal immunity");
        public override bool TestEffect(CardEffectState s, CardEffectParams p, ICoreGameManagers c)
            => p.targets.Any(t => TestEffectOnTarget(s, p, t, c));
        public override IEnumerator ApplyEffect(CardEffectState s, CardEffectParams p, ICoreGameManagers c, ISystemManagers m)
        { yield break; }
    }

    public sealed class RelicEffectFirstChampionReward : RelicEffectBase, ITurnPhaseStartOfBattleRelicEffect
    {
        private bool used;
        public override PropDescriptions CreateEditorInspectorDescriptions() => new PropDescriptions();
        public bool TestEffectStartOfBattle(RelicEffectParams p, ICoreGameManagers c) => true;
        public IEnumerator ApplyEffectStartOfBattle(RelicEffectParams p, ICoreGameManagers c) { used = false; yield break; }
        public override IEnumerator OnCharacterAdded(CharacterState character, CardState fromCard, ICoreGameManagers c)
        {
            if (used || c.GetSaveManager().PreviewMode || character.GetTeamType() != Team.Type.Monsters ||
                character.GetIsClone() || fromCard == null || !fromCard.IsChampionCard()) yield break;
            used = true;
            c.GetPlayerManager().AddEnergy(SourceRelicEffectData.GetParamInt());
            c.GetCardManager().DrawCards(SourceRelicEffectData.GetParamInt2(), null, CardType.Monster);
            NotifyRoomRelicTriggered(c);
            yield break;
        }
    }

    // A marker effect: the native trigger-count relic has no per-character filter.
    public sealed class RelicEffectSocialTriggerCount : RelicEffectBase
    {
        public override bool CanApplyInPreviewMode => true;
        public override PropDescriptions CreateEditorInspectorDescriptions() => new PropDescriptions();
    }

    [HarmonyPatch(typeof(CharacterState), nameof(CharacterState.GetTriggerFireCount))]
    internal static class SocialTriggerCountPatch
    {
        private static void Postfix(CharacterState __instance, CharacterTriggerState triggerStateContext,
            RelicManager ___relicManager, ref int __result)
        {
            if (__instance.GetTeamType() != Team.Type.Monsters || triggerStateContext?.isNoCountModifiersAllowed == true) return;
            // Social can come from an upgrade as well as a unit's printed subtype.
            var effects = ___relicManager.GetRelicEffects(new List<RelicEffectSocialTriggerCount>());
            foreach (var effect in effects)
                if (effect.SourceRelicEffectData.GetTriggers().Any(required =>
                    __instance.GetTriggers().Any(t => t.GetTriggerData() == required)))
                    __result += effect.SourceRelicEffectData.GetParamInt();
        }
    }

    public sealed class RelicEffectFixedServiceCosts : RelicEffectBase
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new PropDescriptions();
    }

    [HarmonyPatch(typeof(MerchantGoodState), nameof(MerchantGoodState.GetCost))]
    internal static class FixedServiceCostsPatch
    {
        private static void Prefix(MerchantGoodState __instance, ref int ___numPurchases, out int? __state)
        {
            __state = null;
            if (!(__instance.RewardData is PurgeRewardData) && !(__instance.RewardData is DuplicatorRewardData)) return;
            if (!__instance.RewardData.saveManager.GetAllRelics().Any(r => r.GetEffects().Any(e => e is RelicEffectFixedServiceCosts))) return;
            __state = ___numPurchases;
            ___numPurchases = 0;
        }
        // Restore even if another price modifier throws; purchase history is never erased.
        private static void Finalizer(ref int ___numPurchases, int? __state)
        { if (__state.HasValue) ___numPurchases = __state.Value; }
    }
}

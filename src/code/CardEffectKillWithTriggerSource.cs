using System.Collections;

namespace mt2_equestrian.Plugin
{
    // Native CardEffectKill passes no attacker for a character trigger. Supplying
    // its owner lets Sacrifice run the owner's Slay triggers through the engine.
    public sealed class CardEffectKillWithTriggerSource : CardEffectBase
    {
        public override PropDescriptions CreateEditorInspectorDescriptions()
            => new CardEffectKill().CreateEditorInspectorDescriptions();

        public override IEnumerator ApplyEffect(CardEffectState effect, CardEffectParams args,
            ICoreGameManagers managers, ISystemManagers systems)
        {
            var owner = args.selfTarget;
            if (owner == null || owner.IsDead || owner.IsDestroyed)
                yield break;

            // Death/Slay effects may remove other targets while this sequence runs.
            var targets = args.targets.ToArray();
            foreach (var target in targets)
            {
                if (owner.IsDead || owner.IsDestroyed)
                    yield break;
                if (target == null || target == owner || target.IsDestroyed || target.IsDead)
                    continue;
                yield return target.Sacrifice(owner.GetSpawnerCard(), ignoreTriggers: false,
                    doNotFullyRemove: false, attacker: owner);
            }
        }
    }
}

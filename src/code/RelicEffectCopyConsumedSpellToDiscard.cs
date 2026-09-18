using System.Collections;

namespace mt2_equestrian.Plugin
{
    // Reuse native eligibility (direct play, card type, required traits, copyable).
    // The JSON condition limits successful activations to one per player turn.
    public sealed class RelicEffectCopyConsumedSpellToDiscard : RelicEffectBase, ICardPlayedRelicEffect
    {
        private readonly RelicEffectCopyCardOnCardPlay eligibility = new();
        private int copies;
        public CardType CardType => eligibility.CardType;
        public override PropDescriptions CreateEditorInspectorDescriptions()
            => eligibility.CreateEditorInspectorDescriptions();

        public override void Initialize(RelicState relic, RelicData data, RelicEffectData effect)
        {
            base.Initialize(relic, data, effect);
            eligibility.Initialize(relic, data, effect);
            copies = effect.GetParamInt();
        }

        public bool TestEffectOnCardPlayed(CardPlayedRelicEffectParams args, ICoreGameManagers managers)
            => eligibility.TestEffectOnCardPlayed(args, managers);

        public IEnumerator ApplyEffectOnCardPlayed(CardPlayedRelicEffectParams args, ICoreGameManagers managers)
        {
            var source = args.cardState;
            var data = managers.GetAllGameData().FindCardData(source.GetCardDataID());
            var upgrades = new CardManager.AddCardUpgradingInfo
            {
                upgradingCardSource = source,
                copyModifiersFromCard = source,
                ignoreTempUpgrades = false
            };
            for (int i = 0; i < copies; i++)
            {
                var copy = managers.GetCardManager().AddNewCardCopy(data, source,
                    CardPile.DiscardPile, false, upgrades);
                if (copy != null && source.IsEventCachedCard)
                    copy.SetIsEventCachedCardDuplicate(true);
            }
            NotifyRoomRelicTriggered(managers);
            yield break;
        }
    }
}

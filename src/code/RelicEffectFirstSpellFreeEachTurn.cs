// Efecto de reliquia propio: el primer hechizo que juegas cada turno sale gratis.
//
// Portado de "CustomRelicEffecyModifyCardCostByType" del Equestrian de MT1
// (ThreeFishies/Equestrian-Clan, Relics/RoyalScroll.cs).
//
// Diferencias de API entre MT1 y MT2, ya resueltas aqui:
//   - MT1 IOnTurnStartRelicEffect        -> MT2 ITurnPhaseStartOfPlayerTurnAfterDrawRelicEffect
//                                           (marcador; los metodos son de ITurnPhaseTimingRelicEffect)
//   - MT1 los managers venian dentro de los params -> MT2 llegan en ICoreGameManagers
//   - MT1 CardUpgradeState.Setup(data, bool)       -> MT2 Setup(data, bool, bool)
//   - MT1 NotifyRelicTriggered(relicManager)       -> MT2 (relicManager, character, texto, centro, delay)

using System.Collections;

namespace mt2_equestrian.Plugin
{
    public sealed class RelicEffectFirstSpellFreeEachTurn
        : RelicEffectBase,
          IRelicEffect,
          ITurnPhaseStartOfPlayerTurnAfterDrawRelicEffect,
          ICardPlayedRelicEffect,
          IOnCardAddedToHandRelicEffect,
          IOnDiscardRelicEffect
    {
        private CardUpgradeData? _upgradeData;
        private CardType _cardType = CardType.Invalid;
        private readonly CardUpgradeState _upgradeState = new();
        private readonly List<CardState> _discounted = new();
        private bool _spentThisTurn;

        public override PropDescriptions CreateEditorInspectorDescriptions()
        {
            PropDescriptions propDescriptions = new();
            propDescriptions[RelicEffectFieldNames.ParamCardUpgradeData.GetFieldName()] =
                new PropDescription("Upgrade applied while the discount is live", "", null, false);
            propDescriptions[RelicEffectFieldNames.ParamCardType.GetFieldName()] =
                new PropDescription("Card Type to discount", "", null, false);
            return propDescriptions;
        }

        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, relicData, relicEffectData);
            _upgradeData = relicEffectData.GetParamCardUpgradeData();
            _cardType = relicEffectData.GetParamCardType();
            if (_upgradeData != null)
            {
                _upgradeState.Setup(_upgradeData, false, false);
            }
        }

        // --- Inicio del turno del jugador, DESPUES del robo: descontar toda la mano.

        public bool TestEffectTurnPhaseTiming(RelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
        {
            return _upgradeData != null;
        }

        public IEnumerator ApplyEffectTurnPhaseTiming(RelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
        {
            ClearAllDiscounts();
            _spentThisTurn = false;

            CardManager cardManager = coreGameManagers.GetCardManager();
            foreach (CardState cardState in cardManager.GetHand(true))
            {
                ApplyDiscount(cardState);
            }
            yield break;
        }

        // --- Al jugar una carta descontada: se consume para el resto del turno.

        public bool TestEffectOnCardPlayed(CardPlayedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
        {
            return !_spentThisTurn && _discounted.Contains(relicEffectParams.cardState);
        }

        public IEnumerator ApplyEffectOnCardPlayed(CardPlayedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
        {
            _spentThisTurn = true;
            ClearAllDiscounts();
            NotifyRelicTriggered(coreGameManagers.GetRelicManager(), null, "", false, 0f);
            yield break;
        }

        // --- Cartas que entran en la mano a mitad de turno: tambien entran al descuento.

        public bool OnCardAdded(CardAddedToHandRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
        {
            if (!_spentThisTurn)
            {
                ApplyDiscount(relicEffectParams.cardState);
            }
            return false;
        }

        // --- Descartada: quitarle el descuento para que no se lo lleve puesto.

        public bool TestEffectOnCardDiscarded(CardDiscardedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
        {
            return _discounted.Contains(relicEffectParams.discardCardParams.discardCard);
        }

        public IEnumerator ApplyEffectOnCardDiscarded(CardDiscardedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
        {
            RemoveDiscount(relicEffectParams.discardCardParams.discardCard);
            yield break;
        }

        // --- Interno

        private void ApplyDiscount(CardState? cardState)
        {
            if (cardState == null || _upgradeData == null)
            {
                return;
            }
            if (_cardType != CardType.Invalid && cardState.GetCardType() != _cardType)
            {
                return;
            }
            if (_discounted.Contains(cardState))
            {
                return;
            }

            cardState.GetTemporaryCardStateModifiers().AddUpgrade(_upgradeState, null);
            cardState.UpdateCardBodyText(null);
            _discounted.Add(cardState);
        }

        private void RemoveDiscount(CardState? cardState)
        {
            if (cardState == null)
            {
                return;
            }
            cardState.GetTemporaryCardStateModifiers().RemoveUpgrade(_upgradeState);
            cardState.UpdateCardBodyText(null);
            _discounted.Remove(cardState);
        }

        private void ClearAllDiscounts()
        {
            foreach (CardState cardState in _discounted)
            {
                cardState.GetTemporaryCardStateModifiers().RemoveUpgrade(_upgradeState);
                cardState.UpdateCardBodyText(null);
            }
            _discounted.Clear();
        }
    }
}
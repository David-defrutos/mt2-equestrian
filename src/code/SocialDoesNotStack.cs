using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;

namespace mt2_equestrian.Plugin
{
    internal static class SocialDoesNotStack
    {
        // Match the two registered Equestrian triggers, never other on-spawn/death effects.
        internal static string? Key(CharacterTriggerData trigger)
        {
            string key = trigger.GetDescriptionKey();
            foreach (string id in new[] { "SocialSpawn", "SocialDeath" })
                if (key == "CharacterTriggerData_descriptionKey-" + MyPluginInfo.PLUGIN_GUID.GetId("CTrigger", id))
                    return key;
            return null;
        }

        internal static void Filter(CardState card, ref CardUpgradeState upgrade, bool temporary)
        {
            if (!upgrade.GetTriggerUpgrades().Any(t => Key(t) != null)) return;
            var present = new HashSet<string>();
            void Remember(IEnumerable<CharacterTriggerData> triggers)
            {
                foreach (var trigger in triggers)
                    if (Key(trigger) is string key) present.Add(key);
            }
            var character = card.GetSpawnCharacterData();
            if (!card.IsPurified && character != null)
                Remember(character.GetTriggers());
            foreach (var existing in card.GetCardStateModifiers().GetCardUpgrades())
                Remember(existing.GetTriggerUpgrades());
            // A temporary source must not prevent granting Social permanently.
            if (temporary)
                foreach (var existing in card.GetTemporaryCardStateModifiers().GetCardUpgrades())
                    Remember(existing.GetTriggerUpgrades());

            var copy = new CardUpgradeState();
            copy.Setup(upgrade);
            copy.GetTriggerUpgrades().RemoveAll(t => Key(t) is string key && !present.Add(key));
            // Never mutate a shared upgrade: the next target may still need Social.
            upgrade = copy;
        }

        internal static bool CanAdd(CharacterState character, CharacterTriggerData trigger)
        {
            string? key = Key(trigger);
            return key == null || !character.GetTriggers().Any(t => Key(t.GetTriggerData()) == key);
        }
    }

    [HarmonyPatch(typeof(CardState), nameof(CardState.ApplyTemporaryUpgrade))]
    internal static class SocialTemporaryUpgradePatch
    {
        private static void Prefix(CardState __instance, ref CardUpgradeState upgradeState)
            => SocialDoesNotStack.Filter(__instance, ref upgradeState, temporary: true);
    }

    [HarmonyPatch(typeof(CardState), nameof(CardState.ApplyPermanentUpgrade))]
    internal static class SocialPermanentUpgradePatch
    {
        private static void Prefix(CardState __instance, ref CardUpgradeState upgradeState)
            => SocialDoesNotStack.Filter(__instance, ref upgradeState, temporary: false);
    }

    // Also protects spawning from older saves containing duplicate upgrades.
    [HarmonyPatch(typeof(CharacterState), "AddNewCharacterTriggerState")]
    internal static class SocialSpawnTriggerPatch
    {
        private static bool Prefix(CharacterState __instance, CharacterTriggerData triggerData)
            => SocialDoesNotStack.CanAdd(__instance, triggerData);
    }

    [HarmonyPatch(typeof(CharacterState), nameof(CharacterState.AddTrigger))]
    internal static class SocialAddedTriggerPatch
    {
        private static bool Prefix(CharacterState __instance, CharacterTriggerData triggerData)
            => SocialDoesNotStack.CanAdd(__instance, triggerData);
    }
}

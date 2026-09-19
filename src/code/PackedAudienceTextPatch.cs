using HarmonyLib;

namespace mt2_equestrian.Plugin
{
    // Packed Audience shows the shared pony description through its first spawn effect.
    // Keep the remaining spawn effects unchanged, including relics and summon ordering.
    [HarmonyPatch(typeof(CardEffectSpawnMonster), nameof(CardEffectSpawnMonster.GetCardText))]
    internal static class PackedAudienceTextPatch
    {
        private static bool Prefix(CardEffectState cardEffectState, ref string __result)
        {
            if (cardEffectState.GetParamStr() != "Equestrian.PackedAudience.HideRepeatedText") return true;
            __result = string.Empty;
            return false;
        }
    }
}

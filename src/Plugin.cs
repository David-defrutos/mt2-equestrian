using BepInEx;
using BepInEx.Logging;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Extensions;

namespace mt2_equestrian.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);

        public void Awake()
        {
            Logger = base.Logger;

            var builder = Railhead.GetBuilder();
            builder.Configure(
                MyPluginInfo.PLUGIN_GUID,
                c =>
                {
                    // TODA ruta JSON nueva tiene que aparecer aqui y hay que RECOMPILAR:
                    // el DLL lleva dentro la lista de ficheros que carga.
                    // Ver docs\64-anadir-json-nuevos-y-recompilar.md
                    c.AddMergedJsonFile(
                        "json/plugin.json",

                        // Clase
                        "json/class.json",

                        // Campeones
                        "json/champions/champion_MareaLee.json",
                        "json/champions/champion_Tantabus.json",

                        // Unidades
                        "json/units/unit_TrashPanda.json",
                        "json/units/unit_Carrot.json",
                        "json/units/unit_YoLo.json",
                        "json/units/unit_Finchy.json",
                        "json/units/unit_CrunchieMunchie.json",
                        "json/units/unit_MistyStep.json",
                        "json/units/unit_Snackasmacky.json",
                        "json/units/unit_SqueakyBooBoo.json",
                        "json/units/unit_StaticJoy.json",
                        "json/units/unit_BackgroundPony.json",
                        "json/units/unit_GuardianOfTheGates.json",
                        "json/units/unit_HeartsDesire.json",
                        "json/units/unit_LordOfEmber.json",
                        "json/units/unit_MareYouKnow.json",
                        "json/units/unit_MissingMare.json",
                        "json/units/unit_PoisonJoke.json",
                        "json/units/unit_PreenySnuggle.json",
                        "json/units/unit_TavernAce.json",

                        // Hechizos
                        "json/spells/card_Alicornification.json",
                        "json/spells/card_AppleCider.json",
                        "json/spells/card_BlankFlank.json",
                        "json/spells/card_BuckOff.json",
                        "json/spells/card_ChangelingInfiltrator.json",
                        "json/spells/card_Checklist.json",
                        "json/spells/card_DressedToKill.json",
                        "json/spells/card_EquestrianRailspike.json",
                        "json/spells/card_FanClub.json",
                        "json/spells/card_FirstAid.json",
                        "json/spells/card_Interrogation.json",
                        "json/spells/card_NightTerrors.json",
                        "json/spells/card_PackedAudience.json",
                        "json/spells/card_PartyCannon.json",
                        "json/spells/card_PartyInvitation.json",
                        "json/spells/card_PastryWarfare.json",
                        "json/spells/card_RainbowPower.json",
                        "json/spells/card_Reserves.json",
                        "json/spells/card_SecondChance.json",
                        "json/spells/card_Shenanigans.json",
                        "json/spells/card_SpaTreatment.json",
                        "json/spells/card_SpontaneousSongAndDance.json",
                        "json/spells/card_TheElementsOfHarmony.json",
                        "json/spells/card_TimeToShine.json",
                        "json/spells/card_Tom.json",
                        "json/spells/card_VIPList.json",
                        "json/spells/card_WinterWrapUp.json",
                        // Reliquias
                        "json/relics/relic_AChildsDrawing.json",
                        "json/relics/relic_ACollectionOfRibbons.json",
                        "json/relics/relic_Bloomberg.json",
                        "json/relics/relic_BottledCutieMark.json",
                        "json/relics/relic_ImaginaryFriends.json",
                        "json/relics/relic_JunkFood.json",
                        "json/relics/relic_MareInTheMoon.json",
                        "json/relics/relic_MysteriousGoldenRod.json",
                        "json/relics/relic_TheSecondSeventhElement.json",
                        "json/relics/relic_TheSeventhElement.json",
                        "json/relics/relic_TinyMouseCrutches.json",
                        "json/relics/relic_TornLapelPin.json",

                        // Potenciadores
                        "json/enhancers/enhancer_Friendstone.json",
                        "json/enhancers/enhancer_Gradstone.json",
                        "json/enhancers/enhancer_Playstone.json"
                    );
                }
            );

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}

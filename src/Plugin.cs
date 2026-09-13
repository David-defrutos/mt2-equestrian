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

                        // Hechizos
                        "json/spells/card_AppleCider.json"
                    );
                }
            );

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}

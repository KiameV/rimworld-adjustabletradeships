using System;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace AdjustableTradeShips
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.modifyresearchtime.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message(
                "AdjustableTradeShips Harmony Patches:" + Environment.NewLine +
                "  Prefix:" + Environment.NewLine +
                "    Game.InitNewGame" + Environment.NewLine +
                "  Postfix:" + Environment.NewLine +
                "    Storyteller.Notify_DefChanged");
        }
    }

    [HarmonyPatch(typeof(Game), "InitNewGame")]
    static class Patch_Game_InitNewGame
    {
        static void Prefix()
        {
#if DEBUG
            Log.Warning("Patch_Game_InitNewGame Postfix");
#endif
            WorldComp.Initialize();
        }
    }

    [HarmonyPatch(typeof(Storyteller), "Notify_DefChanged")]
    static class Patch_Storyteller_Notify_DefChanged
    {
        static void Postfix()
        {
            WorldComp.Initialize();
        }
    }
}

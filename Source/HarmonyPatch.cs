using Harmony;
using System.Reflection;
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

            Log.Message("AdjustableTradeShips: Adding Harmony Postfix to Game.InitNewGame");
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
            if (StoryTellerUtil.HasOrbitalTraders())
            {
                Settings.GameMTBOT = Settings.GlobalMTBOT;
                StoryTellerUtil.ApplyMTBOT(Settings.GameMTBOT);
            }
        }
    }
}

using HarmonyLib;
using LuckyTower;
using TMPro;

namespace LTUSpeedrunMod.Patches
{
    [HarmonyPatch(typeof(DebugCanvas))]
    internal class DebugCanvasPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void canvasUpdate(ref TMP_Text ___m_combat)
        {
            ___m_combat.text += $"\n\nRun: {GameManager.Instance.RunCounter} / Timer: {GameManager.Instance.RunTimer.Elapsed.ToString(@"hh\:mm\:ss\.fff")}";
        }
    }
}

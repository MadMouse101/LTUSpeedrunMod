using HarmonyLib;

namespace LTUSpeedrunMod.Patches
{
    [HarmonyPatch(typeof(DebugCanvas))]
    internal class F9Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void debugDefaultEnable(ref UnityEngine.GameObject ___m_parentObject)
        {
            ___m_parentObject.SetActive(!___m_parentObject.activeSelf);
        }
    }
}

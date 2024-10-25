using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LTUSpeedrunMod.Patches;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LTUSpeedrunMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class SpeedrunModBase : BaseUnityPlugin
    {
        // bepinex setup
        private const string modGUID = "Mad.LTUSpeedrunMod";
        private const string modName = "LTU Speedrun Mod";
        private const string modVersion = "1.0.1";

        private Harmony harmony = new Harmony(modGUID);

        // config setup
        public static ConfigEntry<bool> overhaulManager;
        public static ConfigEntry<bool> alwaysOnF9Patch;
        public static ConfigEntry<KeyboardShortcut> restartManager;

        // log setup
        public static ManualLogSource mls;

        private void Awake()
        {
            // log
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("LTUSpeedrunMod loaded.");

            // config binds
            overhaulManager = Config.Bind("Toggles", "Additional stats", false, "Turns on Run № and ingame timer.");
            alwaysOnF9Patch = Config.Bind("Toggles", "F9 always on", false, "F9 turned on by default.");
            restartManager = Config.Bind("Buttons", "Fast restart button", new KeyboardShortcut(KeyCode.F2));

            // initalize patches
            harmony.PatchAll(typeof(SpeedrunModBase));
            ApplyPatches();

            // patch/unpatch state
            overhaulManager.SettingChanged += overhaulManagerChanged;
            alwaysOnF9Patch.SettingChanged += alwaysOnF9Changer;
        }

        // restart manager key press check
        private void Update()
        {
            if (SpeedrunModBase.restartManager.Value.IsDown())
            {
                SceneManager.LoadSceneAsync("MasterScene");
            }
        }

        // apply on init
        private void ApplyPatches()
        {
            if (overhaulManager.Value)
            {
                harmony.PatchAll(typeof(DebugCanvasPatch));
                mls.LogInfo("Additional stats patch applied.");
            }

            if (alwaysOnF9Patch.Value)
            {
                harmony.PatchAll(typeof(F9Patch));
                mls.LogInfo("F9 always on patch applied.");
            }
        }

        // overhaul patch on/off
        private void overhaulManagerChanged(object sender, System.EventArgs e)
        {
            if (overhaulManager.Value)
            {
                harmony.PatchAll(typeof(DebugCanvasPatch));
                mls.LogInfo("Overhaul patch enabled.");
            }
            else
            {
                var original = AccessTools.Method(typeof(DebugCanvas), "Update");
                harmony.Unpatch(original, HarmonyPatchType.Postfix, modGUID);
                mls.LogInfo("Overhaul patch disabled.");
            }
        }

        // f9 patch on/off
        private void alwaysOnF9Changer(object sender, System.EventArgs e)
        {
            if (alwaysOnF9Patch.Value)
            {
                harmony.PatchAll(typeof(F9Patch));
                mls.LogInfo("F9 patch enabled.");
            }
            else
            {
                var original = AccessTools.Method(typeof(DebugCanvas), "Start");
                harmony.Unpatch(original, HarmonyPatchType.Postfix, modGUID);
                mls.LogInfo("F9 patch disabled.");
            }
        }

        // unpatch all on exit
        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}

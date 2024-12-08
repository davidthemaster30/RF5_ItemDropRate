using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;

namespace RF5ItemDropRate;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess(GameProcessName)]
public class RF5ItemDropRate : BasePlugin
{
    private const string PluginConfigSection = "RF5 Item Drop Rate Modifier";
    private const string GameProcessName = "Rune Factory 5.exe";

    internal static readonly ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("ItemDropRate");

    internal void LoadConfig()
    {
        ItemDropRatePatch.MonsterItemDrops = Config.Bind(PluginConfigSection, nameof(ItemDropRatePatch.MonsterItemDrops), 5, new ConfigDescription("Base monster item drop rate multiplier.", new AcceptableValueRange<int>(1, 1000)));
        ItemDropRatePatch.MonsterItemDrops.SettingChanged += ItemDropRatePatch.OnSettingChanged;
        ItemDropRatePatch.MonsterBonusItemDrops = Config.Bind(PluginConfigSection, nameof(ItemDropRatePatch.MonsterBonusItemDrops), 5, new ConfigDescription("Bonus monster item drop rate multiplier.", new AcceptableValueRange<int>(1, 1000)));
        ItemDropRatePatch.MonsterBonusItemDrops.SettingChanged += ItemDropRatePatch.OnSettingChanged;
        ItemDropRatePatch.MonsterSealItemDrops = Config.Bind(PluginConfigSection, nameof(ItemDropRatePatch.MonsterSealItemDrops), 5, new ConfigDescription("Sealed monster item drop rate multiplier.", new AcceptableValueRange<int>(1, 1000)));
        ItemDropRatePatch.MonsterSealItemDrops.SettingChanged += ItemDropRatePatch.OnSettingChanged;
    }

    public override void Load()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loading!");

        LoadConfig();
        Harmony.CreateAndPatchAll(typeof(ItemDropRatePatch));

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loaded!");
    }
}
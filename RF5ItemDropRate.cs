using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;

namespace RF5ItemDropRate;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInProcess(GameProcessName)]
public class RF5ItemDropRate : BasePlugin
{
    #region PluginInfo
    private const string PluginGUID = "RF5ItemDropRate";
    private const string PluginName = "RF5ItemDropRate";
    private const string PluginConfigSection = "RF5 Item Drop Rate Modifier";
    private const string PluginVersion = "1.0.0";
    private const string GameProcessName = "Rune Factory 5.exe";
    #endregion

    internal static readonly ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("ItemDropRate");

    internal static void LoadConfig(ConfigFile Config)
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
        // Plugin startup logic
        Log.LogInfo($"Plugin {PluginName} is loaded!");

        // Config
        LoadConfig(Config);

        Harmony.CreateAndPatchAll(typeof(ItemDropRatePatch));
    }
}
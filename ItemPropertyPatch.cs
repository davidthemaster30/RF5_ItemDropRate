using BehaviorDesigner.Runtime.Tasks;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace RF5ItemDropRate;

[HarmonyPatch]
internal static class ItemDropRatePatch
{
    internal static ConfigEntry<int> MonsterItemDrops;
    internal static readonly Dictionary<MonsterDropItemDataID, int> LastMonsterItemDrops = new Dictionary<MonsterDropItemDataID, int>();
    internal static ConfigEntry<int> MonsterBonusItemDrops;
    internal static readonly Dictionary<MonsterDropItemDataID, int> LastMonsterBonusItemDrops = new Dictionary<MonsterDropItemDataID, int>();
    internal static ConfigEntry<int> MonsterSealItemDrops;
    internal static readonly Dictionary<MonsterDropItemDataID, int> LastMonsterSealItemDrops = new Dictionary<MonsterDropItemDataID, int>();

    internal static void OnSettingChanged(object sender, EventArgs e)
    {
        var eventArgs = e as SettingChangedEventArgs;
        RF5ItemDropRate.Log.LogInfo($"{eventArgs?.ChangedSetting.Definition} Setting has changed.");

        if (MonsterDropItemDataTable._MonsterDropItemDataTableArray?.DataTables is null)
        {
            //Change was before data was loaded (on title screen?)
            return;
        }

        //force update toggle on all
        foreach (var key in LastMonsterItemDrops.Keys)
        {
            //force update
            _ = MonsterDropItemDataTable.GetDataTable(key);
        }
    }

    [HarmonyPatch(typeof(MonsterDropItemDataTable), nameof(MonsterDropItemDataTable.GetDataTable))]
    [HarmonyPostfix]
    internal static void MonsterDropAugment(MonsterDropItemDataID dataID, ref MonsterDropItemDataTable __result)
    {
        RF5ItemDropRate.Log.LogInfo($"MonsterDropItemDataTable.GetDataTable dataID {dataID}");
        bool wasUpdated = LastMonsterItemDrops.TryGetValue(dataID, out int lastItemDropRate);
        RF5ItemDropRate.Log.LogInfo($"Drops: wasUpdated {wasUpdated} dataID {dataID} lastItemDropRate {lastItemDropRate} MonsterItemDrops.Value {MonsterItemDrops.Value}");
        if (MonsterItemDrops.Value != lastItemDropRate)
        {
            for (int i = 0; i < __result.DropItemParamList.Count; i++)
            {
                var dropItem = __result.DropItemParamList[i];
                PrintNameAndDropRate(dropItem, MonsterItemDrops.Value);

                dropItem.DropPercent = wasUpdated ? dropItem.DropPercent / lastItemDropRate * MonsterItemDrops.Value : dropItem.DropPercent * MonsterItemDrops.Value;
                __result.DropItemParamList[i] = dropItem;
            }

            LastMonsterItemDrops[dataID] = MonsterItemDrops.Value;
        }
        else{

        }

        wasUpdated = LastMonsterBonusItemDrops.TryGetValue(dataID, out lastItemDropRate);
        RF5ItemDropRate.Log.LogInfo($"Bonus Drops: wasUpdated {wasUpdated} lastItemDropRate {lastItemDropRate} MonsterItemDrops.Value {MonsterBonusItemDrops.Value}");
        if (MonsterBonusItemDrops.Value != lastItemDropRate)
        {
            for (int i = 0; i < __result.BonusDropItemParamList.Count; i++)
            {
                var dropItem = __result.BonusDropItemParamList[i];
                PrintNameAndDropRate(dropItem, MonsterItemDrops.Value);

                dropItem.DropPercent = wasUpdated ? dropItem.DropPercent / lastItemDropRate * MonsterItemDrops.Value : dropItem.DropPercent * MonsterItemDrops.Value;
                __result.BonusDropItemParamList[i] = dropItem;
            }

            LastMonsterBonusItemDrops[dataID] = MonsterBonusItemDrops.Value;
        }

        wasUpdated = LastMonsterSealItemDrops.TryGetValue(dataID, out lastItemDropRate);
        RF5ItemDropRate.Log.LogInfo($"Seal Drops: wasUpdated {wasUpdated} lastItemDropRate {lastItemDropRate} MonsterItemDrops.Value {MonsterSealItemDrops.Value}");
        if (MonsterSealItemDrops.Value != lastItemDropRate)
        {
            for (int i = 0; i < __result.HandcuffsDropItemParamList.Count; i++)
            {
                var dropItem = __result.HandcuffsDropItemParamList[i];
                PrintNameAndDropRate(dropItem, MonsterItemDrops.Value);

                dropItem.DropPercent = wasUpdated ? dropItem.DropPercent / lastItemDropRate * MonsterItemDrops.Value : dropItem.DropPercent * MonsterItemDrops.Value;
                __result.HandcuffsDropItemParamList[i] = dropItem;
            }

            LastMonsterSealItemDrops[dataID] = MonsterSealItemDrops.Value;
        }
    }

    internal static void PrintNameAndDropRate(DropItemParam item, int rate)
    {
        RF5ItemDropRate.Log.LogInfo($"{ItemDataTable.GetDataTable(item.ItemID).ScreenName} Old Drop Rate : {string.Format("{0:F}", item.DropPercent / 10.0)}% New Drop Rate : {string.Format("{0:F}", item.DropPercent * rate / 10.0)}%");
    }
}

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
        bool wasUpdated = LastMonsterItemDrops.TryGetValue(dataID, out int lastItemDropRate);
        RF5ItemDropRate.Log.LogInfo($"wasUpdated {wasUpdated} dataID {dataID} lastItemDropRate {lastItemDropRate} MonsterItemDrops.Value {MonsterItemDrops.Value}");
        if (MonsterItemDrops.Value != lastItemDropRate)
        {
            Il2CppSystem.Collections.Generic.List<DropItemParam> list = new Il2CppSystem.Collections.Generic.List<DropItemParam>();
            
            foreach (var dropItem in __result.DropItemParamList.ToArray())
            {
                PrintNameAndDropRate(dropItem, MonsterItemDrops.Value);

                dropItem.DropPercent = 100;
                list.Add(new DropItemParam()
                {
                    ItemID = dropItem.ItemID,
                    DropPercent = wasUpdated ? dropItem.DropPercent / lastItemDropRate * MonsterItemDrops.Value : dropItem.DropPercent * MonsterItemDrops.Value,
                });
            }

            __result.DropItemParamList = list;
            LastMonsterItemDrops[dataID] = MonsterItemDrops.Value;
        }

        wasUpdated = LastMonsterBonusItemDrops.TryGetValue(dataID, out lastItemDropRate);
        if (MonsterBonusItemDrops.Value != lastItemDropRate)
        {
            Il2CppSystem.Collections.Generic.List<DropItemParam> list = new Il2CppSystem.Collections.Generic.List<DropItemParam>();
            foreach (var dropItem in __result.BonusDropItemParamList.ToArray())
            {
                PrintNameAndDropRate(dropItem, MonsterItemDrops.Value);

                list.Add(new DropItemParam()
                {
                    ItemID = dropItem.ItemID,
                    DropPercent = wasUpdated ? dropItem.DropPercent / lastItemDropRate * MonsterItemDrops.Value : dropItem.DropPercent * MonsterItemDrops.Value,
                });
            }

            __result.BonusDropItemParamList = list;
            LastMonsterBonusItemDrops[dataID] = MonsterBonusItemDrops.Value;
        }

        wasUpdated = LastMonsterSealItemDrops.TryGetValue(dataID, out lastItemDropRate);
        if (MonsterSealItemDrops.Value > 1)
        {
            Il2CppSystem.Collections.Generic.List<DropItemParam> list = new Il2CppSystem.Collections.Generic.List<DropItemParam>();
            foreach (var dropItem in __result.HandcuffsDropItemParamList.ToArray())
            {
                PrintNameAndDropRate(dropItem, MonsterItemDrops.Value);

                list.Add(new DropItemParam()
                {
                    ItemID = dropItem.ItemID,
                    DropPercent = wasUpdated ? dropItem.DropPercent / lastItemDropRate * MonsterItemDrops.Value : dropItem.DropPercent * MonsterItemDrops.Value,
                });
            }

            __result.HandcuffsDropItemParamList = list;
            LastMonsterSealItemDrops[dataID] = MonsterSealItemDrops.Value;
        }
    }

    internal static void PrintNameAndDropRate(DropItemParam item, int rate)
    {
        RF5ItemDropRate.Log.LogInfo($"{ItemDataTable.GetDataTable(item.ItemID).ScreenName} Old Drop Rate : {string.Format("{0:F}", item.DropPercent / 10.0)}% New Drop Rate : {string.Format("{0:F}", item.DropPercent * rate / 10.0)}%");
    }
}
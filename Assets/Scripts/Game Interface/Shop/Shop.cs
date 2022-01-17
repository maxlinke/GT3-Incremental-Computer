using System.Collections;
using System.Collections.Generic;
using Cores;
using Cores.Components;
using UnityEngine;
using System.Linq;
using Shops.Items;

namespace Shops {

    public class Shop : ScriptableObject {

        public const string CAT_CORE_UNLOCKS = "Core Unlocks";
        public const string CAT_PROCESSORS = "Processors";
        public const string CAT_SCHEDULERS = "Schedulers";
        public const string CAT_COOLERS = "Coolers";

        public const string CMD_CAT_CORE = "core";
        public const string CMD_CAT_PROC = "proc";
        public const string CMD_CAT_SCHED = "sched";
        public const string CMD_CAT_COOLER = "cooler";

        public static string GetUpgradeSuffixForLevel (int level) {
            return new string('*', level);
        }

        private static readonly IEnumerable<Item> emptyItemList = new List<Item>();

        [Header("Cores")]
        [SerializeField] int m_firstUnlockableCoreCost = 100;
        [SerializeField] int m_eachNewUnlockableCoreCostFactor = 10;

        [SerializeField] ProcessorPurchase[] m_processorPurchases;
        [SerializeField] SchedulerPurchase[] m_schedulerPurchases;

        public int firstUnlockableCoreCost => m_firstUnlockableCoreCost;
        public int eachNewUnlockableCoreCostFactor => m_eachNewUnlockableCoreCostFactor;
        public IEnumerable<ProcessorPurchase> processorPurchases => m_processorPurchases;
        public IEnumerable<SchedulerPurchase> schedulerPurchases => m_schedulerPurchases;

        public IEnumerable<string> categories { get {
            yield return CAT_CORE_UNLOCKS;
            yield return CAT_PROCESSORS;
            yield return CAT_SCHEDULERS;
            yield return CAT_COOLERS;
        } }

        public string GetCategoryCommand (string category) {
            switch(category){
                case CAT_CORE_UNLOCKS:
                case CAT_PROCESSORS:
                case CAT_SCHEDULERS:
                case CAT_COOLERS:
                    return Commands.Command.buyCommandId;
                default:
                    Debug.LogError($"Unknown category \"{category}\"!");
                    return "???";
            }
        }

        public IEnumerable<Item> GetItemsInCategory (string category) {
            switch(category){
                case CAT_CORE_UNLOCKS:
                    return CoreUnlock.allUnlocks;
                case CAT_PROCESSORS:
                    return processorPurchases;
                case CAT_SCHEDULERS:
                    return schedulerPurchases;
                case CAT_COOLERS:
                    // TODO these probably feed from serialized lists, so i can easier tweak stuff
                    return emptyItemList;
                default:
                    Debug.LogError($"Unknown category \"{category}\"!");
                    return emptyItemList;
            }
        }

        public IEnumerable<string> itemNamesForCommands { get {
            yield return CMD_CAT_CORE;
            foreach(var procPurchase in processorPurchases){
                yield return procPurchase.name;
            }
            foreach(var schedPurchase in schedulerPurchases){
                yield return schedPurchase.name;
            }
        } }

        public void EnsureInitialized () {
            CoreUnlock.EnsureListInitialized(this);
            SetNames(CoreUnlock.allUnlocks, CMD_CAT_CORE);
            SetNames(m_processorPurchases, CMD_CAT_PROC);
            Processor.Level.EnsureLevelsInitialized(this);
            SetNames(m_schedulerPurchases, CMD_CAT_SCHED);
            Scheduler.Level.EnsureLevelsInitialized(this);

            void SetNames (IReadOnlyList<BuyItem> items, string prefix) {
                for(int i=0; i<items.Count; i++){
                    var compPurchase = items[i];
                    compPurchase.SetName($"{prefix}{i}");
                }
            }
        }

        public bool TryGetBuyItemForCommand (string itemName, out BuyItem item) {
            itemName = itemName.ToLower();
            if(TryGetBuyItem(CMD_CAT_CORE, CoreUnlock.allUnlocks, out item)){
                return true;
            }
            if(TryGetBuyItem(CMD_CAT_PROC, processorPurchases, out item)){
                return true;
            }
            if(TryGetBuyItem(CMD_CAT_SCHED, schedulerPurchases, out item)){
                return true;
            }
            item = default;
            return false;

            bool TryGetBuyItem (string prefix, IEnumerable<BuyItem> items, out BuyItem output) {
                if(itemName.StartsWith(prefix)){
                    output = items.Where(compPurchase => compPurchase.name == itemName).FirstOrDefault();
                    return output != null;
                }
                output = default;
                return false;
            }
        }

        public bool TryGetUpgradeItemForCommand (string itemName, out UpgradeItem item) {
            // if startswith cmd-cat-core
            // if startswith task
            // otherwise look for the component with the id
            // maybe i should make a dictionary for this? nah, i can just scan the cores, it's not that many...
            // if component is processor
            // if component is scheduler
            // ...
            // TODO
            // luckily, since i know which list contains what stuff, i can just do this the easy way
            Debug.Log("TODO");
            item = default;
            return false;
        }

        public bool TryGetBuyItemForComponent (CoreComponent component, out BuyItem item) {
            if(component is Processor){
                item = m_processorPurchases[component.levelIndex];
                return true;
            }else if(component is Scheduler){
                item = m_schedulerPurchases[component.levelIndex];
                return true;
            // }else if(component is Cooler){

            }
            item = default;
            return false;
        }

    }

}
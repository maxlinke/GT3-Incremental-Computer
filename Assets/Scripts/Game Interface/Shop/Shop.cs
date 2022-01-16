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
            SetNames(m_processorPurchases, CMD_CAT_PROC);
            Processor.Level.EnsureLevelsInitialized(this);
            SetNames(m_schedulerPurchases, CMD_CAT_SCHED);
            Scheduler.Level.EnsureLevelsInitialized(this);

            void SetNames (IList<ComponentPurchase> compPurchases, string prefix) {
                for(int i=0; i<compPurchases.Count; i++){
                    var compPurchase = compPurchases[i];
                    compPurchase.SetName($"{prefix}{i}");
                }
            }
        }

        public bool TryGetItemForCommand (string itemName, Core core, out Item item) {
            itemName = itemName.ToLower();
            if(itemName == CMD_CAT_CORE){
                item = CoreUnlock.allUnlocks[core.index];
                return true;
            }
            if(TryGetComponentPurchase(CMD_CAT_PROC, processorPurchases, out item)){
                return true;
            }
            if(TryGetComponentPurchase(CMD_CAT_SCHED, schedulerPurchases, out item)){
                return true;
            }
            item = default;
            return false;

            bool TryGetComponentPurchase (string prefix, IEnumerable<ComponentPurchase> compPurchases, out Item output) {
                if(itemName.StartsWith(prefix)){
                    output = compPurchases.Where(compPurchase => compPurchase.name == itemName).FirstOrDefault();
                    return output != null;
                }
                output = default;
                return false;
            }
        }

        public bool TryGetItemForComponent (CoreComponent component, out Item item) {
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
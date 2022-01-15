using System.Collections;
using System.Collections.Generic;
using Cores;
using Cores.Components;
using UnityEngine;
using System.Linq;

namespace Shops {

    [CreateAssetMenu]
    public class ComponentShop : Shop {

        public const string CAT_CORE_UNLOCKS = "Core Unlocks";
        public const string CAT_PROCESSORS = "Processors";
        public const string CAT_SCHEDULERS = "Schedulers";
        public const string CAT_COOLERS = "Coolers";

        const string CMD_CAT_CORE = "core";
        const string CMD_CAT_PROC = "proc";

        [Header("Cores")]
        [SerializeField] int m_firstUnlockableCoreCost = 100;
        [SerializeField] int m_eachNewUnlockableCoreCostFactor = 10;

        [SerializeField] ProcessorPurchase[] m_processorPurchases;

        public override string displayName => "Components";

        public IEnumerable<ProcessorPurchase> processorPurchases => m_processorPurchases;

        public override IEnumerable<string> categories { get {
            yield return CAT_CORE_UNLOCKS;
            yield return CAT_PROCESSORS;
            yield return CAT_SCHEDULERS;
            yield return CAT_COOLERS;
        } }

        public override IEnumerable<Item> GetItemsInCategory (string category) {
            switch(category){
                case CAT_CORE_UNLOCKS:
                    return CoreUnlock.allUnlocks;
                case CAT_PROCESSORS:
                    return processorPurchases;
                case CAT_SCHEDULERS:
                case CAT_COOLERS:
                    // TODO these probably feed from serialized lists, so i can easier tweak stuff
                    return emptyItemList;
                default:
                    Debug.LogError($"Unknown category \"{category}\"!");
                    return emptyItemList;
            }
        }

        public override IEnumerable<string> itemNamesForCommands { get {
            yield return CMD_CAT_CORE;
            foreach(var procPurchase in processorPurchases){
                yield return procPurchase.name;
            }
        } }

        public override void OnShopDisplayInitialized () {
            CoreUnlock.EnsureListInitialized(this);
            for(int i=0; i<m_processorPurchases.Length; i++){
                var procPurchase = m_processorPurchases[i];
                procPurchase.SetName($"{CMD_CAT_PROC}{i}");
            }
            Processor.Level.EnsureLevelsInitialized(this);
        }

        public override bool TryGetItemForCommand (string itemName, Core core, out Item item) {
            itemName = itemName.ToLower();
            if(itemName == CMD_CAT_CORE){
                item = CoreUnlock.allUnlocks[core.index];
                return true;
            }
            if(itemName.StartsWith(CMD_CAT_PROC)){
                item = processorPurchases.Where(pp => pp.name == itemName).FirstOrDefault();
                return item != null;
            }
            item = default;
            return false;
        }

        public override bool TryGetItemForComponent (Cores.Components.CoreComponent component, out Item item) {
            if(component is Processor){
                item = m_processorPurchases[component.levelIndex];
                return true;
            }else if(component is Scheduler){

            // }else if(component is Cooler){

            }
            item = default;
            return false;
        }

        public class CoreUnlock : Item {

            public static void EnsureListInitialized (ComponentShop shop) {
                if(allUnlocks != null){
                    return;
                }
                var output = new List<CoreUnlock>();
                for(int i=0; i<CoreDisplay.NUMBER_OF_CORES; i++){
                    output.Add(new CoreUnlock(shop, i));
                }
                allUnlocks = output;
            }

            public static IReadOnlyList<CoreUnlock> allUnlocks { get; private set; }

            public override string name => m_name;
            public override int price => m_price;

            public override bool IsPurchaseableAtAll (out string message) {
                return PurchaseableForCore(GameState.current.cores[m_coreIndex], out message);
            }

            protected override bool PurchaseableForCore (Core targetCore, out string errorMessage) {
                if(GameState.current.cores[m_coreIndex].unlocked){
                    errorMessage = $"Core {m_coreIndex} is already unlocked.";
                    return false;
                }
                errorMessage = default;
                return true;
            }

            protected override void OnPurchased (Core targetCore) {
                targetCore.Unlock();
            }

            private readonly int m_coreIndex;
            private readonly string m_name;
            private readonly int m_price;

            private CoreUnlock (ComponentShop shop, int index) {
                this.m_coreIndex = index;
                this.m_name = $"{CMD_CAT_CORE} {index}";
                if(index == 0){
                    this.m_price = 0;
                }else{                    
                    this.m_price = shop.m_firstUnlockableCoreCost;
                    for(int i=1; i<index; i++){
                        this.m_price *= shop.m_eachNewUnlockableCoreCostFactor;
                    }
                }
            }

        }

        public abstract class ComponentPurchase : Item {

            protected abstract int slotSize { get; }

            public override bool IsPurchaseableAtAll (out string message) { 
                message = default;
                return true;
            }

            protected override bool PurchaseableForCore (Core targetCore, out string errorMessage) {
                if(!targetCore.unlocked){
                    errorMessage = $"Core {targetCore.index} isn't unlocked yet!";
                    return false;
                }
                if(targetCore.remainingSlots < slotSize){
                    errorMessage = $"Core {targetCore.index} doesn't have enough free slots, sell or move components to make room!";
                    return false;
                }
                errorMessage = default;
                return true;
            }

        }

        [System.Serializable] 
        public class ProcessorPurchase : ComponentPurchase {

            [SerializeField] int m_price;
            [SerializeField] Processor.Level m_actualLevel;

            public override string name => m_name;
            public override int price => m_price;
            public Processor.Level levelData => m_actualLevel;

            protected override int slotSize => levelData.slotSize;

            [System.NonSerialized] string m_name;
            public void SetName (string newName) => m_name = newName;

            protected override void OnPurchased (Core targetCore) {
                targetCore.AddProcessor(Processor.Level.LevelIndex(levelData));
            }

        }

    }

}
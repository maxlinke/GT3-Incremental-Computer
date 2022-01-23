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

        public struct Category {
            public string command;
            public string name;
            public IEnumerable<Item> items;
            public Category (string command, string name, IEnumerable<Item> items){
                this.command = command;
                this.name = name;
                this.items = items;
            }
        }

        private static readonly IEnumerable<Item> emptyItemList = new List<Item>();

        [Header("Cores")]
        [SerializeField] int m_firstUnlockableCoreCost = 100;
        [SerializeField] int m_eachNewUnlockableCoreCostFactor = 10;

        [SerializeField, InlineProperty] ProcessorPurchase[] m_processorPurchases;
        [SerializeField, InlineProperty] SchedulerPurchase[] m_schedulerPurchases;
        [SerializeField, InlineProperty] CoolerPurchase[] m_coolerPurchases;

        public int firstUnlockableCoreCost => m_firstUnlockableCoreCost;
        public int eachNewUnlockableCoreCostFactor => m_eachNewUnlockableCoreCostFactor;

        private IReadOnlyList<ComponentUpgrade<Processor>> processorUpgrades;

        public IEnumerable<Category> categories { get {
            yield return new Category(Commands.Command.buyCommandId, CAT_CORE_UNLOCKS, CoreUnlock.allUnlocks);
            yield return new Category(Commands.Command.buyCommandId, CAT_PROCESSORS, m_processorPurchases);
            yield return new Category(Commands.Command.buyCommandId, CAT_SCHEDULERS, m_schedulerPurchases);
            yield return new Category(Commands.Command.buyCommandId, CAT_COOLERS, m_coolerPurchases);
            yield return new Category(Commands.Command.upgradeCommandId, CAT_PROCESSORS, processorUpgrades);
        } }

        public void EnsureInitialized () {
            CoreUnlock.EnsureListInitialized(this);
            SetNames(CoreUnlock.allUnlocks, CMD_CAT_CORE);
            Processor.Level.EnsureLevelsInitialized(m_processorPurchases.Select(pp => pp.levelData));
            SetNames(m_processorPurchases, CMD_CAT_PROC);
            Scheduler.Level.EnsureLevelsInitialized(m_schedulerPurchases.Select(sp => sp.levelData));
            SetNames(m_schedulerPurchases, CMD_CAT_SCHED);
            Cooler.Level.EnsureLevelsInitialized(m_coolerPurchases.Select(cp => cp.levelData));
            SetNames(m_coolerPurchases, CMD_CAT_COOLER);
            processorUpgrades = GetUpgradeItems(m_processorPurchases);

            void SetNames (IReadOnlyList<BuyItem> items, string prefix) {
                for(int i=0; i<items.Count; i++){
                    var compPurchase = items[i];
                    compPurchase.SetName($"{prefix}{i}");
                }
            }

            IReadOnlyList<ComponentUpgrade<T>> GetUpgradeItems<T, U> (IReadOnlyList<IUpgradeSource<T, U>> upgradeSources) 
                where T : IUpgradeable<U>
                where U : IUpgrade
            {
                var output = new List<ComponentUpgrade<T>>();
                for(int i=0; i<upgradeSources.Count; i++){
                    var upgradeSource = upgradeSources[i];
                    output.Add(new UpgradeableComponentUpgrade<T, U>(
                        name: $"{typeof(T).Name} Level {(i+1)}",
                        typeName: typeof(T).Name,
                        upgrades: upgradeSource.upgrades,
                        price: upgradeSource.upgradeCost
                    ));
                }
                return output;
            }
        }

        public bool TryGetBuyItemForCommand (string itemName, out BuyItem item) {
            itemName = itemName.ToLower();
            // foreach(var category in categories){ // TODO refactor categories a little to make this possible
            //     if(category.command == Commands.Command.buyCommandId){
            //         if(category.
            //     }
            // }
            if(TryGetBuyItem(CMD_CAT_CORE, CoreUnlock.allUnlocks, out item)){
                return true;
            }
            if(TryGetBuyItem(CMD_CAT_PROC, m_processorPurchases, out item)){
                return true;
            }
            if(TryGetBuyItem(CMD_CAT_SCHED, m_schedulerPurchases, out item)){
                return true;
            }
            if(TryGetBuyItem(CMD_CAT_COOLER, m_coolerPurchases, out item)){
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

        public bool TryGetUpgradeItemForId (string id, out UpgradeItem item) {
            if(id.StartsWith(CMD_CAT_CORE)){
                // i can just directly return the thing and the item decides whether it's a valid index...

                // var coreIndexString = id.Remove(0, CMD_CAT_CORE.Length);
                // if(GameState.current.TryFindCoreForIndex(coreIndexString, out _)){
                //     // return core upgrade item
                // }
            }else if(GameState.current.TryFindComponentForId(id, out var foundComponent)){
                if(foundComponent is Processor processor){
                    item = processorUpgrades[processor.levelIndex];
                    return true;
                }else if(foundComponent is Scheduler scheduler){

                }else if(foundComponent is Cooler cooler){

                }
            }
            item = default;
            return false;
        }

        public bool TryGetBuyItemForComponent (CoreComponent component, out BuyItem item) {
            item = default;
            if(component is Processor){
                item = m_processorPurchases[component.levelIndex];
            }else if(component is Scheduler){
                item = m_schedulerPurchases[component.levelIndex];
            }else if(component is Cooler){
                item = m_coolerPurchases[component.levelIndex];
            }
            return (item != default);
        }

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shops {

    public abstract class Shop : ScriptableObject {

        public abstract class Item {

            public bool CurrentlyPurchaseable (out string message) {
                if(!IsPurchaseableAtAll(out message)){
                    return false;
                }
                if(GameState.current.currency < price){
                    message = $"Not enough {GameState.CURRENCY_SYMBOL}.";
                    return false;
                }
                message = default;
                return true;
            }

            public bool TryPurchase (Cores.Core targetCore, out string errorMessage) {
                if(!CurrentlyPurchaseable(out errorMessage)){
                    return false;
                }
                if(!PurchaseableForCore(targetCore, out errorMessage)){
                    return false;
                }
                GameState.current.currency -= price;
                OnPurchased(targetCore);
                return true;
            }

            public abstract bool IsPurchaseableAtAll (out string message);

            protected abstract bool PurchaseableForCore (Cores.Core targetCore, out string errorMessage);

            protected abstract void OnPurchased (Cores.Core targetCore);

            public abstract string name { get; }

            public abstract int price { get; }


        }

        protected static readonly IReadOnlyCollection<Item> emptyItemList = new List<Item>();

        public abstract string displayName { get; }

        public abstract IEnumerable<string> categories { get; }

        public abstract void OnShopDisplayInitialized ();

        public abstract IEnumerable<Item> GetItemsInCategory (string category);

        public IEnumerable<Item> items { get {
            foreach(var category in categories){
                foreach(var item in GetItemsInCategory(category)){
                    yield return item;
                }
            }
        } }

        public abstract IEnumerable<string> itemNamesForCommands { get; }

        public abstract bool TryGetItemForCommand (string itemName, Cores.Core core, out Item item);

    }

}
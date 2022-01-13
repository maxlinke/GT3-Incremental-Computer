using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shops {

    public abstract class Shop : ScriptableObject {

        public abstract class Item {

            public abstract bool CurrentlyPurchaseable (out string reasonWhyNot);

            public abstract bool TryPurchase (Cores.Core targetCore, out string errorMessage);

            public abstract string name { get; }

            public abstract int price { get; }

            public abstract bool isPurchaseableAtAll { get; }

        }

        protected static readonly IReadOnlyCollection<Item> emptyItemList = new List<Item>();

        public abstract string displayName { get; }

        public abstract IEnumerable<string> categories { get; }

        public abstract IEnumerable<Item> GetItemsInCategory (string category);

        public IEnumerable<Item> items { get {
            foreach(var category in categories){
                foreach(var item in GetItemsInCategory(category)){
                    yield return item;
                }
            }
        } }

    }

}
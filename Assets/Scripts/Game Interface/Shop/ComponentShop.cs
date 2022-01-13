using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shops {

    [CreateAssetMenu]
    public class ComponentShop : Shop {

        const string CAT_CORE_UNLOCKS = "Core Unlocks";
        const string CAT_PROCESSORS = "Processors";
        const string CAT_SCHEDULERS = "Schedulers";
        const string CAT_COOLERS = "Coolers";

        public override string displayName => "Components";

        public override IEnumerable<string> categories { get {
            yield return CAT_CORE_UNLOCKS;
            yield return CAT_PROCESSORS;
            yield return CAT_SCHEDULERS;
            yield return CAT_COOLERS;
        } }

        public override IEnumerable<Item> GetItemsInCategory (string category) {
            switch(category){
                case CAT_CORE_UNLOCKS:
                case CAT_PROCESSORS:
                case CAT_SCHEDULERS:
                case CAT_COOLERS:
                    return emptyItemList;
                default:
                    Debug.LogError($"Unknown category \"{category}\"!");
                    return emptyItemList;
            }
        }

    }

}
using System.Collections.Generic;
using Cores;

namespace Shops.Items {

    public abstract class ComponentUpgrade<T> : UpgradeItem {

        public abstract string targetTypeName { get; }

        public override bool TryPurchase (string targetId, out string errorMessage) {
            if(!CurrentlyPurchaseable(out errorMessage)){
                return false;
            }
            if(string.IsNullOrWhiteSpace(targetId)){
                errorMessage = $"Please specify what you want to upgrade. For components, specify their id.";
                return false;
            }
            if(!TryFindTargetForId(targetId, out var target)){
                errorMessage = $"Found no {targetTypeName} with id \"{targetId}\"!";
                return false;
            }
            if(!CanBeUpgraded(target)){
                errorMessage = $"{targetTypeName} \"{targetId}\" can't be upgraded!";
                return false;
            }
            GameState.current.currency -= price;
            OnPurchased(target);
            return true;
        }

        protected virtual bool TryFindTargetForId (string targetId, out T target) {
            if(GameState.current.TryFindComponentForId(targetId, out var foundComponent)){
                if(foundComponent is T foundT){
                    target = foundT;
                    return true;
                }
            }
            target = default;
            return false;
        }

        protected abstract bool CanBeUpgraded (T target);

        protected abstract void OnPurchased (T target);

    }

    public class UpgradeableComponentUpgrade<T, U> : ComponentUpgrade<T>
        where T : IUpgradeable<U>
        where U : IUpgrade
    {

        public UpgradeableComponentUpgrade (string name, string typeName, IEnumerable<U> upgrades, int price) : base () {
            m_price = price;
            var infoSb = new System.Text.StringBuilder();
            var i = 0;
            foreach(var upgrade in upgrades){
                if(i>0){
                    infoSb.AppendLine($"{i}{GameState.UPGRADE_SYMBOL} - {upgrade.description}");
                }
                i++;
            }
            m_maxLevel = i - 1;
            m_info = infoSb.ToString().Trim();
            m_name = name;
            m_targetTypeName = typeName;
        }

        int m_price;
        int m_maxLevel;
        string m_info;
        string m_name;
        string m_targetTypeName;

        public override string targetTypeName => m_targetTypeName;
        public override string name => m_name;
        public override int price => m_price;
        public override string info => m_info;

        protected override bool CanBeUpgraded (T target) => target.currentUpgradeLevel < m_maxLevel;

        protected override void OnPurchased (T target) => target.Upgrade();

    }

}
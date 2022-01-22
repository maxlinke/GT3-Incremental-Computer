using System.Collections.Generic;
using Cores;

namespace Shops.Items {

    public interface IUpgradeSource<T, U> where T : IUpgradeable<U> where U : IUpgrade {

        int upgradeCost { get; }
        IReadOnlyList<U> upgrades { get; }

    }

}
namespace Cores {

    public interface IUpgradeable<T> where T : IUpgrade {

        int currentUpgradeLevel { get; }

        void Upgrade ();

    }

}
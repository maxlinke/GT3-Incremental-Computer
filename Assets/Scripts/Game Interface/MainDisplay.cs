using UnityEngine;

public class MainDisplay : MonoBehaviour {

    private static MainDisplay instance;

    [SerializeField, RedIfEmpty] CoreDisplay m_coreDisplay;
    [SerializeField, RedIfEmpty] ShopDisplay m_shopDisplay;

    public static bool showingCores => instance.m_coreDisplay.visible;
    public static bool showingShop => instance.m_shopDisplay.visible;

    public void Initialize () {
        instance = this;
        m_shopDisplay.Initialize();
        m_coreDisplay.Initialize();
        GameState.onGameStateChanged += (_) => ShowCores();
        ShowCores();
    }

    public static void ShowCores () {
        instance.m_shopDisplay.visible = false;
        instance.m_coreDisplay.visible = true;
    }

    public static void ShowShop () {
        instance.m_coreDisplay.visible = false;
        instance.m_shopDisplay.visible = true;
    }

}

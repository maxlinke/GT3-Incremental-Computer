using UnityEngine;

public class MainDisplay : MonoBehaviour {

    private static MainDisplay instance;

    [SerializeField, RedIfEmpty] CoreDisplay m_coreDisplay;
    [SerializeField, RedIfEmpty] ShopDisplay m_shopDisplay;

    public void Initialize () {
        instance = this;
        m_shopDisplay.Initialize();
        m_coreDisplay.Initialize();
        GameState.onGameStateChanged += (_) => ShowCores();
        ShowCores();
    }

    public static void ShowCores () {
        instance.m_shopDisplay.visible = false;
    }

    public static void ShowShop () {
        instance.m_shopDisplay.visible = true;
    }

}

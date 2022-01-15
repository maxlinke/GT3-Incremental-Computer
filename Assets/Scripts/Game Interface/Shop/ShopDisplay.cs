using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Shops;

public class ShopDisplay : MonoBehaviour {

    private static ShopDisplay instance;

    [Header("Components")]
    [SerializeField, RedIfEmpty] Canvas m_canvas;
    [SerializeField, RedIfEmpty] Text m_headerText;
    [SerializeField, RedIfEmpty] Text m_pageText;

    [Header("Shops")]
    [SerializeField, RedIfEmpty] Shop m_shop;

    public bool visible {
        get => m_canvas.enabled;
        set {
            var changed = (value != visible);
            m_canvas.enabled = value;
            if(changed && visible){
                ShowPage(0);
            }
        }
    }

    public void Initialize () {
        instance = this;
        gameObject.SetActive(true);
        InputHandler.onScrollCommand
            .Where(_ => visible)
            .Where(dir => dir.x > 0)
            .Subscribe(_ => NextPage());
        InputHandler.onScrollCommand
            .Where(_ => visible)
            .Where(dir => dir.x < 0)
            .Subscribe(_ => PreviousPage());
        InputHandler.onCancel
            .Where(_ => visible)
            .Subscribe(_ => MainDisplay.ShowCores());
        m_shop.OnShopDisplayInitialized();
    }

    void ShowPage (int index) {

    }

    void NextPage () {
        // var currentIndex = GetIndexOfShop(currentShop);
        // var newIndex = (currentIndex + 1) % m_shops.Length;
        // ShowShop(m_shops[newIndex]);
    }

    void PreviousPage () {
        // var currentIndex = GetIndexOfShop(currentShop);
        // if(currentIndex == 0){
        //     ShowShop(m_shops[m_shops.Length - 1]);
        // }else{
        //     ShowShop(m_shops[currentIndex - 1]);
        // }
    }

    public static IEnumerable<string> GetCommandItemNames () => instance.m_shop.itemNamesForCommands;

    private static int GetPriceOfComponent (Cores.Components.CoreComponent component) {
        if(instance.m_shop.TryGetItemForComponent(component, out var item)){
            var output = item.price;
            // for(int i=0; i<component.upgrades; i++){
            //     // ???
            // }
            return output;
        }
        return 0;
    }

    public static bool OnBuyCommand (string itemName, Cores.Core targetCore, out string message) {
        if(instance.m_shop.TryGetItemForCommand(itemName, targetCore, out var item)){
            return item.TryPurchase(targetCore, out message);
        }
        message = $"\"{itemName}\" isn't a valid item name!";
        return false;
    }

    public static bool OnSellCommand (string id, out string message) {
        var targetComponent = default(Cores.Components.CoreComponent);
        foreach(var core in GameState.current.cores){
            foreach(var component in core.components){
                if(component.id.Equals(id)){
                    targetComponent = component;
                    break;
                }
            }
        }
        if(targetComponent != default){
            targetComponent.core.RemoveComponent(targetComponent);
            GameState.current.currency += GetPriceOfComponent(targetComponent);
            message = string.Empty;
            return true;
        }
        message = $"There is no component with the id \"{id}\"!";
        return false;
    }

}

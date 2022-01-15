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
    [SerializeField, RedIfEmpty] Shop[] m_shops;

    public bool visible {
        get => m_canvas.enabled;
        set {
            var changed = (value != visible);
            m_canvas.enabled = value;
            if(changed){
                if(visible && m_shops.Length > 0){
                    ShowShop(m_shops[0]);
                }else{
                    ShowShop(null);
                }
            }
        }
    }

    public Shop currentShop { get; private set; }

    int shopCount => m_shops.Length;

    int GetIndexOfShop (Shop shop) { 
        for(int i=0; i<m_shops.Length; i++){
            if(m_shops[i] == shop){
                return i;
            }
        }
        return -1;
    }

    public void Initialize () {
        instance = this;
        gameObject.SetActive(true);
        InputHandler.onScrollCommand
            .Where(_ => visible)
            .Where(dir => dir.x > 0)
            .Subscribe(_ => NextShop());
        InputHandler.onScrollCommand
            .Where(_ => visible)
            .Where(dir => dir.x < 0)
            .Subscribe(_ => PreviousShop());
        InputHandler.onCancel
            .Where(_ => visible)
            .Subscribe(_ => MainDisplay.ShowCores());
        foreach(var shop in m_shops){
            shop.OnShopDisplayInitialized();
        }
    }

    void ShowShop (Shop shop) {
        currentShop = shop;
        var shopName = (shop != null ? shop.displayName : "null");
        m_headerText.text = $"Shop - {shopName}";
        m_pageText.text = $"{GetIndexOfShop(shop) + 1}/{shopCount}";
        // clear all the things
        if(shop != null){
            // generate the things
            // they display their name
            // items do need a property "sellable" so i can print that next to the ones you can sell again
            // description of what a thing does? no. 
        }
    }

    void NextShop () {
        var currentIndex = GetIndexOfShop(currentShop);
        var newIndex = (currentIndex + 1) % m_shops.Length;
        ShowShop(m_shops[newIndex]);
    }

    void PreviousShop () {
        var currentIndex = GetIndexOfShop(currentShop);
        if(currentIndex == 0){
            ShowShop(m_shops[m_shops.Length - 1]);
        }else{
            ShowShop(m_shops[currentIndex - 1]);
        }
    }

    public static IEnumerable<string> GetCommandItemNames () {
        foreach(var shop in instance.m_shops){
            foreach(var itemName in shop.itemNamesForCommands){
                yield return itemName;
            }
        }
    }

    private int GetPriceOfComponent (Cores.Components.CoreComponent component) {
        foreach(var shop in m_shops){
            if(shop.TryGetItemForComponent(component, out var item)){
                var output = item.price;
                // for(int i=0; i<component.upgrades; i++){
                //     // ???
                // }
                return output;
            }
        }
        return 0;
    }

    public static bool OnBuyCommand (string itemName, Cores.Core targetCore, out string message) {
        foreach(var shop in instance.m_shops){
            if(shop.TryGetItemForCommand(itemName, targetCore, out var item)){
                return item.TryPurchase(targetCore, out message);
            }
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
            GameState.current.currency += instance.GetPriceOfComponent(targetComponent);
            message = string.Empty;
            return true;
        }
        message = $"There is no component with the id \"{id}\"!";
        return false;
    }

}

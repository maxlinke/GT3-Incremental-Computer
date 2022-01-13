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
    }

    void ShowShop (Shop shop) {
        currentShop = shop;
        var shopName = (shop != null ? shop.displayName : "null");
        m_headerText.text = $"Shop - {shopName}";
        m_pageText.text = $"{GetIndexOfShop(shop) + 1}/{shopCount}";
        // clear all the things
        if(shop != null){
            // generate the things
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

}

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

    [Header("Page Components")]
    [SerializeField, RedIfEmpty] RectTransform m_pagesParent;
    [SerializeField, RedIfEmpty] Text m_sectionHeaderTemplate;
    [SerializeField, RedIfEmpty] ItemDisplay m_itemDisplayTemplate;

    [Header("Shops")]
    [SerializeField, RedIfEmpty] Shop m_shop;

    [Header("Settings")]
    [SerializeField] float m_spaceBetweenSectionHeaderAndFirstItem;
    [SerializeField] float m_spaceBetweenItemDisplays;
    [SerializeField] float m_spaceBetweenSections;

    List<GameObject> m_pages;
    int m_shownPageIndex;

    public bool visible {
        get => m_canvas.enabled;
        set {
            var changed = (value != visible);
            m_canvas.enabled = value;
            if(changed){
                ShowPage(visible ? 0 : -1);
            }
        }
    }

    public void Initialize () {
        instance = this;
        gameObject.SetActive(true);
        m_canvas.enabled = true;
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
        m_headerText.text = "Shop";
        m_shop.EnsureInitialized();
        GeneratePages();
        ShowPage(0);
    }

    void ShowPage (int index) {
        for(int i=0; i<m_pages.Count; i++){
            m_pages[i].gameObject.SetActive(i == index);
        }
        m_shownPageIndex = index;
        m_pageText.text = $"{index + 1}/{m_pages.Count}";
    }

    void NextPage () {
        var newIndex = (m_shownPageIndex + 1) % m_pages.Count;
        ShowPage(newIndex);
    }

    void PreviousPage () {
        if(m_shownPageIndex == 0){
            ShowPage(m_pages.Count - 1);
        }else{
            ShowPage(m_shownPageIndex - 1);
        }
    }

    void GeneratePages () {
        m_pages = new List<GameObject>();
        m_sectionHeaderTemplate.SetGOActive(false);
        m_itemDisplayTemplate.SetGOActive(false);
        var currentPage = GetNewPage();
        var sectionY = 0f;
        var elementY = 0f;
        foreach(var category in m_shop.categories){
            var section = GetNewSection(category);
            foreach(var item in category.items){
                var newDisplay = Instantiate(m_itemDisplayTemplate, section);
                newDisplay.Initialize(item);
                newDisplay.rectTranform.anchoredPosition = new Vector2(0, elementY);
                elementY -= newDisplay.rectTranform.rect.height;
                elementY -= m_spaceBetweenItemDisplays;
            }
            section.SetHeight(Mathf.Abs(elementY));
            if((section.rect.height - sectionY) > currentPage.rect.height){
                currentPage = GetNewPage();
                section.SetParent(currentPage, false);
                section.anchoredPosition = Vector2.zero;
                sectionY = 0;
            }
            sectionY -= section.rect.height;
            sectionY -= m_spaceBetweenSections;
        }

        RectTransform GetNewPage () {
            var newPage = new GameObject($"Page {m_pages.Count + 1}", typeof(RectTransform)).transform as RectTransform;
            newPage.SetParent(m_pagesParent, false);
            newPage.anchorMin = Vector2.zero;
            newPage.anchorMax = Vector2.one;
            newPage.pivot = new Vector2(0.5f, 0.5f);
            newPage.sizeDelta = Vector2.zero;
            newPage.anchoredPosition = Vector2.zero;
            m_pages.Add(newPage.gameObject);
            return newPage;
        }

        RectTransform GetNewSection (Shop.Category category) {
            var sectionName = $"{category.command.ToLower()} {category.name.ToUpper()}";
            var newSection = new GameObject($"Section {sectionName}", typeof(RectTransform)).transform as RectTransform;
            newSection.SetParent(currentPage, false);
            newSection.anchorMin = new Vector2(0, 1);
            newSection.anchorMax = new Vector2(1, 1);
            newSection.pivot = new Vector2(0.5f, 1);
            newSection.sizeDelta = Vector2.zero;
            newSection.anchoredPosition = new Vector2(0, sectionY);
            var sectionHeader = Instantiate(m_sectionHeaderTemplate, newSection);
            sectionHeader.gameObject.SetActive(true);
            sectionHeader.text = sectionName;
            elementY = 0;
            elementY -= sectionHeader.preferredHeight;
            elementY -= m_spaceBetweenSectionHeaderAndFirstItem;
            return newSection;
        }
    }

    public static int GetPriceOfComponent (Cores.Components.CoreComponent component) {
        if(instance.m_shop.TryGetBuyItemForComponent(component, out var item)){
            var output = item.price;
            return output;
        }
        return 0;
    }

    public static bool OnBuyCommand (string itemName, Cores.Core targetCore, out string message) {
        if(instance.m_shop.TryGetBuyItemForCommand(itemName, out var item)){
            return item.TryPurchase(targetCore, out message);
        }
        message = $"\"{itemName}\" isn't a valid item name!";
        return false;
    }

    public static bool OnSellCommand (string id, out string message) {
        if(GameState.current.TryFindComponentForId(id, out var targetComponent)){
            targetComponent.core.RemoveComponent(targetComponent);
            GameState.current.currency += GetPriceOfComponent(targetComponent);
            message = string.Empty;
            return true;
        }
        message = $"There is no component with the id \"{id}\"!";
        return false;
    }

    public static bool OnUpgradeCommand (string id, out string message) {
        if(instance.m_shop.TryGetUpgradeItemForId(id, out var item)){
            return item.TryPurchase(id, out message);
        }
        if(GameState.current.TryFindComponentForId(id, out var component)){
            message = $"{component.GetType().Name} isn't upgradeable!";
        }else{
            message = $"\"{id}\" is neither a valid item name or component id!";
        }
        return false;
    }

}

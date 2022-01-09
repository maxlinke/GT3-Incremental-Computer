using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ShopDisplay : MonoBehaviour {

    private static ShopDisplay instance;

    static ShopDisplay () {
        var rawPages = System.Enum.GetValues(typeof(Page));
        var pageList = new List<Page>();
        var pageIndexDict = new Dictionary<Page, int>();
        var i = 0;
        foreach(Page page in rawPages){
            pageList.Add(page);
            pageIndexDict[page] = ++i;
        }
        pages = pageList;
        pageCount = pages.Count;
        pageIndex = pageIndexDict;
        var nextPageDict = new Dictionary<Page, Page>();
        var prevPageDict = new Dictionary<Page, Page>();
        var lastPage = pages[pages.Count-1];
        foreach(var page in pages){
            nextPageDict[lastPage] = page;
            prevPageDict[page] = lastPage;
            lastPage = page;
        }
        nextPage = nextPageDict;
        prevPage = prevPageDict;
    }

    private static readonly IReadOnlyList<Page> pages;
    private static readonly int pageCount;
    private static readonly IReadOnlyDictionary<Page, int> pageIndex;
    private static readonly IReadOnlyDictionary<Page, Page> nextPage;
    private static readonly IReadOnlyDictionary<Page, Page> prevPage;

    public enum Page {
        Unlocks = 0,
        Components = 1,
        Upgrades = 2
    }

    [SerializeField, RedIfEmpty] Canvas m_canvas;
    [SerializeField, RedIfEmpty] Text m_headerText;
    [SerializeField, RedIfEmpty] Text m_pageText;

    public bool visible {
        get => m_canvas.enabled;
        set => m_canvas.enabled = value;
    }

    public Page currentPage { get; private set; }

    public void Initialize () {
        instance = this;
        gameObject.SetActive(true);
        InputHandler.onScrollCommand
            .Where(_ => visible)
            .Where(dir => dir.x > 0)
            .Subscribe(_ => ShowPage(nextPage[currentPage]));
        InputHandler.onScrollCommand
            .Where(_ => visible)
            .Where(dir => dir.x < 0)
            .Subscribe(_ => ShowPage(prevPage[currentPage]));
        InputHandler.onCancel
            .Where(_ => visible)
            .Subscribe(_ => MainDisplay.ShowCores());
    }

    public void ShowPage (Page page) {
        m_headerText.text = GetPageHeader(page);
        m_pageText.text = $"{pageIndex[page]}/{pageCount}";
        switch(page){
            case Page.Unlocks:
            case Page.Components:
            case Page.Upgrades:
                // TODO, probably make individual (static) classes to generate the layout
                Debug.Log("TODO, probably make individual (static) classes to generate the layout");
                break;
            default:
                Debug.LogError($"Unknown Page \"{page}\"!");
                break;
        }
        currentPage = page;
    }

    string GetPageHeader (Page page) {
        switch(page){
            case Page.Unlocks:    return "Shop - Unlocks";
            case Page.Components: return "Shop - Components";
            case Page.Upgrades:   return "Shop - Upgrades";
            default:
                Debug.LogError($"Unknown Page \"{page}\"!");
                return "Shop - ???";
        }
    }

}

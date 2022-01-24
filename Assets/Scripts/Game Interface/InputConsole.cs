using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

public class InputConsole : MonoBehaviour, IScrollHandler {

    public static InputConsole instance { get; private set; }
    
    public const string inputPrefix = "\\>";

    [Header("Components")]
    [SerializeField, RedIfEmpty] Image m_backgroundImage;
    [SerializeField, RedIfEmpty] RectTransform m_scrollWindow;
    [SerializeField, RedIfEmpty] RectTransform m_scrollContent;
    [SerializeField, RedIfEmpty] Text m_textTemplate;
    [SerializeField, RedIfEmpty] RectTransform m_scrollbarParent;
    [SerializeField, RedIfEmpty] Image m_scrollbarImage;

    [Header("Settings")]
    [SerializeField, Min(0)] int m_firstTextVerticalOffset = 2;
    [SerializeField, Min(1)] int m_maxCommandLength = 64;
    [SerializeField, Min(1)] int m_maxCommandMemoryLength = 32;
    [SerializeField, Min(0)] float m_caretBlinkInterval = 1;
    [SerializeField, Min(0)] int m_scrollSpeed = 10;

    RectTransform rectTransform => (RectTransform)transform;

    Text m_activeText = null;
    float m_caretTimer = 0;
    bool m_caretShouldBeVisible = false;
    bool m_caretIsVisible = false;
    int m_shownMemoryCommandIndex = -1;

    readonly System.Text.StringBuilder m_inputBuffer = new System.Text.StringBuilder();
    readonly List<string> m_commandMemory = new List<string>();

    public void Initialize () {
        instance = this;
        m_backgroundImage.raycastTarget = true;
        m_textTemplate.raycastTarget = false;
        m_textTemplate.transform.SetParent(this.transform, false);
        m_textTemplate.SetGOActive(false);
        Clear(true);
        InputHandler.onCancel
            .Where(_ => !PopupDialogue.IsOrWasJustVisible)
            .Subscribe(_ => AbortInput());
        InputHandler.onCharEntered
            .Where(_ => !PopupDialogue.IsOrWasJustVisible)
            .Where(ch => (m_inputBuffer.Length < m_maxCommandLength))
            .Subscribe(AppendChar);
        InputHandler.onTextEditCommand
            .Where(_ => !PopupDialogue.IsOrWasJustVisible)
            .Where(keyCode => (m_inputBuffer.Length > 0))
            .Where(keyCode => (keyCode == KeyCode.Backspace))
            .Subscribe(RemoveChar);
        InputHandler.onConfirm
            .Where(_ => !PopupDialogue.IsOrWasJustVisible)
            .Subscribe(_ => SubmitLine());
        InputHandler.onDirection
            .Where(_ => !PopupDialogue.IsOrWasJustVisible)
            .Where(dir => (dir.y != 0 && dir.x == 0))
            .Select(dir => (int)(dir.y) + m_shownMemoryCommandIndex)
            .Where(newIndex => HasMemoryCommandAtIndex(newIndex))
            .Subscribe(UseMemoryCommandAtIndex);
        InputHandler.onScrollCommand
            .Where(_ => !PopupDialogue.IsOrWasJustVisible)
            .Where(dir => (dir.y != 0 && dir.x == 0))
            .Select(dir => (int)(dir.y))
            .Subscribe(offset => ScrollWithOffset(offset * m_scrollSpeed));
    }

    void Update () {
        m_caretTimer += 2 * Time.deltaTime / m_caretBlinkInterval;
        if(m_caretTimer >= 1f){
            m_caretTimer -= (int)(m_caretTimer);
            m_caretShouldBeVisible = !m_caretShouldBeVisible;
            UpdateInputText();
        }
    }

    public void Clear (bool alsoClearInput = false) {
        int cc = m_scrollContent.childCount;
        for(int i=cc-1; i>=0; i--){
            Destroy(m_scrollContent.GetChild(i).gameObject);
        }
        if(alsoClearInput){
            m_inputBuffer.Clear();
        }
        m_activeText = Instantiate(m_textTemplate);
        m_activeText.SetGOActive(true);
        m_activeText.rectTransform.SetParent(m_scrollContent, false);
        m_activeText.rectTransform.anchoredPosition = new Vector2(0, -m_firstTextVerticalOffset);
        UpdateInputText();
        ResetScroll();
    }

    void AbortInput () {
        m_inputBuffer.Clear();
        UpdateInputText();
        ResetScroll();
    }

    public void PrintMessage (string message) {
        var textCache = m_activeText.text;
        m_activeText.text = message;
        ApplyAndCloneActiveText();
        m_activeText.text = textCache;
    }

    void SubmitLine () {
        if(m_caretIsVisible){
            m_activeText.text = m_activeText.text.Substring(0, m_activeText.text.Length - 1);
        }
        ApplyAndCloneActiveText();
        var bufferString = m_inputBuffer.ToString();
        if(bufferString.Length > 0){
            if(m_commandMemory.Count < 1 || !m_commandMemory[0].Equals(bufferString)){
                m_commandMemory.Insert(0, bufferString);
            }
            if(m_commandMemory.Count > m_maxCommandMemoryLength){
                m_commandMemory.RemoveAt(m_commandMemory.Count - 1);
            }
            Commands.Command.TryExecute(bufferString, out var commandMessage);
            if(!string.IsNullOrWhiteSpace(commandMessage)){
                m_activeText.text = $"{commandMessage.Trim()}\n";
                ApplyAndCloneActiveText();
            }
        }
        m_inputBuffer.Clear();
        m_shownMemoryCommandIndex = -1;
        UpdateInputText();
        ResetScroll();
        ResetShownCommandIndex();
    }

    void ApplyAndCloneActiveText () {
        m_activeText.gameObject.name = $"Text #{m_scrollContent.childCount}";
        var appliedText = m_activeText;
        m_activeText = Instantiate(m_activeText, m_activeText.transform.parent);
        var preferredHeight = m_activeText.preferredHeight;
        appliedText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
        m_activeText.rectTransform.anchoredPosition += Vector2.down * preferredHeight;
    }

    void AppendChar (char charToAppend) {
        m_inputBuffer.Append(charToAppend);
        m_caretShouldBeVisible = false;
        m_caretTimer = 0;
        UpdateInputText();
        ResetScroll();
    }

    void RemoveChar (KeyCode removeKey) {
        m_inputBuffer.Remove(m_inputBuffer.Length - 1, 1);
        m_caretShouldBeVisible = false;
        m_caretTimer = 0;
        UpdateInputText();
        ResetScroll();
    }

    void UpdateInputText () {
        if(m_caretShouldBeVisible && (m_inputBuffer.Length < m_maxCommandLength)){
            m_activeText.text = $"{inputPrefix}{m_inputBuffer}_";
            m_caretIsVisible = true;
        }else{
            m_activeText.text = $"{inputPrefix}{m_inputBuffer}";
            m_caretIsVisible = false;
        }
    }

    void ResetScroll () {
        var newScrollHeight = Mathf.Abs(m_activeText.rectTransform.anchoredPosition.y) + m_activeText.preferredHeight;
        m_scrollContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newScrollHeight);
        var deltaHeight = newScrollHeight - m_scrollWindow.rect.height;
        if(deltaHeight > 0 ){
            m_scrollContent.SetAnchoredY(deltaHeight);
        }else{
            m_scrollContent.SetAnchoredY(0);
        }
        UpdateScrollbar();
    }

    void UpdateScrollbar () {
        var heightRatio = m_scrollWindow.rect.height / m_scrollContent.rect.height;
        var barHeight = m_scrollbarParent.rect.height;
        if(heightRatio > 1){
            m_scrollbarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barHeight);
            m_scrollbarImage.rectTransform.SetAnchorAndPivot(0.5f * Vector2.one);
            m_scrollbarImage.rectTransform.anchoredPosition = Vector2.zero;
            return;
        }
        m_scrollbarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barHeight * heightRatio);
        var normedPos = m_scrollContent.anchoredPosition.y / (m_scrollContent.rect.height - m_scrollWindow.rect.height);
        m_scrollbarImage.rectTransform.SetAnchorAndPivot(new Vector2(0.5f, 1f - normedPos));
        m_scrollbarImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    void ScrollWithOffset (int scrollOffset) {
        var deltaHeight = m_scrollContent.rect.height - m_scrollWindow.rect.height;
        if(deltaHeight > 0){
            var tempY = m_scrollContent.anchoredPosition.y - scrollOffset;
            var newY = Mathf.Clamp(tempY, 0, deltaHeight);
            m_scrollContent.SetAnchoredY(newY);
        }else{
            m_scrollContent.SetAnchoredY(0);
        }
        UpdateScrollbar();
    }

    void ResetShownCommandIndex () {
        m_shownMemoryCommandIndex = -1;
    }

    bool HasMemoryCommandAtIndex (int index) {
        return (index >= 0) && (index < m_commandMemory.Count);
    }

    void UseMemoryCommandAtIndex (int index) {
        m_inputBuffer.Clear();
        m_inputBuffer.Append(m_commandMemory[index]);
        UpdateInputText();
        ResetScroll();
        m_shownMemoryCommandIndex = index;
    }

    void IScrollHandler.OnScroll(PointerEventData eventData) {
        ScrollWithOffset((int)(eventData.scrollDelta.y * m_scrollSpeed));
    }

}

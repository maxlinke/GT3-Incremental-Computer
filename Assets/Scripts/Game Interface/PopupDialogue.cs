using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;

public class PopupDialogue : MonoBehaviour {

    const char yesKey = 'Y';
    const char noKey = 'N';
    const KeyCode cancelKey = InputHandler.CANCEL_KEY;
    const KeyCode confirmKey = InputHandler.CONFIRM_KEY;
    
    enum OptionId {
        Yes,
        No,
        Cancel,
        Confirm
    }

    class Option {
        public System.Action action;
        public readonly Text text;
        public Option (Text text) {
            this.text = text;
            this.action = null;
        }
    }

    private static PopupDialogue instance;

    public static bool IsVisible => instance.visible;
    public static bool JustBecameVisible => Time.frameCount == instance.m_showFrame;
    public static bool WasJustVisible => Time.frameCount == instance.m_hideFrame;
    public static bool IsOrWasJustVisible => IsVisible || WasJustVisible;

    [SerializeField, RedIfEmpty] Text m_messageText;
    [SerializeField, RedIfEmpty] Text m_optionTextTemplate;

    bool visible {
        get => gameObject.activeSelf;
        set { 
            if(value != visible){
                if(value){
                    m_showFrame = Time.frameCount;
                }else{
                    m_hideFrame = Time.frameCount;
                }
            }
            gameObject.SetActive(value);
        }
    }

    RectTransform rectTransform => (RectTransform)transform;

    float m_initWidth;
    int m_hideFrame;
    int m_showFrame;
    IReadOnlyDictionary<OptionId, Option> m_options;

    void EmptyButNotNull () { }

    string TextForOption (OptionId option) {
        switch(option){
            case OptionId.Yes:     return $"[{yesKey}] Yes";
            case OptionId.No:      return $"[{noKey}] No";
            case OptionId.Cancel:  return $"[{cancelKey.ToString().Substring(0, 3)}] Cancel";
            case OptionId.Confirm: return $"[{confirmKey}] Confirm";
            default: return "???";
        }
    }

    private static bool NoisyAbortIfOpen () {
        if(instance.visible){
            Debug.LogWarning($"{nameof(PopupDialogue)} is still visible, aborting call!");
            return true;
        }
        return false;
    }

    private void Show (string message) {
        visible = true;
        m_messageText.text = message;
        SetTextsActiveDependingOnActionAndCount(out var textCount);
        var newWidth = m_initWidth * ((float)(Mathf.Max(2, textCount)) / 2);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        MakeActiveTextsFillWidth();

        void SetTextsActiveDependingOnActionAndCount (out int output) {
            output = 0;
            foreach(var option in m_options.Values){
                if(option.action == null){
                    option.text.SetGOActive(false);
                }else{
                    option.text.SetGOActive(true);
                    output++;
                }
            }
        }

        void MakeActiveTextsFillWidth () {
            var i = 0;
            foreach(var option in m_options.Values){
                var optionText = option.text;
                if(!optionText.gameObject.activeSelf){
                    continue;
                }
                var l = (float)i / textCount;
                var r = (float)(i+1) / textCount;
                optionText.rectTransform.anchorMin = new Vector2(l, 0);
                optionText.rectTransform.anchorMax = new Vector2(r, 0);
                optionText.rectTransform.anchoredPosition = Vector2.zero;
                optionText.rectTransform.sizeDelta = new Vector2(0, optionText.rectTransform.sizeDelta.y);
                i++;
            }
        }
    }

    public static void ShowSimpleConfirmDialogue (string message, System.Action onConfirm) {
        if(NoisyAbortIfOpen()) return;
        instance.m_options[OptionId.Yes].action =     null;
        instance.m_options[OptionId.No].action =      null;
        instance.m_options[OptionId.Cancel].action =  null;
        instance.m_options[OptionId.Confirm].action = onConfirm ?? instance.EmptyButNotNull;
        instance.Show(message);
    }

    public static void ShowYesNoDialogue (string message, System.Action onYes, System.Action onNo) {
        if(NoisyAbortIfOpen()) return;
        instance.m_options[OptionId.Yes].action =     onYes ?? instance.EmptyButNotNull;
        instance.m_options[OptionId.No].action =      onNo ?? instance.EmptyButNotNull;
        instance.m_options[OptionId.Cancel].action =  null;
        instance.m_options[OptionId.Confirm].action = null;
        instance.Show(message);
    }

    public static void ShowYesNoCancelDialogue (string message, System.Action onYes, System.Action onNo, System.Action onCancel) {
        if(NoisyAbortIfOpen()) return;
        instance.m_options[OptionId.Yes].action =     onYes ?? instance.EmptyButNotNull;
        instance.m_options[OptionId.No].action =      onNo ?? instance.EmptyButNotNull;
        instance.m_options[OptionId.Cancel].action =  onCancel ?? instance.EmptyButNotNull;
        instance.m_options[OptionId.Confirm].action = null;
        instance.Show(message);
    }

    public void Initialize () {
        instance = this;
        m_initWidth = rectTransform.rect.width;
        var options = new Dictionary<OptionId, Option>();
        foreach(OptionId option in System.Enum.GetValues(typeof(OptionId))){
            options[option] = new Option(Instantiate(m_optionTextTemplate, m_optionTextTemplate.transform.parent));
            options[option].text.text = TextForOption(option);
        }
        m_options = options;
        m_optionTextTemplate.SetGOActive(false);
        InputHandler.onCharEntered
            .Where(_ => visible)
            .Where(_ => !JustBecameVisible)
            .Subscribe(OnCharacterEntered);
        InputHandler.onCancel
            .Where(_ => visible)
            .Where(_ => !JustBecameVisible)
            .Where(_ => m_options[OptionId.Cancel].action != null)
            .Subscribe(_ => InvokeAndClose(OptionId.Cancel));
        InputHandler.onConfirm
            .Where(_ => visible)
            .Where(_ => !JustBecameVisible)
            .Where(_ => m_options[OptionId.Confirm].action != null)
            .Subscribe(_ => InvokeAndClose(OptionId.Confirm));
        visible = false;
    }

    void InvokeAndClose (OptionId option) {
        m_options[option].action?.Invoke();
        visible = false;
    }

    void OnCharacterEntered (char ch) {
        switch(char.ToUpper(ch)){
            case yesKey:
                InvokeAndClose(OptionId.Yes);
                break;
            case noKey:
                InvokeAndClose(OptionId.No);
                break;
            default:
                break;
        }
    }

}

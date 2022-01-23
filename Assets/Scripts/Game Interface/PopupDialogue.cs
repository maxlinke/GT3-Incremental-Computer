using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class PopupDialogue : MonoBehaviour {

    const char yesKey = 'Y';
    const char noKey = 'N';
    const KeyCode cancelKey = KeyCode.Escape;

    private static PopupDialogue instance;

    public static bool IsVisible => instance.visible;
    public static bool WasJustVisible => Time.frameCount == instance.m_hideFrame;
    public static bool IsOrWasJustVisible => IsVisible || WasJustVisible;

    [SerializeField, RedIfEmpty] Text m_messageText;
    [SerializeField, RedIfEmpty] Text m_optionTextTemplate;

    bool visible {
        get => gameObject.activeSelf;
        set { 
            if(!value && visible){
                m_hideFrame = Time.frameCount;
            }
            gameObject.SetActive(value);
        }
    }

    RectTransform rectTransform => (RectTransform)transform;

    float m_initWidth;
    int m_hideFrame;
    System.Action m_onYes;
    System.Action m_onNo;
    System.Action m_onCancel;
    Text[] m_optionTexts;

    // TODO using null like this for the oncancel thingy is not ideal. refactor if i have time, but it does work...

    public static void ShowYesNoDialogue (string message, System.Action onYes, System.Action onNo) {
        ShowYesNoCancelDialogue(message, onYes, onNo, null);
    }

    public static void ShowYesNoCancelDialogue (string message, System.Action onYes, System.Action onNo, System.Action onCancel) {
        if(instance.visible){
            if(instance.m_onCancel != null){
                instance.m_onCancel();
            }else if(instance.m_onNo != null){
                instance.m_onNo();
            }
        }
        instance._ShowYesNoCancelDialogue(message, onYes, onNo, onCancel);
    }

    private void _ShowYesNoCancelDialogue (string message, System.Action onYes, System.Action onNo, System.Action onCancel) {
        visible = true;
        m_messageText.text = message;
        m_onYes = onYes;
        m_onNo = onNo;
        m_onCancel = onCancel;
        var textCount = (onCancel == null ? 2 : 3);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_initWidth * ((float)textCount / 2));
        for(int i=0; i<textCount; i++){
            var optionText = m_optionTexts[i];
            optionText.SetGOActive(true);
            var l = (float)i / textCount;
            var r = (float)(i+1) / textCount;
            optionText.rectTransform.anchorMin = new Vector2(l, 0);
            optionText.rectTransform.anchorMax = new Vector2(r, 0);
            optionText.rectTransform.anchoredPosition = Vector2.zero;
            optionText.rectTransform.sizeDelta = new Vector2(0, optionText.rectTransform.sizeDelta.y);
        }
        for(int i=textCount; i<m_optionTexts.Length; i++){
            m_optionTexts[i].SetGOActive(false);
        }
    }

    public void Initialize () {
        instance = this;
        m_initWidth = rectTransform.rect.width;
        m_optionTexts = new Text[]{
            Instantiate(m_optionTextTemplate, m_optionTextTemplate.transform.parent),
            Instantiate(m_optionTextTemplate, m_optionTextTemplate.transform.parent),
            Instantiate(m_optionTextTemplate, m_optionTextTemplate.transform.parent)
        };
        m_optionTexts[0].text = $"[{yesKey}] Yes";
        m_optionTexts[1].text = $"[{noKey}] No";
        m_optionTexts[2].text = $"[{InputHandler.CANCEL_COMMAND}] Cancel";
        m_optionTextTemplate.SetGOActive(false);
        InputHandler.onCharEntered
            .Where(_ => visible)
            .Subscribe(OnCharacterEntered);
        InputHandler.onCancel
            .Where(_ => visible)
            .Where(_ => m_onCancel != null)
            .Subscribe(_ => OnCancel());
        visible = false;
    }

    void OnCharacterEntered (char ch) {
        switch(char.ToUpper(ch)){
            case yesKey:
                m_onYes?.Invoke();
                visible = false;
                break;
            case noKey:
                m_onNo?.Invoke();
                visible = false;
                break;
            default:
                break;
        }
    }

    void OnCancel () {
        m_onCancel?.Invoke();
        visible = false;
    }

}

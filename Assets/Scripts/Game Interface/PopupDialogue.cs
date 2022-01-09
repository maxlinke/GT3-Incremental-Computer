using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class PopupDialogue : MonoBehaviour {

    const char confirmKey = 'Y';
    const char abortKey = 'N';

    private static PopupDialogue instance;

    public static bool IsVisible => instance.visible;
    public static bool WasJustVisible => Time.frameCount == instance.m_hideFrame;
    public static bool IsOrWasJustVisible => IsVisible || WasJustVisible;

    [SerializeField, RedIfEmpty] Text m_messageText;
    [SerializeField, RedIfEmpty] Text m_leftText;
    [SerializeField, RedIfEmpty] Text m_rightText;

    bool visible {
        get => gameObject.activeSelf;
        set { 
            if(!value && visible){
                m_hideFrame = Time.frameCount;
            }
            gameObject.SetActive(value);
        }
    }

    int m_hideFrame;
    System.Action m_onConfirm;
    System.Action m_onAbort;

    public static void ShowYesNoDialogue (string message, System.Action onConfirm, System.Action onAbort) {
        if(instance.visible){
            instance.m_onAbort?.Invoke();
        }
        instance.visible = true;
        instance.m_messageText.text = message;
        instance.m_leftText.text = $"[{confirmKey}] Yes";
        instance.m_rightText.text = $"[{abortKey}] No";
        instance.m_onConfirm = onConfirm;
        instance.m_onAbort = onAbort;
    }

    public void Initialize () {
        instance = this;
        visible = false;
        InputHandler.onCharEntered
            .Where(_ => visible)
            .Subscribe(OnCharacterEntered);
    }

    void OnCharacterEntered (char ch) {
        switch(char.ToUpper(ch)){
            case confirmKey:
                m_onConfirm?.Invoke();
                visible = false;
                break;
            case abortKey:
                m_onAbort?.Invoke();
                visible = false;
                break;
            default:
                break;
        }
    }

}

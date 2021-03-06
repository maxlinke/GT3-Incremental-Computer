using UnityEngine;
using UniRx;

public class InputHandler : MonoBehaviour {

    public const string CANCEL_COMMAND = "Ctrl+C";
    public const string SCROLL_COMMAND = "Ctrl+ArrowKey";
    public const KeyCode CANCEL_KEY = KeyCode.Escape;
    public const KeyCode CONFIRM_KEY = KeyCode.Return;
    public const KeyCode CONFIRM_ALT_KEY = KeyCode.KeypadEnter;

    private readonly Subject<Event> m_onKeyEvent = new Subject<Event>();

    public static Subject<char> onCharEntered { get; private set; } = new Subject<char>();
    public static Subject<KeyCode> onTextEditCommand { get; private set; } = new Subject<KeyCode>();
    public static Subject<Vector2> onScrollCommand { get; private set; } = new Subject<Vector2>();
    public static Subject<Vector2> onDirection { get; private set; } = new Subject<Vector2>();
    public static Subject<Event> onCancel { get; private set; } = new Subject<Event>();
    public static Subject<Event> onConfirm { get; private set; } = new Subject<Event>();

    public void Initialize () {
        InitObservables();
    }

    void OnGUI () {
        var curr = Event.current;
        if(curr.type == EventType.KeyDown){
            m_onKeyEvent.OnNext(curr);
        }
    }

    void InitObservables () {
        m_onKeyEvent
            .Where(evt => HasModifier(evt, EventModifiers.Control))
            .Where(evt => evt.keyCode == KeyCode.C)
            .Subscribe(evt => {
                onCancel.OnNext(evt);
                evt.Use();
            });
        m_onKeyEvent
            .Where(evt => evt.keyCode == CANCEL_KEY)
            .Subscribe(evt => {
                onCancel.OnNext(evt);
                evt.Use();
            });
        m_onKeyEvent
            .Where(evt => evt.keyCode == CONFIRM_KEY || evt.keyCode == CONFIRM_ALT_KEY)
            .Subscribe(evt => {
                onConfirm.OnNext(evt);
                evt.Use();
            });
        m_onKeyEvent
            .Where(evt => IsTextCharacter(evt.character))
            .Subscribe(evt => {
                onCharEntered.OnNext(evt.character);
                evt.Use();
            });
        m_onKeyEvent
            .Where(evt => IsTextEditCommand(evt.keyCode))
            .Subscribe(evt => {
                onTextEditCommand.OnNext(evt.keyCode);
                evt.Use();
            });
        m_onKeyEvent
            .Where(evt => HasModifier(evt, EventModifiers.Control))
            .Where(evt => IsDirectionKey(evt.keyCode))
            .Subscribe(evt => {
                onScrollCommand.OnNext(DirectionForKeyCode(evt.keyCode));
                evt.Use();
            });
        m_onKeyEvent
            .Where(evt => !HasModifier(evt, EventModifiers.Control))
            .Where(evt => IsDirectionKey(evt.keyCode))
            .Subscribe(evt => {
                onDirection.OnNext(DirectionForKeyCode(evt.keyCode));
                evt.Use();
            });
    }

    static bool IsTextCharacter (char character) {
        return character >= 32 && character <= 127;
    }

    static bool IsTextEditCommand (KeyCode keyCode) {
        switch(keyCode){
            case KeyCode.Backspace:
            case KeyCode.Delete:
                return true;
            default:
                return false;
        }
    }

    static bool IsDirectionKey (KeyCode keyCode) {
        switch(keyCode){
            case KeyCode.UpArrow:
            case KeyCode.DownArrow:
            case KeyCode.LeftArrow:
            case KeyCode.RightArrow:
                return true;
            default:
                return false;
        }
    }

    static bool HasModifier (Event evt, EventModifiers modifier) {
        return ((evt.modifiers & modifier) == modifier);
    }

    static Vector2 DirectionForKeyCode (KeyCode keyCode) {
        switch(keyCode){
            case KeyCode.UpArrow:
                return Vector2.up;
            case KeyCode.DownArrow:
                return Vector2.down;
            case KeyCode.LeftArrow:
                return Vector2.left;
            case KeyCode.RightArrow:
                return Vector2.right;
            default:
                Debug.LogWarning($"Can't convert key \"{keyCode}\" to direction!");
                return Vector2.zero;
        }
    }

}

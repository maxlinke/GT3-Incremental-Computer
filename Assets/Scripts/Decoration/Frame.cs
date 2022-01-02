using UnityEngine;
using UnityEngine.UI;

public class Frame : MonoBehaviour {

    [Header("Components")]
    [SerializeField, RedIfEmpty] Image m_image;
    [SerializeField, RedIfEmpty] RectTransform m_content;

    [Header("Assets")]
    [SerializeField, RedIfEmpty] Sprite m_singleLineBig;
    [SerializeField, RedIfEmpty] Sprite m_singleLineSmall;
    [SerializeField, RedIfEmpty] Sprite m_doubleLine;
    [SerializeField, RedIfEmpty] Sprite m_offsetBoxes;

    public RectTransform content => m_content;

    public enum Type {
        SingleLineBig,
        SingleLineSmall,
        DoubleLine,
        OffsetBoxes
    }

    public Type type {
        get {
            return GetTypeForSprite(m_image.sprite);
        } set {
            m_image.sprite = GetSpriteForType(type);
        }
    }

    private Sprite GetSpriteForType (Type type) {
        switch(type){
            case Type.SingleLineBig:
                return m_singleLineBig;
            case Type.SingleLineSmall:
                return m_singleLineSmall;
            case Type.DoubleLine:
                return m_doubleLine;
            case Type.OffsetBoxes:
                return m_offsetBoxes;
            default:
                return null;
        }
    }

    private Type GetTypeForSprite (Sprite sprite) {
        if(sprite == m_singleLineBig) return Type.SingleLineBig;
        if(sprite == m_singleLineSmall) return Type.SingleLineSmall;
        if(sprite == m_doubleLine) return Type.DoubleLine;
        if(sprite == m_offsetBoxes) return Type.OffsetBoxes;
        return (Type)(-1);
    }

}

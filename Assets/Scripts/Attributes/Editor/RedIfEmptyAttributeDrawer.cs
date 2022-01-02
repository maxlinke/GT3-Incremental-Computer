using UnityEngine;
using UnityEditor;
using Type = UnityEditor.SerializedPropertyType;

[CustomPropertyDrawer(typeof(RedIfEmptyAttribute))]
public class RedIfEmptyAttributeDrawer : PropertyDrawer {

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        var red = ShouldBeRed(property);
        using(new EditorGUITools.ColorScope(red ? EditorTools.comfyRed : GUI.color)){
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    bool ShouldBeRed (SerializedProperty property) {
        switch(property.propertyType){
            case Type.ObjectReference:
                return property.objectReferenceValue == null;
            case Type.String:
                return string.IsNullOrWhiteSpace(property.stringValue);
            default:
                return false;
        }
    }
    
}

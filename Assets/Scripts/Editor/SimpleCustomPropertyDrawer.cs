using UnityEngine;
using UnityEditor;
using System.Reflection;

public abstract class SimpleCustomPropertyDrawer : PropertyDrawer {

    public enum LabelPosition {
        OnTop,
        OnLeft
    }

    protected SerializedProperty serializedProperty { get; private set; }
    protected GUIContent label { get; private set; }

    protected virtual float rawLabelWidth => 65;
    protected virtual float labelSpace => 2;
    protected virtual bool labelIsExpandButton => false;

    protected abstract LabelPosition labelPosition { get; }
    protected abstract bool alwaysIgnoreLabel { get; }

    public override float GetPropertyHeight (SerializedProperty inputProperty, GUIContent label) {
        this.serializedProperty = inputProperty.Copy();
        if(!inputProperty.isExpanded && labelIsExpandButton){
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        this.label = label;
        float height = 0;
        if((labelPosition == LabelPosition.OnTop) && (label != GUIContent.none) && !alwaysIgnoreLabel){
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        if(WillDrawBeforeProperties(out var beforeHeight)){
            height += beforeHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        foreach(var prop in inputProperty.IterateOverVisibleChildren()){
            if(PropertyIsDrawnCustom(prop, out var customHeight)){
                height += customHeight;
            }else{
                height += EditorGUI.GetPropertyHeight(prop, new GUIContent(prop.displayName), true);
            }
            height += EditorGUIUtility.standardVerticalSpacing;
        }
        if(WillDrawAfterProperties(out var afterHeight)){
            height += afterHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        return height;
    }

    public override void OnGUI (Rect position, SerializedProperty inputProperty, GUIContent label) {
        this.serializedProperty = inputProperty.Copy();
        int localIndent = 0;
        if(label != GUIContent.none && !alwaysIgnoreLabel){
            Rect labelRect;
            if(labelPosition == LabelPosition.OnTop){
                labelRect = EditorGUITools.RemoveLine(ref position);
                localIndent = 1;
            }else{
                var labelWidth = rawLabelWidth - (15 * EditorGUI.indentLevel);
                labelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
                var offset = labelWidth + labelSpace;
                position = new Rect(position.x + offset, position.y, position.width - offset, position.height);
                localIndent = 0;
            }
            if(labelIsExpandButton){
                EditorGUI.BeginChangeCheck();
                EditorGUI.Foldout(labelRect, inputProperty.isExpanded, label);
                GUI.Button(labelRect, string.Empty, EditorStyles.label);
                if(EditorGUI.EndChangeCheck()){
                    inputProperty.isExpanded = !inputProperty.isExpanded;
                }
            }else{
                EditorGUI.LabelField(labelRect, label);
            }
        }
        if(inputProperty.isExpanded || !labelIsExpandButton){
            EditorGUI.indentLevel += localIndent;
            if(WillDrawBeforeProperties(out var beforeHeight)){
                var rect = EditorGUITools.RemoveLine(ref position, beforeHeight);
                DrawBeforeProperties(rect);
            }
            foreach(var prop in inputProperty.IterateOverVisibleChildren()){
                if(PropertyIsDrawnCustom(prop, out var customHeight)){
                    var rect = EditorGUITools.RemoveLine(ref position, customHeight);
                    DrawPropertyCustom(rect, prop);
                }else{
                    var propLabel = new GUIContent(prop.displayName);
                    var height = EditorGUI.GetPropertyHeight(prop, propLabel, true);
                    var rect = EditorGUITools.RemoveLine(ref position, height);
                    EditorGUI.PropertyField(rect, prop, propLabel, true);
                }
            }
            if(WillDrawAfterProperties(out var afterHeight)){
                var rect = EditorGUITools.RemoveLine(ref position, afterHeight);
                DrawAfterProperties(rect);
            }
            EditorGUI.indentLevel -= localIndent;
        }
    }

    protected float DefaultHeight (string propName) {
        return DefaultHeight(serializedProperty.FindPropertyRelative(propName));
    }

    protected float DefaultHeight (SerializedProperty property) {
        return EditorGUI.GetPropertyHeight(property);
    }

    protected abstract bool PropertyIsDrawnCustom (SerializedProperty property, out float height);
    protected abstract void DrawPropertyCustom (Rect position, SerializedProperty property);

    protected virtual bool WillDrawBeforeProperties (out float height) {
        height = 0;
        return false;
    }

    protected virtual void DrawBeforeProperties (Rect position) { }

    protected virtual bool WillDrawAfterProperties (out float height) {
        height = 0;
        return false;
    }

    protected virtual void DrawAfterProperties (Rect position) { }

    protected void CopyAllFields<T> (T source) {
        var fields = source.GetType().GetFields(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance
        );
        var props = this.serializedProperty.Copy().IterateOverVisibleChildren();
        foreach(var prop in props){
            foreach(var field in fields){
                if(field.Name.Equals(prop.name)){
                    object fieldValue = field.GetValue(source);
                    switch(prop.propertyType){
                        case SerializedPropertyType.Boolean:
                            prop.boolValue = (bool)fieldValue;
                            break;
                        case SerializedPropertyType.Integer:
                            prop.intValue = (int)fieldValue;
                            break;
                        case SerializedPropertyType.Float:
                            prop.floatValue = (float)fieldValue;
                            break;
                        case SerializedPropertyType.Color:
                            prop.colorValue = (Color)fieldValue;
                            break;
                        case SerializedPropertyType.Enum:
                            prop.intValue = (int)fieldValue;
                            break;
                        default:
                            Debug.Log($"Don't know how to copy value of field \"{prop.name}\" (prop type: {prop.propertyType}, field type: {fieldValue.GetType()})");
                            break;
                    }
                }
            }
        }
    }

}

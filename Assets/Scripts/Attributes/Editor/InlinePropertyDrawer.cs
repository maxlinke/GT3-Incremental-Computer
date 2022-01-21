using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InlinePropertyAttribute))]
public class InlinePropertyDrawer : SimpleCustomPropertyDrawer {

    protected override LabelPosition labelPosition => default;
    protected override bool alwaysIgnoreLabel => true;

    protected override void DrawPropertyCustom(Rect position, SerializedProperty property) { }

    protected override bool PropertyIsDrawnCustom(SerializedProperty property, out float height) {
        height = default;
        return false;
    }
    
}

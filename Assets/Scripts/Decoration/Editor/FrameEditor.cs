using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(Frame))]
public class FrameEditor : GenericEditor<Frame> {

    protected override bool DrawPropertyCustom(SerializedProperty property) {
        switch(property.name){
            case "m_singleLineBig":
            case "m_singleLineSmall":
            case "m_doubleLine":
            case "m_offsetBoxes":
                using(new EditorGUILayout.HorizontalScope()){
                    EditorGUILayout.PropertyField(property);
                    using(new EditorGUI.DisabledScope(property.objectReferenceValue == null)){
                        var rawBtnRect = EditorGUILayout.GetControlRect(GUILayout.Width(60), GUILayout.ExpandHeight(true));
                        var btnRect = new Rect(
                            x: rawBtnRect.x,
                            y: rawBtnRect.y + (rawBtnRect.height - EditorGUIUtility.singleLineHeight),
                            width: rawBtnRect.width,
                            height: EditorGUIUtility.singleLineHeight
                        );
                        if(GUI.Button(btnRect, "Apply")){
                            var img = property.serializedObject.FindProperty("m_image").objectReferenceValue as Image;
                            if(img != null){
                                Undo.RecordObject(img, "Apply Frame Sprite");
                                img.sprite = property.objectReferenceValue as Sprite;
                                img.SetAllDirty();
                                img.canvasRenderer.Clear();
                            }
                        }
                    }
                }
                return true;
            default:
                return false;
        }
    }

}

using UnityEngine;
using UnityEditor;

public abstract class GenericEditor : Editor {

    public override void OnInspectorGUI () {
        serializedObject.Update();
        var currentProperty = serializedObject.GetIterator();
        currentProperty.NextVisible(true);
        using(new EditorGUI.DisabledScope(true)){
            EditorGUILayout.PropertyField(currentProperty);
        }
        OnBeforeDrawProperties();
        while(currentProperty.NextVisible(false)){
            if(!DrawPropertyCustom(currentProperty)){
                EditorGUILayout.PropertyField(currentProperty);
            }
        }
        OnAfterDrawProperties();
        serializedObject.ApplyModifiedProperties();
    }

    protected abstract bool DrawPropertyCustom (SerializedProperty property);

    protected virtual void OnBeforeDrawProperties () { }

    protected virtual void OnAfterDrawProperties () { }

}

public abstract class GenericEditor<T> : GenericEditor where T : UnityEngine.Object {

    protected new T target => base.target as T;

}
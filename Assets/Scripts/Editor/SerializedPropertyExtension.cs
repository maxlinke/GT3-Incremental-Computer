using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SerializedPropertyExtension {

    public static IEnumerable<SerializedProperty> IterateOverVisibleChildren (this SerializedProperty property) {
        var startDepth = property.depth;
        property.NextVisible(true);
        while(property.depth > startDepth){
            yield return property;
            if(!property.NextVisible(false)){
                break;
            }
        }
    }

    // BIG FAT WARNING HERE: When you increase the array size, Unity initializes the new elements as a copy of the old last element.
    // There's nothing we can do about that as of now.
    public static SerializedProperty AppendArrayElement(this SerializedProperty array) {
        if (!array.isArray) {
            Debug.LogError($"Trying to expand the array size of the property {array.displayName}, but it's not an array");
            return null;
        }

        array.arraySize++;
        return array.GetArrayElementAtIndex(array.arraySize - 1);
    }

    public static void RemoveArrayIndex(this SerializedProperty array, int atIndex) {
        if (!array.isArray) {
            Debug.LogError($"Trying to remove an array index from property {array.displayName}, but it's not an array");
            return;
        }

        if (atIndex < 0 || atIndex >= array.arraySize) {
            Debug.LogError($"Trying to remove array index {atIndex} from array {array.displayName}, but that array's size is {array.arraySize}");
            return;
        }

        var arraySize = array.arraySize;
        array.DeleteArrayElementAtIndex(atIndex);
        if (array.arraySize == arraySize)
            array.DeleteArrayElementAtIndex(atIndex); // deal with Unity just setting the object to null instead of removing the slot.
    }

    public static SerializedProperty InsertArrayElement(this SerializedProperty prop, int atIndex) {
        if (!prop.isArray) {
            Debug.LogError($"Trying to expand the array size of the property {prop.displayName}, but it's not an array");
            return null;
        }

        prop.InsertArrayElementAtIndex(atIndex);
        return prop.GetArrayElementAtIndex(atIndex);
    }

    public static SerializedProperty FindPropRelativeChecked(this SerializedProperty p, string propName) {
        var prop = p.FindPropertyRelative(propName);
        if (prop == null)
            throw new Exception($"Type {p.type} doesn't have the property {propName}! Did you change {p.type}?");
        return prop;
    }

    public static bool NextForObjectRefs(this SerializedProperty property, SerializedProperty endProperty, ref SerializedPropertyType propertyType) {
        var result = property.Next(propertyType == SerializedPropertyType.Generic);
        while (result && property != endProperty) {
            propertyType = property.propertyType;
            if (propertyType == SerializedPropertyType.Generic)
                return true;
            result = property.Next(false);
        }
        return false;
    }

    public static object GetTargetObjectOfProperty(this SerializedProperty prop) {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements) {
            if (element.Contains("[")) {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else {
                obj = GetValue_Imp(obj, element);
            }
        }

        return obj;
    }

    private static object GetValue_Imp(object source, string name) {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null) {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }

        return null;
    }

    private static object GetValue_Imp(object source, string name, int index) {
        var enumerable = GetValue_Imp(source, name) as IEnumerable;
        if (enumerable == null)
            return null;
        var enm = enumerable.GetEnumerator();

        for (int i = 0; i <= index; i++) {
            if (!enm.MoveNext())
                return null;
        }

        return enm.Current;
    }

        /// <summary>
    /// Use to set the underlying object of a serialized property dirty, if you have
    /// edited the object directly through SerializedPropertyHelper.GetTargetObjectOfProperty
    ///
    /// Using property.serializedObject.ApplyModifiedProperties() will apply the changes done through
    /// the property, and discard any changes done directly to the object
    /// </summary>
    public static void SetUnderlyingObjectDirty(this SerializedProperty property)
    {
        var targetObject = property.serializedObject.targetObject;
        EditorUtility.SetDirty(targetObject);
        var asComp = targetObject as Component;
        if (asComp != null)
        {
            EditorSceneManager.MarkSceneDirty(asComp.gameObject.scene);
        }
    }

    public static SerializedProperty ParentProperty(this SerializedProperty prop) {
        var path = prop.propertyPath;
        var parentPathParts = path.Split('.');
        string parentPath = "";
        for (int i = 0; i < parentPathParts.Length - 1; i++) {
            parentPath += parentPathParts[i];
            if (i < parentPathParts.Length - 2)
                parentPath += ".";
        }

        var parentProp = prop.serializedObject.FindProperty(parentPath);
        if (parentProp == null) {
            Debug.LogError("Couldn't find parent " + parentPath + ", child path is " + prop.propertyPath);
        }

        return parentProp;
    }

    public static SerializedProperty FindSiblingAttribute(this SerializedProperty prop, string siblingName) {
        var oldPath = prop.propertyPath;

        var pathSplit = oldPath.Split('.');

        StringBuilder siblingBuilder = new StringBuilder();
        for (int i = 0; i < pathSplit.Length; i++) {
            if (i < pathSplit.Length - 1) {
                siblingBuilder.Append(pathSplit[i]);
                siblingBuilder.Append('.');
            }
            else {
                siblingBuilder.Append(siblingName);
            }

        }

        prop = prop.serializedObject.FindProperty(siblingBuilder.ToString());
        return prop;
    }

    public static IEnumerable<SerializedProperty> IterateArray(this SerializedProperty prop) {
        if (!prop.isArray) {
            Debug.LogError($"Trying to iterate the property {prop.displayName}, but it's not an array");
            yield break;
        }

        for (int i = 0; i < prop.arraySize; i++) {
            yield return prop.GetArrayElementAtIndex(i);
        }
    }

    public static IEnumerable<(int, SerializedProperty)> IterateArrayWithIndex(this SerializedProperty prop) {
        if (!prop.isArray) {
            Debug.LogError($"Trying to iterate the property {prop.displayName}, but it's not an array");
            yield break;
        }

        for (int i = 0; i < prop.arraySize; i++) {
            yield return (i, prop.GetArrayElementAtIndex(i));
        }
    }

    public static (int, SerializedProperty) FindIndex(this SerializedProperty arrayProp, Predicate<SerializedProperty> predicate) {
        foreach (var (idx, prop) in IterateArrayWithIndex(arrayProp)) {
            if (predicate(prop))
                return (idx, prop);
        }
        return (-1, null);
    }

    public static IEnumerable<(int, SerializedProperty)> IterateArrayWithIndex_Reverse(this SerializedProperty prop) {
        if (!prop.isArray) {
            Debug.LogError($"Trying to iterate the property {prop.displayName}, but it's not an array");
            yield break;
        }

        for (int i = prop.arraySize - 1; i >= 0; i--) {
            yield return (i, prop.GetArrayElementAtIndex(i));
        }
    }

    public static string ListProperties(this SerializedProperty property, bool includeInvisible = false) {
        StringBuilder sb = new StringBuilder();

        var start = property.Copy();
        var end   = property.Copy();

        end.Next(false);

        ListProperties(start, end, includeInvisible, sb, 0);

        return sb.ToString();
    }

    private static void ListProperties(SerializedProperty startProp, SerializedProperty endProp, bool includeInvisible, StringBuilder sb, int indent) {
        var currentProp = startProp;

        bool cont = true;
        do {
            for (int i = 0; i < indent; i++) {
                sb.Append(' ');
                sb.Append(' ');
            }

            sb.Append(currentProp.name);
            sb.Append(": ");
            sb.Append(PrintValue(currentProp));
            sb.Append('\n');

            var hasChildren = includeInvisible ? currentProp.hasChildren : currentProp.hasVisibleChildren;

            if (hasChildren) {
                var childIterator = currentProp.Copy();
                var childEnd = currentProp.Copy();

                if (includeInvisible)
                    childIterator.Next(true);
                else
                    childIterator.NextVisible(true);

                childEnd.Next(false);

                ListProperties(childIterator, childEnd, includeInvisible, sb, indent + 1);
            }

            cont = currentProp.Next(false);
        } while (cont && currentProp.propertyPath != endProp.propertyPath);
    }

    public static bool StringArrayDifferentFrom(this SerializedProperty stringArrayProp, IList<string> array, StringComparison comparison = StringComparison.Ordinal) {
        if (!stringArrayProp.isArray) {
            throw new ArgumentException($"property isn't an array! It's property type is {stringArrayProp.propertyType}", nameof(stringArrayProp));
        }
        if (stringArrayProp.arrayElementType != "string") {
            throw new ArgumentException($"property isn't an array of strings! It's an array of {stringArrayProp.arrayElementType}", nameof(stringArrayProp));
        }

        if (array == null)
            return true;

        if (stringArrayProp.arraySize != array.Count)
            return true;

        for (int i = 0; i < array.Count; i++) {
            if (!string.Equals(stringArrayProp.GetArrayElementAtIndex(i).stringValue, array[i], comparison))
                return true;
        }

        return false;
    }

    public static string PrintValue(this SerializedProperty prop) {
        switch (prop.propertyType) {
            case SerializedPropertyType.Generic:
                return prop.type ?? "Generic?";
            case SerializedPropertyType.Integer:
                return prop.intValue.ToString(CultureInfo.InvariantCulture);
            case SerializedPropertyType.Boolean:
                return prop.boolValue.ToString(CultureInfo.InvariantCulture);
            case SerializedPropertyType.Float:
                return prop.floatValue.ToString(CultureInfo.InvariantCulture);
            case SerializedPropertyType.String:
                return prop.stringValue;
            case SerializedPropertyType.Color:
                return prop.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue == null ? "null" : prop.objectReferenceValue.ToString();
            case SerializedPropertyType.LayerMask:
                return ((LayerMask) prop.intValue).ToString();
            case SerializedPropertyType.Enum:
                return prop.enumValueIndex.ToString();
            case SerializedPropertyType.Vector2:
                return prop.vector2Value.ToString();
            case SerializedPropertyType.Vector3:
                return prop.vector3Value.ToString();
            case SerializedPropertyType.Vector4:
                return prop.vector4Value.ToString();
            case SerializedPropertyType.Rect:
                return prop.rectValue.ToString();
            case SerializedPropertyType.ArraySize:
                return "Array size = " + prop.intValue;
            case SerializedPropertyType.Character:
                return $"{(char) prop.intValue}";
            case SerializedPropertyType.AnimationCurve:
                return "Animation Curve";
            case SerializedPropertyType.Bounds:
                return prop.boundsValue.ToString();
            case SerializedPropertyType.Gradient:
                return "Where's gradient stored?";
            case SerializedPropertyType.Quaternion:
                return prop.quaternionValue.ToString();
            case SerializedPropertyType.ExposedReference:
                return prop.exposedReferenceValue.ToString();
            case SerializedPropertyType.FixedBufferSize:
                return prop.fixedBufferSize.ToString();
            case SerializedPropertyType.Vector2Int:
                return prop.vector2IntValue.ToString();
            case SerializedPropertyType.Vector3Int:
                return prop.vector3IntValue.ToString();
            case SerializedPropertyType.RectInt:
                return prop.rectIntValue.ToString();
            case SerializedPropertyType.BoundsInt:
                return prop.boundsIntValue.ToString();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

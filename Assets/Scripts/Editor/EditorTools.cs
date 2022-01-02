using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

public static class EditorTools {

    public static readonly Color comfyRed = new Color(1f, 0.4f, 0.4f, 1f);
    public static readonly Color comfyGreen = new Color(0.4f, 1f, 0.4f, 1f);
    public static readonly Color comfyBlue = new Color(0.4f, 0.4f, 1f, 1f);

    public static readonly Vector2 defaultPadding = new Vector2(15, 15);

    public static IEnumerable<T> GetAllAssetsOfType<T> () where T : UnityEngine.Object {
        return AssetDatabase.FindAssets($"t:{typeof(T).FullName}").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<T>);
    }

    public static bool TryFindAssetOfType<T> (out T output) where T : UnityEngine.Object {
        output = GetAllAssetsOfType<T>().FirstOrDefault();
        return output != null;
    }

    public static void BeginPadding () => BeginPadding(new Vector2(15, 15));

    public static void BeginPadding (Vector2 padding) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(padding.x);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(padding.y);
    }

    public static void EndPadding () => EndPadding(defaultPadding);

    public static void EndPadding (Vector2 padding) {
        GUILayout.Space(padding.y);
        EditorGUILayout.EndVertical();
        GUILayout.Space(padding.x);
        EditorGUILayout.EndHorizontal();
    }

    public class PaddingScope : System.IDisposable {

        private readonly Vector2 padding;

        public PaddingScope () {
            this.padding = defaultPadding;
            BeginPadding(padding);
        }

        public PaddingScope (Vector2 padding) {
            this.padding = padding;
            BeginPadding(padding);
        }

        void IDisposable.Dispose () {
            EndPadding(padding);
        }
        
    }

}
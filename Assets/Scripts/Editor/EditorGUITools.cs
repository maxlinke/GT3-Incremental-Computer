using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class EditorGUITools {

    private static float SLH => EditorGUIUtility.singleLineHeight;
    private static float SVS => EditorGUIUtility.standardVerticalSpacing;

    public static Rect ShrinkRect (Rect inputRect, float shrinkAmount) {
        return new Rect(
            x: inputRect.x + shrinkAmount,
            y: inputRect.y + shrinkAmount,
            width: inputRect.width - 2 * shrinkAmount,
            height: inputRect.height - 2 * shrinkAmount
        );
    }

    public static Rect RemoveLine (ref Rect position) {
        return RemoveLines(ref position, 1, SLH);
    }

    public static Rect RemoveLine (ref Rect position, float lineHeight) {
        return RemoveLines(ref position, 1, lineHeight);
    }

    public static Rect RemoveLines (ref Rect position, int count) {
        return RemoveLines(ref position, count, SLH);
    }

    public static Rect RemoveLines (ref Rect position, int count, float lineHeight) {
        var height = (count * lineHeight) + ((count - 1) * SVS);
        var output = new Rect(position.x, position.y, position.width, height);
        position = new Rect(position.x, position.y + height + SVS, position.width, position.height - height - SVS);
        return output;
    }

    public static Rect RemoveRectFromLeft (ref Rect position, float width, float space = 2) {
        var output = new Rect(
            x: position.x,
            y: position.y,
            width: width,
            height: position.height
        );
        position = new Rect(
            x: position.x + width + space,
            y: position.y,
            width: position.width - width - space,
            height: position.height
        );
        return output;
    }

    public static Rect RemoveRectFromRight (ref Rect position, float width, float space = 2) {
        position = new Rect(
            x: position.x,
            y: position.y,
            width: position.width - width - space,
            height: position.height
        );
        return new Rect(
            x: position.x + position.width + space,
            y: position.y,
            width: width,
            height: position.height
        );
    }

    public static void DrawSprite (Rect position, Sprite sprite, ScaleMode scaleMode = ScaleMode.StretchToFill) {
        var spriteTex = sprite.texture;
        var texSize = new Vector2Int(spriteTex.width, spriteTex.height);
        var spriteRect = sprite.textureRect;
        var spriteAspectRatio = spriteRect.width / spriteRect.height;
        var positionAspectRatio = position.width / position.height;
        var ratioRatio = (positionAspectRatio / spriteAspectRatio);
        spriteRect.x /= texSize.x;
        spriteRect.y /= texSize.y;
        spriteRect.width /= texSize.x;
        spriteRect.height /= texSize.y;
        switch(scaleMode){
            case ScaleMode.ScaleToFit:
                if(spriteAspectRatio > positionAspectRatio){
                    var newHeight = position.height * ratioRatio;
                    position.y += (position.height - newHeight) / 2;
                    position.height = newHeight;
                }else{
                    var newWidth = position.width / ratioRatio;
                    position.x += (position.width - newWidth) / 2;
                    position.width = newWidth;
                }
                break;
            case ScaleMode.ScaleAndCrop:
                if(spriteAspectRatio < positionAspectRatio){
                    var newHeight = spriteRect.height / ratioRatio;
                    spriteRect.y += (spriteRect.height - newHeight) / 2;
                    spriteRect.height = newHeight;
                }else{
                    var newWidth = spriteRect.width * ratioRatio;
                    spriteRect.x += (spriteRect.width - newWidth) / 2;
                    spriteRect.width = newWidth;
                }
                break;
        }
        GUI.DrawTextureWithTexCoords(position, spriteTex, spriteRect);
    }

    public class ColorScope : System.IDisposable {

        private readonly Color revertColor;

        public ColorScope (Color color) {
            revertColor = GUI.color;
            GUI.color = color;
        }

        void IDisposable.Dispose () {
            GUI.color = revertColor;
        }
        
    }

    public class BackgroundColorScope : System.IDisposable {

        private readonly Color revertColor;

        public BackgroundColorScope (Color color) {
            revertColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        void IDisposable.Dispose () {
            GUI.backgroundColor = revertColor;
        }
        
    }

}

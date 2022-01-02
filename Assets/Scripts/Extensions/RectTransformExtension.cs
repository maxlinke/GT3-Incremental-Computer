using UnityEngine;

public static class RectTransformExtension {

    public static RectTransform RectTransformParent (this RectTransform rt) {
        if(rt.parent == null){
            return null;
        }
        return (RectTransform)rt.parent;
    }

    public static void SetAnchoredX(this RectTransform rt, float x) {
        var anchoredPosition = rt.anchoredPosition;
        anchoredPosition.x = x;
        rt.anchoredPosition = anchoredPosition;
    }

    public static void SetAnchoredY(this RectTransform rt, float y) {
        var anchoredPosition = rt.anchoredPosition;
        anchoredPosition.y = y;
        rt.anchoredPosition = anchoredPosition;
    }

    public static void MoveAnchoredX(this RectTransform rt, float targetX, float moveSpeed) {
        var anchoredPosition = rt.anchoredPosition;
        anchoredPosition.x = Mathf.MoveTowards(anchoredPosition.x, targetX, moveSpeed);
        rt.anchoredPosition = anchoredPosition;
    }

    public static void LerpAnchoredX(this RectTransform rt, float targetX, float moveSpeed) {
        var anchoredPosition = rt.anchoredPosition;
        anchoredPosition.x = Mathf.Lerp(anchoredPosition.x, targetX, moveSpeed);
        rt.anchoredPosition = anchoredPosition;
    }

    public static void MoveAnchoredY(this RectTransform rt, float targetY, float moveSpeed) {
        var anchoredPosition = rt.anchoredPosition;
        anchoredPosition.y = Mathf.MoveTowards(anchoredPosition.y, targetY, moveSpeed);
        rt.anchoredPosition = anchoredPosition;
    }

    public static void SetAnchorAndPivot (this RectTransform rt, float x, float y) {
        rt.SetAnchorAndPivot(new Vector2(x, y));
    }

    public static void SetAnchorAndPivot (this RectTransform rt, Vector2 newValue) {
        rt.anchorMin = newValue;
        rt.anchorMax = newValue;
        rt.pivot = newValue;
    }

    public static void SetAnchor(this RectTransform rt, float newAnchorX, float newAnchorY) {
        rt.SetAnchor(new Vector2(newAnchorX, newAnchorY));
    }

    public static void SetAnchor(this RectTransform rt, Vector2 newAnchor) {
        rt.anchorMin = newAnchor;
        rt.anchorMax = newAnchor;
    }

    public static void SetAnchorMaxX(this RectTransform rt, float x) {
        var anchorMax = rt.anchorMax;
        anchorMax.x = x;
        rt.anchorMax = anchorMax;
    }

    public static void SetAnchorMaxY(this RectTransform rt, float y) {
        var anchorMax = rt.anchorMax;
        anchorMax.y = y;
        rt.anchorMax = anchorMax;
    }

    public static void SetAnchorMinX(this RectTransform rt, float x) {
        var anchorMin = rt.anchorMin;
        anchorMin.x = x;
        rt.anchorMin = anchorMin;
    }

    public static void SetAnchorMinY(this RectTransform rt, float y) {
        var anchorMin = rt.anchorMin;
        anchorMin.y = y;
        rt.anchorMin = anchorMin;
    }

    public static void SetSizeDeltaX(this RectTransform rt, float x) {
        var sizeDelta = rt.sizeDelta;
        sizeDelta.x = x;
        rt.sizeDelta = sizeDelta;
    }

    public static void SetSizeDeltaY(this RectTransform rt, float y) {
        var sizeDelta = rt.sizeDelta;
        sizeDelta.y = y;
        rt.sizeDelta = sizeDelta;
    }

    public static void SetWidth(this RectTransform rt, float width) {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static void SetHeight(this RectTransform rt, float height) {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetSize(this RectTransform rt, Vector2 size) {
        rt.SetSize(size.x, size.y);
    }

    public static void SetSize(this RectTransform rt, float width, float height) {
        rt.SetWidth(width);
        rt.SetHeight(height);
    }

    public static Vector2 AnchorAverage(this RectTransform rt) {
        return 0.5f * (rt.anchorMin + rt.anchorMax);
    }
    
}
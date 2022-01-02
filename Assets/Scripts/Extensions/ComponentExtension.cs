using UnityEngine;

public static class ComponentExtension {

    public static void SetGOActive(this Component component, bool value) {
        component.gameObject.SetActive(value);
    }

    public static bool GOActiveSelf(this Component component) {
        return component.gameObject.activeSelf;
    }

}

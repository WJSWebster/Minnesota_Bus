using UnityEngine;
using UnityEditor;
using UnityScript;

public class ExtendedHotkeys : ScriptableObject {

    public static GameObject go;
    public static Vector3 goPos;

    [MenuItem("Will's Extended Hotkeys/Create new C# Script %&_n")]
    static void CreateNewScript()
    {
        Debug.Log("Creating new script...");
    }
}

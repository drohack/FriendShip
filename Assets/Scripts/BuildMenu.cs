#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
public class BuildMenu : ScriptableObject
{
    //-------------------------------------------------------------------------------------------------------------------------
    [MenuItem("Config/Config Oculus", false, 2000)]
    public static void BuildSetOculus()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT;OCULUS;DISABLESTEAMWORKS");

        Debug.Log("Setting build to OCULUS.");
    }

    //-------------------------------------------------------------------------------------------------------------------------
    [MenuItem("Config/Config Vive", false, 2000)]
    public static void BuildSetVive()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT;VIVE");

        Debug.Log("Setting build to VIVE.");
    }

    //-------------------------------------------------------------------------------------------------------------------------

}
#endif
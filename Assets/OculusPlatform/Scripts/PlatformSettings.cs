namespace Oculus.Platform
{
  using UnityEngine;
  using System.Collections;

#if UNITY_EDITOR
  [UnityEditor.InitializeOnLoad]
#endif
  public sealed class PlatformSettings : ScriptableObject
  {
    public static string AppID
    {
      get { return Instance.ovrAppID; }
      set { Instance.ovrAppID = value; }
    }

    public static string MobileAppID
    {
      get { return Instance.ovrMobileAppID; }
      set { Instance.ovrMobileAppID = value; }
    }

    public static bool UseStandalonePlatform
    {
      get { return Instance.ovrUseStandalonePlatform; }
      set { Instance.ovrUseStandalonePlatform = value; }
    }

    [SerializeField]
    private string ovrAppID = "";

    [SerializeField]
    private string ovrMobileAppID = "";

#if UNITY_EDITOR_WIN
    [SerializeField]
    private bool ovrUseStandalonePlatform = false;
#else
    [SerializeField]
    private bool ovrUseStandalonePlatform = true;
#endif

    private static PlatformSettings instance;
    public static PlatformSettings Instance
    {
      get
      {
        if (instance == null)
        {
          instance = Resources.Load<PlatformSettings>("OculusPlatformSettings");
        }
        return instance;
      }

      set
      {
        instance = value;
      }
    }
  }
}

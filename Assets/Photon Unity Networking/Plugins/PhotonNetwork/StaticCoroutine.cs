using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

internal class StaticCoroutine : MonoBehaviour
{
    private static object instanceLock = new object();
    static public StaticCoroutine instance; //the instance of our class that will do the work

    public static StaticCoroutine Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StaticCoroutine();
            }
            return instance;
        }
    }

    void Awake()
    {
        lock (instanceLock)
        {
            if (instance == null)
            {
                DontDestroyOnLoad(this);
                instance = this;
            }
            else
            {
                Debug.LogError("StaticCoroutine: Attempt to create multiple instances of StaticCoroutine");
            }
        }
    }

    System.Collections.IEnumerator LoadLevelAsync(string levelName)
    { //the coroutine that runs on our monobehaviour instance
        AsyncOperation async = SceneManager.LoadSceneAsync(levelName);
        async.allowSceneActivation = true;
        while (!async.isDone)
        {
            yield return null;
        }
    }

    System.Collections.IEnumerator LoadLevelAsync(int levelNum)
    { //the coroutine that runs on our monobehaviour instance
        AsyncOperation async = SceneManager.LoadSceneAsync(levelNum);
        async.allowSceneActivation = true;
        while (!async.isDone)
        {
            yield return null;
        }
    }

    static public void DoCoroutine(string levelName)
    {
        instance.StartCoroutine("LoadLevelAsync", levelName); //this will launch the coroutine on our instance
    }

    static public void DoCoroutine(int levelNum)
    {
        instance.StartCoroutine("LoadLevelAsync", levelNum); //this will launch the coroutine on our instance
    }
}
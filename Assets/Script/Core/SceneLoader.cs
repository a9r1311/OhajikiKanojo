using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    //シーン読み込み用
    public static void Load(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadSceneManager
{
    public static List<int> VisitedSceneIndexList = new List<int>();
    public static int LastVisitedSceneIndex;

    public static void LoadScene(int sceneIndex, bool addtolist=true)
    {
        if(addtolist) VisitedSceneIndexList.Add(sceneIndex);
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}


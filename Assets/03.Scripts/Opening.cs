using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Opening : MonoBehaviour
{

    AssetBundle assetBundle;

    string url = "";
    int version = 1;
    [System.Obsolete]
    WWW www;

     void Awake()
    {
        //스크린 절전 모드 안가게 
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // ftp 서버 or 웹 서버 주소 + 애셋번들 파일 위치
        //  url = "http://210.122.7.164/dinoScenes/ScLevel_1.0.6.unity3d";
       // url = "file:///C:\\Bundles\\study.study";
        url = "http://localhost:8080/study.study";

    }

    [System.Obsolete]
    IEnumerator Start()
    {
        www = WWW.LoadFromCacheOrDownload(url, version);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error.ToString());
        }
        else
        {
            //내려받은 번들을 메모리에 로드
            assetBundle = www.assetBundle;
        }
    }

    [System.Obsolete]
    void OnGUI()
    {
        if(www.isDone && GUI.Button(new Rect(20,50,100,30), "Start Game"))
        {
            LoadScenes();
        }

        GUI.Label(new Rect(20, 20, 200, 30), "Downloading..." + (www.progress * 100.0f).ToString() + "%");
    }

    void LoadScenes()
    {
        SceneManager.LoadScene("scLevel1");
        SceneManager.LoadScene("scLogic", LoadSceneMode.Additive);
    }
}

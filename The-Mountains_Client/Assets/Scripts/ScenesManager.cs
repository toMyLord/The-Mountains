using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager m_instance;
    public static ScenesManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    private AsyncOperation asyncRes;
    private GameObject changeSceneCanvas;
    private Slider progressSlider;

    private void Awake()
    {
        m_instance = this;
        DontDestroyOnLoad(this);    //切换场景保留
        changeSceneCanvas = transform.Find("ChangeSceneCanvas").gameObject;
        changeSceneCanvas.SetActive(false);
        progressSlider = changeSceneCanvas.transform.Find("ProgressSlider").GetComponent<Slider>();
        progressSlider.value = 0;
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine("ChangeSceneAsync", sceneName);
    }

    IEnumerator ChangeSceneAsync(string sceneName)
    {
        changeSceneCanvas.SetActive(true);
        asyncRes = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncRes.allowSceneActivation = false;
        while(asyncRes.progress<0.9f)
        {
            progressSlider.value = asyncRes.progress;
            yield return new WaitForEndOfFrame();
        }
        progressSlider.value = asyncRes.progress;
        asyncRes.allowSceneActivation = true;
        changeSceneCanvas.SetActive(false);
        yield return null;
    }
}

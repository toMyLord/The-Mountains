using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    private static DebugManager instance;
    public static DebugManager Instance
    {
        get
        {
            return instance;
        }
    }
    private Text fpsText;
    private Button debugInfoPanelSwitch;    //debug面板显示/关闭按钮
    private Text debugInfoText;
    private bool debugInfoPanelIsOpen;
    private GameObject debugPanel;
    private Scrollbar debugInfoScrollBar;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        debugInfoPanelIsOpen = false;
        Application.targetFrameRate = 60;
        fpsText = transform.Find("FPSText").GetComponent<Text>();
        debugInfoPanelSwitch = transform.Find("SwitchButton").GetComponent<Button>();
        debugInfoText = transform.Find("DebugBG/DebugInfo/DebugInfoContent/DebugText").GetComponent<Text>();
        debugInfoScrollBar = transform.Find("DebugBG/DebugInfo/Scrollbar").GetComponent<Scrollbar>();
        debugPanel = transform.Find("DebugBG").gameObject;
        debugInfoPanelSwitch.onClick.AddListener(OnSwitchClick);
        StartCoroutine("FPSTextUpdate");
    }

    IEnumerator FPSTextUpdate() //FPS更新
    {
        while (true)
        {
            fpsText.text = "FPS: " + (int)(1 / Time.deltaTime);
            yield return new WaitForSeconds(1);
        }
    }   

    void OnSwitchClick()
    {
        debugInfoPanelIsOpen = !debugInfoPanelIsOpen;
        if(debugInfoPanelIsOpen)
        {
            debugPanel.SetActive(true);
        }
        else
        {
            debugPanel.SetActive(false);
        }
    }

    public void Log(string debugInfo)
    {
        debugInfoText.text += (debugInfo + '\n');
        debugInfoScrollBar.value = -1f;
    }
}

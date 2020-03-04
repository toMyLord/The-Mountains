using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedbackSpawner : MonoBehaviour  //该类用于生成UI提示信息
{
    [SerializeField]
    private GameObject testTip;
    private static UIFeedbackSpawner m_instance;
    public static UIFeedbackSpawner Instance
    {
        get
        {
            return m_instance;
        }
    }
    private List<GameObject> tipList;
    private GameObject tipCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(this);    //切换场景保留
        m_instance = this;
        tipList = new List<GameObject>();
        tipCanvas = transform.Find("TipCanvas").gameObject;
    }

    /// <summary>
    /// 在屏幕指定位置生成UI Tip
    /// </summary>
    /// <param name="tipStr">提示框内容</param>
    /// <param name="tipPos">提示框位置</param>
    /// <param name="tipLifeTime">提示框持续时间,0表示不消失</param>
    public void ShowTip(string tipStr, Vector3 tipPos, float tipLifeTime)
    {
        ClearTip();
        GameObject tip = Instantiate(testTip, tipCanvas.transform.position + tipPos, Quaternion.identity, tipCanvas.transform);
        Text tiptext = tip.transform.Find("TipText").GetComponent<Text>();
        tiptext.text = tipStr;
        if (tipLifeTime != 0)
        {
            Destroy(tip, tipLifeTime);
        }
        else
        {
            tipList.Add(tip);
        }
    }

    public void ClearTip()  //清楚屏幕上的永久Tip
    {
        int count = tipList.Count;
        if(count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Destroy(tipList[i]);
            }
            tipList.Clear();
        }
    }
}

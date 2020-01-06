using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private static UIController m_instance;
    public static UIController Instance
    {
        get
        {
            return m_instance;
        }
    }

    private GameObject m_initWindow;
    private GameObject m_loginWindow;
    private GameObject m_registerWindow;
    public GameObject InitWindow
    {
        get
        {
            return m_initWindow;
        }
    }
    public GameObject LoginWindow
    {
        get
        {
            return m_loginWindow;
        }
    }
    public GameObject RegisterWindow
    {
        get
        {
            return m_registerWindow;
        }
    }
    private void Start()
    {
        m_instance = this;
        m_initWindow = transform.Find("InitWindow").gameObject;
        m_loginWindow = transform.Find("LoginWindow").gameObject;
        m_registerWindow = transform.Find("RegisterWindow").gameObject;
        m_initWindow.SetActive(true);         //初始化登录场景各个窗口
        m_loginWindow.SetActive(false);
        m_registerWindow.SetActive(false);
    }

}

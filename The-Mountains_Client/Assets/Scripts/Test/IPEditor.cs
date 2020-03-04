using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class IPEditor : MonoBehaviour
{
    public static IPEditor Instance;
    //Test For Server
    public InputField loginIPInputField;
    public InputField loginPortInputField;
    public InputField GameIPInputField;
    public InputField GamePortInputField;
    private Button okButton;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitIPInputField();
    }
    void InitIPInputField()
    {
        loginIPInputField = transform.Find("LoginIPInputField").GetComponent<InputField>();
        loginPortInputField = transform.Find("LoginPortInputField").GetComponent<InputField>();
        GameIPInputField = transform.Find("GameIPInputField").GetComponent<InputField>();
        GamePortInputField = transform.Find("GamePortInputField").GetComponent<InputField>();
        okButton = transform.Find("OKButton").GetComponent<Button>();
        okButton.onClick.AddListener(OnIPButtonClick);
    }
    void OnIPButtonClick()
    {
        Debug.Log("跳转场景至Login");
        ScenesManager.Instance.ChangeScene("Login");
    }
    //Test For Server
}

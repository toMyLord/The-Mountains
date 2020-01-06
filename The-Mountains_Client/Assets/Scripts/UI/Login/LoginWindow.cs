using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{
    private InputField AccountInputField;
    private InputField PasswordInputField;
    private Button LoginButton;
    private Button RegisterButton;
    private Button TouristButton;

    private void Awake()
    {
        AccountInputField = transform.Find("Account/InputField").GetComponent<InputField>();
        PasswordInputField = transform.Find("Password/InputField").GetComponent<InputField>();
        LoginButton = transform.Find("LoginButton").GetComponent<Button>();
        RegisterButton = transform.Find("RegisterButton").GetComponent<Button>();
        TouristButton = transform.Find("TouristButton").GetComponent<Button>();
        LoginButton.onClick.AddListener(OnLoginButtonClicked);
        RegisterButton.onClick.AddListener(OnRegisterButtonClicked);
        TouristButton.onClick.AddListener(OnTouristButtonClicked);
    }

    private void OnEnable()
    {
        AccountInputField.text = "";
        PasswordInputField.text = "";
    }

    //TODO 检查输入格式

    void OnLoginButtonClicked()
    {
        //TODO 向服务器发送填写好的账号密码
        Debug.Log("正在验证用户");
    }

    void OnRegisterButtonClicked()
    {
        UIController.Instance.InitWindow.SetActive(false);
        UIController.Instance.LoginWindow.SetActive(false);
        UIController.Instance.RegisterWindow.SetActive(true);
    }

    void OnTouristButtonClicked()
    {
        //TODO 发送游客登录请求
        Debug.Log("正在以游客登录");
    }
}
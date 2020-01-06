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

    void OnLoginButtonClicked()     //登录按钮
    {
        //TODO 向服务器发送填写好的账号密码
        Debug.Log("正在登录，验证用户中");
    }

    void OnRegisterButtonClicked()  //注册按钮
    {
        UIController.Instance.InitWindow.SetActive(false);
        UIController.Instance.LoginWindow.SetActive(false);
        UIController.Instance.RegisterWindow.SetActive(true);
    }

    void OnTouristButtonClicked()   //游客登录
    {
        //TODO 发送游客登录请求
        Debug.Log("正在以游客登录");
    }

    void OnTouristApplicationSuccess()  //收到服务器的登录申请通过包时调用
    {
        //TODO 确认保存用户信息，切换场景，同RegisteWindow的注册按钮一样
        Debug.Log("游客登录成功");
    }
}
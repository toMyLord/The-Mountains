using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterWindow : MonoBehaviour
{
    public InputField AccountInputField;
    public InputField PasswordInputField;
    public InputField ConfirmPasswordInputField;
    public InputField EmailInputField;
    public Button RegisterButton;
    public Button BackButton;

    private void Awake()
    {
        AccountInputField = transform.Find("Account/InputField").GetComponent<InputField>();
        PasswordInputField = transform.Find("Password/InputField").GetComponent<InputField>();
        ConfirmPasswordInputField = transform.Find("ConfirmPassword/InputField").GetComponent<InputField>();
        EmailInputField = transform.Find("Email/InputField").GetComponent<InputField>();
        RegisterButton = transform.Find("RegisterButton").GetComponent<Button>();
        BackButton = transform.Find("BackButton").GetComponent<Button>();
        BackButton.onClick.AddListener(OnBackButtonClicked);
        RegisterButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private void OnEnable()
    {
        AccountInputField.text = "";
        PasswordInputField.text = "";
        ConfirmPasswordInputField.text = "";
        EmailInputField.text = "";
    }

    //TODO 检查输入格式

    void OnRegisterButtonClicked()
    {
        //TODO 向服务器发送注册信息
        Debug.Log("正在注册");
    }

    void OnBackButtonClicked()
    {
        UIController.Instance.InitWindow.SetActive(false);
        UIController.Instance.LoginWindow.SetActive(true);
        UIController.Instance.RegisterWindow.SetActive(false);
    }
}

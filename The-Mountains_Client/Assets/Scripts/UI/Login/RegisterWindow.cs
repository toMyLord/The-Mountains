#define DEBUG_MODE
#define TEXT_DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterWindow : MonoBehaviour
{
    private InputField AccountInputField;    //输入框
    private InputField PasswordInputField;
    private InputField ConfirmPasswordInputField;
    private InputField EmailInputField;
    private Text AccountTipText;     //提示信息
    private Text PasswordTipText;
    private Text ConfirmPasswordTipText;
    private Text EmailTipText;
    private Button RegisterButton;   //按钮
    private Button BackButton;
    private bool AccountCorrect;    //点击注册时会检测这些信息，所有都正确时才会发送注册包
    private bool PasswordCorrect;
    private bool ConfirmPasswordCorrect;
    private bool EmailCorrect;
    private bool AccountExist;
    private bool EmailExist;

    private void Awake()    //初始化
    {
        AccountInputField = transform.Find("Account/InputField").GetComponent<InputField>();
        PasswordInputField = transform.Find("Password/InputField").GetComponent<InputField>();
        ConfirmPasswordInputField = transform.Find("ConfirmPassword/InputField").GetComponent<InputField>();
        EmailInputField = transform.Find("Email/InputField").GetComponent<InputField>();

        AccountTipText = transform.Find("Account/TipText").GetComponent<Text>();
        PasswordTipText = transform.Find("Password/TipText").GetComponent<Text>();
        ConfirmPasswordTipText = transform.Find("ConfirmPassword/TipText").GetComponent<Text>();
        EmailTipText = transform.Find("Email/TipText").GetComponent<Text>();

        RegisterButton = transform.Find("RegisterButton").GetComponent<Button>();
        BackButton = transform.Find("BackButton").GetComponent<Button>();

        AccountInputField.onEndEdit.AddListener(OnAccountInputFieldEndEdit);
        PasswordInputField.onEndEdit.AddListener(OnPasswordInputFieldEndEdit);
        ConfirmPasswordInputField.onEndEdit.AddListener(OnConfirmPasswordInputFieldEndEdit);
        EmailInputField.onEndEdit.AddListener(OnEmailInputFieldEndEdit);

        BackButton.onClick.AddListener(OnBackButtonClicked);    //按钮触发函数
        RegisterButton.onClick.AddListener(OnRegisterButtonClicked);
        BackButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);    //播放按钮音频
        RegisterButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);

        ConnectionManager.Instance.RecvRegisterDetectInfo += OnOnlineRegisterDetectionFinish;  //收到来自服务器的检测包时触发

        Init();
    }
    private void OnDestroy()
    {
        ConnectionManager.Instance.RecvRegisterDetectInfo -= OnOnlineRegisterDetectionFinish;
    }

    void UIStatusSetting(bool status)
    {
        AccountInputField.enabled = status;
        PasswordInputField.enabled = status;
        ConfirmPasswordInputField.enabled = status;
        EmailInputField.enabled = status;
        BackButton.enabled = status;
        RegisterButton.enabled = status;
    }

    void OnOnlineRegisterDetectionFinish(RegisterDetectFeedback registerDetectFeedback)  //注册存在检测后收到回复时更新tiptext
    {
#if DEBUG_MODE
                    Debug.Log("已收到注册检测包回执");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到注册检测包回执");
#endif
        AccountExist = registerDetectFeedback.IsAccountExist;
        EmailExist = registerDetectFeedback.IsEmailExist;
        if (AccountInputField.text != "")
        {
            if (AccountInputField.text != null && AccountExist)
            {
                TipTextSetting(AccountTipText, "该账号已存在", Color.red);
            }
            else
            {
                TipTextSetting(AccountTipText, "该账号可以使用", Color.green);
            }
        }
        if (EmailInputField.text != "")
        {
            if (EmailInputField.text != null && EmailExist)
            {
                TipTextSetting(EmailTipText, "该邮箱已存在", Color.red);
            }
            else
            {
                TipTextSetting(EmailTipText, "该邮箱可以使用", Color.green);
            }
        }
    }

    void OnAccountInputFieldEndEdit(string str)     //账号编辑结束时触发
    {
        string detectInfo = "";
        AccountCorrect = FormatDetecter.AccountDetect(str, out detectInfo);  //使用FormatDetecter类的static函数检测账号格式
        if (AccountCorrect)
        {
            TipTextSetting(AccountTipText, "正在检测账号...", Color.grey);
            //创建RegisterDetect包并发送给Login服务器
            RegisterDetect registerDetect = new RegisterDetect();
            registerDetect.Account = AccountInputField.text;
            registerDetect.Email = EmailInputField.text;
#if DEBUG_MODE
            Debug.Log("正在发送账号存在检测包至Login服务器");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("正在发送账号存在检测包至Login服务器");
#endif
            ConnectionManager.Instance.SendMSGToLoginConnection(ConnectionManager.SendLoginMSGType.RegisterDetect, registerDetect);
        }
        else
        {
            TipTextSetting(AccountTipText, detectInfo, Color.red);
#if DEBUG_MODE
            Debug.Log("账号不符合规范");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("账号不符合规范");
#endif
        }
    }

    void OnPasswordInputFieldEndEdit(string str)     //密码编辑结束时触发
    {
        string detectInfo = "";
        PasswordCorrect = FormatDetecter.PasswordDetect(str, out detectInfo);
        if(PasswordCorrect)
        {
            TipTextSetting(PasswordTipText, detectInfo, Color.green);
        }
        else
        {
            TipTextSetting(PasswordTipText, detectInfo, Color.red);
        }
        OnConfirmPasswordInputFieldEndEdit(ConfirmPasswordInputField.text);
    }

    void OnConfirmPasswordInputFieldEndEdit(string str)     //确认密码编辑结束时触发
    {
        //检测确认密码格式
        ConfirmPasswordCorrect = (str == PasswordInputField.text);
        if(ConfirmPasswordCorrect)
        {
            TipTextSetting(ConfirmPasswordTipText, "√", Color.green);
        }
        else
        {
            TipTextSetting(ConfirmPasswordTipText, "两次密码输入不一致", Color.red);
        }
    }

    void OnEmailInputFieldEndEdit(string str)     //邮箱编辑结束时触发
    {
        //检测邮箱格式
        string detectInfo = "";
        EmailCorrect = FormatDetecter.EmailDetect(str, out detectInfo);
        if (EmailCorrect)
        {
            TipTextSetting(EmailTipText, "正在检测邮箱...", Color.gray);
            //创建RegisterDetect包并发送给Login服务器
            RegisterDetect registerDetect = new RegisterDetect();
            registerDetect.Account = AccountInputField.text;
            registerDetect.Email = EmailInputField.text;
#if DEBUG_MODE
            Debug.Log("正在发送邮箱存在检测包至Login服务器");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("正在发送邮箱存在检测包至Login服务器");
#endif
            ConnectionManager.Instance.SendMSGToLoginConnection(ConnectionManager.SendLoginMSGType.RegisterDetect, registerDetect);
        }
        else
        {
            TipTextSetting(EmailTipText, "电子邮箱格式错误", Color.red);
        }
    }

    private void OnEnable() //弹出时触发
    {
        Init();
        
    }

    void TipTextSetting(Text tipText, string tipStr, Color tipColor)        //设置指定tiptext的内容、颜色
    {
        tipText.text = tipStr;
        tipText.color = tipColor;
    }

    void Init() //初始化，enable时调用
    {
        AccountInputField.text = "";
        PasswordInputField.text = "";
        ConfirmPasswordInputField.text = "";
        EmailInputField.text = "";
        TipTextSetting(AccountTipText, "", Color.green);
        TipTextSetting(PasswordTipText, "", Color.green);
        TipTextSetting(ConfirmPasswordTipText, "", Color.green);
        TipTextSetting(EmailTipText, "", Color.green);
        AccountCorrect = false;
        PasswordCorrect = false;
        ConfirmPasswordCorrect = false;
        EmailCorrect = false;
        AccountExist = true;
        EmailExist = true;
        UIStatusSetting(true);
    }

    void OnRegisterButtonClicked()  //注册按钮调用
    {
        if(AccountCorrect && PasswordCorrect && ConfirmPasswordCorrect && EmailCorrect && !AccountExist && !EmailExist)
        {
            //向服务器发送 RegisterLogin 包
            UIStatusSetting(false);
            RegisterLogin registerLogin = new RegisterLogin();
            UIFeedbackSpawner.Instance.ShowTip("正在注册...", Vector3.zero, 0);
            registerLogin.Account = AccountInputField.text;
            registerLogin.Password = PasswordInputField.text;
            registerLogin.Email = EmailInputField.text;
#if DEBUG_MODE
            Debug.Log("正在发送 RegisterLogin 包:" + "Account->" + registerLogin.Account + " " + "Password->" + registerLogin.Password + " " + "Email->" + registerLogin.Email);
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("正在发送 RegisterLogin 包:" + "Account->" + registerLogin.Account + " " + "Password->" + registerLogin.Password + " " + "Email->" + registerLogin.Email);
#endif
            ConnectionManager.Instance.SendMSGToLoginConnection(ConnectionManager.SendLoginMSGType.RegisterLogin, registerLogin);
        }
        else
        {
            string errorInfo = "";
            if(!AccountCorrect || !PasswordCorrect || !ConfirmPasswordCorrect || !EmailCorrect)
            {
                errorInfo = "注册信息有误,请重新填写";
            }
            else if(AccountExist)
            {
                errorInfo = "账号已存在";
            }
            else if(EmailExist)
            {
                errorInfo = "邮箱已被注册";
            }
            UIFeedbackSpawner.Instance.ShowTip(errorInfo, Vector3.zero, 1);
        }
    }

    void OnBackButtonClicked()      //返回按钮调用
    {
        LoginUIController.Instance.InitWindow.SetActive(false);
        LoginUIController.Instance.LoginWindow.SetActive(true);
        LoginUIController.Instance.RegisterWindow.SetActive(false);
    }
}

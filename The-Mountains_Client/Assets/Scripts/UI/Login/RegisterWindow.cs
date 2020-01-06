using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterWindow : MonoBehaviour
{
    public InputField AccountInputField;    //输入框
    public InputField PasswordInputField;
    public InputField ConfirmPasswordInputField;
    public InputField EmailInputField;
    public Text AccountTipText;     //提示信息
    public Text PasswordTipText;
    public Text ConfirmPasswordTipText;
    public Text EmailTipText;
    public Button RegisterButton;   //按钮
    public Button BackButton;
    private bool AccountCorrect;    //点击注册时会检测这些信息，所有都正确时才会发送注册包
    private bool PasswordCorrect;
    private bool ConfirmPasswordCorrect;
    private bool EmailCorrect;

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

        BackButton.onClick.AddListener(OnBackButtonClicked);
        RegisterButton.onClick.AddListener(OnRegisterButtonClicked);

        Init();
    }

    void OnOnlineAccountDetectionFinish(bool isExit)  //账号存在检测后收到回复时更新账号tiptext
    {
        Debug.Log("已收到账号检测包回执");
        if (isExit)
        {
            TipTextSetting(AccountTipText, "该账号已存在", Color.red);
            AccountCorrect = false;
        }
        else
        {
            TipTextSetting(AccountTipText, "该账号可以使用", Color.green);
            AccountCorrect = true;
        }
    }
    void OnOnlineEmailDetectionFinish(bool isExit)  //邮箱存在检测后收到回复时更新邮箱tiptext
    {
        Debug.Log("已收到电子邮箱检测包回执");
        if (isExit)
        {
            TipTextSetting(EmailTipText, "该邮箱已存在", Color.red);
            EmailCorrect = false;
        }
        else
        {
            TipTextSetting(EmailTipText, "该邮箱可以使用", Color.green);
            EmailCorrect = true;
        }
    }

    void OnAccountInputFieldEndEdit(string str)     //账号编辑结束时触发
    {
        //本地检测账号格式1.不含空格 2.开头字母 3.长度8~16位 4.不为空 5.账号是否已存在
        string detectInfo = "";
        bool isok = FormatDetecter.AccountDetect(str, out detectInfo);  //使用FormatDetecter类的static函数检测账号格式
        if (isok)
        {
            TipTextSetting(AccountTipText, "正在查询账号是否已被使用...", Color.grey);
            //TODO 发送存在检测包给Login服务器
            Debug.Log("正在发送账号存在检测包至Login服务器");
        }
        else
        {
            TipTextSetting(AccountTipText, detectInfo, Color.red);
            AccountCorrect = false;
            Debug.Log("账号不符合规范");
        }
    }

    void OnPasswordInputFieldEndEdit(string str)     //密码编辑结束时触发
    {
        //检测密码格式 1.不含空格2.长度8~16位 3.不为空
        string detectInfo = "";
        bool isok = FormatDetecter.PasswordDetect(str, out detectInfo);
        if(isok)
        {
            TipTextSetting(PasswordTipText, detectInfo, Color.green);
            PasswordCorrect = true;
        }
        else
        {
            TipTextSetting(PasswordTipText, detectInfo, Color.red);
            PasswordCorrect = false;
        }
    }

    void OnConfirmPasswordInputFieldEndEdit(string str)     //确认密码编辑结束时触发
    {
        //检测确认密码格式
        bool isSame = (str == PasswordInputField.text);
        if(isSame)
        {
            TipTextSetting(ConfirmPasswordTipText, "ok", Color.green);
            ConfirmPasswordCorrect = true;
        }
        else
        {
            TipTextSetting(ConfirmPasswordTipText, "两次密码输入不一致", Color.red);
            ConfirmPasswordCorrect = false;
        }
    }

    void OnEmailInputFieldEndEdit(string str)     //邮箱编辑结束时触发
    {
        //检测邮箱格式
        int len = str.Length;
        int indexOfAt = str.IndexOf("@");
        if (indexOfAt == -1 || indexOfAt == len - 1 || indexOfAt == 0 || indexOfAt != str.LastIndexOf("@"))
        {
            TipTextSetting(EmailTipText, "电子邮箱格式错误", Color.red);
            EmailCorrect = false;
        }
        else
        {
            TipTextSetting(EmailTipText, "ok", Color.green);
            //TODO 发送存在检测包给Login服务器
            Debug.Log("正在发送电子邮箱存在检测包至Login服务器");
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
    }

    void OnRegisterButtonClicked()  //注册按钮调用
    {
        
        if(AccountCorrect && PasswordCorrect && ConfirmPasswordCorrect && EmailCorrect)
        {
            //TODO 向服务器发送注册信息
            Debug.Log("正在注册");
        }
        else
        {
            Debug.Log("注册信息有误,请重新填写");
        }
    }

    void OnRegisteApplicationSuccess()  //收到服务器的注册申请通过包时调用
    {
        //TODO 确认保存用户信息，切换场景
        Debug.Log("用户注册成功");
    }

    void OnBackButtonClicked()      //返回按钮调用
    {
        UIController.Instance.InitWindow.SetActive(false);
        UIController.Instance.LoginWindow.SetActive(true);
        UIController.Instance.RegisterWindow.SetActive(false);
    }
}

#define DEBUG_MODE
#define TEXT_DEBUG_MODE
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
    private bool AccountCorrect;    //点击登录时会检测这些信息，所有都正确时才会发送登陆包
    private bool PasswordCorrect;

    private void Awake()
    {
        AccountInputField = transform.Find("Account/InputField").GetComponent<InputField>();
        PasswordInputField = transform.Find("Password/InputField").GetComponent<InputField>();

        AccountInputField.onEndEdit.AddListener(OnAccountInputFieldEndEdit);
        PasswordInputField.onEndEdit.AddListener(OnPasswordInputFieldEndEdit);

        LoginButton = transform.Find("LoginButton").GetComponent<Button>();
        RegisterButton = transform.Find("RegisterButton").GetComponent<Button>();
        TouristButton = transform.Find("TouristButton").GetComponent<Button>();

        LoginButton.onClick.AddListener(OnLoginButtonClicked);
        RegisterButton.onClick.AddListener(OnRegisterButtonClicked);
        TouristButton.onClick.AddListener(OnTouristButtonClicked);
        LoginButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);   //播放按钮音频
        RegisterButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);
        TouristButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);

        ConnectionManager.Instance.RecvUserInfo += OnLoginApplicationFinish;   //服务器反馈包触发事件
        ConnectionManager.Instance.RecvLoginDetectInfo += OnLoginDetectFinish;
        ConnectionManager.Instance.RecvTouristInfo += OnTouristInfoRecvd;
        Init();
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.RecvUserInfo -= OnLoginApplicationFinish;
        ConnectionManager.Instance.RecvLoginDetectInfo -= OnLoginDetectFinish;
        ConnectionManager.Instance.RecvTouristInfo -= OnTouristInfoRecvd;
    }

    void OnAccountInputFieldEndEdit(string str)     //账号编辑结束时触发
    {
        string detectInfo = "";
        AccountCorrect = FormatDetecter.AccountDetect(str, out detectInfo);  //使用FormatDetecter类的static函数检测账号格式
    }

    void OnPasswordInputFieldEndEdit(string str)     //密码编辑结束时触发
    {
        string detectInfo = "";
        PasswordCorrect = FormatDetecter.PasswordDetect(str, out detectInfo);
    }

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        AccountInputField.text = "";
        PasswordInputField.text = "";
        AccountCorrect = false;
        PasswordCorrect = false;
        UIStatusSetting(true);
    }

    private void UIStatusSetting(bool status)
    {
        AccountInputField.enabled = status;
        PasswordInputField.enabled = status;
        LoginButton.enabled = status;
        RegisterButton.enabled = status;
        TouristButton.enabled = status;
    }

    void OnLoginButtonClicked()     //登录按钮
    {
        if(AccountCorrect && PasswordCorrect)
        {
            //向服务器发送填写好的账号密码
            UIStatusSetting(false);
            UIFeedbackSpawner.Instance.ShowTip("正在登录...", Vector3.zero, 0);
            UserLogin usrLoginInfo = new UserLogin();
            usrLoginInfo.Account = AccountInputField.text;
            usrLoginInfo.Password = PasswordInputField.text;
#if DEBUG_MODE
            Debug.Log("正在登录，验证用户中;Account : " + AccountInputField.text + "  " + "Password : " + PasswordInputField.text);
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("正在登录，验证用户中;Account : " + AccountInputField.text + "  " + "Password : " + PasswordInputField.text);
#endif
            //直接发送用UserLogin包给Login Connection
            ConnectionManager.Instance.SendMSGToLoginConnection(ConnectionManager.SendLoginMSGType.UserLogin, usrLoginInfo);
        }
        else
        {
            if(!AccountCorrect)
            {
                UIFeedbackSpawner.Instance.ShowTip("账号格式有误", Vector3.zero, 0.7f);
            }
            else if(!PasswordCorrect)
            {
                UIFeedbackSpawner.Instance.ShowTip("密码格式有误", Vector3.zero, 0.7f);
            }
        }
    }

    void OnRegisterButtonClicked()  //注册按钮
    {
        LoginUIController.Instance.InitWindow.SetActive(false);
        LoginUIController.Instance.LoginWindow.SetActive(false);
        LoginUIController.Instance.RegisterWindow.SetActive(true);
    }

    void OnTouristButtonClicked()   //游客登录
    {
        //检查本地游客文件记录 并发送该记录从而获取游客用户信息
        string touristAccount = PlayerPrefs.GetString("TouristAccount");    //第一次访问为空串
        UIStatusSetting(false);
        UIFeedbackSpawner.Instance.ShowTip("游客登录中...", Vector3.zero, 0);
        TouristLogin touristLogin = new TouristLogin();
        touristLogin.Account = PlayerPrefs.GetString("TouristAccount");
#if DEBUG_MODE
        Debug.Log("正在用游客账号：" + touristLogin.Account + "进行登录");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("正在用游客账号：" + touristLogin.Account + "进行登录");
#endif
        ConnectionManager.Instance.SendMSGToLoginConnection(ConnectionManager.SendLoginMSGType.TouristLogin, touristLogin);
    }

    void OnLoginApplicationFinish(UserInfo userinfo) //  包含游客、正常、注册三种登录
    {
        //获取了userinfo包后读数据并且跳转场景并连接到Room服务器、断开Login服务器
        UIFeedbackSpawner.Instance.ShowTip("登录成功", Vector3.zero, 0.5f);
#if DEBUG_MODE
        Debug.Log("登录成功:" + "ID->" + userinfo.Userid + " " + "Name->" + userinfo.Username + " " + "Score->" + userinfo.Score);
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("登录成功:" + "ID->" + userinfo.Userid + " " + "Name->" + userinfo.Username + " " + "Score->" + userinfo.Score);
#endif
        UIStatusSetting(true);
        ScenesManager.Instance.ChangeScene("Game");
    }

    void OnLoginDetectFinish(LoginDetectFeedback loginDetectFeedback)
    {
        //根据包的值在屏幕上输出账号不存在或者密码错误
        UIFeedbackSpawner.Instance.ClearTip();  //先清空tip再显示新tip
        UIFeedbackSpawner.Instance.ShowTip("账号不存在或密码错误", Vector3.zero, 1);
        UIStatusSetting(true);
    }

    void OnTouristInfoRecvd(TouristFeedback touristFeedback)   //接收到服务器来的
    {
        PlayerPrefs.SetString("TouristAccount", touristFeedback.Account);
#if DEBUG_MODE
        Debug.Log("新的游客账号已保存 ：" + touristFeedback.Account);
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("新的游客账号已保存 ：" + touristFeedback.Account);
#endif
    }
}
#define DEBUG_MODE
#define TEXT_DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameUIController : MonoBehaviour
{
    private static GameUIController instance;
    public static GameUIController Instance
    {
        get
        {
            return instance;
        }
    }
    private Text userNameText;  //标题栏用户名
    private Text scoreText;     //标题栏分数
    private Button matchButton; //匹配按钮
    private Button cancelMatchButton;   //取消匹配按钮
    public bool isMatching;
    private GameObject newUserPanel;
    private InputField newUserNameInputField;
    private Button newUserNameConfirmButton;
    private GameObject AcceptOrRefusePanel;
    private Button AcceptMatchButton;
    private Button RefuseMatchButton;
    private Slider AcceptTimerSlider;   //接受等待计时进度条
    [SerializeField]
    private float AcceptWaitTime;   //接受等待最大时间
    private float AcceptTimer;      //接受等待计时器
    public bool isAccept;
    private GameObject tablePanel;   //牌桌
    public RoomInfo roomInfo;
    private GameObject tipsBG;
    private Button tipsCloseButton;

    private void Awake()
    {
        instance = this;
        isMatching = false;
        userNameText = transform.Find("TitleBG/UserNameBG/UserNameText").GetComponent<Text>();
        scoreText = transform.Find("TitleBG/ScoreBG/ScoreText").GetComponent<Text>();
        matchButton = transform.Find("MatchButton").GetComponent<Button>();
        cancelMatchButton = transform.Find("CancelMatchButton").GetComponent<Button>();
        newUserPanel = transform.Find("NewUserPanel").gameObject;
        newUserNameInputField = transform.Find("NewUserPanel/NewUserNameInputField").GetComponent<InputField>();
        newUserNameConfirmButton = transform.Find("NewUserPanel/NewUserNameConfirmButton").GetComponent<Button>();
        AcceptOrRefusePanel = transform.Find("AcceptOrRefuseMatchPanel").gameObject;
        AcceptMatchButton = transform.Find("AcceptOrRefuseMatchPanel/AcceptMatchButton").GetComponent<Button>();
        RefuseMatchButton = transform.Find("AcceptOrRefuseMatchPanel/RefuseMatchButton").GetComponent<Button>();
        AcceptTimerSlider = transform.Find("AcceptOrRefuseMatchPanel/AcceptTimerSlider").GetComponent<Slider>();
        tipsBG = transform.Find("TipsBG").gameObject;
        tipsCloseButton = transform.Find("TipsBG/CloseButton").GetComponent<Button>();
        tablePanel = transform.Find("TablePanel").gameObject;
        tipsCloseButton.onClick.AddListener(() => { tipsBG.SetActive(false); });
        //用户分数为0时就开启tipsBG
        if(PlayerManager.Instance.localPlayer.Score == 0)
        {
            tipsBG.SetActive(true);
        }
        else
        {
            tipsBG.SetActive(false);
        }
        //LocalPlayer信息更新时触发
        PlayerManager.Instance.localPlayer.OnPlayerInfoUpdate += OnPlayerInfoUpdate;
        //收到匹配/取消确认时触发，把禁用掉的按钮还原并切换按钮
        ConnectionManager.Instance.RecvMatchApplicationFeedback += OnRecvMatchApplicationFeedback;
        //收到匹配成功包时触发，弹出接收拒绝面板
        ConnectionManager.Instance.RecvMatchSuccessFeedback += OnRecvMatchSuccessFeedback;
        //收到RoomInfo时触发，进入房间
        ConnectionManager.Instance.RecvRoomInfo += OnRecvRoomInfo;
        matchButton.gameObject.SetActive(true);
        cancelMatchButton.gameObject.SetActive(false);
        matchButton.onClick.AddListener(OnThreeMatchButtonClick);
        cancelMatchButton.onClick.AddListener(OnCancelMatchButtonClick);
        OnPlayerInfoUpdate(PlayerManager.Instance.localPlayer); //登录之后对大厅信息更新
        InitButtonFunction();
        IDToGameServer();
        NewUserTest();
    }

    /// <summary>
    /// 新用户初次登录检测
    /// </summary>
    void NewUserTest()  
    {
        //收到新用户创建回执的触发函数
        ConnectionManager.Instance.RecvNewUserNameConfirmFeedback += (EditUserInfoFeedback euif) =>
        {
            if (euif.IsSuccessEdit)
            {
                UIFeedbackSpawner.Instance.ShowTip("创建成功", Vector3.zero, 1f);
                PlayerManager.Instance.localPlayer.UserName = newUserNameInputField.text;
                newUserPanel.SetActive(false);
            }
            else
            {
                UIFeedbackSpawner.Instance.ShowTip("创建失败", Vector3.zero, 1f);
            }
        };
        if (PlayerManager.Instance.localPlayer.UserName == "" || PlayerManager.Instance.localPlayer.UserName == null)    //新用户必须设置用户名
        {
            newUserPanel.SetActive(true);
        }
        else
        {
            newUserPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 登录成功之后发送userid
    /// </summary>
    void IDToGameServer()
    {
        
        UserInfoToGameServer uitgs = new UserInfoToGameServer();
        uitgs.Userid = PlayerManager.Instance.localPlayer.UserID;
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.UserInfoToGameServer, uitgs);
    }

    /// <summary>
    /// 初始化按钮对应的点击事件
    /// </summary>
    void InitButtonFunction()
    {
        AcceptMatchButton.onClick.AddListener(() =>
        {
#if DEBUG_MODE
            Debug.Log("接受匹配");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("接受匹配");
#endif
            SendAcceptOrRefusePackage(true);
            AcceptMatchButton.enabled = false;
            RefuseMatchButton.enabled = false;
            isAccept = true;
        });
        RefuseMatchButton.onClick.AddListener(() =>
        {
            isAccept = false;
#if DEBUG_MODE
            Debug.Log("拒绝匹配");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("拒绝匹配");
#endif
            SendAcceptOrRefusePackage(false);
            AcceptMatchButton.enabled = false;
            RefuseMatchButton.enabled = false;

        });

        newUserNameConfirmButton.onClick.AddListener(() => {
            newUserNameConfirmButton.enabled = false;
            UIFeedbackSpawner.Instance.ShowTip("创建中...", Vector3.zero, 0);
            EditUserInfo eui = new EditUserInfo();
            eui.NewUserName = newUserNameInputField.text;
            ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.EditUserInfo, eui);
        });
        InitButtonSound();
    }

    /// <summary>
    /// 音效初始化
    /// </summary>
    void InitButtonSound()
    {
        AcceptMatchButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);
        RefuseMatchButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);
        newUserNameConfirmButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);
        matchButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);
        cancelMatchButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);
#if DEBUG_MODE
        Debug.Log("GameUI音效初始化完成");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("GameUI音效初始化完成");
#endif
    }

    /// <summary>
    /// 向服务器发送接收或者请求的消息
    /// </summary>
    /// <param name="isAccept"></param>
    void SendAcceptOrRefusePackage(bool isAccept)
    {
        AcceptOrRefuse aor = new AcceptOrRefuse();
        aor.IsAccept = isAccept;
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.AcceptORRefuse, aor);
    }

    /// <summary>
    /// 用户信息更新
    /// </summary>
    /// <param name="player"></param>
    void OnPlayerInfoUpdate(Player player)
    {
        userNameText.text = player.UserName;
        scoreText.text = player.Score.ToString();
    }

    /// <summary>
    /// 匹配按钮
    /// </summary>
    void OnThreeMatchButtonClick()
    {
#if DEBUG_MODE
        Debug.Log("正在发送3人匹配请求包");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("正在发送3人匹配请求包");
#endif
        MatchSwitchApplication msa = new MatchSwitchApplication();
        msa.PersonNum = 3;
        matchButton.enabled = false;
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.MatchSwitchApplication, msa);
    }

    /// <summary>
    /// 点击取消匹配按钮
    /// </summary>
    void OnCancelMatchButtonClick()
    {
#if DEBUG_MODE
        Debug.Log("正在发送取消匹配请求包");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("正在发送取消匹配请求包");
#endif
        MatchSwitchApplication msa = new MatchSwitchApplication();
        msa.PersonNum = 0;
        cancelMatchButton.enabled = false;
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.MatchSwitchApplication, msa);
    }

    /// <summary>
    /// 匹配成功时触发
    /// </summary>
    void OnRecvMatchSuccessFeedback()
    {
        isMatching = false;
        #region 读条过程窗口已跳过
        /*isAccept = false;
        AcceptOrRefusePanel.SetActive(true);
        AcceptMatchButton.enabled = true;
        RefuseMatchButton.enabled = true;
        StartCoroutine("UpdateAcceptWaitTimer");*/
        #endregion
        UIFeedbackSpawner.Instance.ShowTip("正在进入房间", Vector3.zero, 0);
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.AcceptORRefuse, null);
        UIFeedbackSpawner.Instance.ShowTip("已发送匹配确认", Vector3.zero, 0);
        HallUIChangeNormalState();
    }
    //还原
    /// <summary>
    /// 收到房间信息初始化牌桌
    /// </summary>
    void OnRecvRoomInfo(RoomInfo ri)
    {
        roomInfo = ri;  //gameUIController先暂存roominfo，table manager之后来拿数据
        tablePanel.SetActive(true);
        SoundManager.Instance.InitGame();
        //TableManager.Instance.roomInfo = ri;
        UIFeedbackSpawner.Instance.ShowTip("正在等待玩家进场", Vector3.zero, 0);
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.RoomInfoArrived, null);    //通知服务器房间信息已到达
    }

    /// <summary>
    /// 已弃用
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateAcceptWaitTimer()
    {
        AcceptTimerSlider.value = 0;
        AcceptTimer = 0;
        while(AcceptTimer<AcceptWaitTime)//每帧更新进度条、计时器
        {
            AcceptTimerSlider.value = AcceptTimer / AcceptWaitTime; 
            AcceptTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(isAccept)
        {
            UIFeedbackSpawner.Instance.ShowTip("正在进入房间...", Vector3.zero, 0);
        }
        else
        {
            AcceptOrRefusePanel.SetActive(false);
            UIFeedbackSpawner.Instance.ShowTip("匹配已拒绝", Vector3.zero, 1f);
        }
        yield return null;
    }

    /// <summary>
    /// 切换，显示匹配的按钮
    /// </summary>
    void HallUIChangeNormalState()
    {
        cancelMatchButton.gameObject.SetActive(false);
        matchButton.gameObject.SetActive(true);
        matchButton.enabled = true;
    }

    /// <summary>
    /// 切换，显示取消匹配的按钮
    /// </summary>
    void HallUIChangeMatchingState()
    {
        matchButton.gameObject.SetActive(false);
        cancelMatchButton.gameObject.SetActive(true);
        cancelMatchButton.enabled = true;

    }

    void OnRecvMatchApplicationFeedback()   //收到匹配或取消匹配确认时调用
    {
        isMatching = !isMatching;
        //Debug.LogError("收到了反馈,当前匹配状态：" + isMatching);
        if (isMatching)      //根据当前匹配状态设置按钮状态
        {
            HallUIChangeMatchingState();
        }
        else
        {
            HallUIChangeNormalState();
        }
    }
}

#define DEBUG_MODE
#define TEXT_DEBUG_MODE
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionManager : MonoBehaviour
{
    private static ConnectionManager m_instance;
    public static ConnectionManager Instance{
        get
        {
            return m_instance;
        }
    }

    private Queue<byte[]> LoginMSGQueue;    //由于子线程不能调用Unity API，所以在主线程上建两个队列来处理子线程传来的消息，委托主线程进行API操作
    private Queue<byte[]> GameMSGQueue;

    private float heartbeatsTimer;

    public enum SendLoginMSGType    //登录连接发送的包类型
    {
        UserLogin = 1,  //正常登陆包
        TouristLogin = 2,   //游客登录包，服务器的数据库需要加标记，定期删除游客账号
        RegisterLogin = 3,   //注册登录包
        RegisterDetect = 4  //注册重复检测包
    };

    public enum RecvLoginMSGType    //登录连接收到的包类型
    {
        MSGTooShort = 0,  //接收消息长度过短
        UserInfo = 1,   //UserLogin、TouristLogin、Register包对应同一种回复UserInfo(包含用户名和分数)
        LoginDetectFeedback = 2,    //账号登录检测反馈（密码错误、账号不存在）
        RegisterDetectFeedback = 3,  //注册时账号和邮箱的重复检测反馈包
        TouristFeedback = 4
    }

    public enum SendGameMSGType //游戏连接发送的包类型
    {
        Heartbeats = 1,     //心跳包，固定间隔时间发送一次
        UserInfoToGameServer = 2,   //登录后把从登录连接收到的UserInfo ID发送给游戏连接
        EditUserInfo = 3,   //用户信息修改包
        MatchSwitchApplication = 4,     //匹配申请/取消包
        AcceptORRefuse = 5, //接收/拒绝进入房间（已跳过，直接进入）
        RoomInfoArrived = 6, //房间信息抵达确认
        PlayerOperation = 7,    //玩家操作信息
        CandleCardFeedback = 8, //烛牌反馈包发送给服务器进行广播
        GameFinish = 9,  //游戏结束包
        //OnLineConfirm = 10,  //重连确认，准备接收操作队列
        OperationQueueGet = 10   //处理了操作队列就给服务器一个反馈
    }
    
    public enum RecvGameMSGType //游戏连接接收的包类型
    {
        MSGTooShort = 0,            //接收消息长度过短
        Heartbeats = 1,             //服务器心跳包
        EditUserInfoFeedback = 2,   //用户信息修改确认包
        MatchSwitchApplicationFeedback = 3,     //匹配申请/取消确认包
        MatchSuccessFeedback = 4,   //匹配成功确认包
        RoomInfo = 5,               //房间信息
        OtherPlayerInfo = 6,        //其他玩家信息
        LocalPlayerInfo = 7,        //本机玩家信息
        GameFinish = 8,             //本轮游戏结束
        CandleCardFeedback = 9,     //烛反馈包，包含下家的手牌信息
        PlayerOperationPackage = 10,  //以广播的形式收到其他玩家的操作
        OffLine = 11,    //有任何玩家掉线或者重连时房间内广播
        OnLine = 12, //有玩家重连的消息
        OperationQueueTail = 13 //重连时收到的操作队列的结尾
    }

    private Connection m_loginConnection;
    public Connection LoginConnection => m_loginConnection;
    private Connection m_gameConnection;
    public Connection GameConnection => m_gameConnection;
    private string m_identityCheckCode;
    public string IdentityCheckCode => m_identityCheckCode;
    [SerializeField]
    private string LoginIP;  //编辑器界面设置连接IP、Port
    [SerializeField]
    private int LoginPort;
    [SerializeField]
    private string GameIP;
    [SerializeField]
    private int GamePort;
    [SerializeField]
    private int HeartbeatsInterval = 3;
    //Login
    public delegate void ServerFeedbackUserInfo(UserInfo userinfo); //接收用户信息触发事件
    public event ServerFeedbackUserInfo RecvUserInfo;
    public delegate void ServerFeedbackLoginDetect(LoginDetectFeedback loginDetectFeedback);    //接收登录检测
    public event ServerFeedbackLoginDetect RecvLoginDetectInfo;
    public delegate void ServerFeedbackRegisterDetect(RegisterDetectFeedback registerDetectFeedback);   //接收注册检测
    public event ServerFeedbackRegisterDetect RecvRegisterDetectInfo;
    public delegate void ServerFeedbackNewTourist(TouristFeedback touristFeedback); //第一次游客登录后从服务器返回的账号（用于下次登录）
    public event ServerFeedbackNewTourist RecvTouristInfo;
    //Game
    public delegate void MatchFeedback();
    public event MatchFeedback RecvMatchApplicationFeedback;    //收到匹配申请/确认回执
    public event MatchFeedback RecvMatchSuccessFeedback;        //收到匹配成功触发事件
    public delegate void UserInfoEditFeedback(EditUserInfoFeedback euif);
    public event UserInfoEditFeedback RecvNewUserNameConfirmFeedback;   //首次创建用户名事件
    public delegate void RoomInfoFeedback(RoomInfo ri);         //收到房间信息
    public event RoomInfoFeedback RecvRoomInfo;
    public delegate void OtherPlayerInfoFeedback(OtherPlayerInfo opi);  //收到其他玩家信息
    public event OtherPlayerInfoFeedback RecvOtherPlayerInfo;
    public delegate void LocalPlayerInfoFeedback(LocalPlayerInfo opi);  //收到本机玩家信息
    public event LocalPlayerInfoFeedback RecvLocalPlayerInfo;
    public delegate void GameFinishFeedback(GameFinish gf);     //收到游戏结束信息
    public event GameFinishFeedback RecvGameFinish;
    public delegate void CandleCardFeedbackInfo(CandleCardFeedback ccf);    //收到下家手牌信息
    public event CandleCardFeedbackInfo RecvCandleCardFeedback;
    public delegate void PlayerOperationPackage(PlayerOperation po);
    public event PlayerOperationPackage RecvPlayerOperationPackage;
    public delegate void PlayerOffLineOrOnLine(OffLineOrOnLine ooo);    //玩家掉线消息
    public event PlayerOffLineOrOnLine RecvPlayerOffLine;   //玩家掉线
    public event PlayerOffLineOrOnLine RecvPlayerOnLine;    //玩家重连
    public delegate void OperationQueueTail();      //操作队列结尾
    public event OperationQueueTail RecvOperationQueueTail;

    private void Awake()
    {
        DontDestroyOnLoad(this);    //切换场景保留
        m_instance = this;
        DontDestroyOnLoad(this.gameObject);    //换场景保留物体
        //EXE测试
        /*m_loginConnection = new Connection(IPEditor.Instance.loginIPInputField.text, Convert.ToInt32(IPEditor.Instance.loginPortInputField.text));    //创建登录连接
        m_gameConnection = new Connection(IPEditor.Instance.loginIPInputField.text, Convert.ToInt32(IPEditor.Instance.GamePortInputField.text));        //创建游戏连接*/
        //EXE测试
        //TODO 正式版要还原的位置
        m_loginConnection = new Connection(LoginIP, LoginPort);    //创建登录连接
        m_gameConnection = new Connection(GameIP, GamePort);        //创建游戏连接
        //TODO 正式版要还原的位置
        LoginMSGQueue = new Queue<byte[]>();
        GameMSGQueue = new Queue<byte[]>();
        m_loginConnection.NotifyMsg += RecvMSGFromLoginConnection;    //登录连接接收消息存入登录消息队列
        m_gameConnection.NotifyMsg += RecvMSGFromGameConnection;    //游戏连接接收消息存入游戏消息队列
    }

    private void RecvMSGFromLoginConnection(byte[] msg) //登录消息入队
    {
        lock (this)
        {
            LoginMSGQueue.Enqueue(msg);
        }
    }

    private void RecvMSGFromGameConnection(byte[] msg)  //游戏消息入队
    {
        lock (this)
        {
            GameMSGQueue.Enqueue(msg);
        }
    }

    private void Start()
    {
        m_loginConnection.StartConnection();    //***开始连接前先把事件连接好（UI相关的方法调用提示用户当前连接状态）
    }

    private void Update()   //主线程循环处理收到的所有消息
    {
        if (LoginMSGQueue.Count > 0)
        {
            byte[] msg = LoginMSGQueue.Dequeue();
            LoginMSGRecive(msg);
        }
        if (GameMSGQueue.Count > 0)
        {
            byte[] msg = GameMSGQueue.Dequeue();
            GameMSGRecive(msg);
        }
    }

    public void ReConnectLoginServer()
    {
        m_loginConnection.StartConnection();
    }

    IEnumerator HeartbeatsTiming()  //Game Connection启动后开始心跳检测，收到心跳包后计时器清0
    {
        heartbeatsTimer = 0;
        while (heartbeatsTimer < HeartbeatsInterval + 2)
        {
            heartbeatsTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        UIFeedbackSpawner.Instance.ShowTip("已掉线", Vector3.zero, 0);
    }

    public void LoginMSGRecive(byte[] msg)    //注册的Connection收到内容消息时回调
    {
#if DEBUG_MODE
        Debug.Log("Login MSG解析中");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("Login MSG解析中");
#endif
        byte msgType = msg[0];
        int len = msg.Length;
        switch((RecvLoginMSGType)msgType)
        {
            case RecvLoginMSGType.MSGTooShort:
#if DEBUG_MODE
                Debug.LogError("MSG too short");
#endif
#if TEXT_DEBUG_MODE
                DebugManager.Instance.Log("MSG too short");
#endif
                break;
            case RecvLoginMSGType.UserInfo:
#if DEBUG_MODE
                Debug.Log("收到UserInfo");
#endif
#if TEXT_DEBUG_MODE
                DebugManager.Instance.Log("收到UserInfo");
#endif
                RecvUserInfo(UserInfo.Parser.ParseFrom(msg, 1, len - 1));
                LoginConnection.CloseConnection();
                GameConnection.StartConnection();
                //TODO 发id
                StartCoroutine("HeartbeatsTiming");     //登录成功之后开始用协程进行心跳检测
                break;
            case RecvLoginMSGType.LoginDetectFeedback:
#if DEBUG_MODE
                Debug.Log("收到LoginDetectFeedback包");
#endif
#if TEXT_DEBUG_MODE
                DebugManager.Instance.Log("收到LoginDetectFeedback包");
#endif
                RecvLoginDetectInfo(LoginDetectFeedback.Parser.ParseFrom(msg, 1, len - 1));
                break;
            case RecvLoginMSGType.RegisterDetectFeedback:
#if DEBUG_MODE
                Debug.Log("收到RegisterDetectFeedback包");
#endif
#if TEXT_DEBUG_MODE
                DebugManager.Instance.Log("收到RegisterDetectFeedback包");
#endif
                RecvRegisterDetectInfo(RegisterDetectFeedback.Parser.ParseFrom(msg, 1, len - 1));
                break;
            case RecvLoginMSGType.TouristFeedback:  //TODO可能出现UserInfo包先到后直接断开了连接的情况
#if DEBUG_MODE
                Debug.Log("收到TouristFeedback包");
#endif
#if TEXT_DEBUG_MODE
                DebugManager.Instance.Log("收到TouristFeedback包");
#endif
                RecvTouristInfo(TouristFeedback.Parser.ParseFrom(msg, 1, len - 1));
                break;
            default:
#if DEBUG_MODE
                Debug.LogError("协议号解析失败");
#endif
#if TEXT_DEBUG_MODE
                DebugManager.Instance.Log("协议号解析失败");
#endif
                break;
        }
    }

    public void GameMSGRecive(byte[] msg)    //Game Connection收到内容消息时回调
    {
#if DEBUG_MODE
        Debug.Log("Game MSG解析中:" + msg.Length + "字节");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("Game MSG解析中:" + msg.Length + "字节");
#endif
        int len = msg.Length;
        if (len == 0)
        {
#if DEBUG_MODE
            Debug.LogError("Game服务器已下线");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("Game服务器已下线");
#endif
            GameMSGQueue.Clear();
            GameConnection.CloseConnection();
        }
        else
        {
            byte msgType = msg[0];
            switch ((RecvGameMSGType)msgType)
            {
                case RecvGameMSGType.MSGTooShort:
#if DEBUG_MODE
                    Debug.LogError("MSG too short");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("MSG too short");
#endif
                    break;
                case RecvGameMSGType.Heartbeats:
#if DEBUG_MODE
                    Debug.Log("已收到Heartbeats");
#endif
                    heartbeatsTimer = 0;
                    SendMSGToGameConnection(SendGameMSGType.Heartbeats, null);
                    break;
                case RecvGameMSGType.EditUserInfoFeedback:
#if DEBUG_MODE
                    Debug.Log("已收到用户信息修改确认包");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到用户信息修改确认包");
#endif
                    RecvNewUserNameConfirmFeedback(EditUserInfoFeedback.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.MatchSwitchApplicationFeedback:
#if DEBUG_MODE
                    Debug.Log("已收到匹配/取消匹配回执包");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到匹配/取消匹配回执包");
#endif
                    RecvMatchApplicationFeedback();
                    break;
                case RecvGameMSGType.MatchSuccessFeedback:
#if DEBUG_MODE
                    Debug.Log("匹配成功");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("匹配成功");
#endif
                    RecvMatchSuccessFeedback();
                    break;
                case RecvGameMSGType.RoomInfo:
#if DEBUG_MODE
                    Debug.Log("已收到房间信息" + msg.Length + "字节");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到房间信息" + msg.Length + "字节");
#endif
                    RecvRoomInfo(RoomInfo.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.OtherPlayerInfo:
#if DEBUG_MODE
                    Debug.Log("已收到其他玩家信息:" + msg.Length + "字节");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到其他玩家信息:" + msg.Length + "字节");
#endif
                    RecvOtherPlayerInfo(OtherPlayerInfo.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.LocalPlayerInfo:
#if DEBUG_MODE
                    Debug.Log("已收到本机玩家信息" + msg.Length + "字节");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到本机玩家信息" + msg.Length + "字节");
#endif
                    RecvLocalPlayerInfo(LocalPlayerInfo.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.GameFinish:
#if DEBUG_MODE
                    Debug.Log("已收到游戏结束信息");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到游戏结束信息");
#endif
                    RecvGameFinish(GameFinish.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.CandleCardFeedback:
#if DEBUG_MODE
                    Debug.Log("已收到下家手牌信息");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到下家手牌信息");
#endif
                    RecvCandleCardFeedback(CandleCardFeedback.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.PlayerOperationPackage:
#if DEBUG_MODE
                    Debug.Log("已收到玩家操作的信息");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到玩家操作的信息");
#endif
                    RecvPlayerOperationPackage(PlayerOperation.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.OffLine:
#if DEBUG_MODE
                    Debug.Log("已收到其他玩家掉线的信息");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到其他玩家掉线的信息");
#endif
                    RecvPlayerOffLine(OffLineOrOnLine.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.OnLine:
#if DEBUG_MODE
                    Debug.Log("已收到其他玩家重连的信息");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到其他玩家重连的信息");
#endif
                    RecvPlayerOnLine(OffLineOrOnLine.Parser.ParseFrom(msg, 1, len - 1));
                    break;
                case RecvGameMSGType.OperationQueueTail:
#if DEBUG_MODE
                    Debug.Log("已收到重连操作队列结尾标记");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("已收到重连操作队列结尾标记");
#endif
                    //恢复isReconnection标记为false，TableManager开始正常处理上家的烛操作
                    RecvOperationQueueTail();
                    SendMSGToGameConnection(SendGameMSGType.OperationQueueGet, null);
                    break;
                default:
#if DEBUG_MODE
                    Debug.LogError("协议号解析失败");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("协议号解析失败");
#endif
                    break;
            }
        }
    }

    public void SendMSGToLoginConnection(SendLoginMSGType msgType, IMessage obj)
    {
        //test
        /*byte[] srcbyte = obj.ToByteArray();
        byte[] msg = new byte[srcbyte.Length + 2];
        msg[0] = (byte)msgType; //协议号
        msg[1] = (byte)srcbyte.Length;  //包长
        srcbyte.CopyTo(msg, 2); //包
#if DEBUG_MODE
        Debug.Log("Login_Connection正在发送数据");
#endif
        m_loginConnection.SendMSG(msg);*/
        //test
        byte[] srcbyte = obj.ToByteArray();
        byte[] msg = new byte[srcbyte.Length + 1];
        msg[0] = (byte)msgType;
        srcbyte.CopyTo(msg, 1);
#if DEBUG_MODE
        Debug.Log("Login_Connection正在发送数据:" + msg.Length + "字节");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("Login_Connection正在发送数据:" + msg.Length + "字节");
#endif
        m_loginConnection.SendMSG(msg);
    }

    public void SendMSGToGameConnection(SendGameMSGType msgType, IMessage obj)   //若obj为空，则发送的是单字节协议包，不使用protobuf
    {
#if DEBUG_MODE
        Debug.Log("发送包的协议号：" + msgType);
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("发送包的协议号：" + msgType);
#endif
        if (obj==null)
        {
            //test
            /*byte[] msg = new byte[2];
            msg[0] = (byte)msgType; //协议号
            msg[1] = 0xff;      //包长，如果只发送协议号就把第二个字节设0xff
            m_gameConnection.SendMSG(msg);*/
            //test
            byte[] msg = new byte[2];
            msg[0] = (byte)msgType;
            m_gameConnection.SendMSG(msg);
        }
        else
        {
            //test
            /*byte[] srcbyte = obj.ToByteArray();
            if (srcbyte.Length > 255)
            {
                Debug.LogError("单个数据包超出最大长度255");
            }
            byte[] msg = new byte[srcbyte.Length + 2];
            msg[0] = (byte)msgType; //协议号
            msg[1] = (byte)srcbyte.Length;  //包长
            srcbyte.CopyTo(msg, 2); //包
#if DEBUG_MODE
            Debug.Log("Game_Connection正在发送数据:" + msg.Length + "字节");
#endif
            m_gameConnection.SendMSG(msg);*/
            //test
            byte[] srcbyte = obj.ToByteArray();
            byte[] msg = new byte[srcbyte.Length + 1];
            msg[0] = (byte)msgType;
            srcbyte.CopyTo(msg, 1);
#if DEBUG_MODE
            Debug.Log("Game_Connection正在发送数据:" + msg.Length + "字节");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("Game_Connection正在发送数据:" + msg.Length + "字节");
#endif
            m_gameConnection.SendMSG(msg);
        }
    }

    private void OnApplicationQuit()
    {
#if DEBUG_MODE
        Debug.Log("连接关闭");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("连接关闭");
#endif
        if (LoginConnection != null && LoginConnection.IsRunning)
        {
            //LoginConnection.CloseConnection();
        }
        if(GameConnection != null && GameConnection.IsRunning)
        {
            GameConnection.CloseConnection();
        }
    }
}

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
    public enum ConnectionType
    {
        Login,Room,Game
    }

    public enum MSGType    //Check类型用于确认消息
    {
        Check,Login,Register    //TODO 待扩充
    }
    private Connection m_loginConnection;
    public Connection LoginConnection => m_loginConnection;
    private Connection m_roomConnection;
    public Connection RoomConnection => m_roomConnection;
    private Connection m_gameConnection;
    public Connection GameConnection => m_gameConnection;
    private string m_identityCheckCode;
    public string IdentityCheckCode => m_identityCheckCode;

    public string LoginIP;  //编辑器界面设置连接IP、Port
    public int LoginPort;
    public string RoomIP;
    public int RoomPort;
    public string GameIP;
    public int GamePort;

    public delegate void LoginServer();
    public event LoginServer LoginServerConnected;

    private void Awake()
    {
        m_instance = this;
        DontDestroyOnLoad(this.gameObject);    //换场景保留物体
        m_loginConnection = new Connection(LoginIP, LoginPort);    //登录服务器连接
        m_loginConnection.NotifyMsg += MSGRecive;    //接收消息事件注册
    }

    private void Start()
    {
        m_loginConnection.StartConnection();    //***开始连接前先把事件连接好（UI相关的方法调用提示用户当前连接状态）
    }

    public void ReConnectLoginServer()
    {
        m_loginConnection.StartConnection();
    }

    void MSGRecive(byte[] msg, int len)    //注册的Connection收到内容消息时回调
    {
        byte[] srcMsg = new byte[len];
        Array.Copy(msg, 0, srcMsg, 0, len); //转存消息到srcmsg中以便后面处理
        int msgType;
        byte[] destMsg;
        MSGHandler.MSGSeparate(msg, out msgType, out destMsg);  //把消息拆分成类型号和内容两部分
        //TODO 把内容还原成对应的类，对每种不同的类采取不同处理方式
        //switch...

    }

    void SendMSGToConnection(int msgType, ref byte[]msg, ConnectionType ctype)
    {
        byte[] destMsg;
        MSGHandler.MSGEncapsulation(msgType, msg, out destMsg);
        switch (ctype)
        {
            case ConnectionType.Login:
            {
                m_loginConnection.SendMSG(ref destMsg);
                break;
            }
            case ConnectionType.Room:
            {
                m_roomConnection.SendMSG(ref destMsg);
                break;
            }
            case ConnectionType.Game:
            {
                m_gameConnection.SendMSG(ref destMsg);
                break;
            }
        }
    }
}

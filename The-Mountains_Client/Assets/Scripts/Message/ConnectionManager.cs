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
    private ConnectionManager m_instance;
    public ConnectionManager Instance{
        get
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
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

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);    //换场景保留物体
        m_loginConnection = new Connection("127.0.0.1", 1688);    //登录服务器连接
        m_loginConnection.NotifyMSGType += MSGTypeHandle;    //接收类型事件注册
        m_loginConnection.NotifyMSGDetail += MSGDetailHandle;    //接收内容事件注册
    }

    void MSGTypeHandle(byte[] msg, int len)    //注册的Connection收到类型消息时回调
    {
        Debug.Log("Call MSG Type Handle");
        //TODO proto定义枚举类型MSG，将MSG字节数组还原成类型
//        MSGType msgType = MSGType.Parser.ParseFrom(msg);
//        switch (msgType)    //根据msg类型决定处理的下一个消息类型
//        {
//            case ...
//        }
    }

    void MSGDetailHandle(byte[] msg, int len)    //注册的Connection收到内容消息时回调
    {
        Debug.Log("Call MSG Detail Handle");
        //TODO 把内容还原成对应的类，对每种不同的类采取不同处理方式
        //switch...
    }

    void SendMSGToConnection(ref byte[]msg, MSGType msgtype , ConnectionType ctype)
    {
        switch (ctype)
        {
            case ConnectionType.Login:
            {
                m_loginConnection.SendMSG(ref msg);
                break;
            }
            case ConnectionType.Room:
            {
                m_roomConnection.SendMSG(ref msg);
                break;
            }
            case ConnectionType.Game:
            {
                m_gameConnection.SendMSG(ref msg);
                break;
            }
        }
    }
}

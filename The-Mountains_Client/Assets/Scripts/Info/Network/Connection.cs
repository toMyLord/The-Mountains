//#define DEBUG_MODE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Object = System.Object;
using Google.Protobuf;

public class Connection
{
    private Socket m_socket;    //封装的套接字
    private bool m_isRunning;    //连接是否正常运行
    public bool IsRunning => m_isRunning;
    private byte[] m_recvBuf;    //消息缓冲
    private int m_recvLen;
    private Thread m_recvThread;    //接收子线程
    private string m_ip;    //保存ip和port方便后面使用
    public string Ip => m_ip;
    private int m_port;
    public int Port => m_port;

    private IPAddress ipAddress;
    private IPEndPoint ipEnd;

    public delegate void NotifyMSG(byte[] msg/*, int len*/);    //消息通知委托

    public event NotifyMSG NotifyMsg;    //消息内容通知事件

    public delegate void ConnectionInfo();
    public event ConnectionInfo ConnectionStart;
    public event ConnectionInfo ConnectionFinish;
    public event ConnectionInfo ConnectionEnd;
    public event ConnectionInfo ConnectionError;

    public Connection(string _ip,int _port)    //构造函数，_ip服务器IP地址， _port服务器端口
    {
        ipAddress = IPAddress.Parse(_ip);
        ipEnd = new IPEndPoint(ipAddress, _port);
        m_socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        m_isRunning = false;
        m_recvBuf = new byte[1024];
        m_ip = _ip;
        m_port = _port;
        
    }

    public void StartConnection()   //连接服务器
    {
        //通知MSG正在连接服务器
        if (ConnectionStart != null)
            ConnectionStart();
#if DEBUG_MODE
        Debug.Log("正在连接服务器:" + m_ip + ":" + m_port);
#endif
        try
        {
            m_socket.Connect(ipEnd);    //socket连接
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //通知MSG连接服务器错误
            if (ConnectionError != null)
                ConnectionError();
#if DEBUG_MODE
            Debug.Log("服务器" + m_ip + ":" + m_port + "连接错误");
#endif
            return;
        }
        
        //TODO 通知MSG连接成功
        if (ConnectionFinish != null)
            ConnectionFinish();
#if DEBUG_MODE
        Debug.Log("服务器" + m_ip + ":" + m_port + "连接成功");
#endif
        m_isRunning = true;    //连接标记设置成true，表示该connection正常运行
        m_recvThread = new Thread(RecvMSG);    //创建接收线程，防止主线程阻塞
        m_recvThread.Start();
#if DEBUG_MODE
        Debug.Log("启动接收子线程");
#endif
    }

    //发送消息
    public void SendMSG(byte[] msg)
    {
        m_socket.Send(msg);
#if DEBUG_MODE
        Debug.Log("消息已发送,消息长度："+ msg.Length);
#endif
    }

    //接收消息
    public void RecvMSG()
    {
        while (m_isRunning)    //检测连接是否应该结束
        {
#if DEBUG_MODE
            Debug.Log("Connection正在等待服务器数据");
#endif
            m_recvLen = m_socket.Receive(m_recvBuf);
#if DEBUG_MODE
            Debug.Log("Connection已接收服务器数据,数据长度:" + m_recvLen);
#endif

            //test  处理粘包
            if (m_recvLen >= 2)
            {
                if (m_recvBuf[0] <= m_recvLen - 1)   //小于时有粘包，等于时无粘包
                {
                    //byte typeNum = m_recvBuf[1];
                    int startIndex = 0, endIndex = startIndex+m_recvBuf[startIndex];  //连续包中每个独立包的包头index和包尾index
                    while (startIndex <= m_recvLen - 1)
                    {
                        Debug.Log("包长：" + m_recvBuf[startIndex] + " " + "收到的协议号：" + m_recvBuf[startIndex + 1]);
                        int len = endIndex - startIndex;
                        byte[] msg = new byte[len];
                        //msg[0] = m_recvBuf[startIndex + 1];
                        Array.Copy(m_recvBuf, startIndex + 1, msg, 0, len); //去掉第二个字节位置的包长信息，留下 协议号+包
                        NotifyMsg(msg); //把包分离出来之后就传到 ConnectionManager
                        startIndex = endIndex + 1;
                        if (startIndex >= m_recvLen)
                        {
                            break;
                        }
                        endIndex = startIndex + m_recvBuf[startIndex];
                    }
                }
            }
            else
            {
                Debug.LogError("数据包过短");
                CloseConnection();
            }
            //test  处理粘包

            /*byte[] msg = new byte[m_recvLen];
            Array.Copy(m_recvBuf, 0, msg, 0, m_recvLen);
            NotifyMsg(msg);*/
        }
    }

    //关闭连接
    public void CloseConnection()
    {
        if (m_recvThread != null)    //关闭接收子线程
        {
            m_recvThread.Interrupt();
            m_recvThread.Abort();
            Debug.Log("接收线程已关闭,端口："+m_port);
        }
        if (m_socket != null)    //关闭socket
        {
            m_socket.Close();
        }
        m_isRunning = false;    //connection状态设置false
#if DEBUG_MODE
        Debug.Log("连接关闭");
#endif
        if (ConnectionEnd != null)
            ConnectionEnd();
    }
}

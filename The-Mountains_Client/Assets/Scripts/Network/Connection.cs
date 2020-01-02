using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Object = System.Object;

public class Connection
{
    private Socket m_socket;    //封装的套接字
    private bool m_isRunning;    //连接是否正常运行
    public bool IsRunning => m_isRunning;
    private byte[] m_recvBuf1;    //消息类型缓冲
    private int m_recvLen1;
    private byte[] m_recvBuf2;    //消息内容缓冲
    private int m_recvLen2;
    private Thread m_recvThread;    //接收子线程
    private string m_ip;    //保存ip和port方便后面使用
    public string Ip => m_ip;
    private int m_port;
    public int Port => m_port;

    public delegate void NotifyMSG(byte[] msg, int len);    //消息通知委托

    public event NotifyMSG NotifyMSGType;    //消息类型通知事件
    public event NotifyMSG NotifyMSGDetail;    //消息内容通知事件
    
    public Connection(string _ip,int _port)    //构造函数，_ip服务器IP地址， _port服务器端口
    {
        IPAddress ipAddress = IPAddress.Parse(_ip);
        IPEndPoint ipEnd = new IPEndPoint(ipAddress, _port);
        m_socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        m_isRunning = false;
        m_recvBuf1 = new byte[1024];
        m_recvBuf2 = new byte[1024];
        m_ip = _ip;
        m_port = _port;
        //TODO 通知MSG正在连接服务器
        Debug.Log("正在连接服务器");
        try
        {
            m_socket.Connect(ipEnd);    //socket连接
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //TODO 通知MSG连接服务器错误
            Debug.Log("服务器连接错误");
            return;
        }
        //TODO 通知MSG连接成功
        Debug.Log("服务器连接成功");
        m_recvThread = new Thread(RecvMSG);    //创建接收线程，防止主线程阻塞
        m_recvThread.Start();
        Debug.Log("启动接收子线程");
        m_isRunning = true;    //连接标记设置成true，表示该connection正常运行
    }
    
    //发送消息
    public void SendMSG(ref byte[] msg)
    {
        m_socket.Send(msg);
        Debug.Log("消息已发送");
    }
    
    //接收消息
    public void RecvMSG()
    {
        while (m_isRunning)    //检测连接是否应该结束
        {
            m_recvLen1 = m_socket.Receive(m_recvBuf1);
            Debug.Log("消息类型已接收");
            Debug.Log("recvBuf1 : " + Encoding.ASCII.GetString(m_recvBuf1, 0, m_recvLen1));
            //TODO消息层回复确认消息并等待具体消息内容
            NotifyMSGType(m_recvBuf1, m_recvLen1);
            
            m_recvLen2 = m_socket.Receive(m_recvBuf2);
            Debug.Log("消息内容已接收");
            //TODO传递到消息层处理
            NotifyMSGDetail(m_recvBuf2, m_recvLen2);
            
            Debug.Log("recvBuf2 : " + Encoding.ASCII.GetString(m_recvBuf2, 0, m_recvLen2));
        }
    }
    
    //关闭连接
    public void CloseConnection()
    {
        if (m_recvThread != null)    //关闭接收子线程
        {
            m_recvThread.Interrupt();
            m_recvThread.Abort();
        }
        if (m_socket != null)    //关闭socket
        {
            m_socket.Close();
        }
        m_isRunning = false;    //connection状态设置false
        Debug.Log("连接关闭");
    }
}

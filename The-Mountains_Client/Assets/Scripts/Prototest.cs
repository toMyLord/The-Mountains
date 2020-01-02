using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Google.Protobuf;
using UnityEngine;

public class Prototest : MonoBehaviour
{
    Socket serverSocket; //服务器端socket
    IPAddress ip; //主机ip
    IPEndPoint ipEnd; 
    byte[] recvData=new byte[1024]; //接收的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程
    
    void InitSocket()
    {
        //定义服务器的IP和端口，端口与服务器对应
        ip=IPAddress.Parse("192.168.137.209"); //可以是局域网或互联网ip，此处是本机
        ipEnd=new IPEndPoint(ip,8888);

        SocketConnet();
        //开启一个线程连接，必须的，否则主线程卡死
        connectThread=new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketConnet()
    {
        if(serverSocket!=null)
            serverSocket.Close();
        //定义套接字类型,必须在子线程中定义
        serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        print("ready to connect");
        //连接
        serverSocket.Connect(ipEnd);
    }

    void SocketSend()
    {
        /*//清空发送缓存
        sendData=new byte[1024];
        //数据类型转换
        sendData=Encoding.ASCII.GetBytes(sendStr);*/
        //发送
        serverSocket.Send(msg,msg.Length,SocketFlags.None);
    }

    void SocketReceive()
    {
        //SocketConnet();
        //不断接收服务器发来的数据
        while(true)
        {
            recvData=new byte[1024];
            recvLen=serverSocket.Receive(recvData);
            if(recvLen==0)
            {
                SocketConnet();
                continue;
            }
            AddressBook ad2 = AddressBook.Parser.ParseFrom(recvData, 0, recvLen);
            Debug.Log("server msg : " + ad2.ID + "--" + ad2.Name + "--" + ad2.PhoneNumber + "--" + ad2.Address);
        }
        SocketQuit();
    }

    void SocketQuit()
    {
        //关闭线程
        if(connectThread!=null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭服务器
        if(serverSocket!=null)
            serverSocket.Close();
        print("diconnect");
    }

    private byte[] msg;
    void Start()
    {
        Connection connection = new Connection("127.0.0.1", 1080);
        connection.CloseConnection();
        InitSocket();
        AddressBook ad1 = new AddressBook();
        ad1.ID = 10001;
        ad1.Name = "wzy";
        ad1.PhoneNumber = "9690";
        ad1.Address = "韵苑16栋";
        int adSize = ad1.CalculateSize();
        msg = ad1.ToByteArray();
        AddressBook ad2 = AddressBook.Parser.ParseFrom(msg);
        
        Debug.Log("local : "+ad2.ID + "-" + ad2.Name + "-" + ad2.PhoneNumber + "-" + ad2.Address);

        SocketSend();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSGHandler
{
    /*private static MSGHandler m_instance;
    public static MSGHandler Instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = new MSGHandler();
            }
            return this;
        }
    }*/
    /// <summary>
    /// 消息解析
    /// </summary>
    /// <param name="srcMsg">原消息</param>
    /// <param name="type">返回消息类型</param>
    /// <param name="destMsg">返回去头后的消息</param>
    public static void MSGSeparate(byte[] srcMsg,out int msgType,out byte[] destMsg)
    {
        int srclen = srcMsg.Length;
        int headlen = 4;
        if(srclen <= 0)
        {
            Debug.LogError("消息类型为空");
            msgType = -1;
            destMsg = null;
        }
        else if(srclen < headlen)
        {
            Debug.LogError("消息内容为空");
            msgType = -2;
            destMsg = null;
        }
        else
        {
            byte[] head = new byte[headlen];
            Array.Copy(srcMsg, 0, head, 0, headlen);
            msgType = BitConverter.ToInt32(head, 0);    //消息头还原成int
            int destlen = srclen - headlen;
            destMsg = new byte[destlen];    //在destmsg中存后面的protobuf数据
            Array.Copy(srcMsg, headlen, destMsg, 0, destlen);
        }
    }
    /// <summary>
    /// 消息发送前的封装方法
    /// </summary>
    /// <param name="msgType">消息类型</param>
    /// <param name="srcMsg">消息内容</param>
    /// <param name="destMsg">封装后返回的消息字节数组</param>
    public static void MSGEncapsulation(int msgType, byte[] srcMsg, out byte[] destMsg)
    {
        int headlen = 4;
        int srclen = srcMsg.Length;
        destMsg = new byte[headlen + srclen];
        byte[] head = BitConverter.GetBytes(msgType);
        Array.Copy(head, 0, destMsg, 0, headlen);
        Array.Copy(srcMsg, 0, destMsg, headlen, srclen);
    }
}

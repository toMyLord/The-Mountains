#define DEBUG_MODE
#define TEXT_DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int userid;
    public int UserID
    {
        get
        {
            return userid;
        }
        set
        {
            //通知UI修改ID；
#if DEBUG_MODE
            Debug.Log("User ID已更新");
#endif
            userid = value;
            UpdateInfo();
        }
    }
    private string username;
    public string UserName
    {
        get
        {
            return username;
        }
        set
        {
#if DEBUG_MODE
            Debug.Log("User Name已更新");
#endif
            username = value;
            UpdateInfo();
        }
    }
    private int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
#if DEBUG_MODE
            Debug.Log("User Score已更新");
#endif
            score = value;
            UpdateInfo();
        }
    }

    public delegate void PlayerInfoUpdate(Player player);
    public event PlayerInfoUpdate OnPlayerInfoUpdate;

    void UpdateInfo()   //每次对player信息写入时都会触发事件，通知UI更新
    {
#if DEBUG_MODE
        Debug.Log("正在通知UI更新PlayerInfo");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("正在通知UI更新PlayerInfo");
#endif
        OnPlayerInfoUpdate(this);
    }


    public Player(int _id, string _name, int _score)
    {
        userid = _id;
        username = _name;
        score = _score;
    }
    //历史战绩；好友列表
}                                                                                                                                                                
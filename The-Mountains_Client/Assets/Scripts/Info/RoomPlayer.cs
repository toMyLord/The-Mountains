using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : MonoBehaviour     //房间内玩家对象，从tablemanager 的 roomplayerlist 中修改属性，这边就会自动更新玩家状态UI显示
{
    private string playerName;
    public string PlayerName
    {
        set
        {
            playerName = value;
            nameText.text = playerName;
        }
        get
        {
            return playerName;
        }
    }
    private int score;
    public int Score
    {
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
        get
        {
            return score;
        }
    }
    private int seatNum;
    public int SeatNum
    {
        set
        {
            seatNum = value;
        }
        get
        {
            return seatNum;
        }
    }
    private int cardNum;
    public int CardNum
    {
        set
        {
            cardNum = value;
            cardNumText.text = cardNum.ToString();
        }
        get
        {
            return cardNum;
        }
    }
    private int witchNum;
    public int WitchNum
    {
        set
        {
            witchNum = value;
            witchNumText.text = witchNum.ToString();
        }
        get
        {
            return witchNum;
        }
    }
    private Text nameText;
    private Text scoreText;
    private Text cardNumText;
    private Text witchNumText;
    private bool isOnLine;
    public bool IsOnLine
    {
        get
        {
            return isOnLine;
        }
        set
        {
            isOnLine = value;
            if(isOnLine)
            {
                offLineIMG.SetActive(false);
            }
            else
            {
                offLineIMG.SetActive(true);
            }
        }
    }
    private GameObject fogIMG;
    private GameObject woodIMG;
    private GameObject offLineIMG;
    public enum PlayerStatus	//玩家当前状态
    {
        Normal = 0,
        Wood = 1,
        Fog = 2,
    }
    private PlayerStatus currPlayerStatus;
    public PlayerStatus CurrPlayerStatus
    {
        set
        {
            currPlayerStatus = value;
            switch (currPlayerStatus)
            {
                case PlayerStatus.Normal:
                    fogIMG.SetActive(false);
                    woodIMG.SetActive(false);
                    break;
                case PlayerStatus.Fog:
                    woodIMG.SetActive(false);
                    fogIMG.SetActive(true);
                    break;
                case PlayerStatus.Wood:
                    woodIMG.SetActive(true);
                    fogIMG.SetActive(false);
                    break;
            }
        }
        get
        {
            return currPlayerStatus;
        }
    }
    public void InitFromOtherPlayerInfo(OtherPlayerInfo opi)
    {
        PlayerName = opi.Name;
        Score = opi.Score;
        SeatNum = opi.SeatNum;
        CardNum = opi.CardNum;
        WitchNum = opi.WitchNum;
        CurrPlayerStatus = (PlayerStatus)(opi.CurrPlayerStatus);
    }

    private void Awake()
    {
        nameText = transform.Find("NameText").GetComponent<Text>();
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        cardNumText = transform.Find("CardNumText").GetComponent<Text>();
        witchNumText = transform.Find("WitchCardNumText").GetComponent<Text>();
        woodIMG = transform.Find("Wood").gameObject;    //初始化特效图片
        fogIMG = transform.Find("Fog").gameObject;
        offLineIMG = transform.Find("OffLineIMG").gameObject;
        offLineIMG.SetActive(false);
        woodIMG.SetActive(false);
        fogIMG.SetActive(false);
    }
}

#define DEBUG_MODE
#define TEXT_DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableManager : MonoBehaviour
{
    private static TableManager instance;
    public static TableManager Instance
    {
        get
        {
            return instance;
        }
    }
    public GameObject cardObject;
    public GameObject roomPlayerObject;
    private Button settingButton;
    private Text timerText;
    private int personNum;
    #region 玩家选项
    private GameObject localPlayerOperationMenu;
    private Button compoundButton;
    private Button transferButton;
    private Button useButton;
    private Slider timerSlider;
    private Image timerSliderFillColor;
    public Color startSliderFillColor;
    public Color endSliderFillColor;
    #endregion
    private GameObject localPlayerCardContent;//本地玩家手卡容器
    #region 牌库Text
    private Text candleCardNumText;
    private Text woodCardNumText;
    private Text fogCardNumText;
    private Text witchCardNumText;
    #endregion
    private GameObject tableFace;
    public List<Card> localPlayerCardList;  //本地玩家手卡list
    public List<Card> choosedCardList;  //点击选中卡牌时把卡牌添加到该list，取消选中时卡牌从该list移除
    public List<RoomPlayer> roomPlayerList;     //接收玩家信息
    public List<GameObject> seatList;   //包含座位obj的list
    private RoomInfo ri;    //本房间信息
    private GameObject threeTable;
    private GameObject fourTable;
    private GameObject fiveTable;
    private GameObject currTable;   //本局正在使用的桌子
    private GameObject currRoundTip;    //UI提示当前在操作的玩家
    public RoomInfo roomInfo
    {
        get
        {
            return ri;
        }
        set
        {
            ri = value;
            UpdateTime();   //同步时间，从GameUIController获取房间信息时触发
            UpdateCardPoolInfoUI();   //同步牌库
        }
    }
    private int playerInfoNum;  //接收服务器消息时统计的玩家信息数量
    private int localPlayerSeatNum; //本机玩家的座位号
    private PlayerOperation.Types.CardType compoundTargetCardType;  //合成的目标卡
    private GameObject nextPlayerCardPoolContentBG;     //下家手牌BG
    private RectTransform nextPlayerCardPoolContentBGRectTransform; //下家手牌BG的Rect Transform组件
    private GameObject nextPlayerCardPoolContent;       //下家手牌容器
    private Button nextPlayerCardPoolContentCloseButton;    //下家手牌面板关闭按钮
    public List<Card> nextPlayerCardList;   //烛牌反馈时使用的临时list
    private int currOperationPlayerNum; //当前是哪个玩家的回合
    public int CurrOperationPlayerNum
    {
        set
        {
            currOperationPlayerNum = value;
        }
        get
        {
            return currOperationPlayerNum;
        }
    }
    private bool isReconnection;  //判断本机目前是否正在准备恢复操作
    public float cardMoveSpeed; //卡牌移动动画的速度
    public GameObject cardBack;
    public GameObject waterCardFace;
    public GameObject fireCardFace;
    public GameObject lightCardFace;
    public GameObject candleCardFace;
    public GameObject woodCardFace;
    public GameObject fogCardFace;
    public GameObject witchCardFace;
    public float maxOperationTime;
    private float timer;
    private GameObject cardAnimationContent;

    #region GameFinish面板的物体
    private GameObject gameFinishPanel;
    private GameObject gameFinishWinText;
    private GameObject gameFinishFailText;
    private Button gameFinishCloseButton;
    private Text gameFinishUserNameText;
    private Text gameFinishScoreText;
    private Text gameFinishWaterNum;
    private Text gameFinishFireNum;
    private Text gameFinishLightNum;
    private Text gameFinishCandleNum;
    private Text gameFinishWoodNum;
    private Text gameFinishFogNum;
    private Text gameFinishWitchNum;
    #endregion

    private void Awake()
    {
        instance = this;
        localPlayerCardList = new List<Card>();
        choosedCardList = new List<Card>();
        roomPlayerList = new List<RoomPlayer>();
        seatList = new List<GameObject>();
        nextPlayerCardList = new List<Card>();
        settingButton = transform.Find("SettingButton").GetComponent<Button>();
        timerText = transform.Find("Table/TimerBG/TimerText").GetComponent<Text>();
        
        localPlayerOperationMenu = transform.Find("Table/LocalPlayerCardPool/LocalPlayerOperationMenu").gameObject;
        compoundButton = transform.Find("Table/LocalPlayerCardPool/LocalPlayerOperationMenu/CompoundButton").GetComponent<Button>();
        transferButton = transform.Find("Table/LocalPlayerCardPool/LocalPlayerOperationMenu/TransferButton").GetComponent<Button>();
        useButton = transform.Find("Table/LocalPlayerCardPool/LocalPlayerOperationMenu/UseButton").GetComponent<Button>();
        timerSlider = transform.Find("Table/LocalPlayerCardPool/LocalPlayerOperationMenu/TimerSlider").GetComponent<Slider>();
        timerSliderFillColor = transform.Find("Table/LocalPlayerCardPool/LocalPlayerOperationMenu/TimerSlider/Fill Area/Fill").GetComponent<Image>();
        cardAnimationContent = transform.Find("CardAnimationContent").gameObject;
        localPlayerCardContent = transform.Find("Table/LocalPlayerCardPool/LocalPlayerCardContent").gameObject;
        candleCardNumText = transform.Find("Table/CardPoolBG/CandleCardNumText").GetComponent<Text>();
        woodCardNumText = transform.Find("Table/CardPoolBG/WoodCardNumText").GetComponent<Text>();
        fogCardNumText = transform.Find("Table/CardPoolBG/FogCardNumText").GetComponent<Text>();
        witchCardNumText = transform.Find("Table/CardPoolBG/WitchCardNumText").GetComponent<Text>();
        tableFace = transform.Find("Table/TableFace").gameObject;
        nextPlayerCardPoolContentBG = transform.Find("Table/NextPlayerCardPoolContentBG").gameObject;
        nextPlayerCardPoolContentBGRectTransform = nextPlayerCardPoolContentBG.GetComponent<RectTransform>();
        nextPlayerCardPoolContent = transform.Find("Table/NextPlayerCardPoolContentBG/NextPlayerCardPoolContent").gameObject;
        nextPlayerCardPoolContentCloseButton = transform.Find("Table/NextPlayerCardPoolContentBG/CloseButton").GetComponent<Button>();
        currRoundTip = transform.Find("Table/PlayerSeats/CurrRoundTip").gameObject;
        nextPlayerCardPoolContentCloseButton.onClick.AddListener(OnNextPlayerCardPoolContentCloseButtonClick);
        gameFinishPanel = transform.Find("Table/GameFinishPanel").gameObject;
        gameFinishWinText = transform.Find("Table/GameFinishPanel/WinText").gameObject;
        gameFinishFailText = transform.Find("Table/GameFinishPanel/FailText").gameObject;
        gameFinishCloseButton = transform.Find("Table/GameFinishPanel/CloseButton").GetComponent<Button>();
        gameFinishCloseButton.onClick.AddListener(() => { 
            for(int i = 0;i<roomPlayerList.Count;i++)
            {
                Destroy(roomPlayerList[i].gameObject);
            }
            for(int i = 0;i<localPlayerCardList.Count;i++)
            {
                Destroy(localPlayerCardList[i].gameObject);
            }
            for(int i = 0;i<nextPlayerCardList.Count;i++)
            {
                Destroy(nextPlayerCardList[i].gameObject);
            }
            Transform[] animationCards = cardAnimationContent.transform.GetComponentsInChildren<Transform>();
            for(int i = 1;i<animationCards.Length;i++)
            {
                Destroy(animationCards[i].gameObject);
            }
            nextPlayerCardPoolContentBG.SetActive(false);
            roomPlayerList.Clear();
            localPlayerCardList.Clear();
            nextPlayerCardList.Clear();
            gameObject.SetActive(false);
            gameFinishPanel.SetActive(false);
        });
        gameFinishUserNameText = transform.Find("Table/GameFinishPanel/StatisticsBG/WinnerNameText").GetComponent<Text>();
        gameFinishScoreText = transform.Find("Table/GameFinishPanel/StatisticsBG/ScoreText").GetComponent<Text>();
        gameFinishWaterNum = transform.Find("Table/GameFinishPanel/StatisticsBG/WaterNumText").GetComponent<Text>();
        gameFinishFireNum = transform.Find("Table/GameFinishPanel/StatisticsBG/FireNumText").GetComponent<Text>();
        gameFinishLightNum = transform.Find("Table/GameFinishPanel/StatisticsBG/LightNumText").GetComponent<Text>();
        gameFinishCandleNum = transform.Find("Table/GameFinishPanel/StatisticsBG/CandleNumText").GetComponent<Text>();
        gameFinishWoodNum = transform.Find("Table/GameFinishPanel/StatisticsBG/WoodNumText").GetComponent<Text>();
        gameFinishFogNum = transform.Find("Table/GameFinishPanel/StatisticsBG/FogNumText").GetComponent<Text>();
        gameFinishWitchNum = transform.Find("Table/GameFinishPanel/StatisticsBG/WitchNumText").GetComponent<Text>();
        threeTable = transform.Find("Table/PlayerSeats/ThreeSeats").gameObject;
        fourTable = transform.Find("Table/PlayerSeats/FourSeats").gameObject;
        fiveTable = transform.Find("Table/PlayerSeats/FiveSeats").gameObject;
        compoundButton.onClick.AddListener(OnCompoundButtonClick);
        transferButton.onClick.AddListener(OnTransferButtonClick);
        useButton.onClick.AddListener(OnUseButtonClick);
        ConnectionManager.Instance.RecvLocalPlayerInfo += OnRecvLocalPlayerInfo;
        ConnectionManager.Instance.RecvOtherPlayerInfo += OnRecvOtherPlayerInfo;
        ConnectionManager.Instance.RecvCandleCardFeedback += OnRecvCandleCardFeedback;
        ConnectionManager.Instance.RecvPlayerOperationPackage += OnRecvPlayerOperationPackage;
        ConnectionManager.Instance.RecvPlayerOffLine += OnRecvPlayerOffLine;
        ConnectionManager.Instance.RecvPlayerOnLine += OnRecvPlayerOnLine;
        ConnectionManager.Instance.RecvGameFinish += OnRecvGameFinish;
        ConnectionManager.Instance.RecvOperationQueueTail += OnRecvOperationQueueTail;
        Init(); //初始化牌桌和变量
    }

    private void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// 打扫牌桌
    /// </summary>
    void Init()
    {
#if DEBUG_MODE
        Debug.Log("牌桌已经打扫干净了~");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("牌桌已经打扫干净了~");
#endif
        gameFinishPanel.SetActive(false);
        threeTable.SetActive(false);    //隐藏座位
        fourTable.SetActive(false);
        fiveTable.SetActive(false);
        SettingOperationPanelStatus(false); //隐藏操作面板
        //timerText.text = "00:00";
        localPlayerCardList.Clear();
        choosedCardList.Clear();
        roomPlayerList.Clear();
        seatList.Clear();
        nextPlayerCardList.Clear();
        playerInfoNum = 0;  //打开牌桌时等待接收玩家信息，同时玩家信息数量为0
        roomInfo = GameUIController.Instance.roomInfo;  //给roominfo赋值时会更新计时器、顶部牌库UI
        currOperationPlayerNum = 1; //table初始化时默认该回合是1号玩家的
        isReconnection = false;   //设置是否处于重连状态的标记
        InitRoomSeats();    //根据玩家数量确定房间人数类型（三人房，四人房，五人房）
    }

    /// <summary>
    /// 收到下家手牌时触发
    /// </summary>
    void OnRecvCandleCardFeedback(CandleCardFeedback ccf)
    {
        int nextPlayerNum = GetNextPlayerNum(localPlayerSeatNum);
        if (ccf.SeatNum == nextPlayerNum)
        {
#if DEBUG_MODE
            Debug.Log("收到下家手牌信息");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("收到下家手牌信息");
#endif
            nextPlayerCardPoolContentBG.SetActive(true);
            int nextPlayerCardNum = roomPlayerList[nextPlayerNum - 1].CardNum - roomPlayerList[nextPlayerNum - 1].WitchNum;
            float BGwidth;
            if (nextPlayerCardNum <= 1)
            {
                BGwidth = 96f + 220.4f * nextPlayerCardNum;
            }
            else
            {
                BGwidth = 96f + 220.4f + 93.64f * (nextPlayerCardNum - 1);
            }
            nextPlayerCardPoolContentBGRectTransform.sizeDelta = new Vector2(BGwidth, nextPlayerCardPoolContentBGRectTransform.rect.height); //没有卡牌的面板长度
#if DEBUG_MODE
            Debug.Log("下家手牌：" + "水->" + ccf.WaterNum + " " + "火->" + ccf.FireNum + " " + "光->" + ccf.LightNum + " " + "烛->" + ccf.CandleNum + " " + "木->" + ccf.WoodNum + " " + "雾->" + ccf.FogNum);
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("下家手牌：" + "水->" + ccf.WaterNum + " " + "火->" + ccf.FireNum + " " + "光->" + ccf.LightNum + " " + "烛->" + ccf.CandleNum + " " + "木->" + ccf.WoodNum + " " + "雾->" + ccf.FogNum);
#endif
            InitCardList(nextPlayerCardList, nextPlayerCardPoolContent.transform, ccf.WaterNum, ccf.FireNum, ccf.LightNum, ccf.CandleNum, ccf.WoodNum, ccf.FogNum); //给下家手牌面板初始化，加牌
        }
    }

    /// <summary>
    /// 关闭烛牌反馈面板
    /// </summary>
    void OnNextPlayerCardPoolContentCloseButtonClick()
    {
#if DEBUG_MODE
        Debug.Log("关闭下家手牌面板");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("关闭下家手牌面板");
#endif
        nextPlayerCardPoolContentBG.SetActive(false);
        ClearCardList(nextPlayerCardList);
    }

    /// <summary>
    /// 根据给的座位号获取下家座位号
    /// </summary>
    /// <param name="_seatNum"></param>
    /// <returns></returns>
    int GetNextPlayerNum(int _seatNum)
    {
        return _seatNum % roomPlayerList.Count + 1;
    }

    /// <summary>
    /// 还原被选中的卡牌
    /// </summary>
    void RevertChoosedCard()
    {
        for (int i = 0; i < choosedCardList.Count; i++)
        {
            choosedCardList[i].IsChoosed = false;
        }
        choosedCardList.Clear();
    }

    /// <summary>
    /// 收到玩家操作（包括自己的操作）时触发，控制牌桌循环
    /// </summary>
    void OnRecvPlayerOperationPackage(PlayerOperation po)
    {
#if DEBUG_MODE
        Debug.Log("收到其他玩家操作");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("收到其他玩家操作");
#endif
        //TODO 根据currOperationPlayerNum更新回合UI，根据操作更新其他玩家的卡牌数量和总牌库，动画显示操作的过程
        int nextPlayerNum = GetNextPlayerNum(po.SeatNum);   //发送该操作包玩家的下一个玩家序号
        StopCoroutine("RoundTimer");    //如果收到任何操作就停止计时，取消计时结束要发送的skip包
        //如果是本机玩家的话就把chooselist的card全都恢复到未选中并且清空chooselist
        if ((po.SeatNum == localPlayerSeatNum) && choosedCardList.Count > 0)
        {
            RevertChoosedCard();
        }
        if (nextPlayerNum == localPlayerSeatNum) //如果下一个玩家就是本机玩家，会根据该消息给的玩家状态更新操作面板，防止出现玩家提前点好合成的组合卡牌卡bug点合成
        {
            UpdateOperationPanel();
        }
        switch (po.Operation)
        {
            case PlayerOperation.Types.OperationType.Compound:  //合成操作：当前玩家牌数减少，牌库减少
                {
                    switch (po.Card)
                    {
                        case PlayerOperation.Types.CardType.Candle:
                            StartCoroutine(InitAndMoveObjectFromAToB(candleCardFace, 0.5f, candleCardNumText.transform.position, roomPlayerList[po.SeatNum - 1].transform.position));   //播放合成动画
                            SoundManager.Instance.PlayCandleSound();
                            roomInfo.CandleNum--;
                            roomPlayerList[po.SeatNum - 1].CardNum--;
                            if (po.SeatNum == localPlayerSeatNum)
                            {
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Fire);
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Light);
                            }
                            break;
                        case PlayerOperation.Types.CardType.Wood:
                            StartCoroutine(InitAndMoveObjectFromAToB(woodCardFace, 0.5f, woodCardNumText.transform.position, roomPlayerList[po.SeatNum - 1].transform.position));
                            SoundManager.Instance.PlayWoodSound();
                            roomInfo.WoodNum--;
                            roomPlayerList[po.SeatNum - 1].CardNum--;
                            if (po.SeatNum == localPlayerSeatNum)
                            {
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Water);
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Light);
                            }
                            break;
                        case PlayerOperation.Types.CardType.Fog:
                            StartCoroutine(InitAndMoveObjectFromAToB(fogCardFace, 0.5f, fogCardNumText.transform.position, roomPlayerList[po.SeatNum - 1].transform.position));
                            SoundManager.Instance.PlayFogSound();
                            roomInfo.FogNum--;
                            roomPlayerList[po.SeatNum - 1].CardNum--;
                            if (po.SeatNum == localPlayerSeatNum)
                            {
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Water);
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Fire);
                            }
                            break;
                        case PlayerOperation.Types.CardType.Witch:
                            StartCoroutine(InitAndMoveObjectFromAToB(witchCardFace, 0.5f, witchCardNumText.transform.position, roomPlayerList[po.SeatNum - 1].transform.position));
                            SoundManager.Instance.PlayWitchSound();
                            roomInfo.WitchNum--;
                            roomPlayerList[po.SeatNum - 1].CardNum -= 2;
                            roomPlayerList[po.SeatNum - 1].WitchNum++;
                            if (po.SeatNum == localPlayerSeatNum)
                            {
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Candle);
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Wood);
                                DeleteCardFromHand(PlayerOperation.Types.CardType.Fog);
                            }
                            break;
                    }
                    UpdateCardPoolInfoUI();   //任何玩家进行了合成操作后需要对牌库UI进行更新
                    if (po.SeatNum == localPlayerSeatNum/* && po.Card != PlayerOperation.Types.CardType.Witch*/)   //如果是本地玩家的操作就需要把目标合成卡牌加入手牌
                    {
                        InsertCardToHand(po.Card);
                    }
                    break;
                }
            case PlayerOperation.Types.OperationType.Transfer:  //转移操作：当前玩家手牌-1，下一玩家手牌+1;
                {
                    SoundManager.Instance.OnCardClick();
                    //如果是该玩家自己的操作就删除卡牌
                    if(po.SeatNum == localPlayerSeatNum)
                    {
                        DeleteCardFromHand(po.Card);
                        StartCoroutine(InitAndMoveObjectFromAToB(cardBack, 0.5f, localPlayerCardContent.transform.position, roomPlayerList[nextPlayerNum - 1].transform.position));
                    }
                    //判断下一个玩家是不是本机玩家，如果是就插入该牌
                    else if (nextPlayerNum == localPlayerSeatNum)
                    {
                        InsertCardToHand(po.Card);
                        switch(po.Card) //根据卡牌类型播放动画，目的是提示本机收到得哪种牌
                        {
                            case PlayerOperation.Types.CardType.Water:
                                StartCoroutine(InitAndMoveObjectFromAToB(waterCardFace, 0.5f, roomPlayerList[po.SeatNum-1].transform.position, localPlayerCardContent.transform.position));
                                break;
                            case PlayerOperation.Types.CardType.Fire:
                                StartCoroutine(InitAndMoveObjectFromAToB(fireCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, localPlayerCardContent.transform.position));
                                break;
                            case PlayerOperation.Types.CardType.Light:
                                StartCoroutine(InitAndMoveObjectFromAToB(lightCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, localPlayerCardContent.transform.position));
                                break;
                            case PlayerOperation.Types.CardType.Candle:
                                StartCoroutine(InitAndMoveObjectFromAToB(candleCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, localPlayerCardContent.transform.position));
                                break;
                            case PlayerOperation.Types.CardType.Wood:
                                StartCoroutine(InitAndMoveObjectFromAToB(woodCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, localPlayerCardContent.transform.position));
                                break;
                            case PlayerOperation.Types.CardType.Fog:
                                StartCoroutine(InitAndMoveObjectFromAToB(fogCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, localPlayerCardContent.transform.position));
                                break;
                        }
                    }
                    else
                    {
                        StartCoroutine(InitAndMoveObjectFromAToB(cardBack, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, roomPlayerList[nextPlayerNum - 1].transform.position));
                    }
                    //else if()
                    roomPlayerList[po.SeatNum - 1].CardNum--;
                    roomPlayerList[nextPlayerNum - 1].CardNum++;
                    break;
                }
            case PlayerOperation.Types.OperationType.Use:   //使用操作：当前玩家牌数-1，下个玩家状态改变
                {
                    //如果是该玩家自己的操作就删除卡牌
                    if (po.SeatNum == localPlayerSeatNum)
                    {
                        DeleteCardFromHand(po.Card);
                    }
                    roomPlayerList[po.SeatNum - 1].CardNum--;
                    switch (po.Card)
                    {
                        case PlayerOperation.Types.CardType.Wood:
                            StartCoroutine(InitAndMoveObjectFromAToB(woodCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, woodCardNumText.transform.position));
                            SoundManager.Instance.PlayWoodSound();
                            roomPlayerList[nextPlayerNum - 1].CurrPlayerStatus = RoomPlayer.PlayerStatus.Wood;
                            ri.WoodNum++;
                            break;
                        case PlayerOperation.Types.CardType.Fog:
                            StartCoroutine(InitAndMoveObjectFromAToB(fogCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, fogCardNumText.transform.position));
                            SoundManager.Instance.PlayFogSound();
                            roomPlayerList[nextPlayerNum - 1].CurrPlayerStatus = RoomPlayer.PlayerStatus.Fog;
                            ri.FogNum++;
                            break;
                        case PlayerOperation.Types.CardType.Candle: //烛操作的下家如果是本机，本机就发一个CandleCardFeedback包
                            StartCoroutine(InitAndMoveObjectFromAToB(candleCardFace, 0.5f, roomPlayerList[po.SeatNum - 1].transform.position, candleCardNumText.transform.position));
                            SoundManager.Instance.PlayCandleSound();
                            if (nextPlayerNum == localPlayerSeatNum && isReconnection == false)     //下家是自己并且目前不是重连恢复状态就发手牌给上家
                            {
                                CandleCardFeedback ccfb = new CandleCardFeedback();
                                ccfb.SeatNum = localPlayerSeatNum;
                                ccfb.WaterNum = ccfb.FireNum = ccfb.LightNum = ccfb.CandleNum = ccfb.WoodNum = ccfb.FogNum = 0;
                                //统计本机玩家各种卡牌数量后发送 ccfb
                                for (int i = 0; i < localPlayerCardList.Count; i++)
                                {
                                    switch (localPlayerCardList[i].CardType)
                                    {
                                        case PlayerOperation.Types.CardType.Water: ccfb.WaterNum++; break;
                                        case PlayerOperation.Types.CardType.Fire: ccfb.FireNum++; break;
                                        case PlayerOperation.Types.CardType.Light: ccfb.LightNum++; break;
                                        case PlayerOperation.Types.CardType.Candle: ccfb.CandleNum++; break;
                                        case PlayerOperation.Types.CardType.Wood: ccfb.WoodNum++; break;
                                        case PlayerOperation.Types.CardType.Fog: ccfb.FogNum++; break;
                                    }
                                }
                                ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.CandleCardFeedback, ccfb);
                            }
                            ri.CandleNum++;
                            roomPlayerList[nextPlayerNum - 1].CurrPlayerStatus = RoomPlayer.PlayerStatus.Normal;
                            break;
                    }
                    /*if (nextPlayerNum == localPlayerSeatNum) //如果下一个玩家就是本机玩家，会根据该消息给的玩家状态更新操作面板，防止出现玩家提前点好合成的组合卡牌卡bug点合成
                    {
                        UpdateOperationPanel();
                    }*/
                    UpdateCardPoolInfoUI();   //任何玩家进行了使用操作后需要对牌库UI进行更新
                    break;
                }
            case PlayerOperation.Types.OperationType.Skip:  //重连时收到Skip操作就直接跳过该回合
                {
#if DEBUG_MODE
                    Debug.Log("跳过本回合");
#endif
#if TEXT_DEBUG_MODE
                    DebugManager.Instance.Log("跳过本回合");
#endif
                    break;
                }
        }
        //某个玩家做了操作后会将自己的状态恢复成Normal
        roomPlayerList[po.SeatNum - 1].CurrPlayerStatus = RoomPlayer.PlayerStatus.Normal;
        if(po.SeatNum == localPlayerSeatNum)    //只有自己做了操作才有可能赢,所以玩家进行操作后收到自己的操作包需要检查是否赢了
        {
            GameFinishCheck();
        }
        UpdateRoundUI();
    }

    /// <summary>
    /// 计时协程，收到roominfo时开始计时
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerCollaboration()
    {
        int timer = ri.Time;
        while(true)
        {
            int min = timer / 60;
            int sec = timer % 60;
            string minStr = min.ToString();
            string secStr = sec.ToString();
            if(min<10)
            {
                minStr = "0" + min.ToString();
            }
            if(sec<10)
            {
                secStr = "0" + sec.ToString();
            }
            string time = minStr + ":" + secStr;
            timerText.text = time;
            timer++;
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// 根据玩家数量决定桌子
    /// </summary>
    void InitRoomSeats()
    {
        switch(ri.PlayerNum)
        {
            case 3:
                threeTable.SetActive(true);
                currTable = threeTable;
                break;
            case 4:
                fourTable.SetActive(true);
                currTable = fourTable;
                break;
            case 5:
                fiveTable.SetActive(true);
                currTable = fiveTable;
                break;
        }
    }

    /// <summary>
    /// 收到其他玩家信息触发
    /// </summary>
    /// <param name="opi"></param>
    void OnRecvOtherPlayerInfo(OtherPlayerInfo opi)
    {
        RoomPlayer rp = Instantiate(roomPlayerObject, gameObject.transform).GetComponent<RoomPlayer>();
        rp.InitFromOtherPlayerInfo(opi);
        roomPlayerList.Add(rp);
        playerInfoNum++;
        PlayerInfoIntegrityCheck();
    }

    /// <summary>
    /// 收到本机玩家信息触发
    /// </summary>
    /// <param name="lpi"></param>
    void OnRecvLocalPlayerInfo(LocalPlayerInfo lpi)
    {
        RoomPlayer rp = Instantiate(roomPlayerObject, gameObject.transform).GetComponent<RoomPlayer>();
        rp.PlayerName = PlayerManager.Instance.localPlayer.UserName;
        rp.Score = PlayerManager.Instance.localPlayer.Score;
        rp.SeatNum = lpi.SeatNum;
        localPlayerSeatNum = lpi.SeatNum;
        rp.CardNum = lpi.CardNum;
        rp.WitchNum = lpi.WitchNum;
        InitCardList(localPlayerCardList, localPlayerCardContent.transform, lpi.WaterNum, lpi.FireNum, lpi.LightNum, lpi.CandleNum, lpi.WoodNum, lpi.FogNum);   //给本地玩家创建手牌
        rp.CurrPlayerStatus = (RoomPlayer.PlayerStatus)lpi.CurrPlayerStatus;
        roomPlayerList.Add(rp);
        playerInfoNum++;
        PlayerInfoIntegrityCheck();
    }

    /// <summary>
    /// 回合操作计时控制
    /// </summary>
    /// <returns></returns>
    IEnumerator RoundTimer()
    {
        timer = 0;
        while(timer<maxOperationTime)
        {
            timer += Time.deltaTime;
            timerSlider.value = 1 - (timer / maxOperationTime); //更新计时条
            timerSliderFillColor.color = Color.Lerp(endSliderFillColor, startSliderFillColor, timerSlider.value);
            yield return 0;
        }
        PlayerOperation po = new PlayerOperation();
        po.SeatNum = localPlayerSeatNum;
        po.Operation = PlayerOperation.Types.OperationType.Skip;
        //po.Card = PlayerOperation.Types.CardType.
#if DEBUG_MODE
        Debug.Log("本回合没有进行操作，跳过");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("本回合没有进行操作，跳过");
#endif
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.PlayerOperation, po);  //规定时间不操作就直接发Skip包
    }

    /// <summary>
    /// 回合结束更新UI Tip状态，判断是否显示操作面板
    /// </summary>
    void UpdateRoundUI()
    {
        currOperationPlayerNum = GetNextPlayerNum(currOperationPlayerNum);
        currRoundTip.transform.position = roomPlayerList[currOperationPlayerNum - 1].transform.position;
        if (localPlayerSeatNum == currOperationPlayerNum)
        {
            //UpdateOperationPanel();
            localPlayerOperationMenu.SetActive(true);
            //开始计时，如果时间内没有操作就发送Skip包
            StartCoroutine("RoundTimer");
        }
        else
        {
            localPlayerOperationMenu.SetActive(false);
            //根据座位号显示玩家回合提示UI
        }
#if DEBUG_MODE
        Debug.Log("进入新的回合");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("进入新的回合");
#endif
    }

    /// <summary>
    /// 检查玩家信息接收是否完整，接收完整后进入游戏
    /// </summary>
    void PlayerInfoIntegrityCheck()
    {
        if (playerInfoNum == roomInfo.PlayerNum)    //当读取的信息数量等于房间人数时触发，进入游戏
        {
            InitRoomPlayerSeats();
            currRoundTip.transform.position = roomPlayerList[currOperationPlayerNum-1].transform.position;
            if (localPlayerSeatNum == currOperationPlayerNum)
            {
                localPlayerOperationMenu.SetActive(true);
                StartCoroutine("RoundTimer");
            }
            else
            {
                localPlayerOperationMenu.SetActive(false);
            }
            UIFeedbackSpawner.Instance.ShowTip("数据读取完成，开始游戏", Vector3.zero, 1);
        }
    }

    /// <summary>
    /// 玩家对号入座
    /// </summary>
    void InitRoomPlayerSeats()
    {
        roomPlayerList.Sort((RoomPlayer a, RoomPlayer b)=> { if (a.SeatNum > b.SeatNum) return 1; else return -1; });   //对room player list 按照座次排序
        for (int i = 0; i < roomInfo.PlayerNum; i++)   //获取桌子上的座位
        {
            GameObject seat = currTable.transform.Find("Seat" + (i+1)).gameObject;
            //将玩家和座位对号
            roomPlayerList[(i + localPlayerSeatNum - 1) % roomInfo.PlayerNum].transform.parent = seat.transform;
            roomPlayerList[(i + localPlayerSeatNum - 1) % roomInfo.PlayerNum].transform.position = seat.transform.position;
            seatList.Add(seat);
        }
    }

    /// <summary>
    /// 手牌生成
    /// </summary>
    /// <param name="list">生成的卡牌所在的list</param>
    /// <param name="trans">生成的卡牌所在的容器</param>
    /// <param name="waterNum"></param>
    /// <param name="fireNum"></param>
    /// <param name="lightNum"></param>
    /// <param name="candleNum"></param>
    /// <param name="woodNum"></param>
    /// <param name="fogNum"></param>
    void InitCardList(List<Card> list, Transform trans,int waterNum,int fireNum,int lightNum,int candleNum,int woodNum,int fogNum/*,int witchNum*/)
    {
        LoopCreateCard(list, trans,PlayerOperation.Types.CardType.Water, waterNum);
        LoopCreateCard(list,trans, PlayerOperation.Types.CardType.Fire, fireNum);
        LoopCreateCard(list,trans, PlayerOperation.Types.CardType.Light, lightNum);
        LoopCreateCard(list,trans, PlayerOperation.Types.CardType.Candle, candleNum);
        LoopCreateCard(list,trans, PlayerOperation.Types.CardType.Wood, woodNum);
        LoopCreateCard(list,trans, PlayerOperation.Types.CardType.Fog, fogNum);
        //LoopCreateCard(PlayerOperation.Types.CardType.Witch, witchNum);
    }

    /// <summary>
    /// 批量生成卡牌
    /// </summary>
    /// <param name="list">卡牌所在列表</param>
    /// <param name="trans">UI的父物体</param>
    /// <param name="ctype">卡牌类型</param>
    /// <param name="num">生成该卡牌的数量</param>
    void LoopCreateCard(List<Card> list,Transform trans,PlayerOperation.Types.CardType ctype,int num)
    {
        for(int i = 0;i<num;i++)
        {
            Card card = Instantiate(cardObject, trans).GetComponent<Card>();
            card.CardType = ctype;
            card.OnCardChoosed += OnCardChoosed;
            card.OnCardRemoved += OnCardRemoved;
            list.Add(card);
        }
    }

    /// <summary>
    /// 管理ChoosedCardList，并更新操作面板
    /// </summary>
    /// <param name="card"></param>
    void OnCardChoosed(Card card)
    {
        choosedCardList.Add(card);
        UpdateOperationPanel();
    }

    void OnCardRemoved(Card card)
    {
        choosedCardList.Remove(card);
        UpdateOperationPanel();
    }

    /// <summary>
    /// 设置操作面板
    /// </summary>
    /// <param name="enable"></param>
    void SettingOperationPanelStatus(bool enable)
    {
        compoundButton.enabled = enable;
        transferButton.enabled = enable;
        useButton.enabled = enable;
    }

    /// <summary>
    /// 更新操作面板
    /// </summary>
    void UpdateOperationPanel()
    {
#if DEBUG_MODE
        Debug.Log("操作面板按钮状态更新");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log("操作面板按钮状态更新");
#endif
        //先判断选中卡牌情况
        if (choosedCardList.Count == 1)
        {
            compoundButton.enabled = false;
            PlayerOperation.Types.CardType ctype = choosedCardList[0].CardType;
            switch (ctype)
            {
                case PlayerOperation.Types.CardType.Water:
                case PlayerOperation.Types.CardType.Fire:
                case PlayerOperation.Types.CardType.Light:
                    //case PlayerOperation.Types.CardType.Witch:
                    useButton.enabled = false;
                    break;
                case PlayerOperation.Types.CardType.Candle:
                case PlayerOperation.Types.CardType.Wood:
                case PlayerOperation.Types.CardType.Fog:
                    useButton.enabled = true;
                    break;
            }
            transferButton.enabled = true;
        }
        else if (choosedCardList.Count == 2)
        {
            transferButton.enabled = false;
            useButton.enabled = false;
            PlayerOperation.Types.CardType ctype1 = choosedCardList[0].CardType;
            PlayerOperation.Types.CardType ctype2 = choosedCardList[1].CardType;
            //PlayerOperation.Types.CardType resType = choosedCardList[0].CardType + choosedCardList[1].CardType;
            //Water = 1,Fire = 2,Light = 4,Candle = 6,Wood = 5,Fog = 3,Witch = 14
            if ((ctype1 == PlayerOperation.Types.CardType.Water || ctype1 == PlayerOperation.Types.CardType.Fire || ctype1 == PlayerOperation.Types.CardType.Light) && (ctype2 == PlayerOperation.Types.CardType.Water || ctype2 == PlayerOperation.Types.CardType.Fire || ctype2 == PlayerOperation.Types.CardType.Light) && ctype1 != ctype2)
            {
                int targetID = choosedCardList[0].CardID + choosedCardList[1].CardID;
                switch (targetID)
                {
                    case 3: compoundTargetCardType = PlayerOperation.Types.CardType.Fog; break;
                    case 5: compoundTargetCardType = PlayerOperation.Types.CardType.Wood; break;
                    case 6: compoundTargetCardType = PlayerOperation.Types.CardType.Candle; break;
                    default:
#if DEBUG_MODE
                        Debug.LogError("合成目标牌ID错误");
#endif
#if TEXT_DEBUG_MODE
                        DebugManager.Instance.Log("Error : 合成目标牌ID错误");
#endif
                        break;
                }
                compoundButton.enabled = true;
            }
            else
            {
                compoundButton.enabled = false;
            }
        }
        else if (choosedCardList.Count == 3 && choosedCardList[0].CardID + choosedCardList[1].CardID + choosedCardList[2].CardID == 14/*choosedCardList[0].CardType < PlayerOperation.Types.CardType.Candle && choosedCardList[1].CardType < PlayerOperation.Types.CardType.Candle && choosedCardList[2].CardType < PlayerOperation.Types.CardType.Candle && choosedCardList[0].CardID + choosedCardList[1].CardID + choosedCardList[2].CardID == 7*/)
        {
            compoundTargetCardType = PlayerOperation.Types.CardType.Witch;
            compoundButton.enabled = true;
        }
        else
        {
            SettingOperationPanelStatus(false);
        }
        //之后判断玩家状态
        if (roomPlayerList[localPlayerSeatNum - 1].CurrPlayerStatus == RoomPlayer.PlayerStatus.Fog)
        {
            compoundButton.enabled = false;
        }
        else if(roomPlayerList[localPlayerSeatNum - 1].CurrPlayerStatus == RoomPlayer.PlayerStatus.Wood)
        {
            transferButton.enabled = false;
        }
    }

    /// <summary>
    /// RoomInfo被赋值时触发，启动计时器和时间UI
    /// </summary>
    void UpdateTime()   //启动协程计时，更新UI时间
    {
        StartCoroutine("TimerCollaboration");
    }

    /// <summary>
    /// 更新顶部牌库UI
    /// </summary>
    void UpdateCardPoolInfoUI()
    {
        candleCardNumText.text = ri.CandleNum.ToString();
        woodCardNumText.text = ri.WoodNum.ToString();
        fogCardNumText.text = ri.FogNum.ToString();
        witchCardNumText.text = ri.WitchNum.ToString();
    }

    /// <summary>
    /// 清空指定List的物体
    /// </summary>
    /// <param name="list"></param>
    void ClearCardList(List<Card> list)
    {
        for(int i = 0;i< list.Count;i++)
        {
            localPlayerCardList.Remove(list[i]);
            list[i].OnCardChoosed -= OnCardChoosed;
            list[i].OnCardRemoved -= OnCardRemoved;
            Destroy(list[i].gameObject);
        }
        list.Clear();
    }

    void InsertCardToHand(PlayerOperation.Types.CardType ctype) //把指定类型的卡牌插入手卡
    {
        //插入一张某种类型的牌到手卡
        LoopCreateCard(localPlayerCardList, localPlayerCardContent.transform, ctype, 1);
        localPlayerCardContent.transform.DetachChildren();  //UI手牌容器分离所有卡牌
        //对手牌排序
        localPlayerCardList.Sort((Card a, Card b) => { if (a.CardType > b.CardType) return 1; else if (a.CardType == b.CardType) return 0 ; else return -1; });
        for(int i = 0;i<localPlayerCardList.Count;i++)  //重新插入手牌容器
        {
            localPlayerCardList[i].transform.parent = localPlayerCardContent.transform;
        }
    }

    void  DeleteCardFromHand(PlayerOperation.Types.CardType ctype)  //从手卡删除一张指定类型的牌
    {
        Card card = localPlayerCardList.Find((Card _card) => { if (_card.CardType == ctype) return true; else return false; });
        if (card != null)
        {
            localPlayerCardList.Remove(card);
            Destroy(card.gameObject);
        }
        else
        {
#if DEBUG_MODE
            Debug.LogError("没有该类型的牌可以删除，操作错误");
#endif
#if TEXT_DEBUG_MODE
            DebugManager.Instance.Log("Error : 没有该类型的牌可以删除，操作错误");
#endif
        }
    }

    /// <summary>
    /// 合成前检查牌库是否充足
    /// </summary>
    /// <param name="ctype"></param>
    /// <returns></returns>
    bool CompoundCheck(PlayerOperation.Types.CardType ctype)
    {
        bool isSuccessCompound = false;
        switch (compoundTargetCardType)
        {
            case PlayerOperation.Types.CardType.Candle:
                {
                    if (roomInfo.CandleNum > 0)
                    {
                        isSuccessCompound = true;
                    }
                    break;
                }
            case PlayerOperation.Types.CardType.Wood:
                {
                    if (roomInfo.WoodNum > 0)
                    {
                        isSuccessCompound = true;
                    }
                    break;
                }
            case PlayerOperation.Types.CardType.Fog:
                {
                    if (roomInfo.FogNum > 0)
                    {
                        isSuccessCompound = true;
                    }
                    break;
                }
            case PlayerOperation.Types.CardType.Witch:
                {
                    if (roomInfo.WitchNum > 0)
                    {
                        isSuccessCompound = true;
                    }
                    break;
                }
        }
        return isSuccessCompound;
    }    

    /// <summary>
    /// 生成玩家操作包用于发送（转移和使用）
    /// </summary>
    /// <param name="card">使用或转移的卡牌</param>
    /// <param name="playerOperation">操作方式</param>
    /// <returns></returns>
    PlayerOperation CreatePlayerOperationPackage(PlayerOperation.Types.CardType ctype, PlayerOperation.Types.OperationType playerOperation)
    {
        PlayerOperation po = new PlayerOperation();
        po.SeatNum = localPlayerSeatNum;
        po.Operation = playerOperation;
        po.Card = ctype;
        return po;
    }

    /// <summary>
    /// 点击组合按钮
    /// </summary>
    void OnCompoundButtonClick()
    {
        Debug.Log("合成手牌中");
        if (CompoundCheck(compoundTargetCardType))  //检查牌库够不够
        {
            StopCoroutine("RoundTimer");
            RevertChoosedCard();    //把选中的卡牌复原
            localPlayerOperationMenu.SetActive(false);
            Debug.Log("手牌合成成功:" + compoundTargetCardType);
            PlayerOperation po = CreatePlayerOperationPackage(compoundTargetCardType, PlayerOperation.Types.OperationType.Compound);
            ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.PlayerOperation, po);  //向服务器发送玩家操作
            Debug.Log("玩家操作已发送");
        }
        else
        {
            UIFeedbackSpawner.Instance.ShowTip("牌库数量不足，无法合成", Vector3.zero, 1f);
        }
    }

    /// <summary>
    /// 点击转移按钮
    /// </summary>
    void OnTransferButtonClick()
    {
        StopCoroutine("RoundTimer");
        localPlayerOperationMenu.SetActive(false);
        PlayerOperation po = CreatePlayerOperationPackage(choosedCardList[0].CardType, PlayerOperation.Types.OperationType.Transfer);
        RevertChoosedCard();    //把选中的卡牌复原
        Debug.Log("已转移：" + po.Card);
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.PlayerOperation, po);  //向服务器发送玩家操作
        Debug.Log("转移操作已发送");
    }

    /// <summary>
    /// 点击使用按钮
    /// </summary>
    void OnUseButtonClick()
    {
        StopCoroutine("RoundTimer");
        localPlayerOperationMenu.SetActive(false);
        PlayerOperation po = CreatePlayerOperationPackage(choosedCardList[0].CardType, PlayerOperation.Types.OperationType.Use);
        RevertChoosedCard();    //把选中的卡牌复原
        Debug.Log("已使用：" + po.Card);
        ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.PlayerOperation, po);  //向服务器发送玩家操作
        Debug.Log("使用操作已发送");
    }

    /// <summary>
    /// 收到本机玩家的操作后本机执行检测
    /// </summary>
    void GameFinishCheck()
    {
        int waterNum = 0, fireNum = 0, lightNum = 0, candleNum = 0, woodNum = 0, fogNum = 0, witchNum = roomPlayerList[localPlayerSeatNum - 1].WitchNum;
        for (int i = 0; i < localPlayerCardList.Count; i++)
        {
            switch(localPlayerCardList[i].CardType)
            {
                case PlayerOperation.Types.CardType.Water: waterNum++;break;
                case PlayerOperation.Types.CardType.Fire: fireNum++;break;
                case PlayerOperation.Types.CardType.Light: lightNum++; break;
                case PlayerOperation.Types.CardType.Candle: candleNum++; break;
                case PlayerOperation.Types.CardType.Wood: woodNum++; break;
                case PlayerOperation.Types.CardType.Fog: fogNum++; break;
            }
        }
        int cardTypeCount = 0;
        if (waterNum > 0)
            cardTypeCount++;
        if (fireNum > 0)
            cardTypeCount++;
        if (lightNum > 0)
            cardTypeCount++;
        if (candleNum > 0)
            cardTypeCount++;
        if (woodNum > 0)
            cardTypeCount++;
        if (fogNum > 0)
            cardTypeCount++;
        if (cardTypeCount <= 1 || localPlayerCardList.Count + roomPlayerList[localPlayerSeatNum - 1].WitchNum <= 2)    //手牌只有一种花色或者不超过两张手牌
        {
            //赢家对局得分
            int incScore = waterNum + fireNum + lightNum + candleNum + woodNum + fogNum + witchNum * 2;
            GameFinish gf = new GameFinish();
            gf.SeatNum = localPlayerSeatNum;
            gf.WaterNum = waterNum;
            gf.FireNum = fireNum;
            gf.LightNum = lightNum;
            gf.CandleNum = candleNum;
            gf.WoodNum = woodNum;
            gf.FogNum = fogNum;
            gf.WitchNum = witchNum;
            gf.GameScore = incScore;
            ConnectionManager.Instance.SendMSGToGameConnection(ConnectionManager.SendGameMSGType.GameFinish, gf);
            Debug.Log("已发送游戏结束包");
        }
    }

    /// <summary>
    /// 收到其他玩家的掉线信息
    /// </summary>
    void OnRecvPlayerOffLine(OffLineOrOnLine ooo)
    {
        Debug.Log("有玩家掉线");
        roomPlayerList[ooo.SeatNum - 1].IsOnLine = false;
    }

  /// <summary>
    /// 收到其他玩家的重连信息,如果是本机掉线则进入重连准备状态,准备接收操作队列,向服务器发送确认协议
    /// </summary>
    void OnRecvPlayerOnLine(OffLineOrOnLine ooo)
    {
        Debug.Log("有玩家重连");
        roomPlayerList[ooo.SeatNum - 1].IsOnLine = true;
        //判断是否准备接收后面的操作队列
        if (ooo.SeatNum == localPlayerSeatNum)
        {
            isReconnection = true;    //标记在接收到烛操作时不再处理烛操作包
        }
    }

    /// <summary>
    /// 收到gamefinish包时调用，初始化gamefinish面板并显示出来
    /// </summary>
    void OnRecvGameFinish(GameFinish gf)
    {
        Debug.Log("打开结算面板");
        gameFinishPanel.SetActive(true);
        if(gf.SeatNum == localPlayerSeatNum)    //本机赢了
        {
            SoundManager.Instance.InitWin();
            gameFinishWinText.SetActive(true);
            gameFinishFailText.SetActive(false);
            PlayerManager.Instance.localPlayer.Score += gf.GameScore;   //如果本机玩家赢了就把本地分数加上
        }
        else//本机输了
        {
            SoundManager.Instance.InitFail();
            gameFinishWinText.SetActive(false);
            gameFinishFailText.SetActive(true);
        }
        gameFinishUserNameText.text = roomPlayerList[gf.SeatNum - 1].PlayerName;
        gameFinishScoreText.text = gf.GameScore.ToString();
        gameFinishWaterNum.text = gf.WaterNum.ToString();
        gameFinishFireNum.text = gf.FireNum.ToString();
        gameFinishLightNum.text = gf.LightNum.ToString();
        gameFinishCandleNum.text = gf.CandleNum.ToString();
        gameFinishWoodNum.text = gf.WoodNum.ToString();
        gameFinishFogNum.text = gf.FogNum.ToString();
        gameFinishWitchNum.text = gf.WitchNum.ToString();
    }

    IEnumerator InitAndMoveObjectFromAToB(GameObject go,float goScale,Vector3 a,Vector3 b)  //播放一次物体移动的动画
    {
        GameObject tempGo = Instantiate(go, a, go.transform.rotation, cardAnimationContent.transform);
        tempGo.transform.localScale = tempGo.transform.localScale * goScale;
        while (/*tempGo.transform.position != b  && */Vector3.Distance(tempGo.transform.position, b) > 0.3f)
        {
            tempGo.transform.position = Vector3.MoveTowards(tempGo.transform.position, b, cardMoveSpeed * Time.deltaTime);
            yield return 0;
        }
        Destroy(tempGo);
    }

    void OnRecvOperationQueueTail()   //队列处理到了队尾，恢复重连状态标记位 isReconnection
    {
        isReconnection = false;
    }
}

#define DEBUG_MODE
#define TEXT_DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    #region 牌面
    public GameObject waterCardFace;
    public GameObject fireCardFace;
    public GameObject lightCardFace;
    public GameObject candleCardFace;
    public GameObject woodCardFace;
    public GameObject fogCardFace;
    public GameObject witchCardFace;
    #endregion

    /*public enum PlayerOperation.Types.CardType{
        Water = 1,
        Fire = 2,
        Light = 3,
        Candle = 4,
        Wood = 5,
        Fog = 6,
        Witch = 7
    }*/
    private Button cardButton;
    private PlayerOperation.Types.CardType cardType;
    public PlayerOperation.Types.CardType CardType   //生成卡牌时一定要给 CardType 手动赋值
    {
        set
        {
            cardType = value;
            UpdateCardFace();
        }
        get
        {
            return cardType;
        }
    }
    private GameObject cardFace;    //牌面子物体的引用
    private bool isChoosed;
    public bool IsChoosed
    {
        set
        {
            isChoosed = value;
            UpdataCardTransform();
        }
        get
        {
            return isChoosed;
        }
    }
    public Vector3 chooseOffset;    //卡牌选中之后的偏移
    public delegate void OperateCard(Card card);
    public event OperateCard OnCardChoosed;
    public event OperateCard OnCardRemoved;
    private int cardID;
    public int CardID
    {
        get
        {
            return cardID;
        }
    }

    private void Awake()
    {
        isChoosed = false;
        cardButton = gameObject.GetComponent<Button>();
        cardButton.onClick.AddListener(OnCardButtonClick);
        cardButton.onClick.AddListener(SoundManager.Instance.OnCardClick);
    }
    
    void UpdataCardTransform()
    {
        if(isChoosed)
        {
            cardFace.transform.position = transform.position + chooseOffset;
        }
        else
        {
            cardFace.transform.position = transform.position;
        }
    }

    void OnCardButtonClick()
    {
#if DEBUG_MODE
        Debug.Log(cardType + " on click");
#endif
#if TEXT_DEBUG_MODE
        DebugManager.Instance.Log(cardType + " on click");
#endif
        if (cardType!= PlayerOperation.Types.CardType.Witch) //让巫牌无法选中
        {
            IsChoosed = !IsChoosed;
            //cardFace.transform.position += chooseOffset;
            //chooseOffset = -chooseOffset;
            if (isChoosed)
            {
                OnCardChoosed(this);
            }
            else
            {
                OnCardRemoved(this);
            }
        }
        else
        {
            UIFeedbackSpawner.Instance.ShowTip("巫牌不可使用", Vector3.zero, 0.5f);
        }
    }

    void UpdateCardFace()   //给卡牌类型赋值时会添加牌面子物体
    {
        //Debug.Log("牌面初始化中");
        if(cardFace != null)
        {
            Destroy(cardFace);
        }
        switch (cardType)
        {
            case PlayerOperation.Types.CardType.Water:
                cardFace = Instantiate(waterCardFace, gameObject.transform);
                cardID = 1;
                break;
            case PlayerOperation.Types.CardType.Fire:
                cardFace = Instantiate(fireCardFace, gameObject.transform);
                cardID = 2;
                break;
            case PlayerOperation.Types.CardType.Light:
                cardFace = Instantiate(lightCardFace, gameObject.transform);
                cardID = 4;
                break;
            case PlayerOperation.Types.CardType.Candle:
                cardFace = Instantiate(candleCardFace, gameObject.transform);
                cardID = 6;
                break;
            case PlayerOperation.Types.CardType.Wood:
                cardFace = Instantiate(woodCardFace, gameObject.transform);
                cardID = 5;
                break;
            case PlayerOperation.Types.CardType.Fog:
                cardFace = Instantiate(fogCardFace, gameObject.transform);
                cardID = 3;
                break;
            case PlayerOperation.Types.CardType.Witch:
                cardFace = Instantiate(witchCardFace, gameObject.transform);
                cardID = 14;
                break;
        }
    }
}

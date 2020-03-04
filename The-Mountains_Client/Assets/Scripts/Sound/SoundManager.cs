using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_instance;
    public static SoundManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    [SerializeField]
    private AudioClip buttonClick_1;
    [SerializeField]
    private AudioClip RoomBGM;
    [SerializeField]
    private AudioClip GameBGM;
    [SerializeField]
    private AudioClip Game2BGM;
    [SerializeField]
    private AudioClip Win;
    [SerializeField]
    private AudioClip Fail;
    [SerializeField]
    private AudioClip ClickCard;
    [SerializeField]
    private AudioClip Candle;
    [SerializeField]
    private AudioClip Wood;
    [SerializeField]
    private AudioClip Fog;
    [SerializeField]
    private AudioClip Witch;
    private AudioSource BGM;

    private void Awake()
    {
        DontDestroyOnLoad(this);    //切换场景保留
        m_instance = this;
        BGM = GetComponent<AudioSource>();
        InitRoom();
    }

    public void InitRoom()
    {
        BGM.clip = RoomBGM;
        BGM.loop = true;
        BGM.Play();
    }

    public void InitGame()
    {
        BGM.clip = GameBGM;
        BGM.loop = true;
        BGM.Play();
    }

    public void InitHighPoint()
    {
        StartCoroutine("HighPoint");
    }

    public void InitWin()
    {
        StartCoroutine("WinSound");
    }

    public void InitFail()
    {
        StartCoroutine("FailSound");
    }

    IEnumerator HighPoint()
    {
        BGM.clip = Game2BGM;
        BGM.loop = false;
        BGM.Play();
        while(BGM.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        InitGame();
        yield return null;
    }

    IEnumerator WinSound()
    {
        BGM.clip = Win;
        BGM.loop = false;
        BGM.Play();
        while (BGM.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        InitRoom();
        yield return null;
    }

    IEnumerator FailSound()
    {
        BGM.clip = Fail;
        BGM.loop = false;
        BGM.Play();
        while (BGM.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        InitRoom();
        yield return null;
    }

    public void OnButtonClick() //按钮监听播放声音
    {
        AudioSource.PlayClipAtPoint(buttonClick_1, Vector3.zero);
    }

    public void OnCardClick()   //点击卡牌声音
    {
        AudioSource.PlayClipAtPoint(ClickCard, Vector3.zero);
    }

    public void PlayCandleSound()  //使用烛
    {
        AudioSource.PlayClipAtPoint(Candle, Vector3.zero);
    }

    public void PlayWoodSound()  //使用木
    {
        AudioSource.PlayClipAtPoint(Wood, Vector3.zero,100);
    }

    public void PlayFogSound()  //使用雾
    {
        AudioSource.PlayClipAtPoint(Fog, Vector3.zero);
    }

    public void PlayWitchSound()  //使用雾
    {
        AudioSource.PlayClipAtPoint(Witch, Vector3.zero);
    }
}

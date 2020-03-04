using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            return instance;
        }
    }
    public Player localPlayer;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        localPlayer = null;
        ConnectionManager.Instance.RecvUserInfo += OnRecvUserInfo;  //收到UserInfo包时更新本地用户信息
        StartCoroutine("QuitDetect");
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.RecvUserInfo -= OnRecvUserInfo;
    }

    void OnRecvUserInfo(UserInfo ui)
    {
        localPlayer = new Player(ui.Userid,ui.Username,ui.Score);
    }

    IEnumerator QuitDetect()
    {
        bool isStartTimer = false;
        float timer = 0;
        int touchCount = 0;
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isStartTimer = true;
                touchCount++;
            }
            if(isStartTimer)
            {
                timer += Time.deltaTime;
            }
            if (timer > 0.8f)
            {
                touchCount = 0;
                timer = 0;
                isStartTimer = false;
            }
            if(touchCount == 1)
            {
                UIFeedbackSpawner.Instance.ShowTip("再按一次退出游戏", Vector3.zero, 0.8f);
            }
            else if(touchCount >= 2)
            {
                Application.Quit();
            }
            yield return 0;
        }
    }
}

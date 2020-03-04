using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InitWindow : MonoBehaviour
{
    private static InitWindow m_instance;
    public static InitWindow Instance
    {
        get
        {
            return m_instance;
        }
    }
    private Text ConnectionInfoText;
    private Button StartGameButton;
    private Button ReConnectButton;

    private void Awake()
    {
        m_instance = this;
        ConnectionInfoText = transform.Find("ConnectionInfoText").GetComponent<Text>();
        StartGameButton = transform.Find("StartGameButton").GetComponent<Button>();
        ReConnectButton = transform.Find("ReConnectButton").GetComponent<Button>();
        ConnectionManager.Instance.LoginConnection.ConnectionStart += ConnectionStart;      //监听loginConnection的各种状态并方便输出到UI上
        ConnectionManager.Instance.LoginConnection.ConnectionFinish += ConnectionFinish;
        ConnectionManager.Instance.LoginConnection.ConnectionError += ConnectionError;
        StartGameButton.onClick.AddListener(OnStartGameButtonClicked);
        ReConnectButton.onClick.AddListener(ConnectionManager.Instance.ReConnectLoginServer);
        StartGameButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);   //播放按钮音频
        ReConnectButton.onClick.AddListener(SoundManager.Instance.OnButtonClick);
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.LoginConnection.ConnectionStart -= ConnectionStart;
        ConnectionManager.Instance.LoginConnection.ConnectionFinish -= ConnectionFinish;
        ConnectionManager.Instance.LoginConnection.ConnectionError -= ConnectionError;
    }

    void OnStartGameButtonClicked()
    {
        LoginUIController.Instance.InitWindow.SetActive(false);
        LoginUIController.Instance.LoginWindow.SetActive(true);
    }
    void ConnectionStart()  //Login连接启动时触发
    {
        ConnectionInfoText.gameObject.SetActive(true);
        StartGameButton.gameObject.SetActive(false);
        ReConnectButton.gameObject.SetActive(false);
        ConnectionInfoText.text = "正在连接服务器...";
    }

    void ConnectionFinish()
    {
        ConnectionInfoText.gameObject.SetActive(true);
        StartGameButton.gameObject.SetActive(true);
        ReConnectButton.gameObject.SetActive(false);
        ConnectionInfoText.text = "连接成功!";
    }

    void ConnectionError()
    {
        ConnectionInfoText.gameObject.SetActive(true);
        StartGameButton.gameObject.SetActive(false);
        ReConnectButton.gameObject.SetActive(true);
        ConnectionInfoText.text = "无法连接到服务器!";
    }
}

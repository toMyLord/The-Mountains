using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Google.Protobuf;
using System.Text;

public class TestManager : MonoBehaviour
{
    float heartbeatsTimer;

    private int seat;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        seat = 1;
    }

    IEnumerator HeartbeatsFunc()
    {
        float timer = 0;
        while(true)
        {
            if(timer>1)
            {
                timer = 0;
                ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(1, null));
            }
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    byte[] EncapsulateMSG(byte i, byte[] msg)
    {
        byte[] resmsg;
        if (msg == null)
        {
            resmsg = new byte[1];
            resmsg[0] = i;
        }
        else
        {
            resmsg = new byte[msg.Length + 1];
            resmsg[0] = i;
            Array.Copy(msg, 0, resmsg, 1, msg.Length);
        }
        return resmsg;
    }
    // Update is called once per frame
    void Update()
    {
        heartbeatsTimer += Time.deltaTime;
        if(heartbeatsTimer>3)
        {
            heartbeatsTimer = 0;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(1, null));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UserInfo ui = new UserInfo();
            ui.Userid = 1;
            ui.Username = "wzytest";
            ui.Score = 3;
            ConnectionManager.Instance.LoginMSGRecive(EncapsulateMSG(1, ui.ToByteArray()));
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(4, null));
            RoomInfo ri = new RoomInfo();
            ri.RoomID = 911;
            ri.Time = 57;
            ri.PlayerNum = 5;
            ri.CandleNum = 3;
            ri.WoodNum = 1;
            ri.FogNum = 6;
            ri.WitchNum = 1;
            //ri.CurrOperationPlayerNum = 3;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(5, ri.ToByteArray()));
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            OtherPlayerInfo opi1 = new OtherPlayerInfo();
            opi1.Name = "wzy2";
            opi1.Score = 11;
            opi1.SeatNum = 1;
            opi1.CardNum = 5;
            opi1.WitchNum = 0;
            opi1.CurrPlayerStatus = OtherPlayerInfo.Types.PlayerStatus.Normal;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(6, opi1.ToByteArray()));
            opi1.Name = "wzy3";
            opi1.Score = 22;
            opi1.SeatNum = 2;
            opi1.CardNum = 7;
            opi1.WitchNum = 3;
            opi1.CurrPlayerStatus = OtherPlayerInfo.Types.PlayerStatus.Normal;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(6, opi1.ToByteArray()));
            opi1.Name = "wzy4";
            opi1.Score = 33;
            opi1.SeatNum = 3;
            opi1.CardNum = 4;
            opi1.WitchNum = 4;
            opi1.CurrPlayerStatus = OtherPlayerInfo.Types.PlayerStatus.Fog;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(6, opi1.ToByteArray()));
            opi1.Name = "wzy5";
            opi1.Score = 44;
            opi1.SeatNum = 4;
            opi1.CardNum = 5;
            opi1.WitchNum = 5;
            opi1.CurrPlayerStatus = OtherPlayerInfo.Types.PlayerStatus.Wood;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(6, opi1.ToByteArray()));
            LocalPlayerInfo lpi = new LocalPlayerInfo();
            lpi.CardNum = 12;
            lpi.WaterNum = 3;
            lpi.FireNum = 3;
            lpi.LightNum = 3;
            lpi.CandleNum = 1;
            lpi.WoodNum = 1;
            lpi.FogNum = 1;
            lpi.WitchNum = 0;
            lpi.CurrPlayerStatus = LocalPlayerInfo.Types.PlayerStatus.Wood;
            lpi.SeatNum = 5;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(7, lpi.ToByteArray()));
        }
        /*if(Input.GetKeyDown(KeyCode.R))
        {
            CandleCardFeedback cffb = new CandleCardFeedback();
            cffb.WaterNum = 2;
            cffb.FireNum = 1;
            cffb.LightNum = 1;
            cffb.CandleNum = 1;
            cffb.WoodNum = 0;
            cffb.FogNum = 2;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(9, cffb.ToByteArray()));
        }*/
        if (Input.GetKeyDown(KeyCode.A))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Transfer;
            po.Card = PlayerOperation.Types.CardType.Candle;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.S))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Transfer;
            po.Card = PlayerOperation.Types.CardType.Wood;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.D))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Transfer;
            po.Card = PlayerOperation.Types.CardType.Fog;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.Z))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Use;
            po.Card = PlayerOperation.Types.CardType.Candle;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.X))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Use;
            po.Card = PlayerOperation.Types.CardType.Wood;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.C))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Use;
            po.Card = PlayerOperation.Types.CardType.Fog;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.V))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Compound;
            po.Card = PlayerOperation.Types.CardType.Candle;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.B))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Compound;
            po.Card = PlayerOperation.Types.CardType.Wood;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.N))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Compound;
            po.Card = PlayerOperation.Types.CardType.Fog;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.M))    //收到玩家信息包
        {
            PlayerOperation po = new PlayerOperation();
            po.SeatNum = seat;
            po.Operation = PlayerOperation.Types.OperationType.Compound;
            po.Card = PlayerOperation.Types.CardType.Witch;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(10, po.ToByteArray()));
            seat = seat % 5 + 1;
        }
        if (Input.GetKeyDown(KeyCode.F))    //收到游戏结束包
        {
            GameFinish gf = new GameFinish();
            gf.SeatNum = 3;
            gf.WaterNum = 0;
            gf.FireNum = 0;
            gf.LightNum = 3;
            gf.CandleNum = 0;
            gf.WoodNum = 0;
            gf.FogNum = 2;
            gf.WitchNum = 3;
            gf.GameScore = 5;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(8, gf.ToByteArray()));
        }
        if (Input.GetKeyDown(KeyCode.G))    //收到游戏结束包
        {
            OffLineOrOnLine ooo = new OffLineOrOnLine();
            ooo.SeatNum = 3;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(11, ooo.ToByteArray()));
        }
        if (Input.GetKeyDown(KeyCode.H))    //收到游戏结束包
        {
            OffLineOrOnLine ooo = new OffLineOrOnLine();
            ooo.SeatNum = 3;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(12, ooo.ToByteArray()));
        }
        if (Input.GetKeyDown(KeyCode.P))    //收到烛反馈包
        {
            CandleCardFeedback ccfb = new CandleCardFeedback();
            ccfb.SeatNum = 1;
            ccfb.WaterNum = 0;
            ccfb.FireNum = 0;
            ccfb.LightNum = 3;
            ccfb.CandleNum = 0;
            ccfb.WoodNum = 0;
            ccfb.FogNum = 2;
            ConnectionManager.Instance.GameMSGRecive(EncapsulateMSG(9, ccfb.ToByteArray()));
        }
    }
}

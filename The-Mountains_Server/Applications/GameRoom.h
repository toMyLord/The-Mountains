//
// Created by mylord on 2020/2/15.
//

#ifndef THE_MOUNTAINS_SERVER_GAMEROOM_H
#define THE_MOUNTAINS_SERVER_GAMEROOM_H

#include "GameSession.h"

class GameRoom {
public:
    enum PlayerStatus {
        normal = 1,
        offline = 2
    };

    enum recvMsgFromClient {
        RoomInfoArrivedCode = 6,            // 房间信息抵达
        PlayerOperationCode = 7,            // 玩家操作包
        CandleCardFeedbackCode = 8,         // 烛牌反馈包
        GameFinishCode = 9,                 // 游戏结束包
        ReconnectionConfirmCode = 10,       // 重连确认包，接受到此包后准备发送操作队列
        ReconnectionProcessedCode = 11      // 重连队列处理完毕
    };

    enum sendMsgToClient {
        RoomInfoCode = 5,                   // 房间信息
        OtherPlayerInfoCode = 6,            // 其他玩家信息
        LocalPlayerInfoCode = 7,            // 本机玩家信息
        GameFinishCode_ = 8,                // 游戏结束
        CandleCardFeedbackCode_ = 9,        // 烛牌反馈包，包含下家的手牌信息
        PlayerOperationCode_ = 10,          // 广播玩家操作包，内容为 recvMsg 的7
        OfflineCode = 11,                   // 掉线
        ReconnectionCode = 12,              // 重连，玩家信息全都收到发送
        EndOfReconnectQueueCode = 13        // 重连队列结尾
    };

    GameRoom(int player_num = 3, int room_num = 0, std::shared_ptr<GameSession> player1 = nullptr,
             std::shared_ptr<GameSession> player2 = nullptr, std::shared_ptr<GameSession> player3 = nullptr,
             std::shared_ptr<GameSession> player4 = nullptr, std::shared_ptr<GameSession> player5 = nullptr);

    void setOfflinePlayer(const int offline_id, const std::shared_ptr<GameSession> & offline_player);

    void start();

    void sendMsgToAll(const std::string & buffer);

    bool isInThisRoom(const std::shared_ptr<AsyncSession> & compare_player);

    void PlayerOperationHandler(std::string buffer, const std::shared_ptr<AsyncSession> & game_player);

    void CandleCardFeedbackHandler(std::string buffer);

    int GameFinishHandler(const std::string & buffer, std::vector<int> & seat_num);

    bool isOffLine(const std::shared_ptr<AsyncSession> & offline_player);

    std::string getPlayerInfo(const std::shared_ptr<AsyncSession> & game_player);

    void SendPlayerInfo(const std::shared_ptr<AsyncSession> & game_player);

    void SendReconnectionInfo(const std::shared_ptr<AsyncSession> & game_player);

    int getRoomID() { return room_id; }

    int getPlayerNumber() { return player_number; }

    void ChangeStatusToNormal(const std::shared_ptr<AsyncSession> & game_player);

    std::string getRoomInfo();

private:
    struct RoomInformation {
        int roomID;
        time_t start_time;
        int playerNum;
        int candleNum;
        int woodNum;
        int fogNum;
        int witchNum;
    };

    struct PlayerInformation {
        int id;
        std::string username;
        int score;
        int seatNum;
        int cardNum;
        int witchNum;
        int status;

        int waterNum;
        int fireNum;
        int lightNum;
        int candleNum;
        int woodNum;
        int fogNum;
    };

    enum cardType {
        water = 1,
        fire = 2,
        light = 3,
        candle = 4,
        wood = 5,
        fog = 6
    };

    std::shared_ptr<GameSession> player[5];
    const int player_number, room_id;
    RoomInformation room;
    PlayerInformation player_info[5];
    bool isCardPoolReady;
    std::vector<std::string> operation_queue;
    int room_status;

    void WaitingForReady();

    void InitRoom();

    void InitPlayer();
};


#endif //THE_MOUNTAINS_SERVER_GAMEROOM_H

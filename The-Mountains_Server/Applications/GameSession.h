//
// Created by mylord on 2020/2/13.
//

#ifndef THE_MOUNTAINS_SERVER_GAMESESSION_H
#define THE_MOUNTAINS_SERVER_GAMESESSION_H

#include "../MsgProtocol/Game.pb.h"
#include "../MsgProtocol/GameContent.pb.h"
#include "../Services/AsyncServices/AsyncSession.h"
#include "../Services/DBServices/DatabaseService.h"
#include <list>

class GameSession;
class GameRoom;

struct MatchClientNode{
    std::shared_ptr<GameSession> client;
};

struct OffLinePlayer {
    int user_id;
    int room_id;
};

class GameSession : public AsyncSession{
public:
    enum clientStatus {
        BeforeMatch,
        Matching,
        InTheGame,
        Reconnection
    };

    enum recvMsgFromClient {
        UserInfoToGameServerCode = 2,       // 客户端发送用户id给game服务器
        EditUserInfoCode = 3,               // 修改用户信息
        MatchSwitchApplicationCode = 4,     // 匹配请求,匹配取消
        AcceptOrRefuseCode = 5              // 接受/拒绝进入房间
    };

    enum sendMsgToClient {
        EditUserInfoFeedbackCode = 2,       // 用户信息修改成功
        MatchConfirmCode = 3,               // 匹配申请/取消确认
        MatchSucceedCode = 4                // 匹配成功包
    };

    GameSession(tcp::socket socket, std::vector<std::shared_ptr<GameSession>> & client,
                std::list<std::shared_ptr<MatchClientNode>> & match_3,
                std::vector<std::shared_ptr<GameRoom>> & room, std::list<OffLinePlayer> & offline);

    int getStatus() { return status; }

    int setStatus(int s) { this->status = s; }

    int getUserId() { return user_id; }

    void clearGameRoom() { game_room = nullptr; }

private:
    std::vector<std::shared_ptr<GameSession>> & client_info;
    std::list<std::shared_ptr<MatchClientNode>> & match_queue_3;
    std::vector<std::shared_ptr<GameRoom>> & room_container;
    std::list<OffLinePlayer> & offline_list;

    std::shared_ptr<GameRoom> game_room;

    int status;         // 用户状态
    int user_id;        // 用户ID

    void center_handler(std::string buffer) override ;

    // 在此函数中确定了client状态为BeforeMatch
    void UserInfoToGameServerHandler(std::string buffer);

    void EditUserInfoHandler(std::string buffer);

    // 在此函数中确定了client状态为Matching
    void MatchSwitchApplicationHandler(std::string buffer);

    // 经过此函数后就不再通过do_read函数捕获消息，并且切换状态为InTheGame
    void AcceptOrRefuseHandler(std::string buffer);

    void RoomInfoArrivedHandler();

    void GameFinishHandler(std::string buffer);

    void ReconnectionProcessedHandler();

    void quit_handler() override;
};

#endif //THE_MOUNTAINS_SERVER_GAMESESSION_H

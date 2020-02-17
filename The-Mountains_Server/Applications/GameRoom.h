//
// Created by mylord on 2020/2/15.
//

#ifndef THE_MOUNTAINS_SERVER_GAMEROOM_H
#define THE_MOUNTAINS_SERVER_GAMEROOM_H

#include "GameSession.h"

class GameRoom {
public:
    GameRoom(int player_num = 3, int room_num = 0, std::shared_ptr<GameSession> player1 = nullptr,
             std::shared_ptr<GameSession> player2 = nullptr, std::shared_ptr<GameSession> player3 = nullptr,
             std::shared_ptr<GameSession> player4 = nullptr, std::shared_ptr<GameSession> player5 = nullptr);

    void start();

    bool isInThisRoom(const std::shared_ptr<AsyncSession> & compare_player);

    void MsgCenter(const std::shared_ptr<AsyncSession> & game_player, const std::string & buffer);

    bool OffLine(const std::shared_ptr<AsyncSession> & offline_player);


private:
    std::shared_ptr<GameSession> player[5];
    const int player_number, room_id;

    void sendMsgToAll(const std::string & buffer);

    enum recvMsgFromClient {
    };

    enum sendMsgToClient {
        RoomInfoCode = 5       // 匹配成功返回房间及玩家信息
    };
};


#endif //THE_MOUNTAINS_SERVER_GAMEROOM_H

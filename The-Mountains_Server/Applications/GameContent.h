//
// Created by mylord on 2020/2/15.
//

#ifndef THE_MOUNTAINS_SERVER_GAMECONTENT_H
#define THE_MOUNTAINS_SERVER_GAMECONTENT_H

#include "GameSession.h"

class GameContent;

struct GameRoom {
    GameRoom(std::shared_ptr<GameContent> ptr, int num): game_content(ptr), room_number(num){}
    std::shared_ptr<GameContent> game_content;
    int room_number;
};

class GameContent {
public:
    GameContent(int player_num = 3, std::shared_ptr<GameSession> player1 = nullptr,
            std::shared_ptr<GameSession> player2 = nullptr, std::shared_ptr<GameSession> player3 = nullptr,
            std::shared_ptr<GameSession> player4 = nullptr, std::shared_ptr<GameSession> player5 = nullptr);

    void start();

private:
    std::shared_ptr<GameSession> player[5];
    int player_number;

    void sendMsgToAll(const std::string & buffer);

    enum recvMsgFromClient {
    };

    enum sendMsgToClient {
        RoomInfoCode = 5       // 匹配成功返回房间及玩家信息
    };
};


#endif //THE_MOUNTAINS_SERVER_GAMECONTENT_H

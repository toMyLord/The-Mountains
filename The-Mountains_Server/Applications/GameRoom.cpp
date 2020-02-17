//
// Created by mylord on 2020/2/15.
//

#include "GameRoom.h"

GameRoom::GameRoom(int player_num, int room_num, std::shared_ptr<GameSession> player1,
                   std::shared_ptr<GameSession> player2, std::shared_ptr<GameSession> player3,
                   std::shared_ptr<GameSession> player4, std::shared_ptr<GameSession> player5) :
            player_number(player_num), room_id(room_num), player{player1, player2, player3, player4, player5} {
}

void GameRoom::sendMsgToAll(const std::string & buffer) {
    for(int i = 0; i < player_number; i++) {
        player[i]->SendMessages(buffer);
    }
}

void GameRoom::start() {
    while(true) {
        int ready_number = 0;
        // 需要完善此时掉线的情况！
        for(int i = 0; i < player_number; i++) {
            if (player[i]->getStatus() == GameSession::InTheGame)
                ready_number++;
        }
        if(ready_number == player_number)
            // 如果全部准备就绪就开始游戏
            break;
        else
            usleep(100000);
    }

    std::string sendMsg = std::to_string(sendMsgToClient::RoomInfoCode);
    sendMsg[0] = sendMsg[0] - '0';
    sendMsgToAll(sendMsg);

    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  Game Setup]:\t Game Setuped!";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}

bool GameRoom::isInThisRoom(const std::shared_ptr<AsyncSession> & compare_player) {
    for(int i = 0; i < player_number; i++) {
        if(player[i] == compare_player)
            return true;
    }
    return false;
}

void GameRoom::MsgCenter(const std::shared_ptr<AsyncSession> & game_player, const std::string & buffer) {

}

bool GameRoom::OffLine(const std::shared_ptr<AsyncSession> & offline_player) {
    for(int i = 0; i < player_number; i++) {
        if(player[i] == offline_player) {
            player[i] = nullptr;

            return true;
        }
    }
}

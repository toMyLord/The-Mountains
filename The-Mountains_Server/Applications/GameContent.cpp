//
// Created by mylord on 2020/2/15.
//

#include "GameContent.h"

GameContent::GameContent(int player_num , std::shared_ptr<GameSession> player1,
            std::shared_ptr<GameSession> player2, std::shared_ptr<GameSession> player3,
            std::shared_ptr<GameSession> player4, std::shared_ptr<GameSession> player5) :
            player_number(player_num), player{player1, player2, player3, player4, player5} {
}

void GameContent::sendMsgToAll(const std::string & buffer) {
    for(int i = 0; i < player_number; i++) {
        player[i]->SendMessages(buffer);
    }
}

void GameContent::start() {
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
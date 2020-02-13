//
// Created by mylord on 2020/2/13.
//

#include "GameSession.h"

GameSession::GameSession(tcp::socket socket, std::vector<std::shared_ptr<GameSession>> & client):
        AsyncSession(std::move(socket)), client_info(client) {
}

void GameSession::center_handler(std::string buffer) {
//        std::cout << "received:" << buffer << std::endl;
    do_read();
    switch(buffer[0]) {
        case heart_beats_code: std::cout << "recv heartbeats\n"; RecvHeartBeats(); break;
        default: {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "   Error]:\tGame Server can't parse client login request!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }
}

void GameSession::quit_handler() {
    auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());
    if(it == client_info.end()) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Quit Error]:\tclient information not found!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
    else
        client_info.erase(it);

    std::string log_buffer;
    log_buffer = "\tclient num is : " + std::to_string(client_info.size());
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}
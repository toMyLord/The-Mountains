//
// Created by mylord on 2020/2/13.
//

#ifndef THE_MOUNTAINS_SERVER_GAMESERVER_H
#define THE_MOUNTAINS_SERVER_GAMESERVER_H

#include "GameSession.h"
#include "../Services/AsyncServices/AsyncServer.h"

template <typename Session>
struct MatchClientNode{
    std::shared_ptr<Session> client;
};

template <typename Session>
class GameServer : public AsyncServer<Session>{
public:
   GameServer(boost::asio::io_context & io_context, short port, bool isbeats);

protected:
    std::queue<MatchClientNode<Session>> match_queue_3;

    void accept_handler(tcp::socket socket) override;

    void MatchDetect_t();
};

template<typename Session>
GameServer<Session>::GameServer(boost::asio::io_context &io_context, short port, bool is_beats)
        : AsyncServer<Session>(io_context, port, is_beats) {
}

template<typename Session>
void GameServer<Session>::accept_handler(tcp::socket socket) {
    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  GameServer Connection]:\tGameServer accepted connection from " +
                 socket.remote_endpoint().address().to_string() + ":" +
                 std::to_string(socket.remote_endpoint().port()) + ".";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);

    auto ptr = std::make_shared<Session>(std::move(socket), this->client_info, this->match_queue_3);
    this->client_info.push_back(ptr);

    std::thread match_detect_t(&GameServer::MatchDetect_t, this);
    match_detect_t.detach();

    ptr->StartSession(); // 开始接受客户端的消息
}

template<typename Session>
void GameServer<Session>::MatchDetect_t() {
    while(true) {
        if (match_queue_3.size() >= 3) {

        }
        else {
            sleep(1);
        }
    }
}

#endif //THE_MOUNTAINS_SERVER_GAMESERVER_H

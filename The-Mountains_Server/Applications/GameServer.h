//
// Created by mylord on 2020/2/13.
//

#ifndef THE_MOUNTAINS_SERVER_GAMESERVER_H
#define THE_MOUNTAINS_SERVER_GAMESERVER_H

#include "GameRoom.h"
#include "../Services/AsyncServices/AsyncServer.h"

template <typename Session>
class GameServer : public AsyncServer<Session>{
public:
   GameServer(boost::asio::io_context & io_context, short port, bool isbeats);

private:
    std::list<std::shared_ptr<MatchClientNode>> match_queue_3;
    std::vector<std::shared_ptr<GameRoom>> room_container;
    std::list<OffLinePlayer> offline_list;

    void accept_handler(tcp::socket socket) override;

    void MatchDetect_t();

    int room_id;

    bool is_detect;
};

template<typename Session>
GameServer<Session>::GameServer(boost::asio::io_context &io_context, short port, bool is_beats)
        : AsyncServer<Session>(io_context, port, is_beats), room_id(1), is_detect(false){
}

template<typename Session>
void GameServer<Session>::accept_handler(tcp::socket socket) {
    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  GameServer Connection]:\tGameServer accepted connection from " +
                 socket.remote_endpoint().address().to_string() + ":" +
                 std::to_string(socket.remote_endpoint().port()) + ".";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);

    auto ptr = std::make_shared<Session>(std::move(socket), this->client_info, this->match_queue_3,
            this->room_container, this->offline_list);
    this->client_info.push_back(ptr);

    if(!is_detect) {
        std::thread match_detect_t(&GameServer::MatchDetect_t, this);
        match_detect_t.detach();
        is_detect = true;
    }

    ptr->StartSession(); // 开始接受客户端的消息
}

template<typename Session>
void GameServer<Session>::MatchDetect_t() {
    while(true) {
        if (match_queue_3.size() >= 3) {
            std::shared_ptr<MatchClientNode> user[3];
            for (int i = 0; i < 3; i++) {
                user[i] = match_queue_3.front();
                match_queue_3.pop_front();
            }

            //建立游戏内容类的容器 房间号还没设置
            auto ptr = std::make_shared<GameRoom>(3, room_id,user[0]->client, user[1]->client, user[2]->client);
            room_container.push_back(ptr);

            if(room_id == INT_MAX) room_id = 1;

            for (int i = 0; i < 3; i++) {
                std::string sendMsg = std::to_string(GameSession::sendMsgToClient::MatchSucceedCode);
                sendMsg[0] = sendMsg[0] - '0';
                user[i]->client->SendMessages(sendMsg);
            }

            ptr->start();
        }
        else {
            sleep(1);
        }
    }
}

#endif //THE_MOUNTAINS_SERVER_GAMESERVER_H

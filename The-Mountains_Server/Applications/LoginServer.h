//
// Created by mylord on 2020/2/14.
//

#ifndef THE_MOUNTAINS_SERVER_LOGINSERVER_H
#define THE_MOUNTAINS_SERVER_LOGINSERVER_H

#endif //THE_MOUNTAINS_SERVER_LOGINSERVER_H

#include "LoginSession.h"
#include "../Services/AsyncServices/AsyncServer.h"

template <typename Session>
class LoginServer : public AsyncServer<Session>{
public:
    LoginServer(boost::asio::io_context & io_context, short port, bool isbeats);

private:
    void accept_handler(tcp::socket socket) override;

};

template<typename Session>
LoginServer<Session>::LoginServer(boost::asio::io_context &io_context, short port, bool is_beats)
        : AsyncServer<Session>(io_context, port, is_beats) {
}


template<typename Session>
void LoginServer<Session>::accept_handler(tcp::socket socket) {
    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  LoginServer Connection]:\tLoginServer accepted connection from " +
                 socket.remote_endpoint().address().to_string() + ":" +
                 std::to_string(socket.remote_endpoint().port()) + ".";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);

    auto ptr = std::make_shared<Session>(std::move(socket), this->client_info);
    this->client_info.push_back(ptr);
    ptr->StartSession(); // 开始接受客户端的消息
}

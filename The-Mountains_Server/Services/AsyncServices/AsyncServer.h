//
// Created by mylord on 2019/12/31.
//

#ifndef THE_MOUNTAINS_SERVER_ASYNCSERVER_H
#define THE_MOUNTAINS_SERVER_ASYNCSERVER_H

#include <thread>
#include <cstdlib>
#include <memory>
#include <utility>
#include <boost/asio.hpp>
#include <iostream>
#include "../LogServices/LogServices.h"

using boost::asio::ip::tcp;

template<typename Session>
class AsyncServer {
public:
    AsyncServer(boost::asio::io_context & io_context, short port, bool isbeats);

protected:
    tcp::acceptor acceptor_;
    std::vector<std::shared_ptr<Session>> client_info;
    bool is_heartbeats;
    bool is_threaded;

    void do_accept();

    virtual void accept_handler(tcp::socket socket);

    void HeartBeats_t();
};



template<typename Session>
AsyncServer<Session>::AsyncServer(boost::asio::io_context &io_context, short port, bool is_beats)
        : acceptor_(io_context, tcp::endpoint(tcp::v4(), port)), is_heartbeats(is_beats), is_threaded(false) {
    do_accept();
}

template<typename Session>
void AsyncServer<Session>::do_accept() {
    acceptor_.async_accept(
            [this](boost::system::error_code ec, tcp::socket socket) {
                if(ec) {
                    // 如果接受连接异常
                    std::string log_buffer;
                    log_buffer = '[' + TimeServices::getTime() + "  Accept Error]:\t" + ec.message() + '.';
                    LogServices::getInstance()->RecordingBoth(log_buffer, false);
                }
                else {
                    accept_handler(std::move(socket));

                    if(is_heartbeats && !is_threaded) {
                        std::thread heartbeats_t(&AsyncServer::HeartBeats_t, this);
                        heartbeats_t.detach();
                        is_threaded = true;

                        std::string log_buffer;
                        log_buffer = '[' + TimeServices::getTime() +
                                "  Heartbeats Started]:\tServer started to send heart beats packet!";
                        LogServices::getInstance()->RecordingBoth(log_buffer, true);
                    }
                }

                do_accept();
            }
    );
}

template<typename Session>
void AsyncServer<Session>::accept_handler(tcp::socket socket) {
    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  Connection Accepted]:\tAccept connection from " +
                 socket.remote_endpoint().address().to_string() + ":" +
                 std::to_string(socket.remote_endpoint().port()) + ".";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);

    auto ptr = std::make_shared<Session>(std::move(socket), this->client_info);
    client_info.push_back(ptr);
    ptr->StartSession(); // 开始接受客户端的消息
}

// 后面需要增加心跳包的格式！
template<typename Session>
void AsyncServer<Session>::HeartBeats_t() {
    while(client_info.size() > 0) {
        for(auto a : client_info){
            try {
                (*a).SendHeartBeats();
            }
            catch (std::exception& e) {
                std::string log_buffer;
                log_buffer = '[' + TimeServices::getTime() +
                        "  Exception Occurred]:\tException occurred during sending heartbeat!";
                LogServices::getInstance()->RecordingBoth(log_buffer, false);
            }
        }

        sleep(3);   // 每3s发送一次心跳包！
    }
    is_threaded = false;
    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() +
                 "  Heartbeats Ended]:\tNo client is online, stop send heart beats packet!";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}

#endif //THE_MOUNTAINS_SERVER_ASYNCSERVER_H

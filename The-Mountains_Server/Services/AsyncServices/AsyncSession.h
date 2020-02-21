//
// Created by mylord on 2019/12/31.
//

#ifndef THE_MOUNTAINS_SERVER_ASYNCSESSION_H
#define THE_MOUNTAINS_SERVER_ASYNCSESSION_H

#include <cstdlib>
#include <memory>
#include <utility>
#include <boost/asio.hpp>
#include <iostream>
#include <mutex>
#include "../LogServices/LogServices.h"

using boost::asio::ip::tcp;

class AsyncSession : public std::enable_shared_from_this<AsyncSession> {
public:
    explicit AsyncSession(tcp::socket socket);

    void StartSession();

    void SendMessages(const std::string & buffer);

    void SendMessagesWithoutLength(const std::string & buffer);

    void SendHeartBeats();

    void RecvHeartBeats();

protected:
//    typedef void (AsyncSession::*handler)(std::string buffer);

    enum {
        max_length = 1024,
        heart_beats_code = 1
    };
    tcp::socket socket_;
    char buffer_[max_length];
    time_t recv_heartbeats_t;
    std::string ip_port;

    void do_read();

    void do_write();

    bool error_code_handler(const boost::system::error_code & ec);

    virtual void center_handler(std::string buffer) = 0;

    virtual void quit_handler() = 0;
};


#endif //THE_MOUNTAINS_SERVER_ASYNCSESSION_H

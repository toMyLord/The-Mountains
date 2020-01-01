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

using boost::asio::ip::tcp;

class AsyncSession : public std::enable_shared_from_this<AsyncSession> {
public:
    AsyncSession(tcp::socket socket);

    void StartSession();

protected:
    void do_read();

    void do_write(std::size_t length);

    virtual void read_handler(int length);

    virtual void write_handler(int length);

    void socket_close(const boost::system::error_code & ec);

    virtual void quit_handler();


    enum {
        max_length = 1024
    };
    tcp::socket socket_;
    char buffer_[max_length];
};


#endif //THE_MOUNTAINS_SERVER_ASYNCSESSION_H

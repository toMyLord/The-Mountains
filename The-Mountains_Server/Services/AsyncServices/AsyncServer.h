//
// Created by mylord on 2019/12/31.
//

#ifndef THE_MOUNTAINS_SERVER_ASYNCSERVER_H
#define THE_MOUNTAINS_SERVER_ASYNCSERVER_H

#include "AsyncSession.h"

class AsyncServer {
public:
    AsyncServer(boost::asio::io_context & io_context, short port);

private:
    void do_accept();

    virtual void accpet_handler(tcp::socket socket);

    tcp::acceptor acceptor_;
};


#endif //THE_MOUNTAINS_SERVER_ASYNCSERVER_H

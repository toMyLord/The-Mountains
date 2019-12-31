//
// Created by mylord on 2019/12/31.
//

#include "AsyncServer.h"

AsyncServer::AsyncServer(boost::asio::io_context &io_context, short port)
        : acceptor_(io_context, tcp::endpoint(tcp::v4(), port)) {
    do_accept();
}

void AsyncServer::do_accept() {
    acceptor_.async_accept(
            [this](boost::system::error_code ec, tcp::socket socket) {
                if(ec) {
                    // 如果接受连接异常
                    std::cerr << "[Accept Error] : " << ec.message() << std::endl;
                }
                else {
                    accpet_handler(std::move(socket));
                }

                do_accept();
            }
            );
}

void AsyncServer::accpet_handler(tcp::socket socket) {
    auto ptr = std::make_shared<AsyncSession>(std::move(socket));
    ptr->StartSession(); // 开始接受客户端的消息
}
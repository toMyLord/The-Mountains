//
// Created by mylord on 2019/12/31.
//

#include "AsyncSession.h"

AsyncSession::AsyncSession(tcp::socket socket): socket_(std::move(socket)) {

}

void AsyncSession::StartSession() {
    do_read();
}

void AsyncSession::do_read() {
    auto self = shared_from_this();
    socket_.async_read_some(boost::asio::buffer(buffer_, max_length),
            [this, self](const boost::system::error_code & ec, std::size_t length) {
                // 调用read_handler
                if(ec) {
                    // 没有判断end of file 即断开连接！
                    if(ec.value() == boost::asio::error::eof){
                        std::cout << "[client exit!]: ";
                        socket_close(ec);
                        return;
                    }
                    std::cout << "[read error]: ";
                    socket_close(ec);
                    return;
                }
                read_handler(length);
            }
            );
}

void AsyncSession::do_write(std::size_t length) {
    auto self = shared_from_this();
    boost::asio::async_write(socket_, boost::asio::buffer(buffer_, length),
            [this, self](const boost::system::error_code & ec, std::size_t length) {
                // 调用write_handler
                write_handler(length);
            }
            );
}

void AsyncSession::read_handler(int length){
    buffer_[length] = '\0';
    std::cout << "received: " << buffer_ << std::endl;
    do_write(length);
}

void AsyncSession::write_handler(int length){
    std::cout << "message send!\n";
    do_read();
}

void AsyncSession::socket_close(const boost::system::error_code & ec) {
    socket_.close();
    std::cout << ec.message() << std::endl;
    quit_handler();
}

void AsyncSession::quit_handler() {
    std::cout << "socket closed!\n";
}
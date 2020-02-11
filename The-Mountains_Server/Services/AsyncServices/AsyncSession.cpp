//
// Created by mylord on 2019/12/31.
//

#include "AsyncSession.h"

AsyncSession::AsyncSession(tcp::socket socket): socket_(std::move(socket)) {
}

void AsyncSession::SendMessages(const std::string & buffer) {
    boost::asio::write(socket_, boost::asio::buffer(buffer.c_str(), buffer.size()));
}

void AsyncSession::StartSession() {
   do_read();
}

void AsyncSession::do_read() {
    auto self = shared_from_this();
    socket_.async_read_some(boost::asio::buffer(buffer_, max_length),
                            [this, self](const boost::system::error_code & ec, std::size_t length) {
                                // 捕获`self`使shared_ptr<session>的引用计数增加1，在该例中避免了async_read()退出时其引用计数变为0
                                buffer_[length] = '\0';
                                std::string buffer(buffer_);

                                if(!error_code_handler(ec)) return;

                                center_handler(buffer);
                            });
}

void AsyncSession::do_write() {
    auto self = shared_from_this();
    socket_.async_read_some(boost::asio::buffer(buffer_, max_length),
                            [this, self](const boost::system::error_code & ec, std::size_t length) {
                                // 捕获`self`使shared_ptr<session>的引用计数增加1，在该例中避免了async_read()退出时其引用计数变为0
                                buffer_[length] = '\0';
                                std::string buffer(buffer_);

                                if(!error_code_handler(ec)) return;

//                                (this->*write_handler)(buffer);
                            });
}

bool AsyncSession::error_code_handler(const boost::system::error_code &ec) {
    if(ec) {
        // 没有判断end of file 即断开连接！
        if(ec.value() == boost::asio::error::eof){
            socket_.close();

            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Client Exit]:\t" + ec.message() + '.';
            LogServices::getInstance()->RecordingBoth(log_buffer, true);

            quit_handler();
            return false;
        }
        socket_.close();

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Read Error]:\t" + ec.message() + ", connection closed.";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);

        quit_handler();
        return false;
    }
    return true;
}


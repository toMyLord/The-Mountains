#include <iostream>

#include "Services/AsyncServices/AsyncServer.h"     // for test！

#include <vector>
class LoginSession : public AsyncSession {
public:
    LoginSession(tcp::socket socket): AsyncSession(std::move(socket)){}

private:
    void read_handler(int length) override {
        buffer_[length] = '\0';
        std::cout << "Login received: " << buffer_ << std::endl;
        strcpy(buffer_, "@@@!");
        do_write(strlen(buffer_));
    }
    void write_handler(int length) override {
        std::cout << "Login message send!\n";
        do_read();
    }

};
class LoginServer : public AsyncServer{
public:
    LoginServer(boost::asio::io_context & io_context, short port): AsyncServer(io_context, port){}
private:
    std::vector<std::shared_ptr<LoginSession>> client_info;

    void accpet_handler(tcp::socket socket) override {
        auto ptr = std::make_shared<LoginSession>(std::move(socket));

        client_info.push_back(ptr);
        std::cout << "size is : " << client_info.size() << std::endl;

        ptr->StartSession(); // 开始接受客户端的消息
    }
};


int main(int argc, char * argv[]) {
    try {
        if(argc != 2) {
            std::cerr << "Usage: port error!\n";
            return 1;
        }

        boost::asio::io_context io_context_;

        LoginServer s(io_context_, std::atoi(argv[1]));

        io_context_.run();
    }

    catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}

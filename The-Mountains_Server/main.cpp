#include <iostream>
#include "Login.pb.h"
#include "Services/AsyncServices/AsyncServer.h"
#include "Services/AsyncServices/AsyncSession.h"

class LoginSession : public AsyncSession {
public:
    LoginSession(tcp::socket socket, std::vector<std::shared_ptr<LoginSession>> & client):
            AsyncSession(std::move(socket)), client_info(client){
    }

private:
    std::vector<std::shared_ptr<LoginSession>> & client_info;

    void center_handler(std::string buffer) override {
//        std::cout << "received:" << buffer << std::endl;
        switch(buffer[0]){
            case '1': User(buffer.substr(1)); break;
            case '2': Tourist(buffer.substr(1)); break;
        }
        do_read();
    }

    void User(std::string buffer){
        UserLogin user;
        user.ParseFromArray(buffer.c_str(), buffer.size());
        std::cout << "User account : " << user.account() << std::endl;
        std::cout << "User password : " << user.password() << std::endl;
    }

    void Tourist(std::string buffer){
        TouristLogin tourist;
        tourist.ParseFromArray(buffer.c_str(), buffer.size());
        std::cout << "Tourist account : " << tourist.account() << std::endl;
    }


    void quit_handler() override{
        auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());
        if(it == client_info.end()) {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Quit Error]:\tclient information not found!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
        else
            client_info.erase(it);

        std::string log_buffer;
        log_buffer = "\tclient num is : " + std::to_string(client_info.size());
        LogServices::getInstance()->RecordingBoth(log_buffer, true);
    }
};



int main(int argc, char * argv[]) {
    try {
        if(argc != 2) {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Usage Error]:\tNo port entered!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
            return 1;
        }

        boost::asio::io_context io_context_;

        AsyncServer<LoginSession> s(io_context_, std::atoi(argv[1]), true);

        io_context_.run();
    }

    catch (std::exception& e) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Exception Occured]:\t" + e.what() + '.';
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }

    return 0;
}

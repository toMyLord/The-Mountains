#include <iostream>
#include "Login.pb.h"
#include "Services/AsyncServices/AsyncServer.h"     // for test！
#include "Services/AsyncServices/AsyncSession.h"

class LoginSession : public AsyncSession {
public:
    LoginSession(tcp::socket socket, std::vector<std::shared_ptr<LoginSession>> & client):
            AsyncSession(std::move(socket)), client_info(client){
    }

private:
    std::vector<std::shared_ptr<LoginSession>> & client_info;

    void center_handler(std::string buffer) override {
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
        if(it == client_info.end())
            std::cout << "[Quit Error] : client information not found!" << std::endl;
        else
            client_info.erase(it);
        std::cout << "client num is : " << client_info.size() << std::endl;
    }
};



int main(int argc, char * argv[]) {
    try {
        if(argc != 2) {
            std::cerr << "Usage: port error!\n";
            return 1;
        }

        std::string buffer;
        UserLogin u;
        u.set_account("www");
        u.set_password("yyy");
        u.SerializeToString(&buffer);
        std::cout << buffer << std::endl;
        std::cout << "end" << std::endl;

        boost::asio::io_context io_context_;

        AsyncServer<LoginSession> s(io_context_, std::atoi(argv[1]), true);

        io_context_.run();
    }

    catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}

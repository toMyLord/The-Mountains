#include <iostream>
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

    }

    void quit_handler() override{
        auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());
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

        boost::asio::io_context io_context_;

        AsyncServer<LoginSession> s(io_context_, std::atoi(argv[1]), true);

        io_context_.run();
    }

    catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}

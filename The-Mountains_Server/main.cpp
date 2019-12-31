#include <iostream>

#include "Services/AsyncServices/AsyncServer.h"     // for test！

int main(int argc, char * argv[]) {
    try {
        if(argc != 2) {
            std::cerr << "Usage: port error!\n";
            return 1;
        }

        boost::asio::io_context io_context_;

        AsyncServer s(io_context_, std::atoi(argv[1]));

        io_context_.run();
    }

    catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}

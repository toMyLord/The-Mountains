#include "Applications/LoginSession.h"
#include "Services/AsyncServices/AsyncServer.h"

int main(int argc, char * argv[]) {
    try {
        if(argc != 2) {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Usage Error]:\tNo port entered!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
            return 1;
        }

        boost::asio::io_context io_context_;

        AsyncServer<LoginSession> login_server(io_context_, std::atoi(argv[1]), false);

        // 新开7个线程，加上原线程总共8个线程用来处理回调函数
        for(int i = 0; i < 7; i++) {
            std::thread th([&io_context_](){ io_context_.run(); });
            th.detach();
        }

        io_context_.run();
    }

    catch (std::exception& e) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Exception Occured]:\t" + e.what() + '.';
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }

    return 0;
}

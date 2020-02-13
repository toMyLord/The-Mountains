#include "Applications/LoginSession.h"
#include "Applications/GameServer.h"
#include "Services/AsyncServices/AsyncServer.h"

int main(int argc, char * argv[]) {
    try {
        if(argc != 3) {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Usage Error]:\tPlease input correct port infomation!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
            return 1;
        }

        boost::asio::io_context login_io_context_, game_io_context_;


        // 登录服务器
        AsyncServer<LoginSession> login_server(login_io_context_, std::atoi(argv[1]), false);

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Server Online]:\tLogin Server is online!";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);

        // 新开7个线程，加上原线程总共8个线程用来处理回调函数
        for(int i = 0; i < 7; i++) {
            std::thread th([&login_io_context_](){ login_io_context_.run(); });
            th.detach();
        }


        // 游戏服务器
        AsyncServer<GameSession> game_server(game_io_context_, std::atoi(argv[2]), true);

        log_buffer = '[' + TimeServices::getTime() + "  Server Online]:\tGame Server is online!";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);

        // 新开8个线程用来处理回调函数
        for(int i = 0; i < 8; i++) {
            std::thread th([&game_io_context_](){ game_io_context_.run(); });
            th.detach();
        }

        login_io_context_.run();
        game_io_context_.run();
    }

    catch (std::exception& e) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Exception Occured]:\t" + e.what() + '.';
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }

    return 0;
}

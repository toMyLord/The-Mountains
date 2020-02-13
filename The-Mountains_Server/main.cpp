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

        // 确定CPU线程数
        const int CORES_NUMBER = std::thread::hardware_concurrency();
        int login_thread_number, game_thread_number;
        // 根据线程数确定每个服务器分配多少线程
        if (CORES_NUMBER <= 4) {
            login_thread_number = 2;
            game_thread_number = 2;
        } else {
            login_thread_number = 2;
            game_thread_number = CORES_NUMBER - 2;
        }


        boost::asio::io_context login_io_context_, game_io_context_;

        // 登录服务器
        AsyncServer<LoginSession> login_server(login_io_context_, std::atoi(argv[1]), false);

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Server Online]:\tLogin Server is online!";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);

        // 新开login_thread_number - 1个线程，加上原线程总共login_thread_number个线程用来处理登录业务
        for(int i = 0; i < login_thread_number - 1; i++) {
            std::thread th([&login_io_context_](){ login_io_context_.run(); });
            th.detach();
        }


        // 游戏服务器
        AsyncServer<GameSession> game_server(game_io_context_, std::atoi(argv[2]), true);

        log_buffer = '[' + TimeServices::getTime() + "  Server Online]:\tGame Server is online!";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);

        // 新开game_thread_number个线程用来处理游戏业务
        for(int i = 0; i < game_thread_number; i++) {
            std::thread th([&game_io_context_](){ game_io_context_.run(); });
            th.detach();
        }

        log_buffer = '[' + TimeServices::getTime() +
                "  Core Info]:\tThe maximum number of threads supported by the CPU is " + std::to_string(CORES_NUMBER) +
                ", Login server use " +  std::to_string(login_thread_number) + " thread, and Game server use " +
                std::to_string(game_thread_number) + " thread.";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);


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

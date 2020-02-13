//
// Created by mylord on 2020/2/13.
//

#ifndef THE_MOUNTAINS_SERVER_GAMESESSION_H
#define THE_MOUNTAINS_SERVER_GAMESESSION_H

#include "../Services/AsyncServices/AsyncSession.h"

class GameSession : public AsyncSession{
public:
    GameSession(tcp::socket socket, std::vector<std::shared_ptr<GameSession>> & client);

private:
    std::vector<std::shared_ptr<GameSession>> & client_info;

    void center_handler(std::string buffer) override ;

    void quit_handler() override;
};


#endif //THE_MOUNTAINS_SERVER_GAMESESSION_H

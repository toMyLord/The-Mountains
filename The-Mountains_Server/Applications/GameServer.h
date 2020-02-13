//
// Created by mylord on 2020/2/13.
//

#ifndef THE_MOUNTAINS_SERVER_GAMESERVER_H
#define THE_MOUNTAINS_SERVER_GAMESERVER_H

#include "GameSession.h"
#include "../Services/AsyncServices/AsyncServer.h"

template <typename Session>
class GameServer : public AsyncServer<Session>{

};


#endif //THE_MOUNTAINS_SERVER_GAMESERVER_H

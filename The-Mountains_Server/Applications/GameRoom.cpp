//
// Created by mylord on 2020/2/15.
//

#include "GameRoom.h"

GameRoom::GameRoom(int player_num, int room_num, std::shared_ptr<GameSession> player1,
                   std::shared_ptr<GameSession> player2, std::shared_ptr<GameSession> player3,
                   std::shared_ptr<GameSession> player4, std::shared_ptr<GameSession> player5):
            player_number(player_num), room_id(room_num), player{player1, player2, player3, player4, player5} {
    isCardPoolReady = false;
}

void GameRoom::sendMsgToAll(const std::string & buffer) {
    for(int i = 0; i < player_number; i++) {
        player[i]->SendMessages(buffer);
    }
}

void GameRoom::WaitingForReady() {
    while(true) {
        int ready_number = 0;
        // 需要完善此时掉线的情况！
        for(int i = 0; i < player_number; i++) {
            if (player[i]->getStatus() == GameSession::InTheGame)
                ready_number++;
        }
        if(ready_number == player_number)
            // 如果全部准备就绪就开始游戏
            break;
        else
            usleep(100000);
    }
}

void GameRoom::InitRoom() {
    room.roomID = room_id;
    room.playerNum = player_number;
    room.candleNum = 3;
    room.woodNum = 3;
    room.fogNum = 3;
    room.witchNum = 3;
    time(&room.start_time);
}

void GameRoom::InitPlayer() {
    //  Inside-Out 洗牌算法
    int card_pool[45], k = 0;
    for (int i = 0; i < 12; i++) card_pool[k++] = cardType::water;
    for (int i = 0; i < 12; i++) card_pool[k++] = cardType::fire;
    for (int i = 0; i < 12; i++) card_pool[k++] = cardType::light;
    for (int i = 0; i < 3; i++) card_pool[k++] = cardType::candle;
    for (int i = 0; i < 3; i++) card_pool[k++] = cardType::wood;
    for (int i = 0; i < 3; i++) card_pool[k++] = cardType::fog;
    for (int i = 0; i < 45 ; ++i)
    {
        srand((unsigned)time(NULL));
        k=rand()%(i+1);
        int temp = card_pool[i];
        card_pool[i]=card_pool[k];
        card_pool[k]=temp;
    }

    // 洗牌完成
    k = 0;
    for(int i = 0; i < player_number; i++) {
        // 设置基础信息
        player_info[i].id = player[i]->getUserId();

        std::string query_buffer;
        query_buffer = "SELECT username, score FROM the_mountains.User_Info WHERE id = \"" +
                std::to_string(player_info[i].id) + "\"";
        DatabaseService db;
        mysqlpp::Connection * conn = db.getConnection();
        mysqlpp::Query query = conn->query(query_buffer.c_str());

        mysqlpp::StoreQueryResult res = query.store();
        mysqlpp::Row row = res[0];

        player_info[i].username = std::string(row[0]);
        player_info[i].score = row[1];
        player_info[i].seatNum = i + 1;
        player_info[i].cardNum = (i == 0 ? 9 : 8);
        player_info[i].witchNum = 0;
        player_info[i].status = PlayerStatus::normal;

        player_info[i].waterNum = 0;
        player_info[i].fireNum = 0;
        player_info[i].lightNum = 0;
        player_info[i].candleNum = 0;
        player_info[i].woodNum = 0;
        player_info[i].fogNum = 0;

        for (int j = 0; j < player_info[i].cardNum; j++) {
            // 设置卡牌信息
            switch(card_pool[k++]) {
                case cardType::water: player_info[i].waterNum++; break;
                case cardType::fire: player_info[i].fireNum++; break;
                case cardType::light: player_info[i].lightNum++; break;
                case cardType::candle: player_info[i].candleNum++; break;
                case cardType::wood: player_info[i].woodNum++; break;
                case cardType::fog: player_info[i].fogNum++; break;
            }
        }
    }

    isCardPoolReady = true;
}

void GameRoom::start() {
    WaitingForReady();

    InitRoom();

    RoomInfo room_if;
    room_if.set_roomid(room.roomID);
    room_if.set_time(0);
    room_if.set_playernum(room.playerNum);
    room_if.set_candlenum(room.candleNum);
    room_if.set_woodnum(room.woodNum);
    room_if.set_fognum(room.fogNum);
    room_if.set_witchnum(room.witchNum);

    std::string temp;
    room_if.SerializeToString(&temp);

    std::string sendMsg = std::to_string(sendMsgToClient::RoomInfoCode) + temp;
    sendMsg[0] = sendMsg[0] - '0';
    sendMsgToAll(sendMsg);

    InitPlayer();
}

void GameRoom::SendPlayerInfo(const std::shared_ptr<AsyncSession> & game_player) {
    while(isCardPoolReady == false) {
        // 等待服务器洗牌
        usleep(100000);
    }
    // 服务器洗牌结束
    for(int i = 0; i < player_number; i++) {
        if(player[i] == game_player) {
            // 本机玩家，发送本机玩家信息
            LocalPlayerInfo local_if;
            local_if.set_cardnum(player_info[i].cardNum);
            local_if.set_waternum(player_info[i].waterNum);
            local_if.set_firenum(player_info[i].fireNum);
            local_if.set_lightnum(player_info[i].lightNum);
            local_if.set_candlenum(player_info[i].candleNum);
            local_if.set_woodnum(player_info[i].woodNum);
            local_if.set_fognum(player_info[i].fogNum);
            local_if.set_witchnum(player_info[i].witchNum);
            local_if.set_currplayerstatus(LocalPlayerInfo::Normal);
            local_if.set_seatnum(player_info[i].seatNum);

            std::string temp;
            local_if.SerializeToString(&temp);

            std::string sendMsg = std::to_string(sendMsgToClient::LocalPlayerInfoCode) + temp;
            sendMsg[0] = sendMsg[0] - '0';
            game_player->SendMessages(sendMsg);
        }
        else {
            // 房间玩家，发送房间玩家信息
            OtherPlayerInfo other_if;
            other_if.set_name(player_info[i].username);
            other_if.set_score(player_info[i].score);
            other_if.set_seatnum(player_info[i].seatNum);
            other_if.set_cardnum(player_info[i].cardNum);
            other_if.set_witchnum(player_info[i].witchNum);
            other_if.set_currplayerstatus(OtherPlayerInfo::Normal);

            std::string temp;
            other_if.SerializeToString(&temp);

            std::string sendMsg = std::to_string(sendMsgToClient::OtherPlayerInfoCode) + temp;
            sendMsg[0] = sendMsg[0] - '0';
            game_player->SendMessages(sendMsg);
        }
    }

    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  Room Setup]:\t Room<" + std::to_string(room_id) + "> Setuped!";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}

void GameRoom::PlayerOperationHandler(std::string buffer) {
    operation_queue.push(buffer.substr(1));
    buffer[0] = sendMsgToClient::PlayerOperationCode_ - '0';
    sendMsgToAll(buffer);
}

void GameRoom::CandleCardFeedbackHandler(std::string buffer) {
    buffer[0] = sendMsgToClient::CandleCardFeedbackCode_ - '0';
    sendMsgToAll(buffer);
}

int GameRoom::GameFinishHandler(const std::string & buffer) {
    GameFinish game_fh;
    game_fh.ParseFromArray(buffer.c_str(), buffer.size());

    std::string query_buffer;
    query_buffer = "UPDATE the_mountains.User_Info SET score = \'" +
            std::to_string(player_info[game_fh.seatnum() - 1].score + game_fh.gamescore()) + "\' WHERE id = \'" +
            std::to_string(player_info[game_fh.seatnum() - 1].id) + "\'";

    DatabaseService db;
    mysqlpp::Connection * conn = db.getConnection();
    mysqlpp::Query query = conn->query(query_buffer.c_str());
    query.exec();

    for(int i = 0; i < player_number; i++) {
        player[i]->clearGameRoom();
        player[i] = nullptr;
    }
}

bool GameRoom::isInThisRoom(const std::shared_ptr<AsyncSession> & compare_player) {
    for(int i = 0; i < player_number; i++) {
        if(player[i] == compare_player)
            return true;
    }
    return false;
}

bool GameRoom::isOffLine(const std::shared_ptr<AsyncSession> & offline_player) {
    for(int i = 0; i < player_number; i++) {
        if(player[i] == offline_player) {
            player[i] = nullptr;

            return true;
        }
    }
}

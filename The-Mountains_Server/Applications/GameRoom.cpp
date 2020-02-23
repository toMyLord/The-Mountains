//
// Created by mylord on 2020/2/15.
//

#include "GameRoom.h"

GameRoom::GameRoom(int player_num, int room_num, std::shared_ptr<GameSession> player1,
                   std::shared_ptr<GameSession> player2, std::shared_ptr<GameSession> player3,
                   std::shared_ptr<GameSession> player4, std::shared_ptr<GameSession> player5):
            player_number(player_num), room_id(room_num), player{player1, player2, player3, player4, player5} {
    isCardPoolReady = false;
    room_status = PlayerStatus::normal;
}

void GameRoom::sendMsgToAll(const std::string & buffer) {
    for(int i = 0; i < player_number; i++) {
        if(player_info[i].status == PlayerStatus::normal)
            player[i]->SendMessages(buffer);
    }
}


void GameRoom::setOfflinePlayer(const int offline_id, const std::shared_ptr<GameSession> & offline_player) {
    for(int i = 0; i < player_number; i++) {
        if(player_info[i].id == offline_id)
            player[i] = offline_player;
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
    room.witchNum = 6;
    time(&room.start_time);
}

std::string GameRoom::getRoomInfo() {
    RoomInfo room_if;
    time_t now;
    time(&now);
    room_if.set_roomid(room.roomID);
    room_if.set_time(int(difftime(now, room.start_time)));
    room_if.set_playernum(room.playerNum);
    room_if.set_candlenum(room.candleNum);
    room_if.set_woodnum(room.woodNum);
    room_if.set_fognum(room.fogNum);
    room_if.set_witchnum(room.witchNum);

    std::string temp;
    room_if.SerializeToString(&temp);

    std::string sendMsg = std::to_string(sendMsgToClient::RoomInfoCode) + temp;
    sendMsg[0] = sendMsg[0] - '0';

    return sendMsg;
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
    // 洗牌两次
    for (int j = 0; j < 2; j++)
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

    InitPlayer();

    sendMsgToAll(getRoomInfo());
}

std::string GameRoom::getPlayerInfo(const std::shared_ptr<AsyncSession> & game_player) {
    std::string player_if;
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

            temp = std::to_string(sendMsgToClient::LocalPlayerInfoCode) + temp;
            temp[0] = temp[0] - '0';
            temp = char(temp.size()) + temp;

            player_if += temp;
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

            temp = std::to_string(sendMsgToClient::OtherPlayerInfoCode) + temp;
            temp[0] = temp[0] - '0';
            temp = char(temp.size()) + temp;

            player_if += temp;
        }
    }

    return player_if;
}

void GameRoom::SendPlayerInfo(const std::shared_ptr<AsyncSession> & game_player) {
    while(isCardPoolReady == false) {
        // 等待服务器洗牌
        usleep(100000);
    }
    // 服务器洗牌结束
    game_player->SendMessagesWithoutLength(getPlayerInfo(game_player));

    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  Room Setup]:\t Room<" + std::to_string(room_id) + "> Setuped!";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}

void GameRoom::SendReconnectionInfo(const std::shared_ptr<AsyncSession> & game_player) {
    std::string sendMsg = getPlayerInfo(game_player);

    int seat_num;
    for(int i = 0; i < player_number; i++) {
        if(player[i] == game_player)
            seat_num = i + 1;
    }

    // 添加协议12
    OffLineOrOnLine online;
    online.set_seatnum(seat_num);
    std::string temp;
    online.SerializeToString(&temp);
    temp = char(sendMsgToClient::ReconnectionCode) + temp;
    // 向其他玩家发送上线信息。
    for(int i = 0; i < player_number; i++) {
        if(player[i] != game_player)
            player[i]->SendMessages(temp);
    }
    temp = char(temp.size()) + temp;

    sendMsg += temp;

    // 添加操作队列信息
    for(auto queue : operation_queue) {
        temp = char(sendMsgToClient::PlayerOperationCode_) + queue;
        temp = char(temp.size()) + temp;
        sendMsg += temp;
    }

    // 添加协议13
    temp = char(sendMsgToClient::EndOfReconnectQueueCode);
    temp = char(temp.size()) + temp;
    sendMsg += temp;

    game_player->SendMessagesWithoutLength(sendMsg);
}


void GameRoom::PlayerOperationHandler(std::string buffer, const std::shared_ptr<AsyncSession> & game_player) {
    operation_queue.push_back(buffer.substr(1));
    buffer[0] = char(sendMsgToClient::PlayerOperationCode_);
    sendMsgToAll(buffer);

    if(room_status == PlayerStatus::offline) {
        PlayerOperation op;
        op.ParseFromArray(buffer.substr(1).c_str(), buffer.size() - 1);
        int next_player = (op.seatnum()) % player_number;

        if(player_info[next_player].status == PlayerStatus::offline) {
            PlayerOperation po;
            po.set_seatnum(next_player + 1);
            po.set_operation(PlayerOperation::Skip);
            po.set_card(PlayerOperation::Water);

            std::string sendMsg;
            po.SerializeToString(&sendMsg);
            operation_queue.push_back(sendMsg);
            sendMsg = char(sendMsgToClient::PlayerOperationCode_) + sendMsg;

            sendMsgToAll(sendMsg);
        }
    }
}

void GameRoom::CandleCardFeedbackHandler(std::string buffer) {
    buffer[0] = char(sendMsgToClient::CandleCardFeedbackCode_);
    sendMsgToAll(buffer);
}

int GameRoom::GameFinishHandler(const std::string & buffer, std::vector<int> & offline_id) {
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

    if(room_status == PlayerStatus::offline) {
        // 如果有掉线玩家
        for(int i = 0; i < player_number; i++) {
            if(player_info[i].status == PlayerStatus::offline) {
                // Session注销掉所有在等待重连队里的信息！
                offline_id.push_back(player_info[i].id);
            }
        }
    }

    for(int i = 0; i < player_number; i++) {
        if(player[i] != nullptr) {
            player[i]->clearGameRoom();
            player[i]->setStatus(GameSession::clientStatus::BeforeMatch);
            player[i] = nullptr;
        }
    }
    return room_id;
}

void GameRoom::ChangeStatusToNormal(const std::shared_ptr<AsyncSession> & game_player) {
    int num = 0;
    for(int i = 0; i < player_number; i++) {
        if(player[i] == game_player)
            player_info[i].status = PlayerStatus::normal;
        if(player_info[i].status == PlayerStatus::normal)
            num++;
    }
    if(num == player_number)
        room_status = PlayerStatus::normal;
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
            player_info[i].status = PlayerStatus::offline;
            room_status = PlayerStatus::offline;

            OffLineOrOnLine offline;
            offline.set_seatnum(i + 1);
            std::string temp;
            offline.SerializeToString(&temp);
            char code = sendMsgToClient::OfflineCode;
            std::string sendMsg = code + temp;
            sendMsgToAll(sendMsg);

            auto last_operation = operation_queue.end();
            last_operation--;

            PlayerOperation last_op;
            last_op.ParseFromArray(last_operation->c_str(), last_operation->size());

            int last_seat = last_op.seatnum();
            if(last_seat % player_number == i) {
                PlayerOperation skip;
                skip.set_seatnum(i + 1);
                skip.set_operation(PlayerOperation::Skip);
                skip.set_card(PlayerOperation::Water);

                std::string temp;
                skip.SerializeToString(&temp);
                operation_queue.push_back(temp);
                temp = char(sendMsgToClient::PlayerOperationCode_) + temp;

                sendMsgToAll(temp);
            }
            return true;
        }
    }
}

//
// Created by mylord on 2020/2/13.
//

#include "GameSession.h"
#include "GameRoom.h"

GameSession::GameSession(tcp::socket socket, std::vector<std::shared_ptr<GameSession>> & client,
        std::list<std::shared_ptr<MatchClientNode>> & match_3, std::vector<std::shared_ptr<GameRoom>> & room,
        std::list<OffLinePlayer> & offline):
        AsyncSession(std::move(socket)), client_info(client), match_queue_3(match_3), room_container(room),
        offline_list(offline){
}

void GameSession::center_handler(std::string buffer) {
    do_read();

    switch(buffer[0]) {
        case heart_beats_code: RecvHeartBeats(); break;
        case recvMsgFromClient::UserInfoToGameServerCode:
            UserInfoToGameServerHandler(buffer.substr(1)); break;
        case recvMsgFromClient::EditUserInfoCode:
            EditUserInfoHandler(buffer.substr(1)); break;
        case recvMsgFromClient::MatchSwitchApplicationCode:
            MatchSwitchApplicationHandler(buffer.substr(1)); break;
        case recvMsgFromClient::AcceptOrRefuseCode:
            AcceptOrRefuseHandler(buffer.substr(1)); break;
        case GameRoom::recvMsgFromClient::RoomInfoArrivedCode:
            RoomInfoArrivedHandler(); break;
        case GameRoom::recvMsgFromClient::PlayerOperationCode:
            this->game_room->PlayerOperationHandler(buffer, shared_from_this()); break;
        case GameRoom::recvMsgFromClient::CandleCardFeedbackCode:
            this->game_room->CandleCardFeedbackHandler(buffer); break;
        case GameRoom::recvMsgFromClient::GameFinishCode:
            GameFinishHandler(buffer.substr(1)); break;
        case GameRoom::recvMsgFromClient::ReconnectionProcessedCode:
            ReconnectionProcessedHandler(); break;
        default: {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Parse Error]:\t" + ip_port +
                         " can't pares message!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }
}

void GameSession::UserInfoToGameServerHandler(std::string buffer) {
    UserInfoToGameServer user_info;
    user_info.ParseFromArray(buffer.c_str(), buffer.size());

    user_id = user_info.userid();

    auto it = std::find_if(offline_list.begin(), offline_list.end(),
            [user_info](const OffLinePlayer & compare) {
        return compare.user_id == user_info.userid();
    });
    if(it != offline_list.end()) {
        // 新连接的用户是掉线用户。
        auto room = std::find_if(room_container.begin(), room_container.end(),
                [it](const std::shared_ptr<GameRoom> & compare) {
            return compare->getRoomID() == it->room_id;
        });

        this->game_room = *room;
        this->status = clientStatus::Reconnection;

        auto self = shared_from_this();
        auto client = std::find_if(client_info.begin(), client_info.end(),
                [self](const std::shared_ptr<GameSession> & compare){
            return compare == self;
        });
        this->game_room->setOfflinePlayer(it->user_id, *client);

        SendMessages(this->game_room->getRoomInfo());
    }
    else {
        status = clientStatus::BeforeMatch;     //确定状态为进入匹配前的状态
    }
}

void GameSession::EditUserInfoHandler(std::string buffer) {
    EditUserInfo edit_info;
    EditUserInfoFeedback edit_fb;
    edit_info.ParseFromArray(buffer.c_str(), buffer.size());

    std::string query_buffer;
    query_buffer = "UPDATE the_mountains.User_Info SET username = \'" + edit_info.newusername() +
            "\' WHERE id = \'" + std::to_string(user_id) + "\'";
    DatabaseService db;
    mysqlpp::Connection * conn = db.getConnection();
    mysqlpp::Query query = conn->query(query_buffer.c_str());

    if (query.exec()) {
        // 修改用户名成功
        edit_fb.set_issuccessedit(true);

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Username Edith]:\t" + ip_port +
                     " Changed username<" + edit_info.newusername() + "> successfully!";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);
    }
    else {
        // 修改用户名失败
        edit_fb.set_issuccessedit(false);

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Username Edith]:\t" + ip_port +
                     " Changed username<" + edit_info.newusername() + "> Failed!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }

    std::string temp;
    edit_fb.SerializeToString(&temp);
    std::string sendMsg = std::to_string(sendMsgToClient::EditUserInfoFeedbackCode) + temp;
    sendMsg[0] = sendMsg[0] - '0';
    SendMessages(sendMsg);
}

void GameSession::MatchSwitchApplicationHandler(std::string buffer) {
    MatchSwitchApplication match_sw;
    match_sw.ParseFromArray(buffer.c_str(), buffer.size());
    if(match_sw.personnum() == 0) {
        // 取消匹配
        auto it = std::find_if(match_queue_3.begin(), match_queue_3.end(),
                [this](const std::list<std::shared_ptr<MatchClientNode>>::value_type & compare) {
            return compare->client == shared_from_this(); });

        if(it != match_queue_3.end())
            match_queue_3.erase(it);

        status = BeforeMatch;

        std::string sendMsg = std::to_string(sendMsgToClient::MatchConfirmCode);
        sendMsg[0] = sendMsg[0] - '0';
        SendMessages(sendMsg);

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Cancel Matching]:\t" + ip_port +
                     " is cancel matching, " + std::to_string(match_queue_3.size()) + " user is in match queue!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);

    }
    else if (match_sw.personnum() == 3){
        // 三人房匹配
        auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());

        auto match_client = std::make_shared<MatchClientNode>();
        match_client->client = *it;

        match_queue_3.push_back(match_client);

        status = clientStatus::Matching;

        std::string sendMsg = std::to_string(sendMsgToClient::MatchConfirmCode);
        sendMsg[0] = sendMsg[0] - '0';
        SendMessages(sendMsg);

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Start Matching]:\t" + ip_port +
                     " is start matching, " + std::to_string(match_queue_3.size()) + " user is in match queue!";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);
    }
}

void GameSession::AcceptOrRefuseHandler(std::string buffer) {
    auto self = shared_from_this();
    auto it = std::find_if(room_container.rbegin(), room_container.rend(),
            [self](const std::shared_ptr<GameRoom> & compare){ return compare->isInThisRoom(self); });
    this->game_room = *it;

    status = InTheGame;
}

void GameSession::RoomInfoArrivedHandler() {
    if (status != clientStatus::Reconnection)
        this->game_room->SendPlayerInfo(shared_from_this());
    else
        this->game_room->SendReconnectionInfo(shared_from_this());
}

void GameSession::GameFinishHandler(std::string buffer) {
    std::string temp = std::to_string(GameRoom::sendMsgToClient::GameFinishCode_) + buffer;
    temp[0] = temp[0] - '0';
    game_room->sendMsgToAll(temp);

    std::vector<int> offline_id;
    int room_id = game_room->GameFinishHandler(buffer, offline_id);
    for(auto a : offline_id) {
        auto it = std::find_if(offline_list.begin(), offline_list.end(),
                [a](const OffLinePlayer & compare) {
            return compare.user_id == a;
        });
        if(it != offline_list.end())
            offline_list.erase(it);
    }

    // 注销房间
    auto it = find_if(room_container.begin(), room_container.end(),
            [room_id](const std::shared_ptr<GameRoom> & compare){ return compare->getRoomID() == room_id; });
    if (it != room_container.end())
        room_container.erase(it);

    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  Game Over]:\tGame over, room<" +
            std::to_string(room_id) + "> is destroyed!";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}

void GameSession::ReconnectionProcessedHandler() {
    status = clientStatus::InTheGame;

    int id = user_id;
    auto it = find_if(offline_list.begin(), offline_list.end(),
            [id](const OffLinePlayer & compare){
        return compare.user_id == id;
    });
    if(it != offline_list.end())
        offline_list.erase(it);

    this->game_room->ChangeStatusToNormal(shared_from_this());
    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  Reconnection Setup]:\tUser<" + std::to_string(user_id)
                 + "> reconnected in Room<" + std::to_string(game_room->getRoomID()) + ">!";
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}

void GameSession::quit_handler() {
    auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());
    if(it == client_info.end()) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Quit Error]:\tClient information not found!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
    else {
        client_info.erase(it);

        if((*it)->status == Matching) {
            // 如果正在匹配中
            auto match_3_it = std::find_if(match_queue_3.begin(), match_queue_3.end(),
                                   [this](const std::list<std::shared_ptr<MatchClientNode>>::value_type & compare) {
                                       return compare->client == shared_from_this(); });
            if (match_3_it != match_queue_3.end())
                match_queue_3.erase(match_3_it);
        }
        else if((*it)->status == InTheGame) {
            // 如果正在游戏中
            auto game_room = std::find_if(room_container.begin(), room_container.end(),
                                   [this](const std::vector<std::shared_ptr<GameRoom>>::value_type & compare) {
                                            return compare->isOffLine(shared_from_this());
                                        });

            OffLinePlayer off_player;
            off_player.user_id = user_id;
            off_player.room_id = this->game_room->getRoomID();
            offline_list.push_back(off_player);
            this->game_room = nullptr;

            // game_room 是有掉线用户的room的迭代器
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Player Dropped]:\tUser<" + std::to_string(off_player.user_id)
                         + "> is offline in Room<" + std::to_string(off_player.room_id) +
                         ">! Offline player number is " + std::to_string(offline_list.size()) + ".";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }
    std::string log_buffer;
    log_buffer = "\tGame Server's client number is : " + std::to_string(client_info.size());
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}
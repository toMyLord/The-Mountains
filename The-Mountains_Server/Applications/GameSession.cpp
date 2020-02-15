//
// Created by mylord on 2020/2/13.
//

#include "GameSession.h"
#include "GameContent.h"

GameSession::GameSession(tcp::socket socket, std::vector<std::shared_ptr<GameSession>> & client,
        std::list<std::shared_ptr<MatchClientNode>> & match_3, std::vector<std::shared_ptr<GameRoom>> & room):
        AsyncSession(std::move(socket)), client_info(client), match_queue_3(match_3), room_container(room) {
}

void GameSession::center_handler(std::string buffer) {
    switch(buffer[0]) {
        case heart_beats_code: RecvHeartBeats(); do_read(); break;
        case recvMsgFromClient::UserInfoToGameServerCode: UserInfoToGameServerHandler(buffer.substr(1)); break;
        case recvMsgFromClient::EditUserInfoCode: EditUserInfoHandler(buffer.substr(1)); break;
        case recvMsgFromClient::MatchSwitchApplicationCode: MatchSwitchApplicationHandler(buffer.substr(1)); break;
        case recvMsgFromClient::AcceptOrRefuseCode: AcceptOrRefuseHandler(buffer.substr(1)); break;
        default: {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "   Error]:\tGame Server can't parse client login request!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }
}

void GameSession::UserInfoToGameServerHandler(std::string buffer) {
    do_read();

    status = clientStatus::BeforeMatch;     //确定状态为进入匹配前的状态

    UserInfoToGameServer user_info;
    user_info.ParseFromArray(buffer.c_str(), buffer.size());

    user_id = user_info.userid();
}

void GameSession::EditUserInfoHandler(std::string buffer) {
    do_read();

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
    do_read();

    MatchSwitchApplication match_sw;
    match_sw.ParseFromArray(buffer.c_str(), buffer.size());
    if(match_sw.personnum() == 0) {
        // 取消匹配
        auto it = std::find_if(match_queue_3.begin(), match_queue_3.end(),
                [this](const std::list<std::shared_ptr<MatchClientNode>>::value_type & compare) {
            return compare->client == shared_from_this(); });

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
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
}

void GameSession::AcceptOrRefuseHandler(std::string buffer) {
    status = InTheGame;
}

void GameSession::quit_handler() {
    auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());
    if(it == client_info.end()) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Quit Error]:\tclient information not found!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
    else {
        if((*it)->status == Matching) {
            // 如果正在匹配中
            auto match_3_it = std::find_if(match_queue_3.begin(), match_queue_3.end(),
                                   [this](const std::list<std::shared_ptr<MatchClientNode>>::value_type & compare) {
                                       return compare->client == shared_from_this(); });

            match_queue_3.erase(match_3_it);
        }
        else if((*it)->status == InTheGame) {
            // 如果正在游戏中
            auto game_room = std::find_if(room_container.begin(), room_container.end(),
                                   [this](const std::vector<std::shared_ptr<GameRoom>>::value_type & compare) {
                                            return compare->game_content->OffLine(shared_from_this());
                                        });

            // game_room 是有掉线用户的room
        }
        client_info.erase(it);
    }
    std::string log_buffer;
    log_buffer = "\tGame Server's client number is : " + std::to_string(client_info.size());
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}
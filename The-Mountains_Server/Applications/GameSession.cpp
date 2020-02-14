//
// Created by mylord on 2020/2/13.
//

#include "GameSession.h"

GameSession::GameSession(tcp::socket socket, std::vector<std::shared_ptr<GameSession>> & client,
        std::queue<MatchClientNode<GameSession>> & match_3):
        AsyncSession(std::move(socket)), client_info(client), match_queue_3(match_3) {
}

void GameSession::center_handler(std::string buffer) {
    switch(buffer[0]) {
        case heart_beats_code: RecvHeartBeats(); do_read(); break;
        case recvMsgFromClient::UserInfoToGameServerCode: UserInfoToGameServerHandler(buffer.substr(1)); break;
        case recvMsgFromClient::EditUserInfoCode: EditUserInfoHandler(buffer.substr(1)); break;
        case recvMsgFromClient::MatchSwitchApplicationCode: MatchSwitchApplicationHandler(buffer.substr(1)); break;
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

}

void GameSession::quit_handler() {
    auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());
    if(it == client_info.end()) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Quit Error]:\tclient information not found!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
    else
        client_info.erase(it);

    std::string log_buffer;
    log_buffer = "\tGame Server's client number is : " + std::to_string(client_info.size());
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}
//
// Created by mylord on 2020/2/11.
//

#include "LoginSession.h"

LoginSession::LoginSession(tcp::socket socket, std::vector<std::shared_ptr<LoginSession>> & client):
    AsyncSession(std::move(socket)), client_info(client) {
}

void LoginSession::center_handler(std::string buffer) {
//        std::cout << "received:" << buffer << std::endl;
    do_read();
    switch((buffer[0]) - '0') {
        case recvMsgFromClient::UserLoginCode: UserLoginHandler(buffer.substr(1)); break;
        case recvMsgFromClient::TouristLoginCode: TouristLoginHandler(buffer.substr(1)); break;
        case recvMsgFromClient::RegisterLoginCode: RegisterLoginHandler(buffer.substr(1)); break;
        case recvMsgFromClient::RegisterDetectCode: RegisterDetectHanlder(buffer.substr(1)); break;
        default: {
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Parsing Error]:\tServer can't parse client login request!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }

}

void LoginSession::UserLoginHandler(const std::string & buffer) {
    std::string query_buffer;
    UserLogin user_login;
    user_login.ParseFromArray(buffer.c_str(), buffer.size());
    query_buffer = "SELECT id, username, score FROM the_mountains.User_Info WHERE password = \"" +
            user_login.account() + "\" and account = \"" + user_login.password() + "\"";

    DatabaseService db;
    mysqlpp::Connection * conn = db.getConnection();
    mysqlpp::Query query = conn->query(query_buffer.c_str());

    if (mysqlpp::StoreQueryResult res = query.store()) {
        if(res.size() == 1) {
            // 有相应的用户已注册
            mysqlpp::Row row = res[0];
            UserInfo user_info;
            user_info.set_userid(row[0]);
            user_info.set_username(row[1]);
            user_info.set_score(row[2]);

            std::string temp, sendMsg;
            user_info.SerializeToString(&temp);
            sendMsg = std::to_string(sendMsgToServer::UserInfoCode) + temp;
            sendMsg[0] = sendMsg[0] - '0';

            SendMessages(sendMsg);
        }
        else if (res.size() == 0) {
            // 未找到对应用户

        }
        else {
            // 其他情况
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Login Error]:\tAbnormal number of matches!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }
    else {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Query Error]:\tFailed to get item list!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
}

void LoginSession::TouristLoginHandler(const std::string & buffer) {
    std::cout << "2 sleeping 10s!\n";

}

void LoginSession::RegisterLoginHandler(const std::string & buffer) {
    std::cout << "3 sleeping 10s!\n";

}

void LoginSession::RegisterDetectHanlder(const std::string & buffer) {
    std::cout << "4 sleeping 10s!\n";

}

void LoginSession::quit_handler() {
    auto it = std::find(client_info.begin(), client_info.end(), shared_from_this());
    if(it == client_info.end()) {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Quit Error]:\tclient information not found!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
    else
        client_info.erase(it);

    std::string log_buffer;
    log_buffer = "\tclient num is : " + std::to_string(client_info.size());
    LogServices::getInstance()->RecordingBoth(log_buffer, true);
}
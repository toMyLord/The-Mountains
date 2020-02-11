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
    switch((int)(buffer[0])) {
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
            user_login.password() + "\" and account = \"" + user_login.account() + "\"";

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

            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Login Succeed]:\tUser <" + user_login.account() +
                    "> Login Succeed, id is " + std::to_string(user_info.userid()) + "!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
        else if (res.size() == 0) {
            // 未找到对应用户
            LoginDetectFeedback login_detect;
            login_detect.set_isaccountexist(false);
            login_detect.set_ispasswordcorrect(false);

            std::string temp, sendMsg;
            login_detect.SerializeToString(&temp);
            sendMsg = std::to_string(sendMsgToServer::LoginDetectFeedbackCode) + temp;
            sendMsg[0] = sendMsg[0] - '0';

            SendMessages(sendMsg);

            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Login failed]:\t" + user_login.account() +
                         " Login failed, Incorrect username or password!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
        else {
            // 其他情况
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Login Error]:\tAbnormal number of matches!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }
    else {
        // 数据库查询失败
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Query Error]:\tFailed to get item list!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
}

void LoginSession::TouristLoginHandler(const std::string & buffer) {
    std::string query_buffer;
    TouristLogin tourist_login;
    tourist_login.ParseFromArray(buffer.c_str(), buffer.size());
    query_buffer = "SELECT id, username, score FROM the_mountains.User_Info WHERE account = \""
            + tourist_login.account() + "\"";

    DatabaseService db;
    mysqlpp::Connection * conn = db.getConnection();
    mysqlpp::Query query = conn->query(query_buffer.c_str());

    if (mysqlpp::StoreQueryResult res = query.store()) {
        if(res.size() == 1) {
            // 有相应的游客信息
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

            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Login Succeed]:\tTourist <" + tourist_login.account() +
                         "> Login Succeed, id is " + std::to_string(user_info.userid()) + "!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
        else if (res.size() == 0) {
            // 未找到对应游客，需要建立游客账号并返回给客户端
            query_buffer = "SELECT MAX(id) FROM the_mountains.User_Info";
            mysqlpp::Query query = conn->query(query_buffer.c_str());
            mysqlpp::StoreQueryResult res = query.store();
            int tourist_id = res[0][0] + 1;         // 最大id号+1作为新的账号的id

            // 找到YK×××××× 中×××××最大值加1作为username和account

            LoginDetectFeedback login_detect;
            login_detect.set_isaccountexist(false);
            login_detect.set_ispasswordcorrect(false);

            std::string temp, sendMsg;
            login_detect.SerializeToString(&temp);
            sendMsg = std::to_string(sendMsgToServer::LoginDetectFeedbackCode) + temp;
            sendMsg[0] = sendMsg[0] - '0';

            SendMessages(sendMsg);
        }
        else {
            // 其他情况
            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Login Error]:\tAbnormal number of matches!";
            LogServices::getInstance()->RecordingBoth(log_buffer, false);
        }
    }
    else {
        // 数据库查询失败
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Query Error]:\tFailed to get item list!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }

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
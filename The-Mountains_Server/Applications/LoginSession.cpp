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
            sendMsg = std::to_string(sendMsgToClient::UserInfoCode) + temp;
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
            sendMsg = std::to_string(sendMsgToClient ::LoginDetectFeedbackCode) + temp;
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
            sendMsg = std::to_string(sendMsgToClient::UserInfoCode) + temp;
            sendMsg[0] = sendMsg[0] - '0';

            SendMessages(sendMsg);

            std::string log_buffer;
            log_buffer = '[' + TimeServices::getTime() + "  Login Succeed]:\tTourist <" + tourist_login.account() +
                         "> Login Succeed, id is " + std::to_string(user_info.userid()) + "!";
            LogServices::getInstance()->RecordingBoth(log_buffer, true);
        }
        else if (res.size() == 0) {
            // 未找到对应游客，需要建立游客账号并返回给客户端
            query_buffer = "SELECT MAX(id) FROM the_mountains.User_Info";
            mysqlpp::Query query_1 = conn->query(query_buffer.c_str());
            mysqlpp::StoreQueryResult res_1 = query_1.store();
            long tourist_id = res_1[0][0] + 1;         // 最大id号+1作为新的账号的id

            // 找到YK×××××× 中××××××最大值加1作为username和account
            query_buffer = "SELECT MAX(CAST(SUBSTRING(account,3) AS UNSIGNED INTEGER)) ";
            query_buffer += "FROM User_Info WHERE account REGEXP \'^YK[1-9][0-9]{5}$\'";
            mysqlpp::Query query_2 = conn->query(query_buffer.c_str());
            mysqlpp::StoreQueryResult res_2 = query_2.store();
            int tourist_account_num = res_2[0][0] + 1;

            std::string tourist_account, tourist_username;
            tourist_account = "YK" + std::to_string(tourist_account_num);
            tourist_username = "yk_" + std::to_string(tourist_account_num);

            // 将新生成的用户数据插入数据库
            query_buffer = "INSERT INTO the_mountains.User_Info VALUES(\'" + std::to_string(tourist_id) +
                    "\', \'" + tourist_account + "\', \'88888888\', \'" + tourist_username + "\', \'\', \'0\')";
            mysqlpp::Query query_3 = conn->query(query_buffer.c_str());
            if (query_3.exec()) {
                // 数据插入成功
                std::string log_buffer;
                log_buffer = '[' + TimeServices::getTime() +
                        "  Signup Succeed]:\tCreate new tourist account:" + tourist_account;
                LogServices::getInstance()->RecordingBoth(log_buffer, true);

                // 先发送 “协议4-游客账号反馈” 信息
                std::string temp, sendMsg;
                TouristFeedback tourist_fb;
                tourist_fb.set_account(tourist_account);
                tourist_fb.SerializeToString(&temp);
                sendMsg = std::to_string(sendMsgToClient::TouristFeedbackCode) + temp;
                sendMsg[0] = sendMsg[0] - '0';
                SendMessages(sendMsg);

                // 在发送 “协议1-用户信息”
                temp = "";
                sendMsg = "";
                UserInfo user_if;
                user_if.set_userid(tourist_id);
                user_if.set_username(tourist_username);
                user_if.set_score(0);
                user_if.SerializeToString(&temp);
                sendMsg = std::to_string(sendMsgToClient::UserInfoCode) + temp;
                sendMsg[0] = sendMsg[0] - '0';
                SendMessages(sendMsg);

                log_buffer = '[' + TimeServices::getTime() + "  Login Succeed]:\tTourist <" + tourist_account +
                             "> Login Succeed, id is " + std::to_string(tourist_id) + "!";
                LogServices::getInstance()->RecordingBoth(log_buffer, true);
            }
            else {
                // 数据插入失败
                std::string log_buffer;
                log_buffer = '[' + TimeServices::getTime() + "  Signup Error]:\tCan't create new tourist account!";
                LogServices::getInstance()->RecordingBoth(log_buffer, false);
            }
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
    RegisterLogin register_lg;
    register_lg.ParseFromArray(buffer.c_str(), buffer.size());

    // 连接到数据库
    DatabaseService db;
    mysqlpp::Connection * conn = db.getConnection();

    std::string query_buffer = "SELECT MAX(id) FROM the_mountains.User_Info";
    mysqlpp::Query query_1 = conn->query(query_buffer.c_str());
    mysqlpp::StoreQueryResult res_1 = query_1.store();
    long register_id = res_1[0][0] + 1;         // 最大id号+1作为新的账号的id

    // 将新生成的用户数据插入数据库
    query_buffer = "INSERT INTO the_mountains.User_Info VALUES(\'" + std::to_string(register_id) +
                   "\', \'" + register_lg.account() + "\', \'" + register_lg.password() + "\', \'\', \'" +
                    register_lg.email() + "\', \'0\')";
    mysqlpp::Query query_2 = conn->query(query_buffer.c_str());
    if (query_2.exec()) {
        // 数据插入成功
        UserInfo user_if;
        user_if.set_username("");
        user_if.set_userid(register_id);
        user_if.set_score(0);

        std::string temp, sendMsg;
        user_if.SerializeToString(&temp);
        sendMsg = std::to_string(sendMsgToClient::UserInfoCode) + temp;
        sendMsg[0] = sendMsg[0] - '0';

        SendMessages(sendMsg);

        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  Login Succeed]:\tUser <" + register_lg.account() +
                     "> Login Succeed, id is " + std::to_string(register_id) + "!";
        LogServices::getInstance()->RecordingBoth(log_buffer, true);
    }
    else {
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  SignUp Error]:\tCan't create new user account!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
    }
}

void LoginSession::RegisterDetectHanlder(const std::string & buffer) {
    RegisterDetect register_dt;
    RegisterDetectFeedback register_dt_fb;
    register_dt.ParseFromArray(buffer.c_str(), buffer.size());

    DatabaseService db;
    mysqlpp::Connection * conn = db.getConnection();

    // 判断用户名是否存在
    std::string query_buffer;
    query_buffer = "SELECT * FROM the_mountains.User_Info WHERE account = \"" +
            register_dt.account() + "\"";
    mysqlpp::Query query_1 = conn->query(query_buffer.c_str());
    mysqlpp::StoreQueryResult res_1 = query_1.store();

    bool is_account_exit = (res_1.size() > 0);
    register_dt_fb.set_isaccountexist(is_account_exit);

    // 判断email是否存在
    query_buffer = "SELECT * FROM the_mountains.User_Info WHERE email = \"" +
                   register_dt.email() + "\"";
    mysqlpp::Query query_2 = conn->query(query_buffer.c_str());
    mysqlpp::StoreQueryResult res_2 = query_1.store();

    bool is_email_exit = (res_2.size() > 0);
    register_dt_fb.set_isemailexist(is_email_exit);

    // 将判断结果返回给客户端
    std::string temp, sendMsg;
    register_dt_fb.SerializeToString(&temp);
    sendMsg = std::to_string(sendMsgToClient::RegisterDetectFeedbackCode) + temp;
    sendMsg[0] = sendMsg[0] - '0';

    SendMessages(sendMsg);

    // 志记
    std::string log_buffer;
    log_buffer = '[' + TimeServices::getTime() + "  Detect Result]:\tAccount <" + register_dt.account() +
                 "> is " + (is_account_exit ? "already exist, " : "not exist, ") + "email <" +
                 register_dt.email() + "> is " + + (is_email_exit ? "already exist, " : "not exist, ");
    LogServices::getInstance()->RecordingBoth(log_buffer, false);
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
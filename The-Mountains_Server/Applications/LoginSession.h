//
// Created by mylord on 2020/2/11.
//

#ifndef THE_MOUNTAINS_SERVER_LOGINSESSION_H
#define THE_MOUNTAINS_SERVER_LOGINSESSION_H

#include "../MsgProtocol/Login.pb.h"
#include "../Services/AsyncServices/AsyncSession.h"
#include "../Services/DBServices/DatabaseService.h"

class LoginSession : public AsyncSession {
public:
    LoginSession(tcp::socket socket, std::vector<std::shared_ptr<LoginSession>> & client);

private:
    std::vector<std::shared_ptr<LoginSession>> & client_info;

    void center_handler(std::string buffer) override ;

    void UserLoginHandler(const std::string & buffer);

    void TouristLoginHandler(const std::string & buffer);

    void RegisterLoginHandler(const std::string & buffer);

    void RegisterDetectHanlder(const std::string & buffer);

    void quit_handler() override;

    enum recvMsgFromClient {
        UserLoginCode = 1,      // 用户登录
        TouristLoginCode = 2,   // 游客登录
        RegisterLoginCode = 3,  // 用户注册
        RegisterDetectCode = 4  // 注册时检测
    };

    enum sendMsgToClient {
        UserInfoCode = 1,               // 用户信息
        LoginDetectFeedbackCode = 2,    // 登录反馈
        RegisterDetectFeedbackCode = 3, // 注册反馈
        TouristFeedbackCode = 4         // 游客账号反馈
    };
};



#endif //THE_MOUNTAINS_SERVER_LOGINSESSION_H

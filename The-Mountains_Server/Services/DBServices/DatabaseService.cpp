//
// Created by mylord on 2020/1/12.
//

#include "DatabaseService.h"

DatabaseService::DatabaseService() {
    conn = new mysqlpp::Connection(false);
    if (!conn->connect("the_mountains", "175.24.57.9", "root", "916804665")) {
        // 连接数据库失败
        std::string log_buffer;
        log_buffer = '[' + TimeServices::getTime() + "  DBConnetion Error]:\tCan't connect to database!";
        LogServices::getInstance()->RecordingBoth(log_buffer, false);
        exit(0);
    }
}

DatabaseService::~DatabaseService() {
    conn->disconnect();
    free(conn);
}

mysqlpp::Connection * DatabaseService::getConnection() {
    return this->conn;
}
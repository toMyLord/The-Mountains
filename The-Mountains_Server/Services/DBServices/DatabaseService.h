//
// Created by mylord on 2020/1/12.
//

#ifndef THE_MOUNTAINS_SERVER_DATABASESERVICE_H
#define THE_MOUNTAINS_SERVER_DATABASESERVICE_H

#include <mysql++/mysql++.h>
#include "../TimeServices/TimeService.h"
#include "../LogServices/LogServices.h"

class DatabaseService {
private:
    mysqlpp::Connection * conn;

public:
    DatabaseService();

    ~DatabaseService();

    mysqlpp::Connection * getConnection();
};


#endif //THE_MOUNTAINS_SERVER_DATABASESERVICE_H

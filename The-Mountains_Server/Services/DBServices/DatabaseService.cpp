//
// Created by mylord on 2020/1/12.
//

//#include "DatabaseService.h"
#include <mysql++/mysql++.h>
#include <iostream>

using namespace std;

//int main()
//{
//    // Connect to the sample database.
//    mysqlpp::Connection conn(false);
//    if (conn.connect("mysql_cpp_data", "localhost",
//                     "root", "916804665")) {
//        // Retrieve a subset of the sample stock table set up by resetdb
//        // and display it.
//        mysqlpp::Query query = conn.query("select * from stock");
//        if (mysqlpp::StoreQueryResult res = query.store()) {
//            cout << "We have:" << endl;
//            mysqlpp::StoreQueryResult::const_iterator it;
//            for (it = res.begin(); it != res.end(); ++it) {
//                mysqlpp::Row row = *it;
//                for(int i = 0; i < 6; i++)
//                cout << '\t' << row[i];
//                cout << endl;
//            }
//        }
//
//        else {
//            cerr << "Failed to get item list: " << query.error() << endl;
//            return 1;
//        }
//
//        return 0;
//    }
//    else {
//        cerr << "DB connection failed: " << conn.error() << endl;
//        return 1;
//    }
//}

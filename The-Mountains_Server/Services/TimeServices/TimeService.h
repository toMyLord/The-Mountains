//
// Created by mylord on 2020/1/3.
//

#ifndef THE_MOUNTAINS_SERVER_TIMESERVICE_H
#define THE_MOUNTAINS_SERVER_TIMESERVICE_H

#include <sstream>
#include <string>
#include <time.h>

class SpecificTime {
public:
    std::string getTime(){
        time_t now = time(0);
        tm * t = localtime(&now);


        std::stringstream time_stream;
        time_stream << t->tm_year + 1900 << "/" << t->tm_mon + 1 << "/"
                    << t->tm_mday << " " << t->tm_hour << ":" << t->tm_min << ":"
                    << t->tm_sec;

        return time_stream.str();
    }
};

#endif //THE_MOUNTAINS_SERVER_TIMESERVICE_H

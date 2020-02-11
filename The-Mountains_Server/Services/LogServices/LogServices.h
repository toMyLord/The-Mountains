//
// Created by mylord on 2020/2/11.
//

#ifndef THE_MOUNTAINS_SERVER_LOGSERVICES_H
#define THE_MOUNTAINS_SERVER_LOGSERVICES_H

#include <iostream>
#include <fstream>
#include "../TimeServices/TimeService.h"

class LogServices {
private:
    // 阻止编译器自动生成copying操作
    LogServices(LogServices &) = delete;
    LogServices & operator= (const LogServices &) = delete;
    LogServices();

    static LogServices * log_service;
    std::ofstream log_file;

public:
    // 内联以提高效率
    static inline LogServices * getInstance() {
        if(log_service == nullptr){
            log_service = new LogServices();
        }
        return log_service;
    }

    // 在终端中打印日志
    void RecordingInTerminal(const std::string & buffer, bool is_standard);

    // 在文件中打印日志
    void RecordingInLogfile(const std::string & buffer);

    // 同时在终端和文件中打印日志
    void RecordingBoth(const std::string & buffer, bool is_standard);
};


#endif //THE_MOUNTAINS_SERVER_LOGSERVICES_H

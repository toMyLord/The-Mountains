//
// Created by mylord on 2020/2/11.
//

#include "LogServices.h"

LogServices * LogServices::log_service = nullptr;
std::mutex LogServices::mtx;

LogServices::LogServices(): log_file("./server.log", std::ios::app) {}

// 在终端中打印日志
void LogServices::RecordingInTerminal(const std::string & buffer, bool is_standard) {
    (is_standard ? std::cout : std::cerr) << buffer << std::endl;
}

// 在文件中打印日志
void LogServices::RecordingInLogfile(const std::string & buffer) {
    log_file << buffer << std::endl;
}

// 同时在终端和文件中打印日志
void LogServices::RecordingBoth(const std::string & buffer, bool is_standard) {
    RecordingInTerminal(buffer, is_standard);
    RecordingInLogfile(buffer);
}

cmake_minimum_required(VERSION 3.0)
project(The_Mountains_Server)

set(CMAKE_CXX_STANDARD 11)

aux_source_directory(Services/AsyncServices AsyncServices_src)
aux_source_directory(Services/DBServices DBServices_src)
aux_source_directory(Services/LogServices LogServices_src)
aux_source_directory(Services/TimeServices TimeServices_src)
aux_source_directory(MsgProtocol Msg_src)
aux_source_directory(Applications Applications_src)

#include_directories(/usr/local/include/mysql++)
include_directories(/usr/include/mysql)
#link_libraries(/usr/local/lib)

add_executable(The_Mountains_Server
        main.cpp
        ${Msg_src}
        ${AsyncServices_src}
        ${DBServices_src}
        ${LogServices_src}
        ${TimeServices_src}
        ${Applications_src}
)

target_link_libraries(The_Mountains_Server pthread)
target_link_libraries(The_Mountains_Server mysqlpp)
target_link_libraries(The_Mountains_Server protobuf)
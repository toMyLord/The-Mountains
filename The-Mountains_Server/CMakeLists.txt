cmake_minimum_required(VERSION 3.0)
project(The_Mountains_Server)

set(CMAKE_CXX_STANDARD 14)

aux_source_directory(Services/AsyncServices AsyncServices_src)

#include_directories(/usr/local/include/mysql++)
include_directories(/usr/include/mysql)
#link_libraries(/usr/local/lib)

add_executable(The_Mountains_Server
#        main.cpp
#        Services/AsyncServices/AsyncSession.h
#        Services/AsyncServices/AsyncSession.cpp
#        Services/AsyncServices/AsyncServer.h
#        Services/TimeServices/TimeService.h
        Services/DBServices/DatabaseService.cpp
        Services/DBServices/DatabaseService.h
)

target_link_libraries(The_Mountains_Server pthread)
target_link_libraries(The_Mountains_Server mysqlpp)
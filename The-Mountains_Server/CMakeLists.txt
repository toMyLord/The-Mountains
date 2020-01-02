cmake_minimum_required(VERSION 3.15)
project(The_Mountains_Server)

set(CMAKE_CXX_STANDARD 14)

aux_source_directory(Services/AsyncServices AsyncServices_src)

add_executable(The_Mountains_Server main.cpp ${AsyncServices_src})

target_link_libraries(The_Mountains_Server pthread)
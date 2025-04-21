#!/bin/bash
# filepath: /Users/I528433/Documents/GitHub/SunshineMiner/mklink.sh

# 获取脚本所在目录
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# 定义路径
CLIENT_PATH="SunshineMinerClient"
SERVER_PATH="SunshineMinerServer"
SHARED_PATH="SunshineMinerShared"
SHARED_CODE_RELATIVE="Shared"

# 转换为绝对路径
CLIENT_ABS_PATH="$SCRIPT_DIR/$CLIENT_PATH"
SERVER_ABS_PATH="$SCRIPT_DIR/$SERVER_PATH"
SHARED_CODE_ABS_PATH="$SCRIPT_DIR/$SHARED_PATH/$SHARED_CODE_RELATIVE"

echo "CLIENT_ABS_PATH=$CLIENT_ABS_PATH"
echo "SERVER_ABS_PATH=$SERVER_ABS_PATH"
echo "SHARED_CODE_ABS_PATH=$SHARED_CODE_ABS_PATH"

# 创建客户端的符号链接
if [ ! -L "$CLIENT_ABS_PATH/Assets/Scripts/Shared" ]; then
    echo "Create link for ClientShared..."
    ln -s "$SHARED_CODE_ABS_PATH" "$CLIENT_ABS_PATH/Assets/Scripts/Shared"
else
    echo "ClientShared already exists..."
fi

# 创建服务器的符号链接
if [ ! -L "$SERVER_ABS_PATH/Shared" ]; then
    echo "Create link for ServerShared..."
    ln -s "$SHARED_CODE_ABS_PATH" "$SERVER_ABS_PATH/Shared"
else
    echo "ServerShared already exists..."
fi

echo
echo "Over..."
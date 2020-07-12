#!/bin/bash
executable="./QIES.Frontend/src/bin/Debug/net5.0/QIES.Frontend.dll"

if [ ! -f "$executable" ]
then
  dotnet build
fi

dotnet $executable "$1" "$2"

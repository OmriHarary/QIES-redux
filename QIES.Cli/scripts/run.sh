#!/bin/bash
executable="./QIES.Cli/src/bin/Debug/net5.0/QIES.Cli.dll"

if [ ! -f "$executable" ]
then
  dotnet build > /dev/null
fi

dotnet $executable "$1" "$2"

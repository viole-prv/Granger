@echo off

dotnet build  "Granger" -c "Debug" -f "net6.0-windows"
echo:
dotnet publish "Granger Server" -c "Debug" -f "net6.0" -o "Granger\bin\Debug\net6.0-windows"

pause

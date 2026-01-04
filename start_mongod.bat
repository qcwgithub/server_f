..\mongodb-win32-x86_64-windows-8.2.3\bin\mongod.exe --port 23456 --dbpath my_mongo_data

REM if not exist my_mongo_data md my_mongo_data
REM tasklist /fi "ImageName eq mongod.exe" /fo csv 2>NUL | find /I "mongod.exe">NUL
REM if "%ERRORLEVEL%"=="0" (
REM   echo mongod is running
REM ) else (
REM   echo mongod is not running, start now...
REM   ..\mongodb-win32-x86_64-windows-8.2.3\bin\mongod.exe --port 23456 --dbpath my_mongo_data
REM )

pause
tasklist /fi "ImageName eq redis-server.exe" /fo csv 2>NUL | find /I "redis-server.exe">NUL
if "%ERRORLEVEL%"=="0" (
  echo redis is running
) else (
  echo redis is not running, start now...
  start ..\Redis-8.4.0-Windows-x64-cygwin\redis-server.exe
)

pause
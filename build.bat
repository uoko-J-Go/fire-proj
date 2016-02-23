@echo off
"Fake.exe" "%1"
%errorlevel%
exit /b %errorlevel%
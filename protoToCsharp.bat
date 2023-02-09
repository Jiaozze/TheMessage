@echo off

set "PROTOC_EXE=%cd%\ProtoTool\protoc.exe"
set "WORK_DIR=%cd%\Assets\Network"
set "CS_OUT_PATH=%cd%\Assets\Scripts\Proto"
::if not exist %CS_OUT_PATH% md %CS_OUT_PATH%

for /r %%i in ("Assets\Network\*.proto") do (
   echo gen %%i...
   %PROTOC_EXE%  --proto_path="%WORK_DIR%" --csharp_out="%CS_OUT_PATH%" %%i
   )
echo finish... 

pause

@echo off

setlocal
if "%SUB_NO_PAUSE_SYMBOL%"=="1" set NO_PAUSE_SYMBOL=1
if /I "%COMSPEC%" == %CMDCMDLINE% set NO_PAUSE_SYMBOL=1
set SUB_NO_PAUSE_SYMBOL=1
call :main %*
set EXIT_CODE=%ERRORLEVEL%
if not "%NO_PAUSE_SYMBOL%"=="1" pause
exit /b %EXIT_CODE%

:main
for %%v in (2019 2017) do (
  for %%p in (Enterprise Professional Community BuildTools) do (
    for %%b in (Current 15.0) do (
      if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\%%v\%%p\MSBuild\%%b\Bin\MSBuild.exe" (
        set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\%%v\%%p\MSBuild\%%b\Bin\MSBuild.exe"
        goto MSBuild_Found
      )
    )
  )
)
:MSBuild_Found

%MSBuild% CommDevToolkit.sln /t:Rebuild /p:Configuration=Release || exit /b 1
%MSBuild% NeoRAGEx2002Tools.sln /t:Rebuild /p:Configuration=Release || exit /b 1

copy Doc\Readme.*.txt ..\Bin\ || exit /b 1
copy Doc\UpdateLog.*.txt ..\Bin\ || exit /b 1
copy Doc\License.*.txt ..\Bin\ || exit /b 1
copy Doc\License-NeoRAGEx2002.txt ..\Bin\ || exit /b 1
copy Lib\zlib\zlib.net-license.txt ..\Bin\ || exit /b 1
copy Doc\Localization.*.txt ..\Bin\ || exit /b 1
if exist ..\Bin\Examples del ..\Bin\Examples\ /S /F /Q || exit /b 1
if not exist ..\Bin\Examples md ..\Bin\Examples || exit /b 1
xcopy Examples\*.* ..\Bin\Examples\ /E || exit /b 1
if not exist ..\Bin\Lan md ..\Bin\Lan || exit /b 1
copy Lan\*.*.ini ..\Bin\Lan\ || exit /b 1

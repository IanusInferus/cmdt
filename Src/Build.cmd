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
for %%f in ("%ProgramFiles%" "%ProgramFiles(x86)%") do (
  for %%v in (2022 2019) do (
    for %%p in (Enterprise Professional Community BuildTools) do (
      if exist "%%~f\Microsoft Visual Studio\%%v\%%p\MSBuild\Current\Bin\MSBuild.exe" (
        set MSBuild="%%~f\Microsoft Visual Studio\%%v\%%p\MSBuild\Current\Bin\MSBuild.exe"
        goto MSBuild_Found
      )
    )
  )
)
echo MSBuild not found.
echo You need to install Visual Studio 2019/2022 or add MSBuild environment variable.
exit /b 1
:MSBuild_Found

%MSBuild% CommDevToolkit.sln /t:Rebuild /p:Configuration=Release || exit /b 1
%MSBuild% NeoRAGEx2002Tools.sln /t:Rebuild /p:Configuration=Release || exit /b 1
%MSBuild% REPLACE_ABI_APRIL.sln /t:Rebuild /p:Configuration=Release || exit /b 1

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

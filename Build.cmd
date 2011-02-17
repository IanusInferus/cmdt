PATH %windir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

MSBuild /t:Rebuild /p:Configuration=Release

copy NDGrapher\SampleScr.vb ..\Bin\NDGrapher\
copy Doc\Readme.*.txt ..\Bin\
copy Doc\UpdateLog.*.txt ..\Bin\
copy Doc\License.*.txt ..\Bin\
copy zlib.net-license.txt ..\Bin\
copy Doc\Localization.*.txt ..\Bin\
del ..\Bin\Examples\ /S /F /Q
IF NOT EXIST ..\Bin\Examples MD ..\Bin\Examples
xcopy Examples\*.* ..\Bin\Examples\ /E
IF NOT EXIST ..\Bin\Lan MD ..\Bin\Lan
copy Lan\*.*.ini ..\Bin\Lan\
pause

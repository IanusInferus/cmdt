rd .vs /S /Q
rd "FileSystem\My Project" /S /Q
rd "GraphSystem\My Project" /S /Q
rd "ImageConverter\My Project" /S /Q
rd "GraphD3D\My Project" /S /Q
rd "XmlConverter\My Project" /S /Q
for /d %%a in (*) do if exist %%a\obj rd %%a\obj /S /Q
cd..
if exist Bin (
  cd Bin
  del *.pdb /F /Q
  del *.xml /F /Q
  del *.vshost.exe /F /Q
  del SlimDX.dll /F /Q
  del *.manifest /F /Q
  del *.CodeAnalysisLog.xml /F /Q
  del *.lastcodeanalysissucceeded /F /Q
  del Test.* /F /S /Q
  rd zh-CHS /S /Q
  cd..
)
cd Src
del *.cache /F /Q
pause

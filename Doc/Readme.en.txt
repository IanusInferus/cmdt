Commandos Developing Toolkit

Ianus Inferus(r_ex)



1 General Introduction

The toolkit is mainly used in modification in Comm2 or Comm3.
See "UpdateLog.en.txt" for version and newest changes.
As there is a lack of time, I do not do much exception handling. Sorry.
If there is someone who want to write help or translate the toolkit, then do it. If you contact me, I'll be very happy.
If someone want to join the project, or have found a bug, or have some suggestions, please contact me via GitHub(https://github.com/IanusInferus/cmdt).



2 Thanks
Thanks for the contribution made by these people:
invox              Made much file formats analysis, wrote a few precious documents, which is what the toolkit is based on
PJB                Made much file formats and algorithm analysis, which is what the toolkit is based on
wyel2000           Found many errors in the toolkit
NeoRAGEx2002       As a professional, he provided many valuable opinions
URF(faqy_003)      Provided some information about ABI
guobinnew1977      Provided some information about GRL
jinshengmao        Without his write_bmsb tool, no commandos modding will ever exist
In addition, thanks for the support of 深藏不露sjs and all other people.




3 File Format List

Supported File Format

Y64Manager         Y64
PCKManager         Comm1_DIR PCK CSF_PAK
PCKManagerHD       PCK_HD
MultiConverter     Y64 SEC GRL MBI
ImageConverter     Y64 Comm1_RLE GRL ABI MBI H2O Comm1_RLC
DeBsmb             BSMB(AN2 ANI BAS BIN BRI DAT GSC MIS MSB)
XmlConverter       SEC MA2 DES

Public File Format

Text File          Comm1_SCR Comm1_TIL Comm1_TIP Comm1_VOL CFG DAT FAC GMT LIS MAC MAN OGC ROL STR TXT TUT CSF_FLI CSF_PRP
Image File         BMP JPG PNG
Sound File         WAV Comm3_OGC CSF_rws(need Vorbis Codec)
Video File         POP(need Indeo Codec) BIK(need RAD Video Tools)
D3DX Texture File  DDS(Photoshop)

Not Supported

Encoded Text File  CSF_CSSFBS(BDD CFG FBS TXT)
Image File         Comm1_WAD ARLC
Others             Comm1_FNT Comm1_ZOM FNC FNM PK CSF_FNT

All file formats above refer to files in resource folders or packages.
File formats without a version belong to Comm2 or Comm3, or for all versions, or public formats.
There are some repetition, as there are different formats share the same name. (ex. encoded and not encoded text files)



4 System Requirement

The software needs Microsoft .Net Framework 4.8 or above.
If it is not preinstalled on your Windows, you can download from
https://dotnet.microsoft.com/download/dotnet-framework

SECEditor doesn't require but is allowed to work with the SlimDX runtime(6M), to get correct hidden object elimination and a better performance.
http://slimdx.org/download.php
Download the .NET 4.0 version of End User Runtime. You'll need the x86 (32-bit) version on 32-bit Windows, and the x64 (64-bit) version on 64-bit Windows.

For development, Visual Studio 2019/2022 is needed.



5 License

The software is freeware without charge. All source codes and binary codes are licensed under the BSD license, see License.en.txt.
All the documents are not under the same license. You can copy and distribute these documents without any modification. You can also refer and/or translate these documents. All other copyrights reserved.

The copyright of the library Firefly.Core.dll shipped with the software is held by me. And I permit it use the same license of the software. The library is separately published, see http://www.cnblogs.com/Rex/archive/2008/11/08/1329759.html
The copyright of the library zlib.net.dll shipped with the software is held by ComponentAce. Refer to zlib.net-license.txt for its license.
The copyright of the documents obj_format.en.txt and mtl.en.txt are held by Wavefront, Inc.



6 Reference

Source Code of the Toolkit               Src\

Documents of the Toolkit                 Src\Doc\

URF's Website(Chinese)                   http://www.faqy.cn

neoragex2002's BLOG(Chinese)             http://neoragex2002.cnblogs.com

My BLOG(Chinese)                         http://rex.cnblogs.com

Sina Commandos Forum(Chinese)            http://bbs.games.sina.com.cn/?h=http%3A//bbs.games.sina.com.cn/g_forum/00/92/08/&g=15

Commandos HQ(English)                    http://chq.gamemod.net/

Commandos Editing Center(Russian)        http://cec.h1.ru/files



7 Introduction of Tools

Y64Manager.exe

The software is used in exporting, importing and creating images around the Y64 files of Comm2 and Comm3, and it can also convert Y64 files between the versions.
Notice there is an accumulating error in exporting and importing which is almost the same as that with the JPG editing.

The "Color Index", "RGB", "YCbCr" at the bottom of the Exporter are used to do color space conversions. It helps when you want to know about the internal Y64 file structure.

About the three versions at the center of Converter: There are 4 versions of Y64 files in Comm. Comm2Version1 is for Comm2 Demo. Comm2Version2 is for Comm2 Final. Comm3Version3 is for Comm3 Demo and Final. Comm3Version4 is for Comm3 Final. As Comm3Version4 is not completely analysed, it's not in the target versions.

The detailed theory of the software is on the comming invox4C2_3keyfiles.doc by invox.


PCKManager.exe

The software is used in extracting and creating the PCK files of Comm2 and Comm3.
The software can be used in extracting the DIR files of Comm1 and a few PAK files of CSF.
After opening a PCK file, you can drag the files to the explorer to extract them. But you are highly recommended not to drag too much a time as there are some technic problems that the extracting are running before you can release your mouse.
The textbox in the right top corner with a "*.*" is a textbox for mask, which obeys the rule of Windows wildcards. Only files which match the mask will be extracted during the extraction. For example, if you want to extract all MIS files, you can input "*.MIS" and extract all the directories, and you'll get all the MIS files.

Related softwares:
DIRExtractor v0.3 (by JJ Soft, Netherlands)
CoRE v2.1 (Commandos Resource Extractor, by .slim, Russia)
Dragon UnPACKer v5.0.0 (by Alexandere Devilliers)


ImageConverter.exe

The software is used to translate Commandos image files. 
Now supporting:
Y64 <-> Bmp
RLE <-> Gif
GRL <-> Gif
ABI -> Gif
MBI <-> Obj + Mtl + Gif
SEC <-> Obj
H2O <.-> Bmp
Comm1_RLC <-> Png

Drag the files or directories onto the application file
- Or -
Use console mode like: ImageConverter file1 file2 file3 ...
will run it. 
But you can't drag too much at a time, as there is a limit in Windows that may cause some troubles.
Don't change the last two extended filename of exported file or directory (ex. ".rle.gif", ".grl.files").
For GRL files, when you change the name of the files under the directory, reserve the last extended filename(".gif"), and change the corresponding filename in "Description.ini" and scripts which are related to the file.
Notice: The gif files being exported out may have some problem when you draw new colors on it. (It's not the color you want.) It's caused by the palette. You CAN change the palette. What you need is to save the image as gif at last, then it's OK to be re-imported.
Developing...


DeBsmb.exe

The software is used to decode all the script files(*.MIS, *.GSC, *.BAS, *.ANI, *.BRI, etc).
Drag the files onto the application

- Or -

Use console mode like: DeBsmb file1 file2 file3 ...

will run it. 

Back up your original files first.

Related softwares:
write_bmsb (by jinshengmao, China)


SECEditor.exe

The software is used to edit districts and their attributes.

Controls:
ModeSwitch Tab  Default mode is FirstLand.
Third Mode
RotateUp W  RotateDown S  RotateLeft A  RotateRight D  MoveForward Up  MoveBackward Down  MoveLeft Left  MoveRight Right  MoveUp PageUp  MoveDown PageDown
FirstLand Mode
MoveForwardOnLand W  MoveBackwardOnLand S  MoveLeftOnLand A  MoveRightOnLand D  TurnUp Up  TurnDown Down  TurnLeftAgainstLand Left  TurnRightAgainstLand Right  MoveUpAgainstLand PageUp  MoveDownAgainstLand PageDown
First Mode
MoveForward W  MoveBackward S  MoveLeft A  MoveRight D  TurnUp Up  TurnDown Down  TurnLeft Left  TurnRight Right  MoveUp PageUp  MoveDown PageDown
Others
RotateAnticlockwise Q  RotateClockwise E  RotateWorldZAxis Alt+Left/Right  ZoomIn Alt+Up  ZoomOut Alt+Down  Focus F  ResetToDefaultView DoubleRightClick  Rotate PressRightMouseButtonAndMove
Multiselct PressCtrlAndSelect
SelectAll Ctrl+A

Related softwares:
3D .Sec Viewer v1.2.2 (by NeoRAGEx2002, China)


MultiConverter.exe

The software is used to translate all translatable Comm3 files to Comm2 final version.
When the setup directories is set correctly, the file convertions can be done through command lines.
The program will add a machine environment value "CommDevToolkit". It's used in batch files that convert Comm3 resources.

Supported file formats:
Y64 SEC GRL MBI
Files of other formats will be copied directly.

How to run:
MultiConverter.exe <SrcFile> <TarFile> [-p]
<SrcFile> is the relative path of the source file to the Comm3 setup directory. If there is space in the path, quotes are needed.
<TarFile> is the relative path of the target file to the Comm2 setup directory. If there is space in the path, quotes are needed.
-p refers to parameters.

Only Y64 needs parameters currently.
How to:
-p<ReplacePalette>,<LightnessFactor>,<SaturationFactor>
No space should exist around the commas.
<ReplacePalette> refers to whether to replace the palette with Comm2's, which ranges in {0,1}.
<LightnessFactor> refers to the Lightness Factor, which ranges in [-5,5].
<SaturationFactor> refers to the Saturation Factor, which ranges in [-5,5].
Such as following, when you want to convert TUT1\TUT1.Y64 to TU01\TU02.Y64:
MultiConverter.exe TUT1\TUT1.Y64 TU01\TU02.Y64 -p1,1.00,1.00

If you are trying to run without assigning the setup directories or they do not exist, you will enter the window mode, and you need to assign them and press OK to continue.


SingleInt.exe

Used to translate between single float and 32-bit integer.


Scaler.exe

Used to view binary files.


ImageConverter.exe

The software is used to analyze Commandos file formats.
Now supporting:
SEC <-> Xml
MA2 <-> Xml + Bmp + Png
DES <-> Xml + GRL

Drag the files or directories onto the application file
- Or -
Use console mode like: XmlConverter file1 file2 file3 ...
will run it. 
But you can't drag too much at a time, as there is a limit in Windows that may cause some troubles.


8 History of the Toolkit (Omitted)

9 List of Ancient Commandos Players in China (Omitted)

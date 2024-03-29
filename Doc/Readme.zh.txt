盟军敢死队开发工具箱

Ianus Inferus(地狱门神，r_ex)



1 概论

本工具箱主要用于盟军敢死队2和盟军敢死队3的修改。
查看“UpdateLog.zh.txt”以得到版本号和最新更新的内容。
因为时间不足的问题，本工具箱的错误处理比较简陋，见谅。
如果有谁愿意撰写本工具箱的帮助或者翻译本工具箱，那就干吧。如果能联系我，我将非常高兴。
如果愿意加入本工具箱的编写，发现了BUG，或者有什么意见或建议，请通过GitHub(https://github.com/IanusInferus/cmdt)与我联系。



2 鸣谢
感谢这些人为本工具箱做出的贡献：
盗版钦差(invox)  进行了很多文件格式的解析，制作了很有价值的文档，为本工具箱提供了理论依据
网站大律师(PJB)  进行了很多文件格式及其算法的解析，为本工具箱提供了理论依据
wyel2000         发现了工具箱的许多错误
NeoRAGEx2002     作为专业人士，提供了许多极有价值的意见
URF(faqy_003)    提供了ABI格式的部分资料
guobinnew1977    提供了GRL格式的部分资料
jinshengmao      没有他的脚本文件转换工具，就没有现在的盟军修改
此外，还要感谢深藏不露sjs和其他所有人的支持。



3 文件格式表

支持的文件格式

Y64文件管理器     Y64
PCK文件管理器     Comm1_DIR PCK CSF_PAK
PCK文件管理器HD   PCK_HD
万能文件转换器    Y64 SEC GRL MBI
图像文件转换器    Y64 Comm1_RLE GRL ABI MBI H2O Comm1_RLC
脚本文件转换器    BSMB(AN2 ANI BAS BIN BRI DAT GSC MIS MSB)
XML文件转换器     SEC MA2 DES

公开的文件格式

文本文件          Comm1_SCR Comm1_TIL Comm1_TIP Comm1_VOL CFG DAT FAC GMT LIS MAC MAN OGC ROL STR TXT TUT CSF_FLI CSF_PRP
图像文件          BMP JPG PNG
声音文件          WAV Comm3_OGC CSF_rws(需Vorbis解码器)
视频文件          POP(需Indeo解码器) BIK(需RAD Video Tools)
D3DX贴图文件      DDS(Photoshop)

尚未支持的文件格式

加密文本文件      CSF_CSSFBS(BDD CFG FBS TXT)
图像文件          Comm1_WAD ARLC
其它文件          Comm1_FNT Comm1_ZOM FNC FNM PK CSF_FNT

所有的文件格式都是指资源文件夹下或资源文件包中的文件的格式。
未标注版本的文件格式都是指盟军2或盟军3的，或者各版本都有的，或者通用文件。
有些重复的情况，说明有些不同格式有相同的扩展名。（如加密和未加密的文本文件）



4 环境要求

本软件需要 Microsoft .Net Framework 4.8 或更新版本支持。
如果Windows没有自带，可以从以下地址下载
https://dotnet.microsoft.com/download/dotnet-framework

SEC地形文件编辑器（SECEditor.exe）不必须但可以安装SlimDX运行库文件(6M)，以获得正确的消隐和更好的性能。
http://slimdx.org/download.php
下载其中End User Runtime的.NET 4.0版本，32位系统下载x86 (32-bit)版本，64位系统下载x64 (64-bit)版本。

开发需要Visual Studio 2019/2022。



5 用户使用协议

本软件是免费自由软件，所有源代码和可执行程序按照BSD许可证授权，详见License.zh.txt。
本软件的所有文档不按照BSD许可证授权，你可以不经修改的复制、传播这些文档，你还可以引用、翻译这些文档，其他一切权利保留。

本软件附带的Firefly.Core.dll版权属于我，符合本软件的协议，该库单独发行，参见http://www.cnblogs.com/Rex/archive/2008/11/08/1329759.html。
本软件附带的zlib.net.dll版权属于ComponentAce，其协议参见zlib.net-license.txt。
本软件附带的文档obj_format.en.txt和mtl.en.txt的版权属于Wavefront, Inc。



6 参考

工具箱的源代码                  Src\

工具箱的文档                    Src\Doc\

URF的网站                       http://www.faqy.cn

neoragex2002的BLOG              http://neoragex2002.cnblogs.com

我的BLOG                        http://rex.cnblogs.com

新浪盟军论坛                    http://bbs.games.sina.com.cn/?h=http%3A//bbs.games.sina.com.cn/g_forum/00/92/08/&g=15

浩方盟军论坛                    http://bbs.cga.com.cn/list/list_65.asp?bid=65

Commandos HQ（英语）            http://chq.gamemod.net/

Commandos Editing Center（俄语）http://cec.h1.ru/files



7 工具详细介绍

Y64文件管理器（Y64Manager.exe）

本程序用于导出、回写和创建盟军2和盟军3的Y64文件中的图像，并可将在几个版本之间相互转换。
注意本程序导出和回写图像时会有类似JPG文件转换的积累误差，所以请勿重复转换。

导出器最下面的颜色索引、RGB、YCbCr用于进行RGB、YCbCr颜色空间的转换。这在研究Y64文件的存储格式的时候有一些作用。

关于转换器中间的三个盟军Y64版本：盟军的Y64文件共有4个版本，其中盟军2版本1用于盟军2测试版（相对于正式版修改版来说，修改功能更多），盟军2版本2用于盟军2正式版，盟军3版本3用于盟军3测试版和正式版，盟军3版本4用于盟军3正式版。盟军3版本4尚未完全解析，所以不能作为目标版本。

软件的详细原理载于盗版钦差即将发布的invox4C2_3keyfiles.doc中。


PCK文件管理器（PCKManager.exe）

本程序用于解开和创建盟军2、盟军3的PCK文件。
本程序可用于解开盟军1的DIR文件和几个打击力量的PAK文件。
打开PCK文件后，你可以用鼠标拽拖其中文件到资源管理器中的目录下，以解开相应文件。但是一次不能太多，因为技术问题，解压是在你能够释放鼠标之前处理的。
右上角有“*.*”的文本框表示掩码，使用Windows的通配符规则。解压时只解压符合这些掩码的文件。例如：要解压所有的MIS文件，则输入“*.MIS”，然后框住所有文件夹，解压，就会得到所有的MIS文件。

相关软件：
DIRExtractor v0.3 (by JJ Soft, Netherlands)
CoRE v2.1 (Commandos Resource Extractor, by .slim, Russia)
Dragon UnPACKer v5.0.0 (by Alexandere Devilliers)


图像文件转换器（ImageConverter.exe）

本程序用于转换盟军图像文件。
现支持：
Y64 <-> Bmp
RLE <-> Gif
GRL <-> Gif
ABI -> Gif
MBI <-> Obj + Mtl + Gif
SEC <-> Obj
H2O <.-> Bmp
Comm1_RLC <-> Png

将文件或文件夹拖到该程序执行文件上
——或——
在命令行中输入
ImageConverter 文件1 文件2 文件3 ...
均可完成转换。但一次不能太多，否则会超过Windows限制，产生错误。

请勿更改导出的文件或文件夹的后两个扩展名（即".rle.gif"、".grl.files"等）。
对于GRL文件，改变导出的文件夹下文件的名称时，请保留最后一个扩展名（即".gif"），同时需改变"Description.ini"内部的文件名和使用该GRL文件的脚本文件中的引用。
注意：转换出来的Gif文件可能会因为调色板的问题导致画新颜色无法实现（画出来的颜色不对），可以通过改变调色板来解决，只要最后存为Gif格式即可进行转换。
开发中……


脚本文件转换器（DeBsmb.exe）

盟军2和盟军3的脚本文件转换工具。
用于将脚本文件（*.MIS, *.GSC, *.BAS, *.ANI, *.BRI等）解码。

将文件或文件夹拖到该程序执行文件上

——或——

在命令行中输入

DeBsmb 文件1 文件2 文件3 ...

均可完成转换。

注意备份原文件。

相关软件：
盟军2的脚本文件转换工具(write_bmsb) (by jinshengmao, China)


SEC地形文件编辑器（SECEditor.exe）

用于编辑SEC地形文件中的区域和属性。

按键如下：
模式切换 Tab  默认为第一人称陆地模式
第三人称模式
上旋 W  下旋 S  左旋 A  右旋 D  前进 Up  后退 Down  左移 Left  右移 Right  上升 PageUp  下降 PageDown
第一人称陆地模式
沿陆地前进 W  沿陆地后退 S  沿陆地左移 A  沿陆地右移 D  向上 Up  向下 Down  沿陆地左转 Left  沿陆地右转 Right  相对陆地上升 PageUp  相对陆地下降 PageDown
第一人称模式
前进 W  后退 S  左移 A  右移 D  向上 Up  向下 Down  左转 Left  右转 Right  上升 PageUp  下降 PageDown
其他按键
逆时针旋转 Q  顺时针旋转 E  绕世界坐标Z轴自转 Alt+Left/Right  放大 Alt+Up  缩小 Alt+Down  聚焦 F  恢复默认视角 双击右键  转动 按住右键移动
多选 按住Ctrl键选择
全选 Ctrl+A

相关软件：
盟军敢死队3D .Sec文件浏览器(3D .Sec Viewer) v1.2.2 (by NeoRAGEx2002, China)


万用文件转换器（MultiConverter.exe）

本程序能将所有已知转换方法的盟军3文件转换成盟军2正式版的格式。
设定好安装文件夹后，可以通过命令行进行文件转换。
本程序会向计算机的环境变量中增加CommDevToolkit一项，供盟军3资源文件转换批处理文件使用。

支持以下文件格式：
Y64 SEC GRL MBI
其他文件会直接复制。

运行方式：
MultiConverter.exe <SrcFile> <TarFile> [-p]
<SrcFile>是源文件相对于盟军3安装目录的相对路径。如果路径之间有空格，必须加引号。
<TarFile>是目标文件相对于盟军2安装目录的相对路径。如果路径之间有空格，必须加引号。
-p 表示参数。

现在只有Y64需要参数。
格式为
-p<ReplacePalette>,<LightnessFactor>,<SaturationFactor>
逗号之间不能有空格。
<ReplacePalette>表示是否替换为盟军2的调色板，取值有0、1。
<LightnessFactor>表示亮度系数，为实数。范围为[-5,5]。
<SaturationFactor>表示饱和度系数，为实数。范围为[-5,5]。
例如，转换TUT1\TUT1.Y64为TU01\TU02.Y64可以输入：
MultiConverter.exe TUT1\TUT1.Y64 TU01\TU02.Y64 -p1,1.00,1.00

如果命令行运行时没有指定安装文件夹或安装文件夹不存在，会进入窗口模式，直到设定好，按下确定才会继续。


SingleInt.exe

用于单精度浮点数与32位整数转换。


Scaler.exe

用于查看二进制文件数据。


XML文件转换器（XmlConverter.exe）

本程序用于解析盟军文件格式。
现支持：
SEC <-> Xml
MA2 <-> Xml + Bmp + Png
DES <-> Xml + GRL

将文件或文件夹拖到该程序执行文件上
——或——
在命令行中输入
XmlConverter 文件1 文件2 文件3 ...
均可完成转换。但一次不能太多，否则会超过Windows限制，产生错误。


8 本工具的历史背景

国内最早进行盟军2修改尝试的是jinshengmao和他的MIS破解工具write_bmsb，大概是2003年的时候，之前虽然有人改过盟军1的地图，但是没有引起太大注意。后来，URF和我在新浪盟军论坛制作了一些修改后的地图，以及《盟军敢死队2修改基础教程》。里面附带了卖克狼汉化的PCK文件解压软件。后来有一两年时间，盟军2的修改都主要是URF在做，给大家带来了《沉默杀手》等地图，以及单位部署察看器、盟军MOD启动器、MOD包制作工具等工具。期间，PJB以网站大律师的帐号发布了多篇关于GSC脚本的文章。
后来盗版钦差和PJB开始了对地形文件Y64、SEC、MA2的破解，又是几年过去了，虽然贴子没有多少人顶，但他们仍然坚持着，最后基本搞清楚了这几个文件的格式，并写成了invox4C2_3keyfiles，invox4C2_someideas和invox4C2_2auxfiles三篇长文。而我，也积累了一些小游戏、小软件的项目开发经验，并且正好放三个月的高考大暑假。于是，我开始编写一些盟军2文件的解析工具。
最开始是处理Y64文件，这种文件其实类似JPEG图片的图片组，包含了游戏中的室外场景的背景地图。通过了解YCbCr颜色空间后，这个工具就做好了。最开始因为没有回写功能，称之为Y64Exporter，后来才叫Y64Manager。
后来慢慢的东西就多起来了，2007年寒假，我就把这些东西统一命名成盟军敢死队开发工具箱。
前面还遗漏了PJB和URF一起汉化破解的debug.exe，guobinnew1977的图片工具，neoragex2002的SEC查看器，URF的盟军MOD版主程序，等等。
既然有这么多人曾投身于盟军2的修改，将来也一定还会有人加入。
所以，不要仅仅看到现在盟军2修改前进的步伐是龟速，要看到历史的大方向，即：
道路是曲折的，前途是光明的。



9 古代盟军玩家表

下面列出我所知的国内盟军玩家，用以纪念我2000-2003年在新浪盟军以及急速盟军度过的时光（以新浪ID为主，括号里是QQ名）：
可爱苹果                        上古时代元老
saly33                          上古时代元老
香烟夫人                        上古时代元老
vivian19791020                  上古时代元老
commanossepro                   上古时代元老
小爽斑竹                        愚人时代坛主
愚人天才                        愚人时代盟军版主
sony8710(微风细雨)              愚人时代核心玩家
打倒宝洁                        愚人时代核心玩家
降降温                          愚人时代核心玩家，后愚人时代版主
役满                            愚人时代核心玩家，后愚人时代版主
愚人天才首席弟子(Cooper)        盟军小站/盟军中文网站长，急速版主
卖克狼                          游戏杂志撰稿人
benju_2hao(阳春白雪)            网易盟军玩家
浪里白条                        网易盟军玩家
小民9155                        愚人时代玩家
crow311617                      愚人时代玩家
commandos_liker                 愚人时代玩家
xmcx910                         愚人时代玩家
wyel2000                        愚人时代玩家
天地笑                          愚人时代玩家
龙在天8209                      愚人时代玩家
射隼于墉                        愚人时代玩家
浪天游                          愚人时代玩家
消消暑                          愚人时代玩家
jinshengmao                     盟军2MIS破解工具制作者
faqy_003(URF)                   愚人时代玩家，盟军2地图《沉默杀手》制作者，盟军2ABI文件工具制作者，《盟军敢死队2修改基础教程》第一作者
r0_ex(地狱门神，Ianus Inferus)  我，愚人时代玩家，盟军2地图《狙击日本人》制作者，《盟军敢死队2修改基础教程》共同作者
orangeink                       愚人时代玩家，降降温小弟
孟夫子xp                        役满时代玩家
99lhc                           役满时代玩家
盗版钦差(invox)                 役满时代玩家，盟军2文件格式分析者，《invox4C2_3keyfiles.doc》作者
刀足轻敢死队                    急速玩家
蓓蕾帽檐儿                      急速玩家
盟军狙击高手                    急速玩家
水鬼飞菜刀                      急速玩家
间谍的毒针                      急速玩家
老顽童5099                      后役满时代玩家
网站大律师(PJB)                 后役满时代玩家，盟军2debug.exe破解发布者，盟军2文件格式分析者
深藏不露sjs                     后役满时代玩家
yybsep                          后役满时代玩家
guobinnew1977                   后役满时代玩家，盟军2ABI、GRL文件浏览研究者

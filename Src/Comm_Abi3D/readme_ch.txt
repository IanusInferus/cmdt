TODO:
1. 效率效率效率!!!!!!!!!!!!!!!!!!
2. 显示方面，目前还是左右翻转的情况，需要修正（源自左右手坐标系的不一致） (Fixed.)
3. 似乎还有一个O对话框弹出时导致不能OnPaint的bug，但一直无法再现，可能跟RenderLoop有关
4. Release版本居然直接失败？检查Debug.Assert里面有没有br.Readxxx (Fixed.)
5. AnimatedModel和ShadowModel之间的耦合，transv，不太优雅.......
6. ShadowModel的计算是否还可以优化？

-------------------------------------------------------------------------------------------------
盟军敢死队3D .Abi文件浏览器(3D .Abi Viewer) ver.1.4

- by <NeoRAGEx2002>  2007.9.24


1. 运行时软、硬件环境

a. Windows XP pro SP2(我只测试了xp下的运行情况，其他的没有测过)
b. Microsoft .Net framework 2.0(22.4MB)，下载地址为:
   http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe
c. DirectX October '06版(55.6MB)，下载地址为:
   http://download.microsoft.com/download/d/4/6/d46cc24d-33df-4727-aa89-9512513c67d3/directx_oct2006_redist.exe
d. 支持DirectX 8.0的显示加速卡，32M显存，1024*768*32位真彩色、24位Z深度、双线性及各向异性材质过滤


2. 键盘热键

上 / 下    : Camera经度
左 / 右    : Camera纬度
J、K       : Camera距离远近
F          : 线架、贴图切换
W          : 阴影显示切换
S          : 全屏反锯齿切换
R          : 恢复默认的Camera位置
O          : 查看其他.abi文件
B          : 背景色切换
I          : 显示相关信息
X          : 显示坐标轴
A / >      : 下一个动作
<          : 上一个动作
+          : 提高动作速度
-          : 降低动作速度
D / PageUp : 下一个模型
PageDown   : 上一个模型
ESC / Q    : 退出


3. 鼠标控制

左键     : 下一个动作
滚轮     : Camera纬度
右键+滚轮: Camera经度
中键+滚轮: Camera距离远近
左键双击 : 查看其他.abi文件
右键双击 : 线架、贴图切换
中键双击 : 阴影显示切换


4. 更新历史

*新增: 平面阴影显示功能
*修正: 坐标系原点位置调整
*修正: 规范化左右手系模型翻转
*新增: 部分自动配置检测功能
*新增: 全屏反锯齿FSAA显示功能
*修正: 左右手系不一致所导致的模型翻转
*修正: 纠正了所有不正确的术语("着装"->"模型")
*新增: 添加了对1050版本abi文件的支持
*修正: 修正了左右手系计算导致的BUG(终于!)
*修正: 修正了骨骼变换的矩阵计算
*新增: 基本的键盘、鼠标控制功能
*新增: .ABI贴图显示功能
*新增: .ABI线框显示功能


5. F.A.Q

*如何获取盟2/盟3的.ABI文件？
首先，必须安装盟2/盟3游戏；然后，使用rex所提供的工具PCKManager在盟2/盟3的.pck文件中提取

*何处下载PCKManager？
http://www.cnblogs.com/Rex/archive/2007/08/26/647203.html


6. 鸣谢

* 感谢jinshengmao的信息提供与帮助
* 感谢rex(rex.cnblogs.com)的信息提供与帮助


--------------------
by Neoragex2002 (http://neoragex2002.cnblogs.com)
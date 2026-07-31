盟军敢死队2及3 ABI文件格式表

NeoRAGEx2002整理

<p>&nbsp;</p>

盟军敢死队2 ABI文件格式

ABI文件包括了所有3维模型、模型的贴图和模型的关键帧动画定义。整个ABI文件由Head Chunk、Texture Chunk、Model Chunk、Animation Chunk四部分构成，分别对应着文件头、贴图定义、模型定义和关键帧动画定义。盟2中有4种版本的ABI文件，其中，1050/1060版本中所有的四部分信息已经弄清楚了，而1040版本中Animation Chunk部分的数据格式还有待进一步澄清。

<p>&nbsp;</p>

Head Chunk

<table>
<colgroup>
<col style="width: 17%" />
<col style="width: 16%" />
<col style="width: 16%" />
<col style="width: 16%" />
<col style="width: 35%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据块</p></th>
<th><p>数据</p></th>
<th><p>数据类型</p></th>
<th><p>长度</p></th>
<th><p>描述</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="4"><p>Basic Info DB</p></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>标志符</p></td>
</tr>
<tr class="even">
<td><p>Version</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>版本：1010、1040、1050、1060</p></td>
</tr>
<tr class="odd">
<td><p>Num TimeAxis</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>时间轴个数</p></td>
</tr>
<tr class="even">
<td><p>Num Animation</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>动画个数</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Texture Chunk

<table>
<colgroup>
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 35%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据块</p></th>
<th colspan="2"><p>数据</p></th>
<th><p>数据类型</p></th>
<th><p>长度</p></th>
<th><p>描述</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="11"><p>Texture Info DB<br />
贴图信息</p></td>
<td colspan="2"><p>Num Texture</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>贴图个数</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Texture Info DS</p></td>
<td><p>Block Identifier</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>恒为0(UNKNOWN)</p></td>
</tr>
<tr class="odd">
<td><p>Width</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>位图的宽度</p></td>
</tr>
<tr class="even">
<td><p>Height</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>位图的高度</p></td>
</tr>
<tr class="odd">
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>位图名称</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Palette DS<br />
色板信息</p></td>
<td><p>Red</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>红色分量</p></td>
</tr>
<tr class="odd">
<td><p>Green</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>绿色分量</p></td>
</tr>
<tr class="even">
<td><p>Blue</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>蓝色分量</p></td>
</tr>
<tr class="odd">
<td colspan="4"><p>(R,G,B)三元组数量为256个，共768字节</p></td>
</tr>
<tr class="even">
<td><p>Indexed  Bitmap DS<br />
位图数据</p></td>
<td><p>Bitmap Data</p></td>
<td><p>byte</p></td>
<td><p>Width * Height</p></td>
<td><p>位图数据</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>Texture Info DS、Pallette DS和Bitmap DS数据组，共Num Texture组</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Model Chunk

<table>
<colgroup>
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 10%" />
<col style="width: 35%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据块</p></th>
<th colspan="3"><p>数据</p></th>
<th><p>数据类型</p></th>
<th><p>长度</p></th>
<th><p>描述</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td colspan="4"><p>Identifier(1060版本专有，1050/1040版本则无)</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>未知标志(并不恒1)</p></td>
</tr>
<tr class="even">
<td colspan="4"><p>Num Model</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>模型个数</p></td>
</tr>
<tr class="odd">
<td colspan="4"><p>Num Bone</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>骨骼个数</p></td>
</tr>
<tr class="even">
<td rowspan="17"><p>Model DB<br />
模型信息</p></td>
<td rowspan="3"><p>Head Info DS</p></td>
<td colspan="2"><p>Num Vertex</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>顶点数</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>Num Polygon</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>多边形数</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>模型名称</p></td>
</tr>
<tr class="odd">
<td rowspan="4"><p>Vertices DS<br />
顶点信息</p></td>
<td colspan="2"><p>x</p></td>
<td><p>float</p></td>
<td><p>4</p></td>
<td><p>顶点坐标</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>y</p></td>
<td><p>float</p></td>
<td><p>4</p></td>
<td><p>顶点坐标</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>z</p></td>
<td><p>float</p></td>
<td><p>4</p></td>
<td><p>顶点坐标</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>(x,y,z)三元组，共Num Vertex个</p></td>
</tr>
<tr class="odd">
<td rowspan="6"><p>Polygon DS<br />
多边形信息</p></td>
<td colspan="2"><p>n</p></td>
<td><p>byte(1040为int)</p></td>
<td><p>1(4)</p></td>
<td><p>边数(注1)</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>Texture ID</p></td>
<td><p>byte(1040为int)</p></td>
<td><p>1(4)</p></td>
<td><p>贴图索引</p></td>
</tr>
<tr class="odd">
<td rowspan="4"><p>Vertex</p></td>
<td><p>Vertex ID 0</p></td>
<td><p>short(1040为int)</p></td>
<td><p>2(4)</p></td>
<td><p>顶点索引</p></td>
</tr>
<tr class="even">
<td><p>U 0</p></td>
<td><p>short(1040为float)</p></td>
<td><p>2(4)</p></td>
<td><p>/4096得贴图坐标Tu (1040中不除)</p></td>
</tr>
<tr class="odd">
<td><p>V 0</p></td>
<td><p>short(1040为float)</p></td>
<td><p>2(4)</p></td>
<td><p>/4096得贴图坐标Tv(1040中不除)</p></td>
</tr>
<tr class="even">
<td colspan="4"><p>(VID, U, V)共n组Vertex，全部是右手系</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>共Num Polygon个Polygon DS</p></td>
</tr>
<tr class="even">
<td rowspan="3"><p>Vidx to Bone DS<br />
骨骼-&gt;顶点对应表</p></td>
<td colspan="2"><p>Start Vertex Index</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>骨骼影响的顶点</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>End Vertex Index</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>骨骼影响的顶点</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>(Start idx,End idx),共Num Bone组</p></td>
</tr>
<tr class="odd">
<td colspan="7"><p>共Num Model个Model DB</p></td>
</tr>
<tr class="even">
<td rowspan="5"><p>Bone Hierarchy <br />
DB</p></td>
<td rowspan="4"><p>Bone Hierarchy <br />
DS<br />
骨骼继承结构</p></td>
<td colspan="2"><p>Parent Index</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>父节点的索引(注2)</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>Global Offset Vector</p></td>
<td><p>float*3</p></td>
<td><p>3*4</p></td>
<td><p>将骨骼本地坐标准换成<br />
世界坐标的偏移矢量</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>Node Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>当前节点名称</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>UNKNOWN</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>未知，恒为0</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>共Num Bone组个Bone Hierarchy DB</p></td>
</tr>
</tbody>
</table>

1.  并不恒为3或者4，如tiger.abi里面，坦克模型里面存在着n=6的情况
2.  即父节点在BoneHierarchy数组中的索引号，注意，这个数组里面的节点是以深度优先顺序保存的，这意味着我们可以顺序访问数组中每个节点，能够保证在访问某个节点时，其所有父节点均已被遍历 。即，其顺序本身构成了骨骼继承树形结构中的一个先序遍历。
3.  盟军2的3D模型数据全部默认采用右手系，进行运算的时候要特别留意

<p>&nbsp;</p>

Animation Chunk

<table>
<colgroup>
<col style="width: 33%" />
<col style="width: 32%" />
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 8%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据</p></th>
<th><p>数据类型</p></th>
<th><p>描述</p></th>
<th><p>描述</p></th>
<th><p>描述</p></th>
<th><p>描述</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td><p>num_translateKeyframes</p></td>
<td><p>int</p></td>
<td><p>平移运动关键桢个数</p></td>
<td></td>
<td rowspan="6"><p><strong>TranslateTimeAxis<br />
</strong>平移关键帧时间轴<strong></strong></p></td>
<td rowspan="13"><p><strong>TransformTimeAxis<br />
</strong>骨骼变换时间轴<strong></strong></p></td>
</tr>
<tr class="even">
<td><p>timestamp</p></td>
<td><p>byte</p></td>
<td><p>关键帧时间戳</p></td>
<td rowspan="4"><p><strong>TranslateKeyFrame<br />
</strong>平移关键帧<strong></strong></p></td>
</tr>
<tr class="odd">
<td><p>dx</p></td>
<td><p>short</p></td>
<td rowspan="3"><p>构成一个平移向量(要/256)</p></td>
</tr>
<tr class="even">
<td><p>dy</p></td>
<td><p>short</p></td>
</tr>
<tr class="odd">
<td><p>dz</p></td>
<td><p>short</p></td>
</tr>
<tr class="even">
<td colspan="4"><p>共num translateKeyframes个TranslateKeyFrame</p></td>
</tr>
<tr class="odd">
<td><p>num_rotateKeyframes</p></td>
<td><p>int</p></td>
<td><p>旋转运动关键桢个数</p></td>
<td></td>
<td rowspan="7"><p><strong>RotateTimeAxis<br />
</strong>旋转关键帧时间轴<strong></strong></p></td>
</tr>
<tr class="even">
<td><p>timestamp</p></td>
<td><p>byte</p></td>
<td><p>关键帧时间戳</p></td>
<td rowspan="5"><p><strong>RotateKeyFrame<br />
</strong>旋转关键帧<br />
　<strong></strong></p></td>
</tr>
<tr class="odd">
<td><p>x</p></td>
<td><p>short</p></td>
<td rowspan="4"><p>构成一个旋转Quaternion<br />
注意：是右手系的<br />
(要/32768)</p></td>
</tr>
<tr class="even">
<td><p>y</p></td>
<td><p>short</p></td>
</tr>
<tr class="odd">
<td><p>z</p></td>
<td><p>short</p></td>
</tr>
<tr class="even">
<td><p>w</p></td>
<td><p>short</p></td>
</tr>
<tr class="odd">
<td colspan="4"><p>共num rotateKeyframes个RotateKeyFrame</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>共num TimeAxis个TransformTimeAxis</p></td>
</tr>
<tr class="odd">
<td><p>animation_name</p></td>
<td><p>String(60字节)</p></td>
<td><p>动作名称</p></td>
<td rowspan="2"></td>
<td rowspan="5"><p><strong>Animation<br />
</strong>整体动画<strong></strong></p></td>
<td></td>
</tr>
<tr class="even">
<td><p>num_related_bones</p></td>
<td><p>int</p></td>
<td><p>动作相关的骨骼数目</p></td>
<td></td>
</tr>
<tr class="odd">
<td><p>bone_id</p></td>
<td><p>int</p></td>
<td><p>对应的骨骼ID</p></td>
<td rowspan="2"><p><strong>BoneAnimationEntry<br />
</strong>骨骼动画<strong></strong></p></td>
<td></td>
</tr>
<tr class="even">
<td><p>transform_time_axis_idx</p></td>
<td><p>int</p></td>
<td><p>该骨骼所对应的变换时间轴</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="4"><p>共num_related_bones个BoneAnimationEntry</p></td>
<td></td>
</tr>
<tr class="even">
<td colspan="5"><p>共num Animation个Animation</p></td>
<td></td>
</tr>
</tbody>
</table>

基于关键帧的骨骼动画对象模型：

1.  一个TranslateTimeAxis由多个TranslateKeyFrame构成
2.  一个RotateTimeAxis由多个RotateKeyFrame构成
3.  一个TransformTimeAxis由一个TranslateTimeAxis外加一个RotateTimeAxis构成
4.  一个BoneAnimationEntry由所对应骨骼的编号、和该骨骼所对应的TransformTimeAxis构成
5.  一个Animation由多个BoneAnimationEntry构成

\*1040 版本里面，貌似直接存的是"时间戳(float)+变换矩阵(4\*4\*short)”，一个关键帧占36字节，并且不区分是平移阵还是旋转阵 ，但验证未成功。

Animation Chunk里面，直接存的是"时间戳(float)+ int（未知，值恒为3）＋ (float)＊7(平移阵＋旋转阵)，一个关键帧占36字节。——老顽童5099

1060版ABI是没有BASE动作的（不动的动作），比1050少了这么个动作。——老顽童5099

平移阵有父骨骼的情况下都是0，ABI中的数值很接近0，所以在1050　1060中将平移分开，只保留开始与最后时间的平移，ABI就小了。——老顽童5099

<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是Little-endian的

2、本文件包括了URF提供的文档中对贴图部分的部分理解

3、本文件包括了jinshengmao提供的角色动画(合成版)源代码中对骨骼动画部分的理解

4、int表示Int32，float表示Float32(Single)，short表示Int16。

<p>&nbsp;</p>
<p>&nbsp;</p>

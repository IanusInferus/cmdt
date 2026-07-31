盟军敢死队2及3 GRL文件格式表

地狱门神（F.R.C.）制作

<table>
<colgroup>
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 30%" />
<col style="width: 25%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据区</p></th>
<th><p>数据块</p></th>
<th><p>数据</p></th>
<th><p>数据类型</p></th>
<th><p>长度</p></th>
<th><p>描述</p></th>
<th><p>样例数据</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="22"><p>Header DA</p></td>
<td rowspan="6"><p>Info DB</p></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>标志符</p></td>
<td><p>4746524C(GFRL)</p></td>
</tr>
<tr class="even">
<td><p>Version Sign</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>版本号</p></td>
<td><p>64000000(d)/65000000(e)</p></td>
</tr>
<tr class="odd">
<td><p>Image Count</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图像数</p></td>
<td><p>08000000</p></td>
</tr>
<tr class="even">
<td><p>Palette Count</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>调色板数</p></td>
<td><p>03000000</p></td>
</tr>
<tr class="odd">
<td><p>Image Info Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>单个图像信息头长度</p></td>
<td><p>40000000/4C000000</p></td>
</tr>
<tr class="even">
<td><p>Palette Info Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>单个调色板信息头长度</p></td>
<td><p>2C000000</p></td>
</tr>
<tr class="odd">
<td rowspan="12"><p>Image Info DB</p></td>
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>图像名称</p></td>
<td></td>
</tr>
<tr class="even">
<td><p>Offset</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图像数据开头相对于第一个图像数据开头的偏移量</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图像数据长度（注意：已发现长度为0的图像，估计是被删除的图像。）</p></td>
<td><p>F0060000</p></td>
</tr>
<tr class="even">
<td><p>Palette Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>调色板索引</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>Compression</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>压缩方法，有三种，</p>
<p>(1) 2、4</p>
<p>(2) 0x142</p>
<p>(3) 0x102、0x302、0x802</p></td>
<td><p>02000000, 02010000</p></td>
</tr>
<tr class="even">
<td><p>Width</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图像宽度</p></td>
<td><p>38000000</p></td>
</tr>
<tr class="odd">
<td><p>Height</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图像高度</p></td>
<td><p>2E000000</p></td>
</tr>
<tr class="even">
<td><p>Center X</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>中心点横坐标</p></td>
<td><p>1C000000</p></td>
</tr>
<tr class="odd">
<td><p>Center Y</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>中心点纵坐标</p></td>
<td><p>17000000</p></td>
</tr>
<tr class="even">
<td><p>Unknown1(Comm3)</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>未知数据</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>Unknown2(Comm3)</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>未知数据</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>Unknown3(Comm3)</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>未知数据</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td rowspan="4"><p>Palette Info DB</p></td>
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>调色板名称</p></td>
<td></td>
</tr>
<tr class="even">
<td><p>Offset</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>调色板数据开头相对于第一个图像数据开头的偏移量</p></td>
<td><p>9E2A0000</p></td>
</tr>
<tr class="odd">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>调色板数据长度</p></td>
<td><p>00030000</p></td>
</tr>
<tr class="even">
<td><p>Count</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>调色板颜色数</p></td>
<td><p>00010000</p></td>
</tr>
<tr class="odd">
<td rowspan="12"><p>Data DA</p></td>
<td rowspan="6"><p>Image DB</p></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>该图像的行数据总长度</p></td>
<td><p>09070000</p></td>
</tr>
<tr class="odd">
<td><p>Offset</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图每一行在数据中相对于第一行开头的偏移量</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>Offset数量为Height</p></td>
</tr>
<tr class="odd">
<td><p>Line Data</p></td>
<td><p>Byte</p></td>
<td><p>不定</p></td>
<td><p>每一行的RLE数据</p></td>
<td></td>
</tr>
<tr class="even">
<td colspan="5"><p>Line Data数量为Height</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>Image DB数量为Image Count</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Palette DB</p></td>
<td><p>Red</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>红色通道</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td><p>Green</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>绿色通道</p></td>
<td><p>00</p></td>
</tr>
<tr class="even">
<td><p>Blue</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>蓝色通道</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>三元组数量为Count</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>有很多个Data DB</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是little-endian的。

<p>&nbsp;</p>

2、盟军2的GRL文件的RLE算法继承自盟军1的RLE文件的压缩算法，但是有所改变。

解压方法(1)：

定义一个数称之为已解压次数，该数每次读入数量并处理后递增1。

当已解压次数 Mod 4为0时，读入的数量为透明像素数（可为0），输出这么多个透明像素即可。

当已解压次数 Mod 4为2时，读入的数量为不透明像素数（可为0），输出紧接着的这么多个不透明像素的索引即可。

当已解压次数 Mod 4为1或3时，读入的数量为半透明像素数（可为0），输出紧接着的这么多个半透明像素的索引即可。（通常应作为不透明像素输出。）

\[例1\] 15 01 27 09 4A FF FF 04 36 04 D4 D4 D4 00 14 表示（用-1表示透明）

-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 27 4A FF FF 04 36 04 D4 D4 D4 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1

解压方法(2)：

透明模式和非透明模式交替。初始是透明模式。

读入一个数量，如果是透明模式，则输出这样数量的透明像素。

如果是非透明模式，则输出后面的这样数量的非透明元素。

\[例2\] 15 01 1C 02 01 57 04 01 3B 0A 02 4A 2C 12 表示

21个-1  1C -1 -1 57 -1 -1 -1 -1 3B  10个-1  4A 2C  18个-1

解压方法(3)：

透明模式和非透明模式交替。初始是透明模式。

读入一个数量，如果是透明模式，则输出这样数量的透明像素。

如果是非透明模式，则输出后面的这样数量的非透明元素，要输出一个跳过一个。

\[例3\] 95 02 E1 01 E0 01 03 01 DC 01 01 01 80 01 A3 表示

149个-1  E1 E0 -1 -1 -1 DC -1 80  163个-1

<p>&nbsp;</p>

3、行的排列顺序与一般BMP不同，是从上到下的。

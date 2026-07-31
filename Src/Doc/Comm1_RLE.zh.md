盟军敢死队1 RLE文件格式表

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
<td rowspan="15"><p>Info DB</p></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>2</p></td>
<td><p>标志符</p></td>
<td><p>424D(BM)</p></td>
</tr>
<tr class="even">
<td><p>File Size</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>文件大小</p></td>
<td><p>931F0000</p></td>
</tr>
<tr class="odd">
<td><p>Reserved</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>Bitmap Data Offset</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>从文件开始到位图数据开始之间的偏移量</p></td>
<td><p>FE050000</p></td>
</tr>
<tr class="odd">
<td><p>Bitmap Header Size</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图信息头的长度，固定为0x28</p></td>
<td><p>28000000</p></td>
</tr>
<tr class="even">
<td><p>Width</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图的宽度</p></td>
<td><p>70000000</p></td>
</tr>
<tr class="odd">
<td><p>Height</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图的高度</p></td>
<td><p>6F000000</p></td>
</tr>
<tr class="even">
<td><p>Planes</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>位图位面数，固定为1</p></td>
<td><p>0100</p></td>
</tr>
<tr class="odd">
<td><p>Bits Per Pixel</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>每个像素的位数，色深</p></td>
<td><p>0800</p></td>
</tr>
<tr class="even">
<td><p>Compression</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>压缩方法，固定为4</p></td>
<td><p>04000000</p></td>
</tr>
<tr class="odd">
<td><p>Bitmap Data Size</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>被忽略，固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>HResolution</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>水平分辨率，固定</p></td>
<td><p>120B0000</p></td>
</tr>
<tr class="odd">
<td><p>VResolution</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>垂直分辨率，固定</p></td>
<td><p>120B0000</p></td>
</tr>
<tr class="even">
<td><p>Colors</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图颜色数，固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>Important Colors</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>重要颜色数，固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>Palette DB</p></td>
<td><p>Color</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>蓝、绿、红各8位，高8位为0</p></td>
<td><p>0C040C00</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>数量为2^Bits Per Pixel</p></td>
</tr>
<tr class="even">
<td rowspan="5"><p>Address DB</p></td>
<td><p>Sign</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>为文件名称的前4个字母</p></td>
<td><p>626F746F(boto)</p></td>
</tr>
<tr class="odd">
<td><p>Width</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图的宽度</p></td>
<td><p>70000000</p></td>
</tr>
<tr class="even">
<td><p>Height</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图的高度</p></td>
<td><p>6F000000</p></td>
</tr>
<tr class="odd">
<td><p>Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图每一行在数据中的首地址</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>Address数量为Height</p></td>
</tr>
<tr class="odd">
<td rowspan="2"><p>Data DA</p></td>
<td><p>Line DB</p></td>
<td></td>
<td><p>Byte</p></td>
<td><p>不定</p></td>
<td><p>每一行的RLE数据</p></td>
<td></td>
</tr>
<tr class="even">
<td colspan="6"><p>有很多个Data DB</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是little-endian的。

2、示例文件为盟军1使命召唤的WAR_MP.DIR中的Datos\RECURSOS\BMPS\SYSTEM\CREDITOS\BOTO0000.RLE。

3、盟军1的RLE文件与BMP极其相似，只是用了一种与一般RLE算法有一点差别的压缩算法。

对于每一行数据，FF xx表示xx个透明像素，FE xx表示后面xx个字节为半透明像素，其他字节xx表示后面有xx个绝对像素。

\[例\] FF 03 FE 02 AC FF 03 EF 02 AE FE 01 2E FF 04 表示（用-1表示透明，\[\]表示半透明）

-1 -1 -1 \[AC\] \[FF\] EF 02 AE \[2E\] -1 -1 -1 -1

4、行的排列顺序与一般BMP不同，是从上到下的。

<p>&nbsp;</p>

参考：

\[1\]《多媒体技术基础及应用》6.1 BMP文件格式，林福宗，http://mti.xidian.edu.cn/multimedia/multi/course1-6-1.html

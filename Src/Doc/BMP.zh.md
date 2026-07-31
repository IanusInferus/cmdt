BMP文件格式表

地狱门神（F.R.C.）整理

概述：BMP文件大家都知道是什么东西，但是网上虽然有很多讲到BMP格式的文章，但都不是很全，所以我整理了这个比较全的BMP文件格式表，其中指出了一些不再使用的参数。

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
<td><p>每个像素的位数，色深<br />
1 - Monochrome bitmap<br />
4 - 16 color bitmap<br />
8 - 256 color bitmap<br />
16 - 16bit (high color) bitmap<br />
24 - 24bit (true color) bitmap<br />
32 - 32bit (true color) bitmap</p></td>
<td><p>0800</p></td>
</tr>
<tr class="even">
<td><p>Compression</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>压缩方法：<br />
0 - none (也使用BI_RGB表示)<br />
1 - RLE 8-bit / pixel (也使用BI_RLE4表示)<br />
2 - RLE 4-bit / pixel (也使用BI_RLE8表示)<br />
3 - Bitfields (也使用BI_BITFIELDS表示)</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>Bitmap Data Size</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图数据的字节数。必须是4的倍数</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>HResolution</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>水平分辨率，不用，固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>VResolution</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>垂直分辨率，不用，固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>Colors</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图颜色数，不用，固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>Important Colors</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>重要颜色数，不用，固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>Palette DB</p></td>
<td><p>Color</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>蓝、绿、红各8位，高8位当色深为32时为Alpha值，否则固定为0</p></td>
<td><p>0C040C00</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>当色深为1、4、8时才有，数量为2^Bits Per Pixel</p></td>
</tr>
<tr class="even">
<td rowspan="5"><p>Mask DB</p></td>
<td><p>Red Mask</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>红色的掩码</p></td>
<td><p>00F80000</p></td>
</tr>
<tr class="odd">
<td><p>Green Mask</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>绿色的掩码</p></td>
<td><p>E0070000</p></td>
</tr>
<tr class="even">
<td><p>Blue Mask</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>蓝色的掩码</p></td>
<td><p>1F000000</p></td>
</tr>
<tr class="odd">
<td><p>Reserved</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>固定为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>当色深为16时才有，此时Compression应为3</p></td>
</tr>
<tr class="odd">
<td><p>Data DA</p></td>
<td><p>Bitmap DB</p></td>
<td></td>
<td><p>Byte</p></td>
<td><p>不定</p></td>
<td><p>位图数据，大小与色深和压缩方法有关</p></td>
<td></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是little-endian的。

2、压缩方法：

① BI_RLE8：每个像素为8位的RLE压缩编码，可使用编码方式和绝对方式中的任何一种进行压缩，这两种方式可在同一幅图中的任何地方使用。

编码方式：由2个字节组成，第一个字节指定使用相同颜色的像素数目，第二个字节指定使用的颜色索引。此外，这个字节对中的第一个字节可设置为0，联合使用第二个字节的值表示：

- 第二个字节的值为0：行的结束。
- 第二个字节的值为1：图像结束。
- 第二个字节的值为2：其后的两个字节表示下一个像素从当前开始的水平和垂直位置的偏移量。

绝对方式：第一个字节设置为0，而第二个字节设置为0x03～0xFF之间的一个值。在这种方式中，第二个字节表示跟在这个字节后面的字节数，每个字节包含单个像素的颜色索引。压缩数据格式需要字边界(word boundary)对齐。

\[例1\] 用十六进制表示的8位压缩图像数据如下：

03 04 05 06 00 03 45 56 67 00 02 78 00 02 05 01 02 78 00 00 09 1E 00 01  
这些压缩数据可解释为 ：

<p>&nbsp;</p>

| 压缩数据          | 扩展数据                          |
|-------------------|-----------------------------------|
| 03 04             | 04 04 04                          |
| 05 06             | 06 06 06 06 06                    |
| 00 03 45 56 67 00 | 45 56 67                          |
| 02 78             | 78 78                             |
| 00 02 05 01       | 从当前位置右移5个位置后向下移一行 |
| 02 78             | 78 78                             |
| 00 00             | 行结束                            |
| 09 1E             | 1E 1E 1E 1E 1E 1E 1E 1E 1E        |
| 00 01             | RLE编码图像结束                   |

<p>&nbsp;</p>

② BI_RLE4：每个像素为4位的RLE压缩编码，同样也可使用编码方式和绝对方式中的任何一种进行压缩，这两种方式也可在同一幅图中的任何地方使用。这两种方式是：

编码方式：由2个字节组成，第一个字节指定像素数目，第二个字节包含两种颜色索引，一个在高4位，另一个在低4位。第一个像素使用高4位的颜色索引，第二个使用低4位的颜色索引，第3个使用高4位的颜色索引，依此类推。

绝对方式：这个字节对中的第一个字节设置为0，第二个字节包含有颜色索引数，其后续字节包含有颜色索引，颜色索引存放在该字节的高、低4位中，一个颜色索引对应一个像素。此外，BI_RLE4也同样联合使用第二个字节中的值表示：

- 第二个字节的值为0：行的结束。
- 第二个字节的值为1：图像结束。
- 第二个字节的值为2：其后的两个字节表示下一个像素从当前开始的水平和垂直位置的偏移量。

\[例2\] 用十六进制数表示的4位压缩图像数据：

03 04 05 06 00 06 45 56 67 00 04 78 00 02 05 01 04 78 00 00 09 1E 00 01

这些压缩数据可解释为 ：

<p>&nbsp;</p>

| 压缩数据          | 扩展数据                          |
|-------------------|-----------------------------------|
| 03 04             | 0 4 0                             |
| 05 06             | 0 6 0 6 0                         |
| 00 06 45 56 67 00 | 4 5 5 6 6 7                       |
| 04 78             | 7 8 7 8                           |
| 00 02 05 01       | 从当前位置右移5个位置后向下移一行 |
| 04 78             | 7 8 7 8                           |
| 00 00             | 行结束                            |
| 09 1E             | 1 E 1 E 1 E 1 E 1                 |
| 00 01             | RLE图像结束                       |

**③ BI_BITFIELDS：**这其实不是一种压缩算法，但是它描述了16位色深位图的存储格式，通常有两种，即红5位、绿6位、蓝5位的r5g6b5和红5位、绿5位、蓝5位的r5g5b5。它们的掩码如下表。

| 掩码 | r5g6b5   | r5g5b5   |
|------|----------|----------|
| 红色 | 00F80000 | 007C0000 |
| 绿色 | E0070000 | E0030000 |
| 蓝色 | 1F000000 | 1F000000 |

3、位图数据：

每个像素按照从下到上从左到右的顺序排列。

对于没有使用BI_RLE8或BI_RLE4的位图，每一行的数据字节数必须按4对齐，不足部分一般补0。

对于色深为1、4的位图，每个字节的高位表示的像素比低位靠左。对于色深为24的位图，每个象素的3个字节按蓝绿红的顺序排列。对于色深为32的位图，每个象素的4个字节按蓝绿红Alpha的顺序排列。

<p>&nbsp;</p>

参考：

\[1\]《多媒体技术基础及应用》6.1 BMP文件格式，林福宗，<http://mti.xidian.edu.cn/multimedia/multi/course1-6-1.html>

\[2\]BMP格式图像文件详析，Homects，<http://my.opera.com/homec/blog/show.dml/330587>

\[3\]BMP图象格式，<http://dev.gameres.com/Program/Visual/Other/BMPFormat.htm>

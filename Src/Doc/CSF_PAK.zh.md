盟军敢死队打击力量 PAK文件格式表

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
<td rowspan="10"><p>Header DA</p></td>
<td rowspan="5"><p>Info DB</p></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>3</p></td>
<td><p>标志符</p></td>
<td><p>"PAK"</p></td>
</tr>
<tr class="even">
<td><p>Type</p></td>
<td><p>Char</p></td>
<td><p>1</p></td>
<td><p>A为普通，C为压缩</p></td>
<td><p>"A","C"</p></td>
</tr>
<tr class="odd">
<td><p>Version</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>PS2原型版为3，DEMO版为4，正式版为5</p></td>
<td><p>04000000, 05000000</p></td>
</tr>
<tr class="even">
<td><p>Platform</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>PC版为1，PS2版为2，XBOX版为3</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>File Count</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>文件数</p></td>
<td><p>46010000</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>File DB</p></td>
<td><p>Path</p></td>
<td><p>String</p></td>
<td><p>不定</p></td>
<td><p>文件路径，以0结尾</p></td>
<td><p>"Gfx/Interfaz.txt", "MENUS/TEXTURES/RELOJ.RAW"</p></td>
</tr>
<tr class="odd">
<td><p>Offset</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>DEMO版为文件数据相对于第一个文件数据开始的偏移量，正式版为该偏移量+0xD</p></td>
<td><p>00000000, 0D000000</p></td>
</tr>
<tr class="even">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>原始文件长度</p></td>
<td><p>E5AC0000</p>
<p>, 08C00000</p></td>
</tr>
<tr class="odd">
<td><p>Timestamp</p></td>
<td><p>FILETIME</p></td>
<td><p>8</p></td>
<td><p>文件创建时间</p></td>
<td><p>2006-01-21 20:00:40 UTC</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>有很多个File DB</p></td>
</tr>
<tr class="odd">
<td rowspan="2"><p>Data DA</p></td>
<td><p>Data DB</p></td>
<td></td>
<td></td>
<td></td>
<td><p>文件数据</p></td>
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

2、文件路径中分隔符为“/”。

3、对于类型A，文件数据可直接得到。而对于类型C则不行。

类型C的每个Data DB结构如下：

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
<th><p>数据块</p></th>
<th><p>子数据块</p></th>
<th><p>数据</p></th>
<th><p>数据类型</p></th>
<th><p>长度</p></th>
<th><p>描述</p></th>
<th><p>样例数据</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="5"><p>Data DB</p></td>
<td rowspan="3"><p>Sub-Data DB</p></td>
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>数据长度</p></td>
<td><p>B6 00 00 00</p></td>
</tr>
<tr class="even">
<td><p>Original Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>该子数据块原始数据长度，除了最后一个子数据块，通常为4096</p></td>
<td><p>00 10 00 00</p></td>
</tr>
<tr class="odd">
<td><p>Compressed File Data</p></td>
<td><p>Byte</p></td>
<td><p>Block Length</p></td>
<td><p>Deflate压缩数据，可用Zlib解压</p></td>
<td><p>58 C3 6B 60 60 60 68 60 ……</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>...... 多个Sub-Data DB</p></td>
</tr>
<tr class="odd">
<td></td>
<td><p>Trailer</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>数据块结束，固定为0</p></td>
<td><p>00 00 00 00</p></td>
</tr>
</tbody>
</table>

具体结构参考\[2\]。该压缩算法即Zip所用之压缩算法。

<p>&nbsp;</p>

参考：

\[1\]Commandos Strike Force - XeNTaX

<a href="http://forum.xentax.com/viewtopic.php?t=1784&amp;postdays=0&amp;postorder=asc&amp;start=0&amp;sid=28fcec0d0cdf1c128c4a16a58ae2a1c8" style="text-decoration: none">http://forum.xentax.com/viewtopic.php?t=1784&amp;postdays=0&amp;postorder=asc&amp;start=0&amp;sid=28fcec0d0cdf1c128c4a16a58ae2a1c8</a>

\[2\]ZLIB Compressed Data Format Specification version 3.3

<a href="https://www.ietf.org/rfc/rfc1950.txt" style="text-decoration: none">https://www.ietf.org/rfc/rfc1950.txt</a>

<p>&nbsp;</p>

附录：

版本C的文件maps\Snipers.pak中前几个文件数据的记录，供以后的研究使用。

Path  
Address  
Length  
Header  
Trailer  
  
MENUS/TEXTURES/RELOJ.RAW  
62531  
49160  
B6 00 00 00 00 10 00 00 58 C3 6B 60 60 60 68  
00 08 00 01 00 00 00 00  
  
MENUS/TEXTURES/RELOJ1.RAW  
73595  
6083  
31 09 00 00 00 10 00 00 58 C3 95 D7 E9 73 96  
66 AF D7 0D 00 00 00 00  
  
MENUS/TEXTURES/RELOJ2.RAW  
77206  
6083  
4B 09 00 00 00 10 00 00 58 C3 5D D7 FD 73 55  
3F 55 C0 33 00 00 00 00  
  
MENUS/TEXTURES/RELOJ3.RAW  
80849  
6083  
2B 09 00 00 00 10 00 00 58 C3 95 D7 DB 53 96  
48 1F D7 61 00 00 00 00  
  
gfx/iconos.txt  
84465  
729  
28 01 00 00 D9 02 00 00 58 C3 73 0E 76 73 73  
D3 A6 B4 70 00 00 00 00  
  
Menus/Textures/Simbolos.png  
84773  
76319  
0B 10 00 00 00 10 00 00 58 C3 01 00 10 FF EF  
8B 3E 6F F2 00 00 00 00  
  
Gfx/Textures/VinylD_1.png  
160587  
8270  
08 10 00 00 00 10 00 00 58 C3 8D 96 E5 57 94  
93 C4 16 80 00 00 00 00  
  
Gfx/Textures/VinylD_2.png  
168812  
8771  
09 10 00 00 00 10 00 00 58 C3 8D 96 F9 37 D4  
15 6B 0E 3D 00 00 00 00  
  
Gfx/Textures/VinylD_3.png  
177557  
8731  
05 10 00 00 00 10 00 00 58 C3 8D 96 F7 3F D5  
EF B4 EA 26 00 00 00 00  
  
BDD/Anims.bdd  
186240  
262626  
0A 03 00 00 00 10 00 00 58 C3 A5 D6 7F 64 94  
48 64 8D 7F 00 00 00 00  
  
BDD/Objetos.bdd  
236437  
117052  
5D 04 00 00 00 10 00 00 58 C3 95 97 09 68 5C  
  
BDD/Armas.bdd  
269790  
95148

注意Menus/Textures/Simbolos.png的PNG头部是显现出来的

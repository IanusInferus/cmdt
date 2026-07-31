盟军敢死队2及3 H2O文件格式表

地狱门神（F.R.C.）根据旧资料和老顽童与ferdinand.graf.zeppelin@gmail.com提供的资料整理制作

<span style="color: blue">老顽童2013－01－29再次更新</span>

<table>
<colgroup>
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 6%" />
<col style="width: 6%" />
<col style="width: 6%" />
<col style="width: 6%" />
<col style="width: 30%" />
<col style="width: 25%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据区</p></th>
<th><p>数据块</p></th>
<th><p>子数据块</p></th>
<th><p>数据节</p></th>
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
<td rowspan="3"><p>Basic Info DB</p></td>
<td rowspan="3"></td>
<td rowspan="3"></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td><p>Num View</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>视角数</p></td>
<td><p>04000000</p></td>
</tr>
<tr class="odd">
<td><p>Num Texture</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">贴图数</span></p></td>
<td><p>01000000</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>View Address  DB</p></td>
<td></td>
<td></td>
<td><p>View Address 0</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>视角地址</p></td>
<td><p>18040100</p></td>
</tr>
<tr class="odd">
<td colspan="7"><p>一共Num View个View Address</p></td>
</tr>
<tr class="even">
<td rowspan="5"><p>Texture Table DB</p></td>
<td rowspan="2"><p>Index Table Sub-DB</p></td>
<td rowspan="2"><p>Index Table DS</p></td>
<td><p>Index</p></td>
<td><p>Byte</p></td>
<td><p>256*256</p></td>
<td><p>表示水的颜色所占的比例，最终颜色=(水的颜色*Alpha+背景颜色*(255-Alpha))/255</p></td>
<td><p>675F7062</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>水的8位Alpha位图，一共长65536</p></td>
</tr>
<tr class="even">
<td rowspan="3"><p>Palette Table Sub-DB</p></td>
<td rowspan="3"><p>Palette Table DB</p></td>
<td><p>Color</p></td>
<td><p>UInt16</p></td>
<td><p>2</p></td>
<td><p>r5g6b5颜色</p></td>
<td><p>8A3A</p></td>
</tr>
<tr class="odd">
<td><p>?</p></td>
<td><p>?</p></td>
<td><p>2</p></td>
<td><p>?</p></td>
<td><p>0000</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>一共长1024</p></td>
</tr>
<tr class="odd">
<td rowspan="51"><p>Main DA</p></td>
<td rowspan="51"><p>View Info DB</p></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td><p>Num Block</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">网格块数</span></p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td rowspan="47"><p>View Block DB</p></td>
<td rowspan="8"><p>Basic Info DS</p></td>
<td><p>DS01</p></td>
<td><p>String</p></td>
<td><p>8</p></td>
<td><p>贴图组名称，C风格字符串</p></td>
<td><p>"rio"</p></td>
</tr>
<tr class="even">
<td><p>DS02-1</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">网格点数</span></p></td>
<td><p>72020000</p></td>
</tr>
<tr class="odd">
<td><p>DS02-2</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">网格面数</span></p></td>
<td><p>4C040000</p></td>
</tr>
<tr class="even">
<td><p>DS03</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">网像素线总条数</span></p></td>
<td><p>94870000</p></td>
</tr>
<tr class="odd">
<td><p>DS04</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">点面连块数据长度</span></p></td>
<td><p>60A80100</p></td>
</tr>
<tr class="even">
<td><p>DS05</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">网格描述块行数</span></p></td>
<td><p>93030000</p></td>
</tr>
<tr class="odd">
<td><p>DS06</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">网格上方空行？</span></p></td>
<td><p>DF010000</p></td>
</tr>
<tr class="even">
<td><p>DS07</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">贴图索引？</span></p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td rowspan="10"><p>DS02-1</p>
<p><span style="color: blue">网格点</span></p>
<p><span style="color: blue">数据长40</span></p></td>
<td><p>X</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00008844</p></td>
</tr>
<tr class="even">
<td><p>Y</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>0000CCC2</p></td>
</tr>
<tr class="odd">
<td><p><span style="color: blue">U</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">贴图坐标U</span></p></td>
<td><p>00E08844</p></td>
</tr>
<tr class="even">
<td><p><span style="color: blue">V</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">贴图坐标V</span></p></td>
<td><p>00808843</p></td>
</tr>
<tr class="odd">
<td><p>Offset X</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">0</span><span style="color: blue">－9随机数</span></p></td>
<td><p>0000A040</p></td>
</tr>
<tr class="even">
<td><p>Offset Y</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">0</span><span style="color: blue">－9随机数</span></p></td>
<td><p>00008040</p></td>
</tr>
<tr class="odd">
<td><p>UNKNOWN</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>UNKNOWN</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN</p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>一共DS02-1个</p></td>
</tr>
<tr class="even">
<td rowspan="20"><p>DS02-2</p>
<p><span style="color: blue">网格面</span></p>
<p><span style="color: blue">长度76</span></p></td>
<td><p>Point A</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">点A索引数＊40</span></p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>Point B</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">点B索引数＊40</span></p></td>
<td><p>28000000</p></td>
</tr>
<tr class="even">
<td><p>Point C</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">点C索引数＊40</span></p></td>
<td><p>50000000</p></td>
</tr>
<tr class="odd">
<td><p>UNKNOWN1</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN2</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>UNKNOWN3</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN4</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>UNKNOWN5</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN6</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>UNKNOWN7</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN8</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p><span style="color: blue"> CX-AX</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> CX-AX</span></p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p><span style="color: blue"> CY-AY</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> CY-AY</span></p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p><span style="color: blue"> BX-AX</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> BX-AX</span></p></td>
<td><p>00001643</p></td>
</tr>
<tr class="even">
<td><p><span style="color: blue"> BY-AY</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> BY-AY</span></p></td>
<td><p>00001643</p></td>
</tr>
<tr class="odd">
<td><p><span style="color: blue"> CX-BX</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> CX-BX</span><span style="color: blue">， CY-BY为负则＊－1</span></p></td>
<td><p>00001643</p></td>
</tr>
<tr class="even">
<td><p><span style="color: blue"> CY-BY</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> CY-BY</span><span style="color: blue">为负则＊－1</span></p></td>
<td><p>000016C3</p></td>
</tr>
<tr class="odd">
<td><p><span style="color: blue">最大Y值</span></p></td>
<td><p><span style="color: blue">Int16</span></p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> 3</span><span style="color: blue">点最大Y值</span></p></td>
<td><p>0000</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN</p></td>
<td><p><span style="color: blue">Int16</span></p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">恒为－1</span></p></td>
<td><p>FFFF</p></td>
</tr>
<tr class="odd">
<td><p><span style="color: blue">正负开关</span></p></td>
<td><p>Float32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue"> 0</span><span style="color: blue">，CY-BY为负则为－1</span></p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>一共DS02-2个</p></td>
</tr>
<tr class="odd">
<td rowspan="2"><p>DS05</p>
<p><span style="color: blue">行，</span></p>
<p><span style="color: blue">从上到下</span></p></td>
<td><p>Offset</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">到DS03（线条）的偏移，从点数据处开始算</span></p>
<p><span style="color: blue">＝点面行数据总长度＋累计行＊每行线条数＊6</span></p></td>
<td><p>3C410000</p></td>
</tr>
<tr class="even">
<td><p>Num Segment</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p><span style="color: blue">一行内线条数</span></p></td>
<td><p>09000000</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>一共DS05个</p></td>
</tr>
<tr class="even">
<td rowspan="3"><p>DS03</p>
<p><span style="color: blue">线条，</span></p>
<p><span style="color: blue">面内水平行被网格分割成的线条，从左到右、从上到下排列</span></p></td>
<td><p><span style="color: blue">面索引</span></p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p><span style="color: blue">线条所在面索引</span></p></td>
<td><p>0100</p></td>
</tr>
<tr class="odd">
<td><p>UNKNOWN</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p><span style="color: blue">线左端X值</span></p></td>
<td><p>A604</p></td>
</tr>
<tr class="even">
<td><p>UNKNOWN</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p><span style="color: blue">线右端X值</span></p>
<p><span style="color: blue">（在面的边上，同一行内与下一线条左端X值相同）</span></p></td>
<td><p>D604</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>一共DS03个</p></td>
</tr>
<tr class="even">
<td colspan="7"><p>一共Num Block个</p></td>
</tr>
<tr class="odd">
<td colspan="7"><p>一共Num View个</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是little-endian的。

2、有“/”的项，盟军2的在左边，盟军3的在右边。

<p>&nbsp;</p>

参考：

\[1\]invox4C2_2auxfiles.doc，盗版钦差，2006

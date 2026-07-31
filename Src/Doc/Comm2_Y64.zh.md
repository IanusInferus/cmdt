盟军敢死队2及3 Y64文件格式表

地狱门神（F.R.C.）整理

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
<th><p>小数据块</p></th>
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
<td rowspan="17"><p>Header DA</p></td>
<td rowspan="7"><p>Identifying DB</p></td>
<td></td>
<td></td>
<td><p>Identifying Sign</p></td>
<td><p>String</p></td>
<td><p>6</p></td>
<td><p>固定</p></td>
<td><p>464344450000(FCDE)</p></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td><p>Version Sign</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>盟军版本。盟军2测试版是1，盟2是2，盟3是3或4。</p></td>
<td><p>0100,0200/0300,0400</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Unknown</p></td>
<td></td>
<td><p>4/5</p></td>
<td><p>盟2中占4字节，盟3中占5字节。</p></td>
<td><p>01000000/0100000000</p></td>
</tr>
<tr class="even">
<td></td>
<td><p>UnknownInt DS</p></td>
<td><p>(<span style="color: red">Comm3</span>)</p></td>
<td></td>
<td><p>32</p></td>
<td><p>8个未知整数。</p></td>
<td><p>48010000</p></td>
</tr>
<tr class="odd">
<td></td>
<td><p>UnknownFloat DS</p></td>
<td><p>(<span style="color: red">Comm3</span>)</p></td>
<td></td>
<td><p>32</p></td>
<td><p>8个未知浮点数。</p></td>
<td><p>6894F144</p></td>
</tr>
<tr class="even">
<td></td>
<td><p>Comment DS</p></td>
<td></td>
<td></td>
<td><p>32</p></td>
<td><p>注释："关卡名称 0"</p></td>
<td><p>"Nombre del Escenario 0"</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Start of Info DB for Angles of View</p></td>
<td></td>
<td><p>4</p></td>
<td><p>Info DB for Angles of View起始位置。</p></td>
<td><p>70010000/B1010000</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>Palette DB 0</p></td>
<td></td>
<td><p>Y DS 0</p></td>
<td></td>
<td></td>
<td><p>16</p></td>
<td rowspan="2"><p>固定。此块包含Y值。每一个数据节均是Y值的递增数列。</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... 16*16 = 总数256</p></td>
<td></td>
</tr>
<tr class="even">
<td><p>Palette DB 1</p></td>
<td></td>
<td><p>Cb DS</p></td>
<td></td>
<td></td>
<td><p>32</p></td>
<td><p>此块是Cb值的递增数列。</p></td>
<td></td>
</tr>
<tr class="odd">
<td><p>Palette DB 2</p></td>
<td></td>
<td><p>Cr DS</p></td>
<td></td>
<td></td>
<td><p>32</p></td>
<td><p>此块是Cr值的递增数列。</p></td>
<td></td>
</tr>
<tr class="even">
<td rowspan="6"><p>Info DB for Angles of View</p></td>
<td rowspan="2"></td>
<td rowspan="2"><p>Number DS</p></td>
<td><p>Number of Views</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了视角的数量。</p></td>
<td><p>04000000</p></td>
</tr>
<tr class="odd">
<td><p>Number of Mipmaps</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了每个视角的Mipmap图片数量。</p></td>
<td><p>02000000</p></td>
</tr>
<tr class="even">
<td rowspan="4"></td>
<td rowspan="4"><p>Address DS</p></td>
<td><p>Pic Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>View 00, Pic 00的起始地址。</p></td>
<td><p>98010000</p></td>
</tr>
<tr class="odd">
<td><p>Pic Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>View 00, Pic 01的起始地址。</p></td>
<td><p>A4210B00</p></td>
</tr>
<tr class="even">
<td><p>Pic Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>View 01, Pic 00的起始地址。</p></td>
<td><p>F01D1100</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... Pic Addresses: 总数由Number DS定义</p></td>
</tr>
<tr class="even">
<td rowspan="23"><p>Picture DA 0</p></td>
<td rowspan="5"><p>Info DB</p></td>
<td></td>
<td></td>
<td><p>Zoom Layer Number</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了此图片的缩放层。</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Pic Width</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图片宽度。</p></td>
<td><p>29040000</p></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td><p>Pic Height</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图片高度。</p></td>
<td><p>E1020000</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Number of Pic Block</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了此图片的图块数（包括附加图块，不包括空图块）。</p></td>
<td><p>CC000000</p></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td><p>Number of Extra Pic Block Index</p>
<p>(版本1中不存在)</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了附加图块的索引数。</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td rowspan="7"><p>Block Locating DB</p></td>
<td rowspan="2"><p>Index Table SDB</p></td>
<td rowspan="2"></td>
<td><p>Index/Address</p></td>
<td><p>UInt16</p>
<p>/</p>
<p>Int32</p></td>
<td><p>2/4</p></td>
<td><p>这是用于在图片上定位图块的索引表/地址表。0xFFFF表示空图块（如水）。</p>
<p>（0xFFFF mod 4 = 3，因此不可能为真实地址。）</p></td>
<td><p>0000,0100</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>...... Index: 总数由Pic Width和Pic Height定义</p></td>
</tr>
<tr class="odd">
<td rowspan="5"><p>Extra Index Table SDB</p></td>
<td rowspan="4"><p>Extra Index DS</p></td>
<td><p>Group Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图块组编号，用于区分不同的爆炸效果。</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>X Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图块级别的横坐标</p></td>
<td><p>09000000</p></td>
</tr>
<tr class="odd">
<td><p>Y Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图块级别的纵坐标</p></td>
<td><p>09000000</p></td>
</tr>
<tr class="even">
<td><p>Index/Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>图块索引/图块地址</p></td>
<td><p>59070000</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>...... Extra Index DS: 总数由Number of Extra Pic Block Index定义</p></td>
</tr>
<tr class="even">
<td rowspan="10"><p>Picture DB</p></td>
<td></td>
<td></td>
<td><p>Separator</p></td>
<td><p>Int64</p></td>
<td><p>8</p></td>
<td><p>通常为0xFFFFFFFFFFFFFFFF，但是某些时候变成了0xFFFFFFF8F8F8F8F8。</p></td>
<td><p>FFFFFFFFFFFFFFFF</p></td>
</tr>
<tr class="odd">
<td rowspan="7"><p>Picture SDB</p></td>
<td rowspan="3"><p>DS 0</p></td>
<td><p>Middle Digits</p></td>
<td></td>
<td><p>4bits</p></td>
<td><p>这些二进制位应该直接放在指针的位7至位4。但是首先你必须交换这两个字节，因为它们在Comm2.exe中被看作为一个little-endian的16位整数。这些位是到Palette Y的指针(7-0)的一部分。</p></td>
<td><p>1000.b</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>...... 4*4bits = 总数2 Bytes</p></td>
</tr>
<tr class="odd">
<td><p>Not Used</p></td>
<td></td>
<td><p>2</p></td>
<td><p>固定</p></td>
<td><p>0000</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>DS 1</p></td>
<td><p>High Digits</p></td>
<td></td>
<td><p>10bits</p></td>
<td><p>这些二进制位应该直接放在指针的位17至位8。但是首先你必须反转DS1的每4个字节，因为它们在Comm2.exe中被看作为一个32位整数。这些位是到Palette Cb的指针(17-13)和到Palette Cr的指针(12-8)。</p></td>
<td><p>1110101001.b</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... 16*10bits = 总共20 Bytes</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>DS 2</p></td>
<td><p>Low Digits</p></td>
<td></td>
<td><p>4bits</p></td>
<td><p>这些二进制位应该直接放在指针的位3至位0。但是首先你必须反转DS2的每4个字节，因为它们在Comm2.exe中被看作为一个32位整数。这些位是到Palette Y的指针(7-0)的一部分。</p></td>
<td><p>0000.b</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... 64*4bits = 总数32 Bytes</p></td>
</tr>
<tr class="even">
<td colspan="7"><p>...... 总共8*8个Picture SDB</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Nothing/Mask</p></td>
<td></td>
<td><p>0/不确定</p></td>
<td><p>由14FF、15FF、16FF三种。</p></td>
<td><p>14FF00000000,</p>
<p>TV.Y64:0001CD33h</p></td>
</tr>
<tr class="even">
<td colspan="8"><p>...... Picture DBs：总数由Number of Pic Block定义</p></td>
</tr>
<tr class="odd">
<td colspan="9"><p>...... Picture DAs: 总数由Number DS定义</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是little-endian的。

2、有“/”的项，盟军2的在左边，盟军3的在右边。

3、生成调色板时，从Palette DB 0,1,2的位置n0,n1,n2选取数据v0,v1,v2。

(v0,v1,v2) 是一个YCbCr空间中的颜色向量，你可以找到如下的转换公式：

R = Y + 1.402 \* (Cr - 128)  
G = Y - 0.34414 \* (Cb - 128) - 0.71414 \* (Cr - 128)  
B = Y + 1.772 \* (Cb - 128)

Y = 0.299 \* R + 0.587 \* G + 0.114 \* B  
Cb = -0.1687 \* R - 0.3313 \* G + 0.5 \* B + 128  
Cr = 0.5 \* R - 0.4187 \* G - 0.0813 \* B + 128

注意这样转换得到的值可能超出区间\[0,255\]，所以可能需要将超出的值替换成0或255。

调色板的索引（指针）为(n1 \<\< 13) Or (n2 \<\< 8) Or n0。

4、Mask的值

Mask数据是盟军3中的新增数据，用于代替盟军2中的MA2文件。

这部分的研究是由老顽童5099和herbert3000（zeppelin）完成的。

Mask数据中所有值均为little-endian的UInt16。

分为3种，分别由14 FF(65300)、15 FF(65301)、16 FF(65302)开头。

格式如下：

Mask ::=

\| 65300 zTop:UInt16 zBottom:UInt16

\| 65301 ZBlock

\| 65302 ZBlock HalfTransparentBlock

每个zTop, zBottom表示64\*64的一块16bit高度图，其中每行的每个高度z相同，且z为zTop和zBottom线性插值而来，即z = (zTop \* (64 - y) + zBottom \* y) / 64。

每个ZBlock表示64\*64的一块16bit高度图，HalfTransparentBlock表示64\*64的一块半透明轮廓图。

ZBlock ::= (ZAtom+){64}，其中ZAtom+压缩的总宽度为64的一行。

ZAtom ::=

\| Head(Head = 65100) z:UInt16 =\> z{64}

\| Head(Head \> 65100) <span style="color: #FF0000">n</span>:UInt16 z:UInt16 =\> z{Head - 65100}

\| Head(Head \> 65000) z:UInt16 =\> z{Head - 65000}

\| Head =\> Head

其中第二分支的n的含义<span style="color: #FF0000">不明</span>。

HalfTransparentBlock ::=

\| Head(Head \> 65000) =\> 0{Head - 65000}

\| Head =\> Head

注意所有的高度图和半透明边界图的像素的值均不大于65000。

<p>&nbsp;</p>

参考：

\[1\]invox4C2_3keyfiles.doc，盗版钦差，2006

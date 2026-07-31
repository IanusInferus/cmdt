Commandos2&3 Y64 File Structure Table

Arranged by F.R.C.

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
<th><p>Data Area</p></th>
<th><p>Data Block</p></th>
<th><p>Sub Data Block</p></th>
<th><p>Data Section</p></th>
<th><p>Data</p></th>
<th><p>Data Type</p></th>
<th><p>Length</p></th>
<th><p>Description</p></th>
<th><p>Sample Data</p></th>
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
<td><p>Fixed</p></td>
<td><p>464344450000(FCDE)</p></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td><p>Version Sign</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>Commandos Version. 1 in Comm2 demo, 2 in  Comm2, 3 or 4 in Comm3.</p></td>
<td><p>0100,0200/0300,0400</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Unknown</p></td>
<td></td>
<td><p>4/5</p></td>
<td><p>4 Bytes in Comm2, 5 Bytes in Comm3.</p></td>
<td><p>01000000/0100000000</p></td>
</tr>
<tr class="even">
<td></td>
<td><p>UnknownInt DS</p></td>
<td><p>(<span style="color: red">Comm3</span>)</p></td>
<td></td>
<td><p>32</p></td>
<td><p>8 unknown integers.</p></td>
<td><p>48010000</p></td>
</tr>
<tr class="odd">
<td></td>
<td><p>UnknownFloat DS</p></td>
<td><p>(<span style="color: red">Comm3</span>)</p></td>
<td></td>
<td><p>32</p></td>
<td><p>8 unknown floats.</p></td>
<td><p>6894F144</p></td>
</tr>
<tr class="even">
<td></td>
<td><p>Comment DS</p></td>
<td></td>
<td></td>
<td><p>32</p></td>
<td><p>Comment: "Name of the Scenario 0"</p></td>
<td><p>"Nombre del Escenario 0"</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Start of Info DB for Angles of View</p></td>
<td></td>
<td><p>4</p></td>
<td><p>Start of Info DB for Angles of View.</p></td>
<td><p>70010000/B1010000</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>Palette DB 0</p></td>
<td></td>
<td><p>Y DS 0</p></td>
<td></td>
<td></td>
<td><p>16</p></td>
<td rowspan="2"><p>Fixed. This block contains Y values. Each DS is formed by a increasing sequence of Y values.</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... 16*16 = 256 in total</p></td>
<td></td>
</tr>
<tr class="even">
<td><p>Palette DB 1</p></td>
<td></td>
<td><p>Cb DS</p></td>
<td></td>
<td></td>
<td><p>32</p></td>
<td><p>This block is formed by a increasing sequence of Cb values.</p></td>
<td></td>
</tr>
<tr class="odd">
<td><p>Palette DB 2</p></td>
<td></td>
<td><p>Cr DS</p></td>
<td></td>
<td></td>
<td><p>32</p></td>
<td><p>This block is formed by a increasing sequence of Cr values.</p></td>
<td></td>
</tr>
<tr class="even">
<td rowspan="6"><p>Info DB for Angles of View</p></td>
<td rowspan="2"></td>
<td rowspan="2"><p>Number DS</p></td>
<td><p>Number of Views</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This indicates the number of angles of view.</p></td>
<td><p>04000000</p></td>
</tr>
<tr class="odd">
<td><p>Number of Mipmaps</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This indicates the number of mipmap pictures of each view.</p></td>
<td><p>02000000</p></td>
</tr>
<tr class="even">
<td rowspan="4"></td>
<td rowspan="4"><p>Address DS</p></td>
<td><p>Pic Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The starting address of  View 00, Pic 00</p></td>
<td><p>98010000</p></td>
</tr>
<tr class="odd">
<td><p>Pic Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The starting address of  View 00, Pic 01</p></td>
<td><p>A4210B00</p></td>
</tr>
<tr class="even">
<td><p>Pic Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The starting address of  View 01, Pic 00</p></td>
<td><p>F01D1100</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... Pic Addresses: total number is defined by Number DS</p></td>
</tr>
<tr class="even">
<td rowspan="23"><p>Picture DA 0</p></td>
<td rowspan="5"><p>Info DB</p></td>
<td></td>
<td></td>
<td><p>Zoom Layer Number</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This indicates the layer number in zooming.</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Pic Width</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Width of the picture.</p></td>
<td><p>29040000</p></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td><p>Pic Height</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Height of the picture.</p></td>
<td><p>E1020000</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Number of Pic Block</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This indicates the number of Picture DBs(including  extras, excluding nulls).</p></td>
<td><p>CC000000</p></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td><p>Number of Extra Pic Block Index</p>
<p>(Not exist in Version 1)</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This indicates the numbers of extra Picture DB index.</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td rowspan="7"><p>Block Locating DB</p></td>
<td rowspan="2"><p>Index Table SDB</p></td>
<td rowspan="2"></td>
<td><p>Index / Address</p></td>
<td><p>UInt16</p>
<p>/</p>
<p>Int32</p></td>
<td><p>2</p></td>
<td><p>This is the index/address table to locate Picture DBs on the map. 0xFFFF stands for a null block filled with things such as water.</p>
<p>(0xFFFF can't be a real address as 0xFFFF mod 4 = 3.)</p></td>
<td><p>0000,0100</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>...... Index: total number is defined by Pic Width and Pic Height</p></td>
</tr>
<tr class="odd">
<td rowspan="5"><p>Extra Index Table SDB</p></td>
<td rowspan="4"><p>Extra Index DS</p></td>
<td><p>Group Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The block group index, which can distinguish different explosion effects.</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>X Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The horizontal index in Picture DB's size.</p></td>
<td><p>09000000</p></td>
</tr>
<tr class="odd">
<td><p>Y Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The vertical index in Picture DB's size.</p></td>
<td><p>09000000</p></td>
</tr>
<tr class="even">
<td><p>Index / Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The Picture DB's index/address.</p></td>
<td><p>59070000</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>...... Extra Index DS: total number is defined by Number of Extra Pic Block Index</p></td>
</tr>
<tr class="even">
<td rowspan="10"><p>Picture DB</p></td>
<td></td>
<td></td>
<td><p>Separator</p></td>
<td><p>Int64</p></td>
<td><p>8</p></td>
<td><p>Usually 0xFFFFFFFFFFFFFFFF, but sometimes it turns to be 0xFFFFFFF8F8F8F8F8.</p></td>
<td><p>FFFFFFFFFFFFFFFF</p></td>
</tr>
<tr class="odd">
<td rowspan="7"><p>Picture SDB</p></td>
<td rowspan="3"><p>DS 0</p></td>
<td><p>Middle Digits</p></td>
<td></td>
<td><p>4bits</p></td>
<td><p>To generate a pointer, these bits should be put directly in Digit 7 to Digit 4. But first you should swap the two bytes of the DS0 as they are a int16 in little-endian. They are part of the pointers to Palette Y(7-0).</p></td>
<td><p>1000.b</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>...... 4*4bits = 2 Bytes in total</p></td>
</tr>
<tr class="odd">
<td><p>Unknown Sign</p></td>
<td></td>
<td><p>2</p></td>
<td><p>Fixed</p></td>
<td><p>0000</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>DS 1</p></td>
<td><p>High Digits</p></td>
<td></td>
<td><p>10bits</p></td>
<td><p>To generate a pointer, these bits should be put directly in Digit 17 to Digit 8. But first you should reverse each 4 bytes of the DS1 as they are considered to be a Int32 in Comm2.exe. They are the pointers to Palette Cb(17-13) and Cr(12-8).</p></td>
<td><p>1110101001.b</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... 16*10bits = 20 Bytes in total</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>DS 2</p></td>
<td><p>Low Digits</p></td>
<td></td>
<td><p>4bits</p></td>
<td><p>To generate a pointer, these bits should be put directly in Digit 3 to Digit 0. But first you should reverse each 4 bytes of the DS2 as they are considered to be a Int32 in Comm2.exe.They are part of the pointers to Palette Y(7-0).</p></td>
<td><p>0000.b</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>...... 64*4bits = 32 Bytes in total</p></td>
</tr>
<tr class="even">
<td colspan="7"><p>...... 8*8 Picture SDBs in total</p></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td><p>Nothing/Mask</p></td>
<td></td>
<td><p>0/</p>
<p>Uncertain</p></td>
<td><p>Data starts with 14FF, 15FF or 16FF.</p></td>
<td><p>14FF00000000, TV.Y64:0001CD33h</p></td>
</tr>
<tr class="even">
<td colspan="8"><p>...... Picture DBs: total number is defined by Number of Pic Block</p></td>
</tr>
<tr class="odd">
<td colspan="9"><p>...... Picture DAs: total number is defined by Number DS</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Notice:

1\. All numerical data types are in little-endian.

2.In the items with "/", names in the right is for Commandos 2, and names in the right is for Commandos 3.

3.When generating the Palette, select v0,v1,v2 from position n0,n1,n2 in Palette DB 0,1,2.

(v0,v1,v2) is a color vector in the YCbCr space, you can find the conversion formula as follows:

R = Y + 1.402 \* (Cr - 128)  
G = Y - 0.34414 \* (Cb - 128) - 0.71414 \* (Cr - 128)  
B = Y + 1.772 \* (Cb - 128)

Y = 0.299 \* R + 0.587 \* G + 0.114 \* B  
Cb = -0.1687 \* R - 0.3313 \* G + 0.5 \* B + 128  
Cr = 0.5 \* R - 0.4187 \* G - 0.0813 \* B + 128

Notice that these values may be out of the range\[0,255\], so you may need to replace some of them with 0 or 255.

The index(pointer) of the color in the Palette is (n1 \<\< 13) Or (n2 \<\< 8) Or n0.

4.The values of Mask

The Mask data is added in Comm3 to replace the MA2 file in Comm2.

This part of research is completed by 老顽童5099 and herbert3000 (zeppelin).

All data in Mask are of little-endian UInt16.

There are three kinds of Mask data, that begins with 14 FF (65300), 15 FF (65301), 16 FF (65302) each.

The format is:

Mask ::=

\| 65300 zTop:UInt16 zBottom:UInt16

\| 65301 ZBlock

\| 65302 ZBlock HalfTransparentBlock

Every zTop and zBottom denotes a 64\*64 altitude map of 16bit, that every line is of the same altitude z, that z is the linear interpolation of zTop and zBottom, that is z = (zTop \* (64 - y) + zBottom \* y) / 64.

Every ZBlock denotes a 64\*64 altitude map of 16bit, and every HalfTransparentBlock denotes a 64\*64 half-transparent contour map of 16bit.

ZBlock ::= (ZAtom+){64}, where ZAtom+ is a compression of 64 pixel in a line.

ZAtom ::=

\| Head(Head = 65100) z:UInt16 =\> z{64}

\| Head(Head \> 65100) <span style="color: #FF0000">n</span>:UInt16 z:UInt16 =\> z{Head - 65100}

\| Head(Head \> 65000) z:UInt16 =\> z{Head - 65000}

\| Head =\> Head

and the meaning of n in the second alternative is <span style="color: #FF0000">unknown</span>.

HalfTransparentBlock ::=

\| Head(Head \> 65000) =\> 0{Head - 65000}

\| Head =\> Head

It shall be noticed that every pixel's value in the altitude maps and the half-transparent contour maps are no more than 65000.

<p>&nbsp;</p>

Reference:

\[1\]invox4C2_3keyfiles.doc, invox, 2006

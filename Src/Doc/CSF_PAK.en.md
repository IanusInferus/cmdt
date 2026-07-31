Commandos Strike Force PAK File Structure Table

written by F.R.C.

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
<th><p>Data Area</p></th>
<th><p>Data Block</p></th>
<th><p>Data</p></th>
<th><p>Data Type</p></th>
<th><p>Length</p></th>
<th><p>Description</p></th>
<th><p>Sample Data</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="10"><p>Header DA</p></td>
<td rowspan="5"><p>Info DB</p></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>3</p></td>
<td><p>Identifier</p></td>
<td><p>"PAK"</p></td>
</tr>
<tr class="even">
<td><p>Type</p></td>
<td><p>Char</p></td>
<td><p>1</p></td>
<td><p>A for normal, C for compressed</p></td>
<td><p>"A","C"</p></td>
</tr>
<tr class="odd">
<td><p>Version</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>3 in PS2 prototype version, 4 in DEMO version, 5 in full version</p></td>
<td><p>04000000, 05000000</p></td>
</tr>
<tr class="even">
<td><p>Platform</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>1 on PC, 2 on PS2, 3 on XBOX</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>File Count</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>File Count</p></td>
<td><p>46010000</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>File DB</p></td>
<td><p>Path</p></td>
<td><p>String</p></td>
<td><p>Varies</p></td>
<td><p>File Path, terminated by 0</p></td>
<td><p>"Gfx/Interfaz.txt", "MENUS/TEXTURES/RELOJ.RAW"</p></td>
</tr>
<tr class="odd">
<td><p>Offset</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The offset of File Data relative to first File Data in DEMO version, and the offset +0xD</p>
<p>in full version</p></td>
<td><p>00000000, 0D000000</p></td>
</tr>
<tr class="even">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Original File Length</p></td>
<td><p>E5AC0000, 08C00000</p></td>
</tr>
<tr class="odd">
<td><p>Timestamp</p></td>
<td><p>FILETIME</p></td>
<td><p>8</p></td>
<td><p>The date and time when the file was created</p></td>
<td><p>2006-01-21 20:00:40 UTC</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>Many File DB</p>
<p>s</p></td>
</tr>
<tr class="odd">
<td rowspan="2"><p>Data DA</p></td>
<td><p>Data DB</p></td>
<td></td>
<td></td>
<td></td>
<td><p>File Data</p></td>
<td></td>
</tr>
<tr class="even">
<td colspan="6"><p>Many Data DB</p>
<p>s</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Notice:

1\. All numerical data types are in little-endian.

2\. File Path separator is "/".

3\. For type A, file data is directly accessible. But for type C, it's compressed.

For each Data DB structure in type C:

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
<th><p>Data Block</p></th>
<th><p>Sub-Data Block</p></th>
<th><p>Data</p></th>
<th><p>Data Type</p></th>
<th><p>Length</p></th>
<th><p>Description</p></th>
<th><p>Sample Data</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="5"><p>Data DB</p></td>
<td rowspan="3"><p>Sub-Data DB</p></td>
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Data Length</p></td>
<td><p>B6 00 00 00</p></td>
</tr>
<tr class="even">
<td><p>Original Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Original Length of data in Sub-Data DB, except the last Sub-Data DB, it is usually 4096</p></td>
<td><p>00 10 00 00</p></td>
</tr>
<tr class="odd">
<td><p>Compressed File Data</p></td>
<td><p>Byte</p></td>
<td><p>Block Length</p></td>
<td><p>Deflate compressed data, can be extracted with Zlib</p></td>
<td><p>58 C3 6B 60 60 60 68 60 ……</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>......  Many Sub-Data DB</p></td>
</tr>
<tr class="odd">
<td></td>
<td><p>Trailer</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Data DB end, fixed to 0</p></td>
<td><p>00 00 00 00</p></td>
</tr>
</tbody>
</table>

Refer to \[2\] for its microstructure. The compression method is the one used by Zip.

<p>&nbsp;</p>

Reference:

\[1\]Commandos Strike Force - XeNTaX

<http://forum.xentax.com/viewtopic.php?t=1784&postdays=0&postorder=asc&start=0&sid=28fcec0d0cdf1c128c4a16a58ae2a1c8>

\[2\]ZLIB Compressed Data Format Specification version 3.3

<a href="https://www.ietf.org/rfc/rfc1950.txt" style="font-family: Times New Roman; text-decoration: none">https://www.ietf.org/rfc/rfc1950.txt</a>

<p>&nbsp;</p>

Annex:

The first a few file data records in the package of version C: maps\\Snipers.pak, which may be  used for further research.

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

Notice that Menus/Textures/Simbolos.png's PNG header is visible.

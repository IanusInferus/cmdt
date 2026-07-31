Commandos2&3 PCK File Structure Table

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
<td rowspan="5"><p>Header DA</p></td>
<td rowspan="4"><p>File DB</p></td>
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>36</p></td>
<td><p>File Name</p></td>
<td><p>44415441(DATA)</p></td>
</tr>
<tr class="even">
<td><p>Type</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>0 for files, 1 for directories,  0xFF</p>
<p>for directory tails</p></td>
<td><p>00000000/01000000/FF000000</p></td>
</tr>
<tr class="odd">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>0xFFFFFFFF</p>
<p>for directories</p></td>
<td><p>DA2E0000</p></td>
</tr>
<tr class="even">
<td><p>Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>File data address. for directories, it's the address of the first File DB in the directory</p></td>
<td><p>00A00300</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>There's many File DBs. For directories, they are matched by the directories' File DBs and their tail File DBs where File DB and Name is null, Type is 0xFF and Length and Address is 0xFFFFFFFF</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>Data DA</p></td>
<td><p>Data DB</p></td>
<td></td>
<td></td>
<td></td>
<td><p>File data, aligned with the unit of 2048 bytes. This part in Comm3 is encoded with a simple XOR method.</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="6"><p>There's many Data DB</p>
<p>s</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Notice:

1\. All numerical data types are in little-endian.

2.The directory File DB match pattern: directories and directory tails match in a way like the multiple brackets, which is different from DIR file of Comm1, namely:

    DATA{
        PARGLOBAL.DAT
        VAR.DAT
        ANIMS{
            ACTIVADOR.AN2
            ...
            ABI{
                ...
            }
        }
        ...
    }

3.The XOR method of Comm3:

Comm3 Xor a key for every first byte of every 16 bytes in each file. The first key for each file is calculated as follows:

The 4 high digits is the second hex number in the right of the offset:

high = ((address mod 16) \>\> 4) and 15

The 4 low digits is:

x = address mod 7

low = x \* 4 + floor(x / 2)

key = (high \<\< 4) or low

Then every key is the result of the previous key added by 1 for high and low digits respectively. And the low digits should be added an extra 1 if it's 7 or 0xF. And we assume that for either high or low digits, 0xF + 1 = 0.

high := (high + 1) mod 16

low := ((low + 1) + 1{(low + 1) mod 8 = 7}) mod 16

Then Xor the first byte of each line with the key, you'll get the original file.

<p>&nbsp;</p>

Appendix:

The analysis process for the XOR method in Comm3

Take DATA.PCK as example.

Comm3 did some XOR stuff to the first byte of each line. (We come out in the first place.)

Suppose the key is x, the original byte is s, the coded byte is t, then

t = s Xor x

s = t Xor x

So we can use

x = s Xor t

to get the key.

First,

| Offset    | Guessed original bytes | Coded bytes | Keys |
|-----------|------------------------|-------------|------|
| 00026000h | 5B                     | 56          | 0D   |
| 00026010h | 20                     | 3E          | 1E   |
| 000260B0h | 20                     | 9A          | BA   |
| 0002A000h | 42                     | 42          | 00   |
| 0002A800h | 42                     | 40          | 02   |
| 0002AAD0h | 74                     | A1          | D5   |
| 0002AAE0h | 00                     | E6          | E6   |
| 0002AAF0h | 61                     | 99          | F8   |
| 0002AB00h | 72                     | 7B          | 09   |
| 0002AB10h | 2E                     | 34          | 1A   |
| 0002AB20h | 6F                     | 44          | 2B   |
| 0002AB30h | 35                     | 09          | 3C   |
| 0002AB40h | 74                     | 39          | 4D   |
| 0002AB50h | 66                     | 38          | 5E   |
| 0002AB60h | 61                     | 01          | 60   |
| 0002AB70h | 61                     | 10          | 71   |
| 0002AB80h | 62                     | E0          | 82   |
|           |                        |             |      |
| 00026000h | 5B                     | 56          | 0D   |
| 00026010h | 20                     | 3E          | 1E   |
| 00026020h | 4F                     | 6F          | 20   |
| 00026030h | 09                     | 38          | 31   |
| 00026040h | 5F                     | 1D          | 42   |
| 00026050h | 4F                     | 1C          | 53   |
| 00026060h | 20                     | 44          | 64   |
| 00026070h | 50                     | 25          | 75   |
| 00026080h | 20                     | A6          | 86   |
| 00026090h | 4E                     | D6          | 98   |
| 000260A0h | 28                     | 89          | A9   |
|           |                        |             |      |
| 00029800h | 20                     | 2E          | 0D   |
| 00029810h | 2D                     | 33          | 1E   |
| 00029820h | 2D                     | 0D          | 20   |
| 00029830h | 2D                     | 1C          | 31   |
| 00029840h | 2D                     | 6F          | 42   |
| 00029850h | 2D                     | 7E          | 53   |
|           |                        |             |      |
| 0002A000h | 42                     | 42          | 00   |
| 0002A800h | 42                     | 40          | 02   |
| 0002B000h | 42                     | 46          | 04   |

We can know that the high digits is the second hex number in the right of the offset. And every key is the result of the previous key added by 1 for high and low digits respectively. And the low digits should be added an extra 1 if it's 7 or 0xF. And we assume that for either high or low digits, 0xF + 1 = 0.

And for the low digits of the key for the first line of each file, we get as follows:

The key is highly suspected to be involved with the file address or the file length.

There are many files started with "BSMB". (We know it because we can find many in the Comm2 files.) We find out all files with the start of "\*SMB", and the we assume the "\*" is "B", then we get the key (\* Xor 42<sub>16</sub>). Sort it, and we get the first two columns of the following table.

We can then find out there's only 7 kinds of key. So we do mod 7 for each file address and length, and get the following table. Then we can know the address mod 7 is exactly mapped one on one, namely

0-0 1-4 2-9 3-D 4-2 5-6 6-B

Notice that the differences of adjacent values are either 4 or 5 (mod 16), we know the mapping would be:

f(x) = x \* 4 + floor(x / 2)

Thus we finished the analysis.

| Offset    | Key | File Address mod 7 | File Address mod 7 |
|-----------|-----|--------------------|--------------------|
| 0002A000h | 00  | 00                 | 05                 |
| 00042800h | 00  | 00                 | 06                 |
| 00049800h | 00  | 00                 | 05                 |
| 0004D000h | 00  | 00                 | 01                 |
| 00050800h | 00  | 00                 | 02                 |
| 00054000h | 00  | 00                 | 03                 |
| 00057800h | 00  | 00                 | 04                 |
| 0005B000h | 00  | 00                 | 02                 |
| 00062000h | 00  | 00                 | 04                 |
| 00065800h | 00  | 00                 | 02                 |
| 00069000h | 00  | 00                 | 00                 |
| 0007E000h | 00  | 00                 | 02                 |
| 00081800h | 00  | 00                 | 02                 |
| 0009D800h | 00  | 00                 | 02                 |
| 000AF000h | 00  | 00                 | 06                 |
| 000B2800h | 00  | 00                 | 04                 |
| 000B6000h | 00  | 00                 | 02                 |
| 000B9800h | 00  | 00                 | 06                 |
| 000BD000h | 00  | 00                 | 03                 |
| 000C0800h | 00  | 00                 | 06                 |
| 000C4000h | 00  | 00                 | 03                 |
| 000D5800h | 00  | 00                 | 01                 |
| 000D9000h | 00  | 00                 | 03                 |
| 000DC800h | 00  | 00                 | 00                 |
| 000E0000h | 00  | 00                 | 02                 |
| 000E3800h | 00  | 00                 | 03                 |
| 000E7000h | 00  | 00                 | 00                 |
| 000EA800h | 00  | 00                 | 05                 |
| 000EE000h | 00  | 00                 | 01                 |
| 000FF800h | 00  | 00                 | 05                 |
| 0010A000h | 00  | 00                 | 05                 |
| 0010D800h | 00  | 00                 | 06                 |
| 00111000h | 00  | 00                 | 06                 |
| 00114800h | 00  | 00                 | 00                 |
| 00118000h | 00  | 00                 | 02                 |
| 0011B800h | 00  | 00                 | 00                 |
| 0011F000h | 00  | 00                 | 00                 |
| 00145800h | 00  | 00                 | 03                 |
| 02418000h | 00  | 00                 | 02                 |
| 0241F000h | 00  | 00                 | 03                 |
| 02429800h | 00  | 00                 | 00                 |
| 03C39800h | 00  | 00                 | 04                 |
| 08E76000h | 00  | 00                 | 04                 |
| 08ECA000h | 00  | 00                 | 03                 |
| 09771800h | 00  | 00                 | 03                 |
| 09FF6000h | 00  | 00                 | 00                 |
| 0BBE1000h | 00  | 00                 | 00                 |
| 0D9FC000h | 00  | 00                 | 03                 |
| 1CAD7800h | 00  | 00                 | 04                 |
| 1CAF0000h | 00  | 00                 | 04                 |
| 0002A800h | 02  | 04                 | 03                 |
| 0003F800h | 02  | 04                 | 00                 |
| 00046800h | 02  | 04                 | 05                 |
| 0004A000h | 02  | 04                 | 05                 |
| 0004D800h | 02  | 04                 | 01                 |
| 00051000h | 02  | 04                 | 04                 |
| 00054800h | 02  | 04                 | 00                 |
| 00058000h | 02  | 04                 | 01                 |
| 0005B800h | 02  | 04                 | 01                 |
| 00062800h | 02  | 04                 | 00                 |
| 00066000h | 02  | 04                 | 05                 |
| 0007E800h | 02  | 04                 | 02                 |
| 00082000h | 02  | 04                 | 01                 |
| 0009E000h | 02  | 04                 | 01                 |
| 000AF800h | 02  | 04                 | 01                 |
| 000B3000h | 02  | 04                 | 04                 |
| 000B6800h | 02  | 04                 | 02                 |
| 000BA000h | 02  | 04                 | 06                 |
| 000BD800h | 02  | 04                 | 00                 |
| 000C1000h | 02  | 04                 | 01                 |
| 000C4800h | 02  | 04                 | 04                 |
| 000D6000h | 02  | 04                 | 06                 |
| 000D9800h | 02  | 04                 | 05                 |
| 000DD000h | 02  | 04                 | 04                 |
| 000E0800h | 02  | 04                 | 03                 |
| 000E4000h | 02  | 04                 | 06                 |
| 000E7800h | 02  | 04                 | 02                 |
| 000EB000h | 02  | 04                 | 01                 |
| 000EE800h | 02  | 04                 | 02                 |
| 00100000h | 02  | 04                 | 01                 |
| 00103800h | 02  | 04                 | 05                 |
| 00107000h | 02  | 04                 | 01                 |
| 0010A800h | 02  | 04                 | 03                 |
| 0010E000h | 02  | 04                 | 01                 |
| 00111800h | 02  | 04                 | 01                 |
| 00115000h | 02  | 04                 | 02                 |
| 00118800h | 02  | 04                 | 02                 |
| 0011C000h | 02  | 04                 | 00                 |
| 02423000h | 02  | 04                 | 01                 |
| 02426800h | 02  | 04                 | 01                 |
| 08490800h | 02  | 04                 | 06                 |
| 08E81000h | 02  | 04                 | 02                 |
| 097A3000h | 02  | 04                 | 04                 |
| 0CD73000h | 02  | 04                 | 01                 |
| 0E1CB000h | 02  | 04                 | 03                 |
| 0E59B800h | 02  | 04                 | 01                 |
| 0E5A6000h | 02  | 04                 | 03                 |
| 0002B000h | 04  | 01                 | 00                 |
| 0003C800h | 04  | 01                 | 03                 |
| 00043800h | 04  | 01                 | 06                 |
| 00047000h | 04  | 01                 | 05                 |
| 0004A800h | 04  | 01                 | 06                 |
| 0004E000h | 04  | 01                 | 01                 |
| 00051800h | 04  | 01                 | 03                 |
| 00055000h | 04  | 01                 | 04                 |
| 00058800h | 04  | 01                 | 06                 |
| 0005C000h | 04  | 01                 | 01                 |
| 00063000h | 04  | 01                 | 04                 |
| 00066800h | 04  | 01                 | 04                 |
| 0007B800h | 04  | 01                 | 06                 |
| 0007F000h | 04  | 01                 | 03                 |
| 0009E800h | 04  | 01                 | 05                 |
| 000B0000h | 04  | 01                 | 06                 |
| 000B3800h | 04  | 01                 | 05                 |
| 000B7000h | 04  | 01                 | 02                 |
| 000BA800h | 04  | 01                 | 03                 |
| 000BE000h | 04  | 01                 | 02                 |
| 000C1800h | 04  | 01                 | 00                 |
| 000D6800h | 04  | 01                 | 04                 |
| 000DA000h | 04  | 01                 | 06                 |
| 000E1000h | 04  | 01                 | 04                 |
| 000E4800h | 04  | 01                 | 00                 |
| 000E8000h | 04  | 01                 | 06                 |
| 000EB800h | 04  | 01                 | 03                 |
| 000EF000h | 04  | 01                 | 01                 |
| 00100800h | 04  | 01                 | 06                 |
| 00104000h | 04  | 01                 | 01                 |
| 00107800h | 04  | 01                 | 01                 |
| 0010B000h | 04  | 01                 | 04                 |
| 0010E800h | 04  | 01                 | 00                 |
| 00112000h | 04  | 01                 | 04                 |
| 00115800h | 04  | 01                 | 02                 |
| 00119000h | 04  | 01                 | 06                 |
| 0011C800h | 04  | 01                 | 00                 |
| 02415800h | 04  | 01                 | 01                 |
| 02419000h | 04  | 01                 | 01                 |
| 0241C800h | 04  | 01                 | 00                 |
| 02423800h | 04  | 01                 | 06                 |
| 02427000h | 04  | 01                 | 00                 |
| 07D60000h | 04  | 01                 | 00                 |
| 07D6E000h | 04  | 01                 | 05                 |
| 084A6000h | 04  | 01                 | 02                 |
| 097B8800h | 04  | 01                 | 01                 |
| 0AFB7000h | 04  | 01                 | 03                 |
| 0ED59000h | 04  | 01                 | 06                 |
| 0002B800h | 06  | 05                 | 06                 |
| 0003D000h | 06  | 05                 | 03                 |
| 00047800h | 06  | 05                 | 03                 |
| 0004B000h | 06  | 05                 | 00                 |
| 0004E800h | 06  | 05                 | 02                 |
| 00052000h | 06  | 05                 | 01                 |
| 00055800h | 06  | 05                 | 01                 |
| 00059000h | 06  | 05                 | 01                 |
| 0005C800h | 06  | 05                 | 06                 |
| 00063800h | 06  | 05                 | 04                 |
| 00067000h | 06  | 05                 | 02                 |
| 0007C000h | 06  | 05                 | 00                 |
| 0007F800h | 06  | 05                 | 04                 |
| 00083000h | 06  | 05                 | 01                 |
| 000AD000h | 06  | 05                 | 01                 |
| 000B0800h | 06  | 05                 | 05                 |
| 000B4000h | 06  | 05                 | 02                 |
| 000B7800h | 06  | 05                 | 06                 |
| 000BB000h | 06  | 05                 | 02                 |
| 000BE800h | 06  | 05                 | 04                 |
| 000C2000h | 06  | 05                 | 04                 |
| 000D7000h | 06  | 05                 | 00                 |
| 000DA800h | 06  | 05                 | 00                 |
| 000DE000h | 06  | 05                 | 05                 |
| 000E1800h | 06  | 05                 | 00                 |
| 000E5000h | 06  | 05                 | 02                 |
| 000EC000h | 06  | 05                 | 02                 |
| 000EF800h | 06  | 05                 | 05                 |
| 00101000h | 06  | 05                 | 05                 |
| 00104800h | 06  | 05                 | 02                 |
| 0010B800h | 06  | 05                 | 03                 |
| 0010F000h | 06  | 05                 | 02                 |
| 00112800h | 06  | 05                 | 00                 |
| 00116000h | 06  | 05                 | 04                 |
| 00119800h | 06  | 05                 | 06                 |
| 0011D000h | 06  | 05                 | 04                 |
| 00127800h | 06  | 05                 | 06                 |
| 00132000h | 06  | 05                 | 00                 |
| 02416000h | 06  | 05                 | 02                 |
| 0241D000h | 06  | 05                 | 04                 |
| 02420800h | 06  | 05                 | 03                 |
| 03C1F000h | 06  | 05                 | 06                 |
| 03F94800h | 06  | 05                 | 06                 |
| 0855C800h | 06  | 05                 | 02                 |
| 0AFB0800h | 06  | 05                 | 01                 |
| 0BB30000h | 06  | 05                 | 01                 |
| 0CD3C000h | 06  | 05                 | 06                 |
| 0CD5B800h | 06  | 05                 | 06                 |
| 0EB49000h | 06  | 05                 | 02                 |
| 0002C000h | 09  | 02                 | 03                 |
| 0003D800h | 09  | 02                 | 00                 |
| 00041000h | 09  | 02                 | 05                 |
| 00044800h | 09  | 02                 | 04                 |
| 00048000h | 09  | 02                 | 01                 |
| 0004B800h | 09  | 02                 | 04                 |
| 0004F000h | 09  | 02                 | 06                 |
| 00052800h | 09  | 02                 | 03                 |
| 00056000h | 09  | 02                 | 05                 |
| 00059800h | 09  | 02                 | 06                 |
| 0005D000h | 09  | 02                 | 06                 |
| 00064000h | 09  | 02                 | 04                 |
| 00067800h | 09  | 02                 | 06                 |
| 0007C800h | 09  | 02                 | 03                 |
| 00080000h | 09  | 02                 | 05                 |
| 00083800h | 09  | 02                 | 04                 |
| 0009C000h | 09  | 02                 | 04                 |
| 000AD800h | 09  | 02                 | 05                 |
| 000B1000h | 09  | 02                 | 01                 |
| 000B4800h | 09  | 02                 | 04                 |
| 000B8000h | 09  | 02                 | 00                 |
| 000BB800h | 09  | 02                 | 01                 |
| 000BF000h | 09  | 02                 | 00                 |
| 000C2800h | 09  | 02                 | 02                 |
| 000D4000h | 09  | 02                 | 00                 |
| 000D7800h | 09  | 02                 | 06                 |
| 000DB000h | 09  | 02                 | 06                 |
| 000DE800h | 09  | 02                 | 05                 |
| 000E2000h | 09  | 02                 | 06                 |
| 000E5800h | 09  | 02                 | 01                 |
| 000E9000h | 09  | 02                 | 03                 |
| 000EC800h | 09  | 02                 | 03                 |
| 00101800h | 09  | 02                 | 04                 |
| 00105000h | 09  | 02                 | 03                 |
| 00108800h | 09  | 02                 | 05                 |
| 0010C000h | 09  | 02                 | 02                 |
| 0010F800h | 09  | 02                 | 02                 |
| 00113000h | 09  | 02                 | 02                 |
| 00116800h | 09  | 02                 | 01                 |
| 0011A000h | 09  | 02                 | 06                 |
| 0011D800h | 09  | 02                 | 03                 |
| 02413000h | 09  | 02                 | 04                 |
| 0241A000h | 09  | 02                 | 00                 |
| 02421000h | 09  | 02                 | 02                 |
| 02424800h | 09  | 02                 | 05                 |
| 02428000h | 09  | 02                 | 04                 |
| 03C2A000h | 09  | 02                 | 05                 |
| 03E61000h | 09  | 02                 | 05                 |
| 05B5D000h | 09  | 02                 | 01                 |
| 07D53000h | 09  | 02                 | 01                 |
| 0B9D6000h | 09  | 02                 | 02                 |
| 0BA31000h | 09  | 02                 | 01                 |
| 0E1D3800h | 09  | 02                 | 03                 |
| 0E5A4000h | 09  | 02                 | 04                 |
| 0ED5A000h | 09  | 02                 | 06                 |
| 0002C800h | 0B  | 06                 | 01                 |
| 0003E000h | 0B  | 06                 | 00                 |
| 00048800h | 0B  | 06                 | 06                 |
| 0004C000h | 0B  | 06                 | 00                 |
| 0004F800h | 0B  | 06                 | 00                 |
| 00053000h | 0B  | 06                 | 02                 |
| 00056800h | 0B  | 06                 | 01                 |
| 0005A000h | 0B  | 06                 | 05                 |
| 00064800h | 0B  | 06                 | 03                 |
| 00068000h | 0B  | 06                 | 03                 |
| 0007D000h | 0B  | 06                 | 03                 |
| 00084000h | 0B  | 06                 | 02                 |
| 00095800h | 0B  | 06                 | 04                 |
| 0009C800h | 0B  | 06                 | 05                 |
| 000AE000h | 0B  | 06                 | 01                 |
| 000B1800h | 0B  | 06                 | 01                 |
| 000B5000h | 0B  | 06                 | 03                 |
| 000B8800h | 0B  | 06                 | 01                 |
| 000BC000h | 0B  | 06                 | 03                 |
| 000BF800h | 0B  | 06                 | 01                 |
| 000C3000h | 0B  | 06                 | 04                 |
| 000D4800h | 0B  | 06                 | 01                 |
| 000D8000h | 0B  | 06                 | 01                 |
| 000DB800h | 0B  | 06                 | 00                 |
| 000DF000h | 0B  | 06                 | 05                 |
| 000E2800h | 0B  | 06                 | 06                 |
| 000E6000h | 0B  | 06                 | 03                 |
| 000E9800h | 0B  | 06                 | 06                 |
| 000ED000h | 0B  | 06                 | 06                 |
| 00105800h | 0B  | 06                 | 01                 |
| 00109000h | 0B  | 06                 | 01                 |
| 0010C800h | 0B  | 06                 | 05                 |
| 00110000h | 0B  | 06                 | 01                 |
| 00113800h | 0B  | 06                 | 03                 |
| 00117000h | 0B  | 06                 | 04                 |
| 0011A800h | 0B  | 06                 | 00                 |
| 0011E000h | 0B  | 06                 | 03                 |
| 0241E000h | 0B  | 06                 | 03                 |
| 05C6E800h | 0B  | 06                 | 04                 |
| 0A05E000h | 0B  | 06                 | 04                 |
| 0AF92000h | 0B  | 06                 | 05                 |
| 0DA25000h | 0B  | 06                 | 04                 |
| 0E1D0800h | 0B  | 06                 | 04                 |
| 0EB2A800h | 0B  | 06                 | 03                 |
| 0003E800h | 0D  | 03                 | 04                 |
| 00045800h | 0D  | 03                 | 04                 |
| 00049000h | 0D  | 03                 | 04                 |
| 0004C800h | 0D  | 03                 | 03                 |
| 00050000h | 0D  | 03                 | 02                 |
| 00053800h | 0D  | 03                 | 04                 |
| 00057000h | 0D  | 03                 | 06                 |
| 0005A800h | 0D  | 03                 | 05                 |
| 00065000h | 0D  | 03                 | 03                 |
| 00068800h | 0D  | 03                 | 01                 |
| 0007D800h | 0D  | 03                 | 03                 |
| 00081000h | 0D  | 03                 | 03                 |
| 00084800h | 0D  | 03                 | 00                 |
| 00096000h | 0D  | 03                 | 05                 |
| 0009D000h | 0D  | 03                 | 05                 |
| 000AE800h | 0D  | 03                 | 02                 |
| 000B2000h | 0D  | 03                 | 02                 |
| 000B5800h | 0D  | 03                 | 02                 |
| 000B9000h | 0D  | 03                 | 04                 |
| 000BC800h | 0D  | 03                 | 01                 |
| 000C0000h | 0D  | 03                 | 01                 |
| 000C3800h | 0D  | 03                 | 01                 |
| 000D5000h | 0D  | 03                 | 04                 |
| 000D8800h | 0D  | 03                 | 00                 |
| 000DF800h | 0D  | 03                 | 02                 |
| 000E3000h | 0D  | 03                 | 03                 |
| 000E6800h | 0D  | 03                 | 05                 |
| 000EA000h | 0D  | 03                 | 00                 |
| 000ED800h | 0D  | 03                 | 00                 |
| 00102800h | 0D  | 03                 | 04                 |
| 00106000h | 0D  | 03                 | 02                 |
| 00110800h | 0D  | 03                 | 05                 |
| 00114000h | 0D  | 03                 | 04                 |
| 00117800h | 0D  | 03                 | 04                 |
| 0011B000h | 0D  | 03                 | 00                 |
| 0011E800h | 0D  | 03                 | 00                 |
| 00145000h | 0D  | 03                 | 02                 |
| 02414000h | 0D  | 03                 | 05                 |
| 0241B000h | 0D  | 03                 | 06                 |
| 02422000h | 0D  | 03                 | 05                 |
| 02425800h | 0D  | 03                 | 06                 |
| 05C1B000h | 0D  | 03                 | 01                 |
| 0A031000h | 0D  | 03                 | 02                 |
| 0BA20800h | 0D  | 03                 | 04                 |
| 0BAEF000h | 0D  | 03                 | 03                 |
| 0DAED000h | 0D  | 03                 | 01                 |
| 0EB0F000h | 0D  | 03                 | 03                 |

<p>&nbsp;</p>

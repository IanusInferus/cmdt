Commandos1 DIR File Structure Table

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
<td rowspan="6"><p>Header DA</p></td>
<td rowspan="5"><p>File DB</p></td>
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>File name, ends with 0x00, and follows with a few 0xCD, directory tails' are "DIRECTOR.FIN"</p></td>
<td><p>4441544F5300CDCD...(DATOS)</p></td>
</tr>
<tr class="even">
<td><p>Type</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>0x00 for files, 0x01 for directories, 0xFF</p>
<p>for directory tails</p></td>
<td><p>00/01/FF</p></td>
</tr>
<tr class="odd">
<td><p>Padding</p></td>
<td></td>
<td><p>3</p></td>
<td><p>Padding</p></td>
<td><p>CDCDCD</p></td>
</tr>
<tr class="even">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>0x00000000 for directories</p></td>
<td><p>90980900</p></td>
</tr>
<tr class="odd">
<td><p>Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>File data address. for directories, it's the address of the first File DB in the directory</p></td>
<td><p>48DA0000</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>There's many File DBs. For directories, they are matched by the directories' File DBs and their tail File DBs where File DB and Name is null, Type is 0xFFCDCDCD and Length and Address is 0xFFFFFFFF</p></td>
</tr>
<tr class="odd">
<td rowspan="2"><p>Data DA</p></td>
<td><p>Data DB</p></td>
<td></td>
<td></td>
<td></td>
<td><p>File data, no alignment</p></td>
<td></td>
</tr>
<tr class="even">
<td colspan="6"><p>There's many Data DB</p>
<p>s</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Notice:

1\. All numerical data types are in little-endian.

2.The directory File DB match pattern: directories and directory tails match in a way like the monolayer brackets, which is different from PCK file of Comm2, namely:

    DATOS{
        BRIEF_MP
        FONTS
        MISIONES
        RECURSOS
    }

    BRIF_MP{
        MO.ZOM
        ...
    }

...

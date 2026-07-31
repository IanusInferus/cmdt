盟军敢死队1 DIR文件格式表

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
<td rowspan="6"><p>Header DA</p></td>
<td rowspan="5"><p>File DB</p></td>
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>文件名，以0x00结束，以0xCD补空，文件夹结束的是"DIRECTOR.FIN"</p></td>
<td><p>4441544F5300CDCD...(DATOS)</p></td>
</tr>
<tr class="even">
<td><p>Type</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>文件是0x00，文件夹是0x01，文件夹结束为0xFF</p></td>
<td><p>00/01/FF</p></td>
</tr>
<tr class="odd">
<td><p>Padding</p></td>
<td></td>
<td><p>3</p></td>
<td><p>填充数据</p></td>
<td><p>CDCDCD</p></td>
</tr>
<tr class="even">
<td><p>Length</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>文件夹为0x00000000</p></td>
<td><p>90980900</p></td>
</tr>
<tr class="odd">
<td><p>Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>文件数据地址，文件夹为第一个文件（夹）的File DB地址</p></td>
<td><p>48DA0000</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>有很多个File DB，其中文件夹由文件夹的File DB和Name为空，Type为0xFF</p>
<p>CDCDCD</p>
，Length和Address均为0xFFFFFFFF的特殊File DB配对</td>
</tr>
<tr class="odd">
<td rowspan="2"><p>Data DA</p></td>
<td><p>Data DB</p></td>
<td></td>
<td></td>
<td></td>
<td><p>文件数据，不对齐</p></td>
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

2、文件夹配对，文件夹和文件夹结尾以类似单层括号的方式配对，这一点与盟军2的PCK文件不同，即：

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

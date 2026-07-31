GIF文件格式表

地狱门神（F.R.C.）整理

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
<td rowspan="13"><p>Header DA</p></td>
<td rowspan="9"><p>Info DB</p></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>6</p></td>
<td><p>标志符，GIF87a或GIF89a</p></td>
<td><p>47494638 3961(GIF89a)</p></td>
</tr>
<tr class="even">
<td><p>Width</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>宽度</p></td>
<td><p>4000</p></td>
</tr>
<tr class="odd">
<td><p>Height</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>高度</p></td>
<td><p>4000</p></td>
</tr>
<tr class="even">
<td><p>Global Color Table Flag</p></td>
<td><p>Bit</p></td>
<td><p>1 Bit</p></td>
<td><p>当置位时表示有全局调色板</p></td>
<td><p>1<sub>2</sub></p></td>
</tr>
<tr class="odd">
<td><p>Bits Per Pixel</p></td>
<td><p>Bit</p></td>
<td><p>3 Bit</p></td>
<td><p>Bits Per Pixel + 1 表示色深</p></td>
<td><p>111<sub>2</sub></p></td>
</tr>
<tr class="even">
<td><p>Sort Flag</p></td>
<td><p>Bit</p></td>
<td><p>1 Bit</p></td>
<td><p>分类标志，不用，置0</p></td>
<td><p>0<sub>2</sub></p></td>
</tr>
<tr class="odd">
<td><p>Global Color Table Size</p></td>
<td><p>Bit</p></td>
<td><p>3 Bit</p></td>
<td><p>2^(Global Color Table Size + 1)表示全局调色板大小</p></td>
<td><p>111<sub>2</sub></p></td>
</tr>
<tr class="even">
<td><p>Background Color</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>背景色索引(在全局颜色列表中的，如果没有全局颜色列表，该值没有意义)</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td><p>Pixel Aspect Radio</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>不用，固定为0</p></td>
<td><p>00</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Global Palette DB</p></td>
<td><p>Red</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>索引0的红色分量</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td><p>Green</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>索引0的绿色分量</p></td>
<td><p>00</p></td>
</tr>
<tr class="even">
<td><p>Blue</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>索引0的蓝色分量</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>数量为2^(Global Color Table Size + 1)</p></td>
</tr>
<tr class="even">
<td rowspan="2"><p>Data DA</p></td>
<td><p>Image DB / Control DB</p></td>
<td></td>
<td></td>
<td></td>
<td><p>图像块，以0x2C开始 / 控制块，以0x21开始</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="6"><p>多个</p></td>
</tr>
<tr class="even">
<td><p>Trailer DA</p></td>
<td></td>
<td><p>Trailer</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>终结器，固定为0x3B</p></td>
<td><p>3B </p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

图象块

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
<th><p>小数据块</p></th>
<th><p>数据</p></th>
<th><p>数据类型</p></th>
<th><p>长度</p></th>
<th><p>描述</p></th>
<th><p>样例数据</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="20"><p>Image DB</p></td>
<td rowspan="10"><p>Info Sub-DB</p></td>
<td><p>Introducer</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>固定为0x2C</p></td>
<td><p>2C</p></td>
</tr>
<tr class="even">
<td><p>Offset X</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>X方向偏移量，通常取0</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td><p>Offset Y</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>Y方向偏移量，通常取0</p></td>
<td><p>00</p></td>
</tr>
<tr class="even">
<td><p>Width</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>宽度</p></td>
<td><p>4000</p></td>
</tr>
<tr class="odd">
<td><p>Height</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>高度</p></td>
<td><p>4000</p></td>
</tr>
<tr class="even">
<td><p>Local Color Table Flag</p></td>
<td><p>Bit</p></td>
<td><p>1 Bit</p></td>
<td><p>当置位时表示有局部调色板</p></td>
<td><p>1<sub>2</sub></p></td>
</tr>
<tr class="odd">
<td><p>Interlace Flag</p></td>
<td><p>Bit</p></td>
<td><p>1 Bit</p></td>
<td><p><span id="交织标志">交织标志</span>，置位时图象数据使用交织方式排列，否则使用顺序排列。</p>
<p>交织图象按下面的方法处理光栅数据：</p>
<p>创建四个通道保存数据，每个通道提取不同行的数据：<br />
第一通道提取从第0行开始每隔8行的数据；<br />
第二通道提取从第4行开始每隔8行的数据；<br />
第三通道提取从第2行开始每隔4行的数据；<br />
第四通道提取从第1行开始每隔2行的数据；</p>
<p>下面的例子演示了提取交织图象数据的顺序：</p>
<pre><code> 0  -------------------  1
 1  -------------------           4
 2  -------------------        3
 3  -------------------           4
 4  -------------------     2
 5  -------------------           4
 6  -------------------        3
 7  -------------------           4
 8  -------------------  1
 9  -------------------           4
 10 -------------------        3
 11 -------------------           4
 12 -------------------     2
 13 -------------------           4
 14 -------------------        3
 15 -------------------           4
 16 -------------------  1
 17 -------------------           4
 18 -------------------        3
 19 -------------------           4</code></pre></td>
<td><p>1<sub>2</sub></p></td>
</tr>
<tr class="even">
<td><p>Sort Flag</p></td>
<td><p>Bit</p></td>
<td><p>1 Bit</p></td>
<td><p>分类标志，不用，置0</p></td>
<td><p>0<sub>2</sub></p></td>
</tr>
<tr class="odd">
<td><p>Reserved</p></td>
<td><p>Bit</p></td>
<td><p>2 Bit</p></td>
<td><p>保留，固定为0</p></td>
<td><p>00<sub>2</sub></p></td>
</tr>
<tr class="even">
<td><p>Local Color Table Size</p></td>
<td><p>Bit</p></td>
<td><p>3 Bit</p></td>
<td><p>2^(Local Color Table Size+1)表示局部调色板大小</p></td>
<td><p>111<sub>2</sub></p></td>
</tr>
<tr class="odd">
<td rowspan="5"><p>Local Palette Sub-DB</p></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td><p>Red</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>索引0的红色分量</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td><p>Green</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>索引0的绿色分量</p></td>
<td><p>00</p></td>
</tr>
<tr class="even">
<td><p>Blue</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>索引0的蓝色分量</p></td>
<td><p>00</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>当Local Color Table Flag置位时才存在，数量为2^(Local Color Table Size + 1)</p></td>
</tr>
<tr class="even">
<td rowspan="5"><p>LZW Data Sub-DB</p></td>
<td><p>Start Code Size</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>LZW编码初始码表大小的位数，通常就等于图象的色深。但单色图像不能为1，必须为2或以上</p></td>
<td><p>08</p></td>
</tr>
<tr class="odd">
<td><p>Data Size</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>后面数据的长度，通常最大为0xFE</p></td>
<td><p>FE</p></td>
</tr>
<tr class="even">
<td><p>Data</p></td>
<td><p>Byte</p></td>
<td><p>Data Size</p></td>
<td><p>LZW编码数据</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="5"><p>Data Length和Data不断重复，直到遇到终结器，所有的Data串联起来组成LZW编码数据流</p></td>
</tr>
<tr class="even">
<td><p>Block Terminator</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>块终结器，固定值0</p></td>
<td><p>00</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

图形扩展控制块（需要GIF89a）

<table>
<colgroup>
<col style="width: 12%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 30%" />
<col style="width: 25%" />
</colgroup>
<thead>
<tr class="header">
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
<td rowspan="10"><p>Graphic Control Extension DB</p></td>
<td><p>Extension Introducer</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>标识这是一个扩展块，固定为0x21</p></td>
<td><p>21</p></td>
</tr>
<tr class="even">
<td><p>Graphic Control Label</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>标识这是一个图形控制扩展块，固定为0xF9</p></td>
<td><p>F9</p></td>
</tr>
<tr class="odd">
<td><p>Block Size</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>不包括块终结器，固定值4</p></td>
<td><p>04</p></td>
</tr>
<tr class="even">
<td><p>Reserved</p></td>
<td><p>Bit</p></td>
<td><p>3 Bit</p></td>
<td><p>保留，固定为0</p></td>
<td><p>000<sub>2</sub></p></td>
</tr>
<tr class="odd">
<td><p>Disposal Method</p></td>
<td><p>Bit</p></td>
<td><p>3 Bit</p></td>
<td><p><span id="处置方法">处置方法</span>：指出处置图形的方法，当值为：<br />
0 - 不使用处置方法<br />
1 - 不处置图形，把图形从当前位置移去<br />
2 - 回复到背景色<br />
3 - 回复到先前状态<br />
4-7 - 自定义</p></td>
<td><p>000<sub>2</sub></p></td>
</tr>
<tr class="even">
<td><p>Use Input Flag</p></td>
<td><p>Bit</p></td>
<td><p>1 Bit</p></td>
<td><p>用户输入标志：指出是否期待用户有输入之后才继续进行下去，置位表示期待，值否表示不期待。用户输入可以是按回车键、鼠标点击等，可以和延迟时间一起使用，在设置的延迟时间内用户有输入则马上继续进行，或者没有输入直到延迟时间到达而继续</p></td>
<td><p>0<sub>2</sub></p></td>
</tr>
<tr class="odd">
<td><p>Transparent Color Flag</p></td>
<td><p>Bit</p></td>
<td><p>1 Bit</p></td>
<td><p>透明颜色标志：置位表示使用透明颜色</p></td>
<td><p>1<sub>2</sub></p></td>
</tr>
<tr class="even">
<td><p>Delay Time</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>延迟时间，单位1/100秒，如果值不为1，表示暂停规定的时间后再继续往下处理数据流</p></td>
<td><p>2800</p></td>
</tr>
<tr class="odd">
<td><p>Transparent Color Index</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>透明色索引</p></td>
<td><p>00</p></td>
</tr>
<tr class="even">
<td><p>Block Terminator</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>块终结器，固定值0</p></td>
<td><p>00</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

其他控制块（需要GIF89a，由于基本不用，不再列出，可参见\[1\]）

<table>
<colgroup>
<col style="width: 12%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 30%" />
<col style="width: 25%" />
</colgroup>
<thead>
<tr class="header">
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
<td rowspan="6"><p>Graphic Control Extension DB</p></td>
<td><p>Extension Introducer</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>标识这是一个扩展块，固定为0x21</p></td>
<td><p>21</p></td>
</tr>
<tr class="even">
<td><p>Control Label</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>标识这是一个什么控制扩展块：</p>
<p>0x01 - 图形文本扩展块</p>
<p>0xFE - 注释块</p>
<p>0xFF - 应用程序扩展块</p></td>
<td><p>FE</p></td>
</tr>
<tr class="odd">
<td><p>Data Size</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>后面数据的长度，通常最大为0xFE</p></td>
<td><p>FE</p></td>
</tr>
<tr class="even">
<td><p>Data</p></td>
<td><p>Byte</p></td>
<td><p>Data Size</p></td>
<td><p>数据</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="5"><p>Data Length和Data不断重复，直到遇到终结器</p></td>
</tr>
<tr class="even">
<td><p>Block Terminator</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>块终结器，固定值0</p></td>
<td><p>00</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是little-endian的 。所有字节内部数据均以上为高位，下为低位，如(abbbcddd)<sub>2</sub>是按a、bbb、c、ddd的顺序表示的。

2、压缩方法：可变长LZW压缩算法（Variable-Length_Code Lempel Ziv Walch Compression）。

\[1\]等文档此处讲述不太清楚，有误导的嫌疑。为方便起见，在此列出伪码，以供参考，其中：

StartCodeSize表示LZW编码长度的初始大小。

ClearCode表示初始化一个编译表，Clear Code + 1表示编码数据流的结束。

ReadSrc、WriteSrc、ReadTar、WriteTar分别表示读取原数据流、写入原数据流、读取编码数据流、写入编码数据流。

原数据流是字节流，而编码数据流是可变长度流，需要编译成固定的8-bit长度的字符流，编译顺序是从右往左。

下面是一个具体例子：编译5位长度编码到8位字符

<table>
<thead>
<tr class="header">
<th>0</th>
<th>b</th>
<th>b</th>
<th>b</th>
<th>a</th>
<th>a</th>
<th>a</th>
<th>a</th>
<th>a</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>1</td>
<td>d</td>
<td>c</td>
<td>c</td>
<td>c</td>
<td>c</td>
<td>c</td>
<td>b</td>
<td>b</td>
</tr>
<tr class="even">
<td>2</td>
<td>e</td>
<td>e</td>
<td>e</td>
<td>e</td>
<td>d</td>
<td>d</td>
<td>d</td>
<td>d</td>
</tr>
<tr class="odd">
<td>3</td>
<td>g</td>
<td>g</td>
<td>f</td>
<td>f</td>
<td>f</td>
<td>f</td>
<td>f</td>
<td>e</td>
</tr>
<tr class="even">
<td>4</td>
<td>h</td>
<td>h</td>
<td>h</td>
<td>h</td>
<td>h</td>
<td>g</td>
<td>g</td>
<td>g</td>
</tr>
<tr class="odd">
<td></td>
<td colspan="8">...</td>
</tr>
<tr class="even">
<td>N</td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

``` vbnet
Function LZW(ByVal SrcBytes As Char()) As Char()
    TarBytes = New List(Of Char)()
    CodeSize = StartCodeSize
    SrcPos = 0
    TarPos = 0
    Dim Table As New List(Of String)
    Dim RTable As New Hashtable
    For n As Integer = 0 To 2 ^ StartCodeSize - 1
        Table.Add(Chr(n))
        RTable.Add(Chr(n), n)
    Next
    Table.Add("CC")
    Table.Add("EC")
    Dim ClearCode As Integer = 2 ^ StartCodeSize
    Dim OverflowCode As Integer = 2 ^ (StartCodeSize + 1)

    Dim Prefix As String = ""
    Dim IndexOfPrefix As Integer
    Dim Root As Char
    WriteTar(2 ^ StartCodeSize)

    Dim CurStr As String

    While True
        Root = Chr(ReadSrc())
        If EOF Src Then Exit While
        CurStr = Prefix & Root

        If RTable.ContainsKey(CurStr) Then
            IndexOfPrefix = RTable(CurStr)
            Prefix = CurStr
        Else
            WriteTar(IndexOfPrefix)
            If Table.Count = OverflowCode Then
                If OverflowCode = 4096  Then
                    WriteTar(Asc(Root))
                    WriteTar(2 ^ StartCodeSize)
                    Table.RemoveRange(2 ^ (StartCodeSize) + 2, Table.Count – 2 ^ (StartCodeSize) - 2)
                    RTable.Clear()
                    For n As Integer = 0 To 2 ^ StartCodeSize - 1
                        RTable.Add(Chr(n), n)
                    Next
                    CodeSize = StartCodeSize
                    OverflowCode = 2 ^ (StartCodeSize + 1)
                    Prefix = ""
                    Continue While
                Else
                    CodeSize += 1
                    OverflowCode = OverflowCode * 2
                End If
            End If
            Table.Add(CurStr)
            RTable.Add(CurStr, Table.Count - 1)
            Prefix = Root
            IndexOfPrefix = Asc(Root)
        End If
    End While
    WriteTar(IndexOfPrefix)
    WriteTar(ClearCode + 1)
    Return TarBytes
End Function

Function UnLZW(ByVal TarBytes As Char()) As Char()
    SrcBytes = New List(Of Char)()
    CodeSize = StartCodeSize
    SrcPos = 0
    TarPos = 0
    Dim Table As New List(Of String)
    For n As Integer = 0 To 2 ^ StartCodeSize - 1
        Table.Add(Chr(n))
    Next
    Table.Add("CC")
    Table.Add("EC")
    Dim ClearCode As Integer = 2 ^ StartCodeSize
    Dim OverflowCode As Integer = 2 ^ (StartCodeSize + 1)
    Dim CurStr As String
    Dim cur As Integer = ReadTar()
    cur = ReadTar()
    WriteSrc(Table(cur))
    Dim old As Integer = cur
    cur = ReadTar()
    While True
        Select Case cur
            Case ClearCode
                Table.RemoveRange(2 ^ (StartCodeSize) + 2, Table.Count  – 2 ^ (StartCodeSize) - 2)
                CodeSize = StartCodeSize
                OverflowCode = 2 ^ (StartCodeSize + 1)
                cur = ReadTar()
                WriteSrc(Table(cur))
            Case ClearCode + 1
                Exit While
            Case Else
                If cur <= Table.Count - 1 Then
                    WriteSrc(Table(cur))
                    Table.Add(Table(old) & Table(cur)(0))
                Else
                    CurStr = Table(old) & Table(old)(0)
                    WriteSrc(CurStr)
                    Table.Add(CurStr)
                End If
                If Table.Count = OverflowCode AndAlso OverflowCode <> 4096 Then
                    CodeSize += 1
                    OverflowCode = OverflowCode * 2
                End If
        End Select
        If EOF Tar Then Exit While
        old = cur
        cur = ReadTar()
    End While
    Return SrcBytes
End Function
```

伪码是从实际代码改写而来，略有点长。

<p>&nbsp;</p>

3、位图数据：

每个像素按照从上到下从左到右的顺序排列。

<p>&nbsp;</p>

参考：

\[1\]GIF文档，foenix，<http://asp.6to23.com/iseesoft/devdoc/imgdoc/gif.htm>

<p>&nbsp;</p>

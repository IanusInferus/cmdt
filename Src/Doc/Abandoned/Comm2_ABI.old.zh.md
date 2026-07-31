盟军敢死队2及3 ABI文件格式表

地狱门神（F.R.C.）整理

<p>&nbsp;</p>

盟军敢死队2 ABI文件格式（样例数据均采自MORADO.ABI）

<table>
<colgroup>
<col style="width: 8%" />
<col style="width: 8%" />
<col style="width: 8%" />
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 30%" />
<col style="width: 25%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据区</p></th>
<th><p>数据块</p></th>
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
<td rowspan="5"><p>Header DA</p></td>
<td rowspan="5"><p>Basic Info DB</p></td>
<td rowspan="5"></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>标志符</p></td>
<td><p>4C444D42("LDMB")</p></td>
</tr>
<tr class="even">
<td><p>Version</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>版本号：盟军2有1010、1040、1050、1060四种。</p></td>
<td><p>31303530("1050")</p></td>
</tr>
<tr class="odd">
<td><p>Num Mesh</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>网格个数</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="even">
<td><p>Num Carton</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>动画个数</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>Num Texture</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>贴图个数</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="even">
<td rowspan="9"><p>Texture  DA</p></td>
<td rowspan="4"><p>Texture Info DB</p></td>
<td rowspan="4"><p>Texture Info DS</p></td>
<td><p>Block Identifier</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>恒为0</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="odd">
<td><p>Width</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图的宽度</p></td>
<td><p>10000000</p></td>
</tr>
<tr class="even">
<td><p>Height</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>位图的高度</p></td>
<td><p>10000000</p></td>
</tr>
<tr class="odd">
<td><p>Name</p>
<p>(1040、1050、1060)</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>位图名称</p></td>
<td></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Palette DB</p></td>
<td rowspan="4"><p>Palette DS</p></td>
<td><p>Red</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>红色分量</p></td>
<td><p>FF</p></td>
</tr>
<tr class="odd">
<td><p>Green</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>绿色分量</p></td>
<td><p>00</p></td>
</tr>
<tr class="even">
<td><p>Blue</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>蓝色分量</p></td>
<td><p>FF</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>三元组数量为256</p></td>
</tr>
<tr class="even">
<td><p>Bitmap DB</p></td>
<td></td>
<td></td>
<td><p>Byte</p></td>
<td><p>Width * Height</p></td>
<td><p>位图数据</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="8"><p>数量为Num Texture</p></td>
</tr>
<tr class="even">
<td><p>Costume DA</p></td>
<td rowspan="4"><p>Costume Info DB</p></td>
<td rowspan="4"><p>Costume Info DS</p></td>
<td><p>Block Identifier()</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>恒为1</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td></td>
<td><p>Block Identifier(1060)</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>恒为1</p></td>
<td><p>01</p></td>
</tr>
<tr class="even">
<td></td>
<td><p>Num Dress</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td></td>
<td><p>Num Bone</p>
<p>(1060)</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td rowspan="14"><p>Dress DB</p></td>
<td rowspan="3"><p>Dress Info DS</p></td>
<td><p>Num Point</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>08000000</p></td>
</tr>
<tr class="odd">
<td></td>
<td><p>Num District</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>06000000</p></td>
</tr>
<tr class="even">
<td></td>
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td></td>
<td><p>"morado.max"</p></td>
</tr>
<tr class="odd">
<td></td>
<td rowspan="3"><p>Point DS</p></td>
<td><p>x</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td><p>y</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td><p>z</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td rowspan="6"><p>District DS</p></td>
<td><p>n</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td><p>Texture ID</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td><p>Point 0</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td><p>U 0</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>使用前乘以32768 ?</p></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td><p>V 0</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td><p>使用前乘以32768 ?</p></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td colspan="5"><p>Point和U、V的数据组，共n组</p></td>
</tr>
<tr class="even">
<td></td>
<td rowspan="2"><p>Bone Info DS</p></td>
<td><p>Start District Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>初始区域索引</p></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td><p>Next Start District Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>终止区域索引+1</p></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td>Bone DB</td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
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

盟军敢死队3 ABI文件格式

<table>
<colgroup>
<col style="width: 8%" />
<col style="width: 8%" />
<col style="width: 8%" />
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 30%" />
<col style="width: 25%" />
</colgroup>
<thead>
<tr class="header">
<th><p>数据区</p></th>
<th><p>数据块</p></th>
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
<td rowspan="31"><p>Header DA</p></td>
<td rowspan="13"><p>Basic Info DB</p></td>
<td rowspan="13"></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>标志符</p></td>
<td><p>4C444D42("LDMB")</p></td>
</tr>
<tr class="even">
<td><p>Version</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>版本号：盟军3有1011一种。</p></td>
<td><p>31303131("1011")</p></td>
</tr>
<tr class="odd">
<td><p>Unknown</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td></td>
<td><p>0000</p></td>
</tr>
<tr class="even">
<td><p>Unknown</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td></td>
<td><p>0100</p></td>
</tr>
<tr class="odd">
<td><p>Unknown</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td></td>
<td><p>0100</p></td>
</tr>
<tr class="even">
<td><p>Image Count</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td></td>
<td><p>0100</p></td>
</tr>
<tr class="odd">
<td><p>Unknown</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td></td>
<td><p>0100</p></td>
</tr>
<tr class="even">
<td><p>Unknown</p></td>
<td><p>Int16</p></td>
<td><p>2</p></td>
<td></td>
<td><p>0100</p></td>
</tr>
<tr class="odd">
<td><p>Image Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>F4030000</p></td>
</tr>
<tr class="even">
<td><p>Unknown File Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>F4130000</p></td>
</tr>
<tr class="odd">
<td><p>Unknown Info Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>B2030000</p></td>
</tr>
<tr class="even">
<td><p>Unknown</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>0C000000</p></td>
</tr>
<tr class="odd">
<td><p>Unknown</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>02000000</p></td>
</tr>
<tr class="even">
<td rowspan="9"><p>Image Info DB</p></td>
<td rowspan="2"><p>Image Info DS</p></td>
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
<td rowspan="4"><p>Palette DS</p></td>
<td><p>Red</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>红色分量</p></td>
<td><p>19</p></td>
</tr>
<tr class="odd">
<td><p>Green</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>绿色分量</p></td>
<td><p>16</p></td>
</tr>
<tr class="even">
<td><p>Blue</p></td>
<td><p>Byte</p></td>
<td><p>1</p></td>
<td><p>蓝色分量</p></td>
<td><p>10</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>三元组数量为256</p></td>
</tr>
<tr class="even">
<td rowspan="3"><p>Image Info DS</p></td>
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>图片的名称</p></td>
<td><p>"ps_tut_02_03_01.bmp"</p></td>
</tr>
<tr class="odd">
<td><p>Unknown</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>25E50700</p></td>
</tr>
<tr class="even">
<td><p>Address</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td></td>
<td><p>F4030000</p></td>
</tr>
<tr class="odd">
<td colspan="7"><p>共Image Count个</p></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td><p>FFFFFFFF</p></td>
</tr>
<tr class="odd">
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td rowspan="12"><p>Image DA</p></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td><p>Image Data DB</p></td>
<td></td>
<td></td>
<td><p>Byte</p></td>
<td><p>Width * Height</p></td>
<td><p>位图数据</p></td>
<td></td>
</tr>
<tr class="even">
<td colspan="7"><p>共Image Count个</p></td>
</tr>
<tr class="odd">
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
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
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td colspan="8"><p>数量为</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

注意:

1、所有的整数数据类型都是little-endian的。

2、位图数据：

各行按从下到上的顺序排列。

3、本文件最初是在URF的提供的文件格式的基础上整理的。

4、本文件加入了jinshengmao提供的角色动画(合成版)源代码中对3D模型和骨架结构等的理解。

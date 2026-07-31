盟军敢死队2 -- MA2文件格式表

PJB、地狱门神（F.R.C.）制作整理

MA = Mask?

<table>
<colgroup>
<col style="width: 8%" />
<col style="width: 8%" />
<col style="width: 8%" />
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 7%" />
<col style="width: 15%" />
<col style="width: 15%" />
<col style="width: 25%" />
</colgroup>
<thead>
<tr class="header">
<th style="text-align: center;">数据区</th>
<th colspan="2" style="text-align: center;">数据块</th>
<th style="text-align: center;">数据</th>
<th style="text-align: center;">数据类型</th>
<th style="text-align: center;">长度</th>
<th colspan="2" style="text-align: center;">描述</th>
<th style="text-align: center;">样例数据</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="11" style="text-align: center;">头数据
<p>Header DA</p></td>
<td colspan="2" rowspan="2" style="text-align: center;">固定数据
<p>Header DB</p></td>
<td style="text-align: center;">Version Sign</td>
<td style="text-align: center;">Byte</td>
<td style="text-align: center;">1</td>
<td colspan="2" style="text-align: left;">盟军2测试版为2，盟军2、盟军3为3</td>
<td style="text-align: center;">03</td>
</tr>
<tr class="even">
<td style="text-align: center;">Zero</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">28</td>
<td colspan="2" style="text-align: left;"><p>未知，全为0</p></td>
<td style="text-align: center;">..00000000..</td>
</tr>
<tr class="odd">
<td colspan="2" rowspan="4" style="text-align: center;">基本信息
<p>Basic Info DB</p></td>
<td style="text-align: center;">Num View</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;"><p>视角方向的数量。一般都为4</p></td>
<td style="text-align: center;">04000000</td>
</tr>
<tr class="even">
<td style="text-align: center;">Num Object</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;"><p>地图内需要渲染的物体总数</p></td>
<td style="text-align: center;">5B000000</td>
</tr>
<tr class="odd">
<td style="text-align: center;">Num Object All View</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">8</td>
<td colspan="2" style="text-align: left;"><p>数据重复双写。</p>
<p>全部视角方向渲染的物体图象总和。同一物体的多视角渲染也算做不同的物体渲染图象次数。另外因为某视角渲染物体互相遮拦重叠或不需要渲染，这个数值不一定是上个数值（渲染物体）的倍数 。 &lt; 修改这个数值不会影响MA2的正常效果，这个数据似乎没用。&gt;</p></td>
<td style="text-align: center;">5F 010000 5F   010000</td>
</tr>
<tr class="even">
<td style="text-align: center;">Zero</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;"><p>未知，为0。可能隔断用</p></td>
<td style="text-align: center;">00000000</td>
</tr>
<tr class="odd">
<td colspan="2" rowspan="5" style="text-align: center;">Address DB</td>
<td style="text-align: center;">View Info Address</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;"><span style="color: red">本文件内视角信息开始的绝对地址</span></td>
<td style="text-align: center;">49000000</td>
</tr>
<tr class="even">
<td style="text-align: center;">Object Info Address</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 本文件内物体信息开始的绝对地址</span></td>
<td style="text-align: center;">19010000</td>
</tr>
<tr class="odd">
<td style="text-align: center;">Render Index Address</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 本文件内渲染索引数据开始的绝对地址</span></td>
<td style="text-align: center;">0D0B0000</td>
</tr>
<tr class="even">
<td style="text-align: center;">Render Data Address</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;"><span style="color: maroon"> 本文件内渲染数据数据开始的绝对地址</span></td>
<td style="text-align: center;">ED360000</td>
</tr>
<tr class="odd">
<td style="text-align: center;">Object District Info Address</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">8</td>
<td colspan="2" style="text-align: left;">重复双写，本文件内物体区域信息开始的绝对地址</td>
<td style="text-align: center;">74580700 74580700</td>
</tr>
<tr class="even">
<td rowspan="18" style="text-align: center;"><span style="color: red"> 视角信息</span>
<p><span style="color: #FF0000">View Info</span></p></td>
<td colspan="2" rowspan="14" style="text-align: center;"><span style="color: red"> 视角信息</span>
<span style="color: red"> 0方向</span></td>
<td style="text-align: center;"><span style="color: #FF0000">Offset X</span></td>
<td style="text-align: center;"><span style="color: red"> Float32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 横坐标X，游戏坐标0点X轴调整值</span></td>
<td style="text-align: center;"><span style="color: red"> 91956844</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #FF0000">Offset Y</span></td>
<td style="text-align: center;"><span style="color: red"> Float32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 纵坐标Y，游戏坐标0点Y轴调整值</span></td>
<td style="text-align: center;"><span style="color: red"> 94F86E43</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #FF0000">View Longitude</span></td>
<td style="text-align: center;"><span style="color: red"> Float32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 视角方向</span></td>
<td style="text-align: center;"><span style="color: red"> 00000000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #FF0000">View Latitude</span></td>
<td style="text-align: center;"><span style="color: red"> Float32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 俯视地图角度，一般都为40度</span></td>
<td style="text-align: center;"><span style="color: red"> 00002042</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #FF0000">Num Object To Render</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 此视角下渲染物体图象总数。&lt;直接仅仅修改此值会变动此视角下渲染的物体图象数量&gt;</span></td>
<td style="text-align: center;"><span style="color: red"> 46000000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #FF0000">Render Index Offset</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 此视角下渲染索引开始的相对于渲染索引起始绝对地址的偏移量</span></td>
<td style="text-align: center;"><span style="color: red"> 00000000</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #FF0000">Render Index Length</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 此视角下渲染索引的总长度</span></td>
<td style="text-align: center;"><span style="color: red"> C00B0000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #FF0000">Water Mask Address</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: #FF0000">流动水渲染数据地址（可为0表示没有）</span></td>
<td style="text-align: center;"><span style="color: red"> 00000000 / 892C0000(TU04EX.MA2)</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #FF0000">Render Data Offset</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 此视角下渲染数据开始的相对于渲染数据起始绝对地址的偏移量 （需要考虑最前面的4字对齐量）</span></td>
<td style="text-align: center;"><span style="color: red"> 03000000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #FF0000">Render Data Length</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: red"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red"> 此视角下渲染数据的总长度 （需考虑该视角所有物体之后的4字对齐量，不考虑最前的4字对齐量）</span></td>
<td style="text-align: center;"><span style="color: red"> 748E0200</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #FF0000">Zero</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: #FF0000">4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red">  <span style="color: red">0</span></span></td>
<td style="text-align: center;"><span style="color: red"> 00000000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #FF0000">Zero</span></td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: #FF0000">4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: red">  <span style="color: red">0</span></span></td>
<td style="text-align: center;"><span style="color: red"> 00000000</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #FF0000">Water Mask High Quality Address </span> (Comm2Demo无)</td>
<td style="text-align: center;"><span style="color: red"> Int32</span></td>
<td style="text-align: center;"><span style="color: #FF0000">4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: #FF0000">高质量流动水渲染数据地址（可为0表示没有）</span></td>
<td style="text-align: center;"><span style="color: red"> 00000000 / C3750400(SBEX.MA2)</span></td>
</tr>
<tr class="odd">
<td colspan="6" style="text-align: center;"><span style="color: red"> 共0x34(52)个字节<span style="color: red">... 这个方向是在游戏中坐标轴0度向右边方向</span></span></td>
</tr>
<tr class="even">
<td colspan="2" style="text-align: center;"><span style="color: red"> 视角信息</span>
<span style="color: red"> 90方向</span></td>
<td colspan="6" style="text-align: center;"><span style="color: red"> 类似0方向数据共0x34(52)个字节</span>
<span style="color: red"> 视角方向为 0000B442</span></td>
</tr>
<tr class="odd">
<td colspan="2" style="text-align: center;"><span style="color: red"> 视角信息</span>
<span style="color: red"> 180方向</span></td>
<td colspan="6" style="text-align: center;"><span style="color: red"> 类似0方向数据共0x34(52)个字节</span>
<span style="color: red"> 视角方向为 00003443</span></td>
</tr>
<tr class="even">
<td colspan="2" style="text-align: center;"><span style="color: red"> 视角信息</span>
<span style="color: red"> 270方向</span></td>
<td colspan="6" style="text-align: center;"><span style="color: red"> 类似0方向数据共0x34(52)个字节</span>
<span style="color: red"> 视角方向为 00008743</span></td>
</tr>
<tr class="odd">
<td colspan="8" style="text-align: center;"><span style="color: red"> 全部视角信息的长度固定为 0xD0(208个字节)</span>
<span style="color: red"> 每个视角方向的XY坐标0点调整直若改变则改变了此方向的游戏坐标0点，人和物的地址会变化 。SEC文件显示也会移动位置，但渲染图象位置不会受到影响.说明游戏图象显示范围是依据Y64的，物体的渲染也是依据Y64的.地形图和人与物的位置则与这个坐标有关，但渲染图象位置与这个坐标无关。</span>
<span style="color: red"> 各个视角方向渲染物体图象总数加在一起的值就等于：头数据=&gt;基本信息=&gt;全部视角方向渲染的物体图象总和。</span></td>
</tr>
<tr class="even">
<td rowspan="9" style="text-align: center;"><span style="color: blue"> 物体信息</span>
<p><span style="color: blue"> (旧称遮盖索引)</span></p>
<p><span style="color: blue"> <span style="color: #0000FF">Object Info DA</span></span></p></td>
<td colspan="2" rowspan="7" style="text-align: center;"><span style="color: blue"> 物体0</span></td>
<td style="text-align: center;"><span style="color: #0000FF">Type</span></td>
<td style="text-align: center;"><span style="color: blue"> Int32</span></td>
<td style="text-align: center;"><span style="color: blue"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 位域，4表示有斜坡</span></td>
<td style="text-align: center;"><span style="color: blue"> 00000000,</span>
<p><span style="color: blue"> 04000000(TU02.MA2:63Dh),</span></p>
<p><span style="color: blue">  00000</span><span style="color: blue">200(SB.MA2)</span></p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #0000FF">n</span></td>
<td style="text-align: center;"><span style="color: blue"> Int32</span></td>
<td style="text-align: center;"><span style="color: blue"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 此物体所在物体区域块的边数，</span><span style="color: blue">0表示圆(RY:&lt;!--153--&gt;)</span></td>
<td style="text-align: center;"><span style="color: blue"> 04000000</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #0000FF">Center X</span></td>
<td style="text-align: center;"><span style="color: blue"> Float32</span></td>
<td style="text-align: center;"><span style="color: blue"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 此物体所在的空间坐标X值</span></td>
<td style="text-align: center;"><span style="color: blue"> 17B922C3</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #0000FF">Center Y</span></td>
<td style="text-align: center;"><span style="color: blue"> Float32</span></td>
<td style="text-align: center;"><span style="color: blue"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 此物体所在的空间坐标Y值</span></td>
<td style="text-align: center;"><span style="color: blue"> 9C84ABC3</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #0000FF">Center Z</span></td>
<td style="text-align: center;"><span style="color: blue"> Float32</span></td>
<td style="text-align: center;"><span style="color: blue"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 此物体所在的空间坐标Z值</span></td>
<td style="text-align: center;"><span style="color: blue"> 00000000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #0000FF">Object District Info Offset</span></td>
<td style="text-align: center;"><span style="color: blue"> Int32</span></td>
<td style="text-align: center;"><span style="color: blue"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 此物体在物体区域信息内描述开始的相对于物体区域信息起始绝对地址的偏移量</span></td>
<td style="text-align: center;"><span style="color: blue"> 00000000</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #0000FF">Object District Info Length</span></td>
<td style="text-align: center;"><span style="color: blue"> Int32</span></td>
<td style="text-align: center;"><span style="color: blue"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: blue"> 此物体在 物体区域信息内描述数据的长度，值为(边数 * 8 + 20)</span></td>
<td style="text-align: center;"><span style="color: blue"> 340000000</span></td>
</tr>
<tr class="odd">
<td colspan="8" style="text-align: center;"><span style="color: blue"> 多个物体，总数Num Object</span></td>
</tr>
<tr class="even">
<td colspan="8" style="text-align: center;"><span style="color: blue"> 这部分物体信息主要是对应物体区域信息的物体信息。</span>
<span style="color: blue"> 物体数值n为：头数据=&gt;基本信息=&gt;地图内需要渲染的物体总数。</span>
<span style="color: blue"> 每个物体信息用0x1C个字节(28个字节)描述，<span style="color: #0000FF">此段数据总长度为: n * 0x1C。</span></span>
<span style="color: blue"> 此物体所在区域块（SEC文件）的边数影响了此物体在 物体区域信息内描述数据的长度。</span></td>
</tr>
<tr class="odd">
<td rowspan="15" style="text-align: center;"><span style="color: teal"> 渲染索引</span>
<p><span style="color: teal">Render Index DA</span></p></td>
<td rowspan="11" style="text-align: center;"><span style="color: teal"> 视角方向0</span></td>
<td rowspan="8" style="text-align: center;"><span style="color: teal"> 渲染物体0</span></td>
<td style="text-align: center;"><span style="color: #008080">Object Index</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 物体索引号(物体信息里物体的序号)</span></td>
<td style="text-align: center;"><span style="color: teal"> 00000000</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #008080">Num Object</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 固定为1，此渲染索引内物体数量。</span></td>
<td style="text-align: center;"><span style="color: teal"> 01000000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #008080">x</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 渲染区域的渲染外框（为四边形框）的左上角的屏幕坐标X值。<span style="color: teal">当此值为负的时候，说明渲染物体在屏幕外的左边</span></span></td>
<td style="text-align: center;"><span style="color: teal"> F5020000</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #008080">y</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 渲染区域的渲染外框（为四边形框）的左上角的屏幕坐标Y值。当此值为负的时候，说明渲染物体在屏幕外的上边</span></td>
<td style="text-align: center;"><span style="color: teal"> 0C010000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #008080">Width</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 渲染区域的渲染外框宽度</span></td>
<td style="text-align: center;"><span style="color: teal"> 17000000</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #008080">Height</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 渲染区域的渲染外框高度</span></td>
<td style="text-align: center;"><span style="color: teal"> C9000000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #008080">Render Data Offset</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 此视角下此物体渲染数据的开始相对于当前视角渲染数据起始地址的偏移量</span></td>
<td style="text-align: center;"><p><span style="color: teal"> 00000000</span></p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #008080">Render Data Length</span></td>
<td style="text-align: center;"><span style="color: teal"> Int32</span></td>
<td style="text-align: center;"><span style="color: teal"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 此视角下此物体渲染数据的长度</span></td>
<td style="text-align: center;"><span style="color: teal"> 9C010000</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: teal"> ...</span></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"><span style="color: teal"> ...</span></td>
<td style="text-align: center;"><span style="color: teal"> ...</span></td>
<td colspan="2" style="text-align: center;"><span style="color: teal"> ...</span></td>
<td style="text-align: center;"><span style="color: teal"> ...</span></td>
</tr>
<tr class="even">
<td colspan="7" style="text-align: center;"><span style="color: teal"> 多个渲染物体， <span style="color: #008080">总数为视角</span> <span style="color: teal"> 信息中对应视角的 <span style="color: #008080">渲染物体图象总数</span> Num Object To Render</span></span></td>
</tr>
<tr class="odd">
<td colspan="7" style="text-align: center;"><span style="color: teal"> 此方向索引S的直为: 视角信息=&gt;视角方向XX=&gt; <span style="color: #008080">此视角下渲染物体图象总数减1</span></span>
<span style="color: teal"> 索引的序列号指向”物体信息”里的物体n顺序号.序列号从小到大排列，不一定是连续数值.</span>
<span style="color: teal">  渲染外框左上角的XY坐标点和渲染外框的宽度和高度决定了此物体渲染外框(白色图象)显示位置.</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: teal"> 视角方向90</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 类似视角方向0</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: teal"> 视角方向180</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 类似视角方向0</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: teal"> 视角方向270</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
<td colspan="2" style="text-align: left;"><span style="color: teal"> 类似视角方向0</span></td>
<td style="text-align: center;"><span style="color: teal"> ..</span></td>
</tr>
<tr class="odd">
<td colspan="8" style="text-align: center;"><span style="color: teal"> 每个索引都需要0x20个字节(32个字节)，那么”头数据=&gt; <span style="color: #008080">基本信息=&gt;全部视角方向渲染的物体图象总和”这个数值乘以0x20就是渲染索引的数据总长度。</span></span></td>
</tr>
<tr class="even">
<td rowspan="7" style="text-align: center;">流动水渲染数据
<p>Water Mask Data DA</p></td>
<td colspan="2" rowspan="6" style="text-align: center;">视角方向 0
<p>View 0</p></td>
<td style="text-align: center;">Width</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: center;">宽度</td>
<td style="text-align: center;">D8060000</td>
</tr>
<tr class="odd">
<td style="text-align: center;">Height</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: center;">高度</td>
<td style="text-align: center;">42050000</td>
</tr>
<tr class="even">
<td style="text-align: center;">Byte Width</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: center;">字节宽度</td>
<td style="text-align: center;">0E000000</td>
</tr>
<tr class="odd">
<td colspan="6" style="text-align: center;">多个字节宽度，总数Height</td>
</tr>
<tr class="even">
<td style="text-align: center;">RLE Encoded Data</td>
<td style="text-align: center;">Byte()</td>
<td style="text-align: center;">Byte Width</td>
<td colspan="2" style="text-align: center;">RLE编码数据，(Value, Count)方式编码，Value和Count各为1字节。编码的数据Value的取值范围为0、1、2，和渲染数据一样</td>
<td style="text-align: center;">00FF00FF..</td>
</tr>
<tr class="odd">
<td colspan="6" style="text-align: center;">多个RLE编码数据，总数Height</td>
</tr>
<tr class="even">
<td colspan="8" style="text-align: center;">多种质量 * 多个视角方向，总数不定</td>
</tr>
<tr class="odd">
<td rowspan="15" style="text-align: center;"><span style="color: maroon"> 渲染数据</span>
<p><span style="color: maroon"> Render Data DA</span></p></td>
<td rowspan="11" style="text-align: center;"><span style="color: maroon"> 视角方向0</span></td>
<td rowspan="10" style="text-align: center;"><span style="color: maroon"> 渲染物体0</span></td>
<td style="text-align: center;"><span style="color: #800000">Identifying Sign</span></td>
<td style="text-align: center;"><span style="color: maroon"> Int32</span></td>
<td style="text-align: center;"><span style="color: maroon"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: maroon"> 标志码，ASC码为Mdlt， <span style="color: #800000">此视角下每个物体开始都为此标志码</span></span></td>
<td style="text-align: center;"><span style="color: maroon"> 4D646C74</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #800000">Width</span></td>
<td style="text-align: center;"><span style="color: maroon"> Int32</span></td>
<td style="text-align: center;"><span style="color: maroon"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: maroon"> 渲染物体的象素宽度，等同于渲染索引中的对应值.</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: #800000">Height</span></td>
<td style="text-align: center;"><span style="color: maroon"> Int32</span></td>
<td style="text-align: center;"><span style="color: maroon"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: maroon"> 渲染物体的象素高度，等同于渲染索引中的对应值.</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: #800000">Remained Length</span></td>
<td style="text-align: center;"><span style="color: maroon"> Int32</span></td>
<td style="text-align: center;"><span style="color: maroon"> 4</span></td>
<td colspan="2" style="text-align: left;"><span style="color: maroon"> 下一双字开始地址到此物体渲染数据结束地址的长度。这个长度加上0x10(即前面这4个双字)，就是此视角下此物体 渲染数据的全部长度 ，等同于渲染索引=&gt; <span style="color: #800000">对应视角方向xx=&gt;对应索引m=&gt;此物体的渲染数据的长度。</span></span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td rowspan="3" style="text-align: center;"><span style="color: maroon"> 渲染子数据索引</span>
<p><span style="color: maroon"> <span style="color: #800000">Sub Render Data Offset</span></span></p></td>
<td rowspan="3" style="text-align: center;"><span style="color: maroon"> Int32</span></td>
<td rowspan="3" style="text-align: center;"><span style="color: maroon"> m * 4</span>
<p><span style="color: maroon"> 其中m=Floor(</span><span style="color: #800000">Height</span><span style="color: maroon">/8)+1</span></p></td>
<td style="text-align: left;"><span style="color: maroon"> 4</span></td>
<td style="text-align: left;"><span style="color: maroon"> 此物体子部分0相对于渲染子数据起始绝对地址的偏移量</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: left;"><span style="color: maroon"> …</span></td>
<td style="text-align: left;"><span style="color: maroon"> …</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: left;"><span style="color: maroon"> 4</span></td>
<td style="text-align: left;"><span style="color: maroon"> 此物体子部分m-1相对于渲染子数据起始绝对地址的偏移量，当高度为8的倍数时，最后一个索引为乱码</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td rowspan="3" style="text-align: center;"><span style="color: maroon"> 渲染子数据起始</span>
<p><span style="color: maroon"> <span style="color: #800000">Sub Render Data</span></span></p></td>
<td rowspan="3" style="text-align: center;"><span style="color: maroon"> Int32</span></td>
<td rowspan="3" style="text-align: center;"><span style="color: #800000">Remained Length - </span> <span style="color: maroon"> m * 4</span></td>
<td style="text-align: left;"><span style="color: maroon"> …</span></td>
<td style="text-align: left;"><span style="color: maroon"> 此物体子部分0渲染数据，调试图象中白色部分就由此而来</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: left;"><span style="color: maroon"> …</span></td>
<td style="text-align: left;"><span style="color: maroon"> …</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: left;"><span style="color: maroon"> …</span></td>
<td style="text-align: left;"><span style="color: maroon"> 此物体子部分m的渲染数据</span></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td colspan="7" style="text-align: center;"><span style="color: maroon">  多个索引，总数为视角信息中对应视角的渲染物体图象总数Num Object To Render</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: maroon"> 视角方向90</span></td>
<td colspan="7" style="text-align: center;"><span style="color: maroon"> 类似视角方向0</span></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><span style="color: maroon"> 视角方向180</span></td>
<td colspan="7" style="text-align: center;"><span style="color: maroon"> 类似视角方向0</span></td>
</tr>
<tr class="even">
<td style="text-align: center;"><span style="color: maroon"> 视角方向270</span></td>
<td colspan="7" style="text-align: center;"><span style="color: maroon"> 类似视角方向0</span></td>
</tr>
<tr class="odd">
<td colspan="8" style="text-align: center;"><span style="color: maroon"> 这部分代码只有俩种可能，一种是指向某个模型文件.一种是说明了此位置的此渲染外框内的各个子部分的小图象块内需要渲染的象素点.但这种情况下渲染点的高度是怎么判断的还不明白???.不过似乎没高度判断， <span style="color: #800000">应该是在平面图上 ，人物覆盖的地方有个渲染判断(前后位置判断)，若有则渲染图片在人物上面显示，否则被人物图象覆盖。</span></span></td>
</tr>
<tr class="even">
<td rowspan="12" style="text-align: center;">物体区域信息
<p>(旧称遮盖索引)</p>
<p>Object District Info DA</p></td>
<td colspan="2" rowspan="8" style="text-align: center;">多边形棱柱
<p>(物体信息中n非0)</p></td>
<td style="text-align: center;">n</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;">物体所在物体区域块的边数或顶点数</td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;">nx</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td colspan="2" rowspan="3" style="text-align: left;">法向量</td>
<td style="text-align: center;">00000000</td>
</tr>
<tr class="even">
<td style="text-align: center;">ny</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td style="text-align: center;">00000000</td>
</tr>
<tr class="odd">
<td style="text-align: center;">nz</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td style="text-align: center;">0000803F (1.0)</td>
</tr>
<tr class="even">
<td style="text-align: center;">D</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;">通常为负的，比如-52，-200，-68</td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;">X 0</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;">点0偏移量横坐标</td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;">Y 0</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: left;">点0偏移量纵坐标</td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td colspan="6" style="text-align: center;">X，Y共有n对</td>
</tr>
<tr class="even">
<td colspan="2" rowspan="2" style="text-align: center;">圆柱
<p>(物体信息中n为0)</p></td>
<td style="text-align: center;">Radius</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: center;">半径</td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;">Altitude</td>
<td style="text-align: center;">Float32</td>
<td style="text-align: center;">4</td>
<td colspan="2" style="text-align: center;">高度</td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td colspan="8" style="text-align: center;">物体共Num Object个</td>
</tr>
<tr class="odd">
<td colspan="8" style="text-align: center;"><p>XY坐标偏移值要加上“视角信息=&gt;某视角方向=&gt;XY游戏坐标0点调整值”才会得到此方向视角游戏坐标的XY值。</p>
<p>物体信息和物体区域信息决定了是否物体。物体信息全变为0或删除 物体区域信息，都不能物体，但渲染数据照旧可以渲染。应该反过来说，物体信息和物体区域信息决定了某些点组成的区域是否会产生物体，不一定需要匹配SEC文件的区域块，这就是换了其他关的MA2文件后，一样会出现物体的原因 。</p>
<p>确定了物体区域后，“渲染索引”里的“渲染外框(白色渲染图象)”必须处于此物体区域内，这才会完成正确的渲染物体。</p>
<p>物体区域的范围可以通过调试工具中的“Volumenes Escenario”选项打开的紫色框图看到。</p>
<p>此紫色框图内部区域满足如下条件：</p>
<p>1) (x, y)在多边形(CenterX, CenterY) + (Xk, Yk), k = 0, 1, .. n - 1内部</p>
<p>2) dot(<strong>nxyz</strong>, <strong>xyz - CenterXYZ</strong>) + D &lt;= 0，</p>
<p>即 nx * (x - CenterX) + ny * (y - CenterY) + nz * (z - CenterZ) + D &lt;= 0</p>
<p>3) nz * (z - CenterZ) &gt;= 0</p>
<p>当Object Info DA中的n = 0时区域为一个圆，但这里的n = 1，X0和Y0分别为半径和高度。</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

<span style="color: #008080">渲染子数据(Render Data)格式：</span>

<span style="color: #008080">按 首字节分为两种格式  
  
  
当首字节最低位为1时：  
首字节除最低位的每一位：如果为0，表示这一行数据均为默认修正数据；如果为1，表示应从数据流中提取一行数据来解压，这一行数据指一个行标志字节和其后的压缩数据。每个数据流有且仅有8行。  
  
压缩数据：首字节 行标志字节 （数据 数据 ……） 行标志字节 （数据 数据 ……） 行标志字节 （数据 数据 ……） ……  
中间数据：（32位整数 \* 5） ……  
最终数据：字节 字节 字节 ……  
  
行标志字节  
标志字节有两种模式：最低位为0和最低位为1。  
如果最低位为0，则高7位每一位（从位1开始）对应一个数据：如果为1，表示应该应读取一个数据（通常是修正数据）；如果为0，表示什么也不做。  
如果最低位为1，则高7位表示后面的数据个数（通常是标准数据，个数通常不大于6）。  
  
数据  
一个数据生成中间数据行内块描述符（共20字节）。  
数据有两种模式：最低位为0和最低位为1。  
如果最低位为0，则表示该数据只有这一个字节，称修正数据。  
如果最低为为1，则表示该数据有4个字节，即一个低字节优先32位整数，称标准数据。  
两种数据均记作32位有符号整数Data。  
修正数据不能位于数据流的最前面，属于修正前一数据的数据。  
  
每一个数据的状态由行内块描述符（共20字节）进行描述  
record SpanDescriptor  
  IsLineStart : Boolean //占用字节0，置位表示整行的开始像素索引在该行内块中，且StartIndex为半透明  
  IsLineEnd : Boolean //占用字节1，置位表示整行的结束像素索引在该行内块中，且EndIndex-1为半透明  
  StartIndex : Int32 //占用字节4-7，开始像素索引  
  EndIndex : Int32 //占用字节8-11，结束像素索引  
  StartAddition : Int32 //占用字节12-15，开始像素索引增量  
  EndAddition : Int32 //占用字节16-19，结束像素索引增量  
  
标准数据  
表示  
new SpanDescriptor  
  IsLineStart = Data\[3\]  
  IsLineEnd = Data\[2\]  
  StartIndex = Data\[31..22\]  
  EndIndex = Data\[21..12\]  
  StartAddition : Data\[11..8\] - 7  
  EndAddition : Data\[7..4\] - 7  
  
修正数据  
记上一行对应块为SpanInPrevLine，则修正数据表示  
new SpanDescriptor  
  IsLineStart = Data\[3\]  
  IsLineEnd = Data\[2\]  
  StartIndex = SpanInPrevLine.StartIndex + SpanInPrevLine.StartAddition  
  EndIndex = SpanInPrevLine.EndIndex + SpanInPrevLine.EndAddition  
  StartAddition = SpanInPrevLine.StartAddition + Data\[7..6\] - 1  
  EndAddition = SpanInPrevLine.EndAddition + Data\[5..4\] - 1  
  
如果某个行标志字节中没有修正数据，则表示  
new SpanDescriptor  
  IsLineStart = SpanInPrevLine.IsLineStart  
  IsLineEnd = SpanInPrevLine.IsLineEnd  
  StartIndex = SpanInPrevLine.StartIndex + SpanInPrevLine.StartAddition  
  EndIndex = SpanInPrevLine.EndIndex + SpanInPrevLine.EndAddition  
  StartAddition = SpanInPrevLine.StartAddition  
  EndAddition = SpanInPrevLine.EndAddition  
  
如果开始像素索引和结束像素索引超过范围，应该使其在两端点上。  
  
  
当首字节最低位为0时：  
首先按8像素一组计算X方向上的组数NumXBlock=Ceil(Width/8)。  
最开始的Floor((1+NumXBlock)/4)+1个字节 （包括首字节）中，除首字节最低两位外，其他每两位表示一个8x8像素组。当Width Mod 32 = 24时，其中的最后一个字节不包含实际信息。  
这每两位，如果为0、1、2，则分别表示这一组全部都是全透明、半透明、不透明。  
如果为3，则表示8x8像素组不是全部都为同种颜色，需从后面读入8个低字节优先的Int16的数据（16个字节），每个Int16表示一行。每个Int16的每两位表示一个像素。  
  
  
简单的例子：  
左边的柱子图块1（宽6）  
01 03 71 47 40 00h  
  
其中第一行只有一个数据71 47 40 00h，即  
0000 0000 0100 0000 0100 0111 0111 0001b  
  
其中间数据为：  
整数0  
00 00 00 00h  
  
整数1  
01 00 00 00h  
  
整数2  
04 00 00 00h  
  
整数3  
00 00 00 00h  
  
整数4  
00 00 00 00h  
  
也就是说，得到的图形为  
011100b  
011100b  
011100b  
011100b  
011100b  
011100b  
011100b  
011100b  
  
  
复杂的例子1：  
牌子图块2（宽41）  
DF 03 75 75 82 06 02 9C 02 10 02 98 02 10 02 A8 02 54h  
  
头数据：DFh = 11011111b，表示仅有第6行重复第5行的数据。  
其中第一行只有一个数据75 75 82 06h，即  
0000 0110 1000 0010 0111 0101 0111 0101b  
其中间数据为：  
00 01 00 00h  
1A 00 00 00h  
27 00 00 00h  
FE FF FF FFh  
00 00 00 00h  
其最终数据为：  
00000000 00000000 00000000 00111111 11111110 0b  
  
其中第二行只有一个数据9Ch，即  
1001 1100b  
其中间数据为：  
00 01 00 00h  
18 00 00 00h  
27 00 00 00h  
01 00 00 00h  
00 00 00 00h  
最终数据为：  
00000000 00000000 00000000 11111111 11111110 0b  
  
以下略。  
  
  
复杂的例子2：以00开始的数据  
牌子图块1（宽41）  
00 0F 00 80 00 90 00 90 00 90 00 A0 00 A0 00 A4 00 AA  
01 00 86 26 86 2A A6 2A AA 2A AA 2A AA 1A AA 1Ah  
  
最开始的几个字节中，除最低两位外，其他相邻两位，如果为11b，则表示应详细解压，否则表示该8像素为该两位表示的状态。  
00 0Fh  
0000 1111 0000 0000b  
这里除最低位00b用于判断模式以外，之后有3个00b，表示有3个8像素区域为空。  
再之后有2个11b，表示有2个8像素区域需要详细解压。  
再之后有2个00b，但只有1个有效，最后1个用于占位。  
  
其他数据每16字节为一组，每组中有8对字节，每对字节表示对应行的相应详细解压的部分的8个像素。每个像素占两位。  
下面是第1、2、8行的数据：  
00 00 00 00 00 00 00 01 10 00 00 00 00 00 00 00b  
00 10 01 10 10 00 01 10 10 01 00 00 00 00 00 00b  
00 01 10 10 10 10 10 10 10 10 10 10 00 00 00 00b  
  
  
最终数据  
最终数据是由字节组成，每个字节和一个像素对应。其中0表示透明，1表示半透明，2表示不透明。  
 </span>

参考：

\[1\]invox4C2_3keyfiles.doc，盗版钦差，2006

盟军敢死队2及3 SEC文件格式表

<p>&nbsp;</p>

地狱门神（F.R.C.）整理

<p>&nbsp;</p>
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
<td rowspan="9"><p>Head DA</p></td>
<td rowspan="5"><p>Identifying DB</p></td>
<td><p>Version Sign DS</p></td>
<td><p>Version Sign</p></td>
<td><p>Int64</p></td>
<td><p>8</p></td>
<td><p>固定</p></td>
<td><p>01000000 01000000</p>
<p>/</p>
<p>02000000 01000000</p></td>
</tr>
<tr class="even">
<td rowspan="3"><p>Zone DS</p></td>
<td><p>Num Zone</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>分区数量</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>Zone Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>分区名称</p></td>
<td><p>"POSTE"</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>Num Zone 个区域</p></td>
</tr>
<tr class="odd">
<td><p>Map Token DS</p></td>
<td><p>Map Token</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>固定</p></td>
<td><p>"MAP1"</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Basic Info DB</p></td>
<td rowspan="4"></td>
<td><p>Number Point</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了点的数量</p></td>
<td><p>15010000</p></td>
</tr>
<tr class="odd">
<td><p>Number Borders</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了边的数量</p></td>
<td><p>47030000</p></td>
</tr>
<tr class="even">
<td><p>Number District</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了区域面的数量，包括特殊面的数量</p></td>
<td><p>A4000000</p></td>
</tr>
<tr class="odd">
<td><p>Number Special District</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>这里定义了特殊面的数量</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td rowspan="27"><p>Main DA</p></td>
<td rowspan="3"><p>Point DB</p></td>
<td rowspan="2"><p>Point DS</p></td>
<td><p>x</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>横坐标</p></td>
<td><p>EE5C0DC3</p></td>
</tr>
<tr class="odd">
<td><p>y</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>纵坐标</p></td>
<td><p>42E04543</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>... Point DSs: 总数由Number of Points定义</p>
<p>点的数据字节总数为: Number of Points * 8</p></td>
</tr>
<tr class="odd">
<td rowspan="6"><p>Border DB</p></td>
<td rowspan="5"><p>Border DS</p></td>
<td><p>Start Point Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>起始点索引</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>End Point Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>结束点索引</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>Parent District ID</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>隶属于的区域面ID，ID并不是索引，区域面的ID由此值决定</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>Neighbor District Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>临界的区域面索引，0xFFFFFFFF表示该线在地图边上，外部不再有区域</p></td>
<td><p>11000000</p></td>
</tr>
<tr class="odd">
<td><p>Adjacency</p></td>
<td><p>UInt32</p></td>
<td><p>4</p></td>
<td><p>邻接信息：1未知（KWEX一处悬崖附近） 2表示上下双重面连接（RYEX河边的木头浮桥码头） 8表示没有邻接面（TU03I01梯子） 16未知（ISEX） 32未知（SBC00） 64未知（SBC00梯子）</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>... Border DSs: 总数由Number of Borders定义， 区域面 的线数据的描述顺序为逆时针。</p>
<p>线数据占字节总数为：Number of Borders * 20</p></td>
</tr>
<tr class="odd">
<td rowspan="18"><p>District DB</p></td>
<td rowspan="17"><p>District DS</p></td>
<td><p>n</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>边数</p></td>
<td><p>05000000</p></td>
</tr>
<tr class="even">
<td><p>Terrain</p></td>
<td><p>TerrainInfo</p></td>
<td><p>8</p></td>
<td><p>地形信息</p></td>
<td><p>00000000 14000000</p></td>
</tr>
<tr class="odd">
<td><p>kx</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>与X轴夹角正切</p></td>
<td><p>00000080</p></td>
</tr>
<tr class="even">
<td><p>ky</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>与Y轴夹角正切</p></td>
<td><p>00000080</p></td>
</tr>
<tr class="odd">
<td><p>bz</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>Z轴的截距</p></td>
<td><p>00001C42</p></td>
</tr>
<tr class="even">
<td><p>Unknown</p></td>
<td><p>Int64</p></td>
<td><p>8</p></td>
<td></td>
<td><p>00000000 00000000</p></td>
</tr>
<tr class="odd">
<td><p>Zone Flags</p></td>
<td><p>Int64</p></td>
<td><p>8</p></td>
<td><p>分区标记，每位二进制表示是否在对应分区中</p></td>
<td><p>02000000 00000000</p></td>
</tr>
<tr class="even">
<td><p>Unknown</p>
<p>(Comm2)</p></td>
<td><p>Int64</p></td>
<td><p>8</p></td>
<td></td>
<td><p>00000000 00000000</p></td>
</tr>
<tr class="odd">
<td><p>MinPx</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>所有点的X坐标的最小值</p></td>
<td><p>21300FC3</p></td>
</tr>
<tr class="even">
<td><p>MinPy</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>所有点的Y坐标的最小值</p></td>
<td><p>87564243</p></td>
</tr>
<tr class="odd">
<td><p>MinPz</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>所有点的Z坐标的最小值</p></td>
<td><p>00001C42</p></td>
</tr>
<tr class="even">
<td><p>MaxPx</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>所有点的X坐标的最大值</p></td>
<td><p>8115D0C2</p></td>
</tr>
<tr class="odd">
<td><p>MaxPy</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>所有点的Y坐标的最大值</p></td>
<td><p>04B65B43</p></td>
</tr>
<tr class="even">
<td><p>MaxPz</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>所有点的Z坐标的最大值</p></td>
<td><p>00001C42</p></td>
</tr>
<tr class="odd">
<td><p>Border Index 0</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>区域的边索引</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>Border Index 1</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>区域的边索引</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>... Border Indices: 总数由n定义</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>... District DSs: 总数由Number of Districts定义</p></td>
</tr>
<tr class="odd">
<td rowspan="9"><p>Tail DA</p></td>
<td><p>Symbol DB</p></td>
<td></td>
<td><p>Symbol</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>尾部数据的开始</p></td>
<td><p>32534148</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Basic info DB</p></td>
<td rowspan="4"></td>
<td><p>Total Number of Districts</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>网格包含区域的数量的总和</p></td>
<td><p>6E050000</p></td>
</tr>
<tr class="odd">
<td><p>Unknown</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>未知数据</p></td>
<td><p>06000000</p></td>
</tr>
<tr class="even">
<td><p>Number Mesh in X</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>X轴方向的网格数量</p></td>
<td><p>17000000</p></td>
</tr>
<tr class="odd">
<td><p>Number Mesh in Y</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Y轴方向的网格数量</p></td>
<td><p>18000000</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Mesh DB</p>
<p>(Reseau)</p>
<p>(网格的X、Y的起点在Point DB中X、Y的最小值处)</p></td>
<td rowspan="3"><p>Mesh DS</p></td>
<td><p>Number District in Mesh</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>此网格包含区域的数量</p></td>
<td><p>03000000</p></td>
</tr>
<tr class="odd">
<td><p>District Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>区域索引</p></td>
<td><p>DC000000</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>... District Index: 总数由Number of Districts in the Mesh确定</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>Mesh DSs: 总数由Number Mesh in X * Number Mesh in Y确定</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Terrain

<table>
<colgroup>
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 35%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 10%" />
</colgroup>
<thead>
<tr class="header">
<th style="text-align: center;"><p>字节</p></th>
<th style="text-align: center;"><p>名称</p></th>
<th style="text-align: center;"><p>描述</p></th>
<th style="text-align: center;"><p>编号</p></th>
<th style="text-align: center;"><p>原文</p></th>
<th style="text-align: center;"><p>类型</p></th>
<th style="text-align: center;"><p>样例</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="5" style="text-align: center;"><p>0</p></td>
<td rowspan="5" style="text-align: center;"><p>MajorType</p></td>
<td rowspan="5" style="text-align: center;"><p>主类型</p></td>
<td style="text-align: center;"><p>0</p></td>
<td style="text-align: center;"><p>TIERRA</p></td>
<td style="text-align: center;"><p>陆地</p></td>
<td style="text-align: center;"><p>TU01</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>1</p></td>
<td style="text-align: center;"><p>NIEVE</p></td>
<td style="text-align: center;"><p>雪地</p></td>
<td style="text-align: center;"><p>HL</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>2</p></td>
<td style="text-align: center;"><p>AGUA</p></td>
<td style="text-align: center;"><p>水</p></td>
<td style="text-align: center;"><p>TU04河</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>3</p></td>
<td style="text-align: center;"><p>ORILLA</p></td>
<td style="text-align: center;"><p>岸边</p></td>
<td style="text-align: center;"><p>TU04河滩</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>4</p></td>
<td style="text-align: center;"><p>SUBMARINO</p></td>
<td style="text-align: center;"><p>水下</p></td>
<td style="text-align: center;"><p>SB水下</p></td>
</tr>
<tr class="even">
<td rowspan="16" style="text-align: center;"><p>1</p></td>
<td rowspan="16" style="text-align: center;"><p>MinorType</p></td>
<td rowspan="16" style="text-align: center;"><p>显示效果、声音效果</p></td>
<td style="text-align: center;"><p>0</p></td>
<td style="text-align: center;"><p>ASFALTO</p></td>
<td style="text-align: center;"><p>沥青</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>TU01</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>1</p></td>
<td style="text-align: center;"><p>HIERBA</p></td>
<td style="text-align: center;"><p>草地</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>TU01</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>2</p></td>
<td style="text-align: center;"><p>TIERRA</p></td>
<td style="text-align: center;"><p>土壤</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>TU01路</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>3</p></td>
<td style="text-align: center;"><p>ADOQUINES</p></td>
<td style="text-align: center;"><p>铺制路</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>KW左边渡口右侧最大的小岛靠岸方向邻近的小岛区域297</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>4</p></td>
<td style="text-align: center;"><p>AZULEJOS</p></td>
<td style="text-align: center;"><p>瓷砖</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>3TE右上角房子的阳台</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>5</p></td>
<td style="text-align: center;"><p>MADERA</p></td>
<td style="text-align: center;"><p>木制地面</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>TU03初始位置</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>6</p></td>
<td style="text-align: center;"><p>ARENA</p></td>
<td style="text-align: center;"><p>沙地</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>CZ下游河滩</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>7</p></td>
<td style="text-align: center;"><p>NIEVE</p></td>
<td style="text-align: center;"><p>雪地</p>
<p>(雪地)</p></td>
<td style="text-align: center;"><p>HL</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>8</p></td>
<td style="text-align: center;"><p>HIELO</p></td>
<td style="text-align: center;"><p>冰面</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>3ST1河的冰面</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>9</p></td>
<td style="text-align: center;"><p>ROCAS</p></td>
<td style="text-align: center;"><p>石头地面</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>CZ中游河滩</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>10</p></td>
<td style="text-align: center;"><p>ARBUSTOS</p></td>
<td style="text-align: center;"><p>灌木</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>3TC长有农作物的农田</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>11</p></td>
<td style="text-align: center;"><p>METAL</p></td>
<td style="text-align: center;"><p>金属地面</p>
<p>(铁栏杆、铁楼梯)</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>SB的灯塔附近的楼梯</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>12</p></td>
<td style="text-align: center;"><p>METAL_ENCHARCADO</p></td>
<td style="text-align: center;"><p>浸水金属地面</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>3TA1车厢顶部</p>
<p>3TV最大房子的楼梯</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>13</p></td>
<td style="text-align: center;"><p>ORILLA</p></td>
<td style="text-align: center;"><p>岸边</p>
<p>(岸边)</p></td>
<td style="text-align: center;"><p>TU04河滩</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>14</p></td>
<td style="text-align: center;"><p>AGUA_PROFUNDA</p></td>
<td style="text-align: center;"><p>深水</p>
<p>(水/水下)</p></td>
<td style="text-align: center;"><p>TU04河</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>15</p></td>
<td style="text-align: center;"><p>GRAVILLA</p></td>
<td style="text-align: center;"><p>石子地</p>
<p>(陆地)</p></td>
<td style="text-align: center;"><p>SB铁路</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>2</p></td>
<td style="text-align: center;"><p>Zero</p></td>
<td style="text-align: center;"></td>
<td colspan="4" style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>3</p></td>
<td style="text-align: center;"><p>Zero</p></td>
<td style="text-align: center;"></td>
<td colspan="4" style="text-align: center;"></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

| 字节（位） |            原文             |           名称            |                                   描述                                    |                                 样例                                 |
|:----------:|:---------------------------:|:-------------------------:|:-------------------------------------------------------------------------:|:--------------------------------------------------------------------:|
|    4(0)    |           TRANSIT           |        Transitable        |                              可通行。可进入                               |                              TU01的陆地                              |
|    4(1)    |           OCLUSOR           |        IsOcclusion        |         阻挡。在区域外面无法看见区域内，进入区域后才能看见区域内          |                              TU01的树丛                              |
|    4(2)    |                             |          IsPlane          |       水平的。0表示是斜坡。如果是斜坡而没有设置为0，则会认为是悬崖        |                  普遍为True，反例TU03碉堡门前的斜坡                  |
|    4(3)    |            DOBLE            |          Double           | 双重区域块。区域块下方为空，上方为实。0表示正常的下方为实，上方为空的情况 |                    TU03初始位置附近的下面有洞的墙                    |
|    4(4)    |        NO_DIBU_VISTA        |      PlotNotVisible       |            不显示图框。区域中不显示视野，但是视野仍然可能存在             |                              TU01的树丛                              |
|    4(5)    |         SOLO_AGACH          |         BendOnly          |                            只能弯腰。必须爬行                             |                              TU01的木牌                              |
|    4(6)    | NO_PERM_EXPL, OCLUSOR EXPLO |  IsExplosionOcclusion　   |                 阻挡爆炸。爆炸无法通过该区域传播到另一边                  |                SB潜艇潜望镜室中央的圆形舱门上方的区域                |
|    4(7)    |          RALENTIZA          |        　MakeSlow         |                                   减速                                    |                            HL最右边的边界                            |
|    5(0)    |         ENTERRABLE          |         Buriable          |                               可挖(1#，5#)                                |                              TU02的土地                              |
|    5(1)    |          ESCALERA           |         IsLadder          |                        楼梯，人物走时会一步一步踏                         |                          SB的灯塔附近的楼梯                          |
|    5(2)    |        DEJAN_HUELLAS        |      LeaveFootprints      |                                会留下脚印                                 |                           TU02盟军所在的田                           |
|    5(3)    |          ALAMBRADA          |       IsBarbedWire        |                         铁丝网。只有小偷才能爬行                          |       TU01中与离初始位置近的铁丝网中间支柱区域共边的三角形区域       |
|    5(4)    |        SOLO CANIJOS         |         PunyOnly          |                  只有小不点。只有老鼠能进入，狗不能进入                   |                  SB碉堡附近的上面有防空炮的圆台下面                  |
|    5(5)    |        SECTOR-VALLA         |       IsFenceSector       |                          栅栏区域。能够架设悬梯                           |                          RY初始位置附近的墙                          |
|    5(6)    |       VALLA-OCLUSORA        |    　IsFenceOcclusion     |                          栅栏式阻挡。(效果不明)                           |                         CZ的有护栏的桥的护栏                         |
|    5(7)    |           SOMBRA            |         IsShadow          |                                  阴影区                                   |                                 TU01                                 |
|    6(0)    |         HASTA TECHO         |         ToCeiling         |               到天花板。一般用于可以击打的墙面。(效果不明)                |                       TU03碉堡内可以击打的墙面                       |
|    6(1)    |         FUERA_MAPA          |         IsMapOut          |                   地图边界。走动时会自动向正常区域行走                    |                                 TU01                                 |
|    6(2)    |                             |    　MouseNotHoverable    |                      鼠标无法进入的块，但人可以路过                       |                 RY关老大的房子外走道的军官上方的区域                 |
|    6(3)    |        NO_ACANTILADO        |         　NoCliff         |                           没有悬崖。(效果不明)                            |                SB潜艇瞭望台中央的圆形舱门上方的区域？                |
|    6(7)    |    AUTOTRANSFER PEATONES    | 　AutotransferPedestrians |                         自动切换行人图标。门标记                          |                                 3TM                                  |
|    7(6)    |       INTR_VEHICULOS        |    InteriorVehicles　     |                           车辆内部。(效果不明)                            | SB围墙上的小碉堡的台子下的区域、KW初始位置右边的树等悬空物体下的区域 |
|    7(7)    |         COLCHONETA          |           IsPad           |              垫子。8#从高处跳下不受伤，海防爬上挂钩跳到航母               |                        PT航母凸出部分的一部分                        |

<p>&nbsp;</p>

注：

1、所有的整数数据类型都是little-endian的。

2、有“/”的项，盟军2的在左边，盟军3的在右边。

3、对盟军2模型的进一步理解表明，

Point实际上是指方程为

x = x0

y = y0

的铅直线，Border实际上是指两条铅直线的连线的面。

District实际上是指一个直柱 （侧面为直角梯形），它的上底面方程为

z = kx \* x + ky \*y + bz

下底面为水平面，其余各面均与水平面垂直。

Special District是指一个District，它的一个或几个侧面是可通过的，用相应的Border的Neighbor District Index = -1表示，所以Special District和其他District的数据结构并没有分开。

以上名称很不准确，但是为了避免改动，姑且沿用旧称。

4、整个模型使用的坐标系为右手坐标系、世界坐标系：亦即x、y表示两个水平方向，z表示高度，z值正方向向上。

5、从俯视坐标系看时，顶点逆时针顺序的区域显示正面。

参考：

\[1\]invox4C2_3keyfiles.doc，盗版钦差，2006

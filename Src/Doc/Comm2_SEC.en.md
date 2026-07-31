Commandos2&3 SEC File Structure Table

<p>&nbsp;</p>

Arranged by F.R.C.

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
<th><p>Data Area</p></th>
<th><p>Data Block</p></th>
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
<td rowspan="9"><p>Head DA</p></td>
<td rowspan="5"><p>Identifying DB</p></td>
<td><p>Version Sign DS</p></td>
<td><p>Version Sign</p></td>
<td><p>Int64</p></td>
<td><p>8</p></td>
<td><p>Fixed</p></td>
<td><p>01000000 01000000</p>
<p>/</p>
<p>02000000 01000000</p></td>
</tr>
<tr class="even">
<td rowspan="3"><p>Zone DS</p></td>
<td><p>Num Zone</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Number of zones</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>Zone Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>Zone name</p></td>
<td><p>"POSTE"</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>total number of zones: Num Zone</p></td>
</tr>
<tr class="odd">
<td><p>Map Token DS</p></td>
<td><p>Identifying Sign</p></td>
<td></td>
<td><p>32</p></td>
<td><p>Fixed</p></td>
<td><p>MAP1...</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Basic Info DB</p></td>
<td rowspan="4"></td>
<td><p>Number of Points</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This define the amount of points</p></td>
<td><p>15010000</p></td>
</tr>
<tr class="odd">
<td><p>Number of Borders</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This define the amount of borders</p></td>
<td><p>47030000</p></td>
</tr>
<tr class="even">
<td><p>Number of Districts</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This define the amount of districts, including special districts</p></td>
<td><p>A4000000</p></td>
</tr>
<tr class="odd">
<td><p>Number of Special Districts</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>This define the amount of special districts</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td rowspan="27"><p>Main DA</p></td>
<td rowspan="3"><p>Point DB</p></td>
<td rowspan="2"><p>Point DS</p></td>
<td><p>x</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>X-coordinate</p></td>
<td><p>EE5C0DC3</p></td>
</tr>
<tr class="odd">
<td><p>y</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>Y-coordinate</p></td>
<td><p>42E04543</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>... Point DSs: total number defined by Number of Points</p>
<p>The total length of the Point DB is: Number of Points * 8</p></td>
</tr>
<tr class="odd">
<td rowspan="6"><p>Border DB</p></td>
<td rowspan="5"><p>Border DS</p></td>
<td><p>Start Point Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Index for Start Point</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>End Point Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Index for End Point</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td><p>Parent District ID</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>ID for the district which the border belongs to. A ID is not a index. The ID is only determined by this value.</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>Neighbor District Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Index for the district with the border as a boundary between it and the Parent District. 0xFFFFFFFF represents the border is on the edge of the map</p></td>
<td><p>11000000</p></td>
</tr>
<tr class="odd">
<td><p>Adjacency</p></td>
<td><p>UInt32</p></td>
<td><p>4</p></td>
<td><p>Adjacency info: 1 Unknown(KWEX near cliff) 2 Double plane adjacency(RYEX floating dock near river) 8 No adjacent district (TU03I01 ladder) 16 Unknown (ISEX) 32 Unknown (SBC00) 64 Unknown(SBC00 ladder)</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>... Border DSs: total number is defined by Number of Borders, and the borders of a district are in anticlockwise order.</p>
<p>The total length of the Border DBs is:  Number of Borders * 20</p></td>
</tr>
<tr class="odd">
<td rowspan="18"><p>District DB</p></td>
<td rowspan="17"><p>District DS</p></td>
<td><p>n</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The number of borders</p></td>
<td><p>05000000</p></td>
</tr>
<tr class="even">
<td><p>Terrain</p></td>
<td><p>TerrainInfo</p></td>
<td><p>8</p></td>
<td><p>The terrain info</p></td>
<td><p>00000000 14000000</p></td>
</tr>
<tr class="odd">
<td><p>kx</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The tangent of the angle to the X-axis</p></td>
<td><p>00000080</p></td>
</tr>
<tr class="even">
<td><p>ky</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The tangent of the angle to the Y-axis</p></td>
<td><p>00000080</p></td>
</tr>
<tr class="odd">
<td><p>bz</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The intercept for Z-axis</p></td>
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
<td><p>Zone flags. Every binary bit represent if district is in the corresponding zone</p></td>
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
<td><p>The minimum X-coordinate of the points</p></td>
<td><p>21300FC3</p></td>
</tr>
<tr class="even">
<td><p>MinPy</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The minimum Y-coordinate of the points</p></td>
<td><p>87564243</p></td>
</tr>
<tr class="odd">
<td><p>MinPz</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The minimum Z-coordinate of the points</p></td>
<td><p>00001C42</p></td>
</tr>
<tr class="even">
<td><p>MaxPx</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The maximum X-coordinate of the points</p></td>
<td><p>8115D0C2</p></td>
</tr>
<tr class="odd">
<td><p>MaxPy</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The maximum Y-coordinate of the points</p></td>
<td><p>04B65B43</p></td>
</tr>
<tr class="even">
<td><p>MaxPz</p></td>
<td><p>Single</p></td>
<td><p>4</p></td>
<td><p>The maximum Z-coordinate of the points</p></td>
<td><p>00001C42</p></td>
</tr>
<tr class="odd">
<td><p>Border 0</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Border of the district</p></td>
<td><p>00000000</p></td>
</tr>
<tr class="even">
<td><p>Border 1</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Border of the district</p></td>
<td><p>01000000</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>... Borders: total number is defined by n</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>... District DSs: total number is defined by Number of Districts</p></td>
</tr>
<tr class="odd">
<td rowspan="9"><p>Tail DA</p></td>
<td><p>Symbol DB</p></td>
<td></td>
<td><p>Symbol</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>The start of the Tail DA</p></td>
<td><p>32534148</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Basic info DB</p></td>
<td rowspan="4"></td>
<td><p>Total Number of Districts</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Total Number of Districts contained in Reseaus</p></td>
<td><p>6E050000</p></td>
</tr>
<tr class="odd">
<td><p>Unknown</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Unknown data</p></td>
<td><p>06000000</p></td>
</tr>
<tr class="even">
<td><p>Number of Reseaus in X</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Number of Reseaus in the X direction</p></td>
<td><p>17000000</p></td>
</tr>
<tr class="odd">
<td><p>Number of Reseaus in Y</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Number of Reseaus in the Y direction</p></td>
<td><p>18000000</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Reseau DB</p></td>
<td rowspan="3"><p>Reseau DS</p></td>
<td><p>Number of Districts in the Reseau</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Number of Districts contained in the Reseau</p></td>
<td><p>03000000</p></td>
</tr>
<tr class="odd">
<td><p>District Index</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Index for the District</p></td>
<td><p>DC000000</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>... District Index: total number is defined by Number of Districts in the Reseau</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>Reseau DSs: total number is defined by Number of Reseaus in X * Number of Reseaus in Y</p></td>
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
<th style="text-align: center;"><p>Byte</p></th>
<th style="text-align: center;"><p>Name</p></th>
<th style="text-align: center;"><p>Description</p></th>
<th style="text-align: center;"><p>Value</p></th>
<th style="text-align: center;"><p>Original Text</p></th>
<th style="text-align: center;"><p>Type</p></th>
<th style="text-align: center;"><p>Sample</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="5" style="text-align: center;"><p>0</p></td>
<td rowspan="5" style="text-align: center;"><p>MajorType</p></td>
<td rowspan="5" style="text-align: center;"><p>Major type</p></td>
<td style="text-align: center;"><p>0</p></td>
<td style="text-align: center;"><p>TIERRA</p></td>
<td style="text-align: center;"><p>Earth</p></td>
<td style="text-align: center;"><p>TU01</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>1</p></td>
<td style="text-align: center;"><p>NIEVE</p></td>
<td style="text-align: center;"><p>Snow</p></td>
<td style="text-align: center;"><p>HL</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>2</p></td>
<td style="text-align: center;"><p>AGUA</p></td>
<td style="text-align: center;"><p>Water</p></td>
<td style="text-align: center;"><p>TU04 river</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>3</p></td>
<td style="text-align: center;"><p>ORILLA</p></td>
<td style="text-align: center;"><p>Shore</p></td>
<td style="text-align: center;"><p>TU04 river shoal</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>4</p></td>
<td style="text-align: center;"><p>SUBMARINO</p></td>
<td style="text-align: center;"><p>Underwater</p></td>
<td style="text-align: center;"><p>SB under water</p></td>
</tr>
<tr class="even">
<td rowspan="16" style="text-align: center;"><p>1</p></td>
<td rowspan="16" style="text-align: center;"><p>MinorType</p></td>
<td rowspan="16" style="text-align: center;"><p>Graphical effects and sound effects</p></td>
<td style="text-align: center;"><p>0</p></td>
<td style="text-align: center;"><p>ASFALTO</p></td>
<td style="text-align: center;"><p>Asphalt</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>TU01</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>1</p></td>
<td style="text-align: center;"><p>HIERBA</p></td>
<td style="text-align: center;"><p>Grass</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>TU01</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>2</p></td>
<td style="text-align: center;"><p>TIERRA</p></td>
<td style="text-align: center;"><p>Earth</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>TU01 road</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>3</p></td>
<td style="text-align: center;"><p>ADOQUINES</p></td>
<td style="text-align: center;"><p>Pavement</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>KW left river crossing, right side, biggest isle, to the shore side, adjacent isle, district 297</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>4</p></td>
<td style="text-align: center;"><p>AZULEJOS</p></td>
<td style="text-align: center;"><p>Tiles</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>3TE right-top corner, house, balcony of which</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>5</p></td>
<td style="text-align: center;"><p>MADERA</p></td>
<td style="text-align: center;"><p>Wood</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>TU03 starting position</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>6</p></td>
<td style="text-align: center;"><p>ARENA</p></td>
<td style="text-align: center;"><p>Sand</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>CZ the down part of river which water runs to, the river shoal</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>7</p></td>
<td style="text-align: center;"><p>NIEVE</p></td>
<td style="text-align: center;"><p>Snow</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>HL</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>8</p></td>
<td style="text-align: center;"><p>HIELO</p></td>
<td style="text-align: center;"><p>Ice</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>3ST1 river, ice surface of which</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>9</p></td>
<td style="text-align: center;"><p>ROCAS</p></td>
<td style="text-align: center;"><p>Rocks</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>CZ the middle part of river, the river shoal of which</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>10</p></td>
<td style="text-align: center;"><p>ARBUSTOS</p></td>
<td style="text-align: center;"><p>Shrubs</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>3TC farmland with plants</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>11</p></td>
<td style="text-align: center;"><p>METAL</p></td>
<td style="text-align: center;"><p>Metal (metal handrail, metal stairs)</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>SB's beacon tower, stairs near which</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>12</p></td>
<td style="text-align: center;"><p>METAL_ENCHARCADO</p></td>
<td style="text-align: center;"><p>Waterlogged Metal</p>
<p>(Earth)</p></td>
<td style="text-align: center;"><p>3TA1 top of trains</p>
<p>3TV biggest house's stairs</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>13</p></td>
<td style="text-align: center;"><p>ORILLA</p></td>
<td style="text-align: center;"><p>Shore</p>
<p>(Shore)</p></td>
<td style="text-align: center;"><p>TU04 river shoal</p></td>
</tr>
<tr class="even">
<td style="text-align: center;"><p>14</p></td>
<td style="text-align: center;"><p>AGUA_PROFUNDA</p></td>
<td style="text-align: center;"><p>Deep Water</p>
<p>(Water/Underwater)</p></td>
<td style="text-align: center;"><p>TU04 river</p></td>
</tr>
<tr class="odd">
<td style="text-align: center;"><p>15</p></td>
<td style="text-align: center;"><p>GRAVILLA</p></td>
<td style="text-align: center;"><p>Gravel</p>
<p>(Land)</p></td>
<td style="text-align: center;"><p>SB train line</p></td>
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

| Byte(Bit) |        Original Text        |           Name            |                                                     Description                                                     |                                                                                        Sample                                                                                        |
|:---------:|:---------------------------:|:-------------------------:|:-------------------------------------------------------------------------------------------------------------------:|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|   4(0)    |           TRANSIT           |        Transitable        |                            Transitable. Characters could enter the district if it's set                             |                                                                                     TU01's land                                                                                      |
|   4(1)    |           OCLUSOR           |        IsOcclusion        |              Blocking district. Not visible from outside the district. Be visible only in the district              |                                                                                     TU01's bosk                                                                                      |
|   4(2)    |                             |          IsPlane          |           Horizontal. 0 stands for slope. A slope without this setting to 0 will be considered as a cliff           |                                                     Mostly True, counterexample: TU03 blockhouse, gate, slope in front of which                                                      |
|   4(3)    |            DOBLE            |          Double           | Double district. District with void bottom and solid top. 0 stands for the situation with solid bottom and void top |                                                                  TU03 starting position, near which, wall with hole                                                                  |
|   4(4)    |        NO_DIBU_VISTA        |      PlotNotVisible       |                  No field of view will be displayed in the district, but the view may still exist                   |                                                                                     TU01's bosk                                                                                      |
|   4(5)    |         SOLO_AGACH          |         BendOnly          |                                        Characters must creep on the district                                        |                                                                                  TU01's wood board                                                                                   |
|   4(6)    | NO_PERM_EXPL, OCLUSOR EXPLO |  IsExplosionOcclusion　   |                          Explosion blocked. Explosion can not propagate past the district.                          |                                                  SB submarine observatory, in the middle of which, round door, district above which                                                  |
|   4(7)    |          RALENTIZA          |        　MakeSlow         |                                            Characters inside are slowed.                                            |                                                                                 HL most right bound                                                                                  |
|   5(0)    |         ENTERRABLE          |         Buriable          |                                       Characters can dig on the area(1#，5#)                                        |                                                                                     TU02's land                                                                                      |
|   5(1)    |          ESCALERA           |         IsLadder          |                                  Stairs, characters will step on them step by step                                  |                                                                         SB's beacon tower, stairs near which                                                                         |
|   5(2)    |        DEJAN_HUELLAS        |      LeaveFootprints      |                                              Footprints can be left on                                              |                                                                     TU02's farmland where allied soldiers locate                                                                     |
|   5(3)    |          ALAMBRADA          |       IsBarbedWire        |                                    Barbed wire. Only the thief can creep through                                    |                                            TU01, in which, metal meshes near the starting position, support wood, delta district of which                                            |
|   5(4)    |        SOLO CANIJOS         |         PunyOnly          |                                        Only mouse can enter. Dog can't enter                                        |                                                          SB blockhouse, near which, round platform with AA gun, under which                                                          |
|   5(5)    |        SECTOR-VALLA         |       IsFenceSector       |                                             A ladder can be set up here                                             |                                                                            RY wall near starting position                                                                            |
|   5(6)    |       VALLA-OCLUSORA        |    　IsFenceOcclusion     |                                                  (Effect Unknown)                                                   |                                                                        CZ bridge with handrail, the handrail                                                                         |
|   5(7)    |           SOMBRA            |         IsShadow          |                                                     Shadow area                                                     |                                                                                         TU01                                                                                         |
|   6(0)    |         HASTA TECHO         |         ToCeiling         |                          This is usually used along with knockable wall. (Effect Unknown)                           |                                                                        TU03 knockable wall in the blockhouse                                                                         |
|   6(1)    |         FUERA_MAPA          |         IsMapOut          |          Bound district, characters automatically goes to regular districts when received a "move" command          |                                                                                         TU01                                                                                         |
|   6(2)    |                             |    　MouseNotHoverable    |                      Mouse can not enter this district, but characters can enter this district                      |                                                  RY house where holds 1#, the walkway out of which, the district above the officer                                                   |
|   6(3)    |        NO_ACANTILADO        |         　NoCliff         |                                                  (Effect Unknown)                                                   |                                                  SB submarine observatory, in the middle of which, round door, district above which                                                  |
|   6(7)    |    AUTOTRANSFER PEATONES    | 　AutotransferPedestrians |                                                     Door icon.                                                      |                                                                                         3TM                                                                                          |
|   7(6)    |       INTR_VEHICULOS        |    InteriorVehicles　     |                                                  (Effect Unknown)                                                   | SB small blockhouse behind the siege wall, district under the platform of which; KW starting position, object in the air: tree, etc on the right side of which, district below which |
|   7(7)    |         COLCHONETA          |          　IsPad          |                                   8# can jump down to this district without hurt                                    |                                                                        PT a part of the bulge of the carrier                                                                         |

<p>&nbsp;</p>

Notice:

1\. All numerical data types are in little-endian.

2.In the items with "/", names in the right is for Commandos 2, and names in the right is for Commandos 3.

3. Further understanding of the Commandos 2 model indicates,

A Point is a plumb line with the equation:

x = x0

y = y0

A Border is a plane between two "Point".

A District is actually a columniation with several trapezoid side, and the equation for the ceiling plane is:

z = kx \* x + ky \*y + bz

The floor plane is a horizontal plane. The side planes are all vertical with the horizontal plane.

A Special District is a District. One or more of its side planes are not solid, and can be distinguished with Neighbor District Index = -1 for the corresponding Borders. So, Special District is not divided from other Districts.

These names are not exact, but to avoid big changes, we still use these names.

4\. The total model use right-hand world coordinate system: x, y represent two horizontal directions, z represents height with the positive direction to the top.

5\. Viewing in a look-down coordinate system, districts with anticlockwise points show the front face.

Reference:

\[1\]invox4C2_3keyfiles.doc, invox, 2006

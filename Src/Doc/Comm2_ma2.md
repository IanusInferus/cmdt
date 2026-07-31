Commandos2 MA2 File Structure Table

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
<th style="text-align: center;">Data Area</th>
<th style="text-align: center;">Data Block</th>
<th style="text-align: center;">Sub Data Block</th>
<th style="text-align: center;">Data Section</th>
<th style="text-align: center;">Data</th>
<th style="text-align: center;">Data Type</th>
<th style="text-align: center;">Length</th>
<th style="text-align: center;">Description</th>
<th style="text-align: center;">Sample Data</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="16" style="text-align: center;">Header DA</td>
<td style="text-align: center;">Symbol DB</td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">29</td>
<td style="text-align: center;">Fixed</td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td rowspan="4" style="text-align: center;">Basic Info DB</td>
<td rowspan="4" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">Number of Views</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td style="text-align: center;"></td>
<td style="text-align: center;">04000000</td>
</tr>
<tr class="odd">
<td style="text-align: center;"></td>
<td style="text-align: center;">Number of Objects</td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td style="text-align: center;"></td>
<td style="text-align: center;">52000000</td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;">The total render index number of 4 views</td>
<td style="text-align: center;">Int32*2</td>
<td style="text-align: center;">8</td>
<td style="text-align: center;">Doubly written</td>
<td style="text-align: center;">04010000 04010000</td>
</tr>
<tr class="odd">
<td style="text-align: center;">Fixed DS</td>
<td style="text-align: center;"></td>
<td style="text-align: center;">Int32</td>
<td style="text-align: center;">4</td>
<td style="text-align: center;"></td>
<td style="text-align: center;">00000000</td>
</tr>
<tr class="even">
<td rowspan="5" style="text-align: center;">Address Info DB</td>
<td rowspan="5" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">Address0</td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">indicates the starting of 4 views DB</td>
<td style="text-align: center;">49000000</td>
</tr>
<tr class="odd">
<td style="text-align: center;"></td>
<td style="text-align: center;">Address1</td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">indicates the starting of objects’ absolute info DB in main DA</td>
<td style="text-align: center;">19010000</td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;">Address2</td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">indicates the starting of render index info DB</td>
<td style="text-align: center;">110A0000</td>
</tr>
<tr class="odd">
<td style="text-align: center;"></td>
<td style="text-align: center;">Address3</td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">indicates the starting of render code DB</td>
<td style="text-align: center;">912A0000</td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;">Address4</td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;">doubly written, indicates the starting of tail DB</td>
<td style="text-align: center;">203E0900 203E0900</td>
</tr>
<tr class="odd">
<td rowspan="6" style="text-align: center;"> 4 Views info DB</td>
<td rowspan="2" style="text-align: center;"></td>
<td rowspan="2" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td rowspan="4" style="text-align: center;"></td>
<td rowspan="4" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td colspan="5" style="text-align: center;"></td>
</tr>
<tr class="odd">
<td rowspan="16" style="text-align: center;"></td>
<td rowspan="5" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td rowspan="9" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td rowspan="7" style="text-align: center;"></td>
<td rowspan="3" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td colspan="5" style="text-align: center;"></td>
</tr>
<tr class="even">
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="odd">
<td rowspan="2" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td colspan="5" style="text-align: center;"></td>
</tr>
<tr class="odd">
<td rowspan="2" style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
<td style="text-align: center;"></td>
</tr>
<tr class="even">
<td colspan="5" style="text-align: center;"></td>
</tr>
<tr class="odd">
<td colspan="7" style="text-align: center;"></td>
</tr>
<tr class="even">
<td colspan="8" style="text-align: center;"></td>
</tr>
<tr class="odd">
<td colspan="9" style="text-align: center;"></td>
</tr>
</tbody>
</table>

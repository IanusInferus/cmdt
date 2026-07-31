Commandos2&3 ABI File Structure Table

Arranged by NeoRAGEx2002, Translated by F.R.C.

<p>&nbsp;</p>

Commandos 2 ABI File Format

ABI files include all 3d models and textures, key frame animation definition of them. ABI file consists of Head Chunk, Texture Chunk, Model Chunk and Animation Chunk, which correspond to file header, texture definition, model definition and key frame animation definition, respectively. In Comm2 there are 4 versions of ABI files, where version 1050/1060 are clear in all four chunks, and version 1040 need further clarification in Animation Chunk.

<p>&nbsp;</p>

Head Chunk

<table>
<colgroup>
<col style="width: 17%" />
<col style="width: 16%" />
<col style="width: 16%" />
<col style="width: 16%" />
<col style="width: 35%" />
</colgroup>
<thead>
<tr class="header">
<th><p>Data Block</p></th>
<th><p>Data</p></th>
<th><p>Data Type</p></th>
<th><p>Length</p></th>
<th><p>Description</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="4"><p>Basic Info DB</p></td>
<td><p>Identifier</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>Identifier</p></td>
</tr>
<tr class="even">
<td><p>Version</p></td>
<td><p>String</p></td>
<td><p>4</p></td>
<td><p>Version: 1010, 1040, 1050, 1060</p></td>
</tr>
<tr class="odd">
<td><p>Num TimeAxis</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Number of Time Axes</p></td>
</tr>
<tr class="even">
<td><p>Num Animation</p></td>
<td><p>Int32</p></td>
<td><p>4</p></td>
<td><p>Number of Animations</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Texture Chunk

<table>
<colgroup>
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 13%" />
<col style="width: 35%" />
</colgroup>
<thead>
<tr class="header">
<th><p>Data Block</p></th>
<th colspan="2"><p>Data</p></th>
<th><p>Data Type</p></th>
<th><p>Length</p></th>
<th><p>Description</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="11"><p>Texture Info DB</p></td>
<td colspan="2"><p>Num Texture</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Number of Textures</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Texture Info DS</p></td>
<td><p>Block Identifier</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Fixed to 0 (UNKNOWN)</p></td>
</tr>
<tr class="odd">
<td><p>Width</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Bitmap Width</p></td>
</tr>
<tr class="even">
<td><p>Height</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Bitmap Height</p></td>
</tr>
<tr class="odd">
<td><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>Bitmap Name</p></td>
</tr>
<tr class="even">
<td rowspan="4"><p>Palette DS</p></td>
<td><p>Red</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>Red  Coordinate</p></td>
</tr>
<tr class="odd">
<td><p>Green</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>Green Coordinate</p></td>
</tr>
<tr class="even">
<td><p>Blue</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>Blue Coordinate</p></td>
</tr>
<tr class="odd">
<td colspan="4"><p>(R,G,B) Triple Count = 256,  and 768 Bytes in total</p></td>
</tr>
<tr class="even">
<td><p>Indexed  Bitmap DS</p></td>
<td><p>Bitmap Data</p></td>
<td><p>byte</p></td>
<td><p>Width * Height</p></td>
<td><p>Bitmap Data</p></td>
</tr>
<tr class="odd">
<td colspan="5"><p>Texture Info DS, Pallette DS and Bitmap DS data array, with Count = Num Texture</p></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>

Model Chunk

<table>
<colgroup>
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 11%" />
<col style="width: 10%" />
<col style="width: 35%" />
</colgroup>
<thead>
<tr class="header">
<th><p>Data Block</p></th>
<th colspan="3"><p>Data</p></th>
<th><p>Data Type</p></th>
<th><p>Length</p></th>
<th><p>Description</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td colspan="4"><p>Identifier(only in 1060, not in 1050/1040)</p></td>
<td><p>byte</p></td>
<td><p>1</p></td>
<td><p>Unknown Identifier(Not Fixed to 1)</p></td>
</tr>
<tr class="even">
<td colspan="4"><p>Num Model</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Number of Models</p></td>
</tr>
<tr class="odd">
<td colspan="4"><p>Num Bone</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Number of Bones</p></td>
</tr>
<tr class="even">
<td rowspan="17"><p>Model DB</p></td>
<td rowspan="3"><p>Head Info DS</p></td>
<td colspan="2"><p>Num Vertex</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Number of Vertices</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>Num Polygon</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Number of Polygon</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>Model Name</p></td>
</tr>
<tr class="odd">
<td rowspan="4"><p>Vertices DS</p></td>
<td colspan="2"><p>x</p></td>
<td><p>float</p></td>
<td><p>4</p></td>
<td><p>Vertex Coordinate</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>y</p></td>
<td><p>float</p></td>
<td><p>4</p></td>
<td><p>Vertex Coordinate</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>z</p></td>
<td><p>float</p></td>
<td><p>4</p></td>
<td><p>Vertex Coordinate</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>(x,y,z) Triple Count = Num Vertex</p></td>
</tr>
<tr class="odd">
<td rowspan="6"><p>Polygon DS</p></td>
<td colspan="2"><p>n</p></td>
<td><p>byte(int in 1040)</p></td>
<td><p>1(4)</p></td>
<td><p>Num of Borders&lt;1&gt;</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>Texture ID</p></td>
<td><p>byte(int in 1040)</p></td>
<td><p>1(4)</p></td>
<td><p>Texture Index</p></td>
</tr>
<tr class="odd">
<td rowspan="4"><p>Vertex</p></td>
<td><p>Vertex ID 0</p></td>
<td><p>short(int in 1040)</p></td>
<td><p>2(4)</p></td>
<td><p>Vertex Index</p></td>
</tr>
<tr class="even">
<td><p>U 0</p></td>
<td><p>short(int in 1040)</p></td>
<td><p>2(4)</p></td>
<td><p>/4096 will get Coordinate Tu in Texture</p>
<p>(no division in 1040)</p></td>
</tr>
<tr class="odd">
<td><p>V 0</p></td>
<td><p>short(int in 1040)</p></td>
<td><p>2(4)</p></td>
<td><p>/4096 will get Coordinate Tv in Texture</p>
<p>(no division in 1040)</p></td>
</tr>
<tr class="even">
<td colspan="4"><p>(VID, U, V) Vertex Triple Count = n, All Right-handed</p></td>
</tr>
<tr class="odd">
<td colspan="6"><p>Polygon DS Count = Num Polygon</p></td>
</tr>
<tr class="even">
<td rowspan="3"><p>Vidx to Bone DS</p></td>
<td colspan="2"><p>Start Vertex Index</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Vertices related to Bone</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>End Vertex Index</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Vertices related to Bone</p></td>
</tr>
<tr class="even">
<td colspan="5"><p>(Start idx,End idx), Count = Num Bone</p></td>
</tr>
<tr class="odd">
<td colspan="7"><p>Model DB Count = Num Model</p></td>
</tr>
<tr class="even">
<td rowspan="5"><p>Bone Hierarchy <br />
DB</p></td>
<td rowspan="4"><p>Bone Hierarchy <br />
DS</p></td>
<td colspan="2"><p>Parent Index</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Parent Node Index&lt;2&gt;</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>Global Offset Vector</p></td>
<td><p>float*3</p></td>
<td><p>3*4</p></td>
<td><p>Offset Vectors to transform Bone’s Local Coordinates to Global Coordinates</p>
<p>(Translator: I think it’s the Transform Matrix)</p></td>
</tr>
<tr class="even">
<td colspan="2"><p>Node Name</p></td>
<td><p>String</p></td>
<td><p>32</p></td>
<td><p>Current Node Name</p></td>
</tr>
<tr class="odd">
<td colspan="2"><p>UNKNOWN</p></td>
<td><p>int</p></td>
<td><p>4</p></td>
<td><p>Unknown, Fixed to 0</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>Bone Hierarchy DB Count = Num Bone</p></td>
</tr>
</tbody>
</table>

1.  This ‘n’ does not equals to 3 or 4 in all cases, e.g. in tiger.abi, there is a tank model where n=6.
2.  Parent Node Index in Array of BoneHierarchy. Notice that the nodes in the array are stored in a depth-first order, which means we can sequentially access each node in the array, ensured that every node is accessed after all its ancestor nodes been accessed. Thus, the order itself is a pre-order iteration of the BoneHierarchy.
3.  All 3D model data in Comm2 defaults in right-handed coordinates, thus it shall be taken care of (by DirectX developers).

<p>&nbsp;</p>

Animation Chunk

<table>
<colgroup>
<col style="width: 33%" />
<col style="width: 32%" />
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 9%" />
<col style="width: 8%" />
</colgroup>
<thead>
<tr class="header">
<th><p>Data</p></th>
<th><p>Data Type</p></th>
<th><p>Description</p></th>
<th><p>Description</p></th>
<th><p>Description</p></th>
<th><p>Description</p></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td><p>num_translateKeyframes</p></td>
<td><p>int</p></td>
<td><p>Number of Translation Motion KeyFrame</p></td>
<td></td>
<td rowspan="6"><p><strong>TranslateTimeAxis</strong><strong><br />
</strong>for keyFrame<strong></strong></p></td>
<td rowspan="13"><p><strong>TransformTimeAxis</strong><strong><br />
</strong>for Bone<strong></strong></p></td>
</tr>
<tr class="even">
<td><p>timestamp</p></td>
<td><p>byte</p></td>
<td><p>KeyFrame TimeStamp</p></td>
<td rowspan="4"><p><strong>TranslateKeyFrame</strong><strong></strong></p></td>
</tr>
<tr class="odd">
<td><p>dx</p></td>
<td><p>short</p></td>
<td rowspan="3"><p>consisit a Translation Vector (need /256)</p></td>
</tr>
<tr class="even">
<td><p>dy</p></td>
<td><p>short</p></td>
</tr>
<tr class="odd">
<td><p>dz</p></td>
<td><p>short</p></td>
</tr>
<tr class="even">
<td colspan="4"><p>TranslateKeyFrame Count = num translateKeyframes</p></td>
</tr>
<tr class="odd">
<td><p>num_rotateKeyframes</p></td>
<td><p>int</p></td>
<td><p>Number of Rotation Motion KeyFrame</p></td>
<td></td>
<td rowspan="7"><p><strong>RotateTimeAxis</strong><strong><br />
</strong>for keyFrame<strong></strong></p></td>
</tr>
<tr class="even">
<td><p>timestamp</p></td>
<td><p>byte</p></td>
<td><p>KeyFrame TimeStamp</p></td>
<td rowspan="5"><p><strong>RotateKeyFrame</strong><strong><br />
<br />
</strong></p></td>
</tr>
<tr class="odd">
<td><p>x</p></td>
<td><p>short</p></td>
<td rowspan="4"><p>consisit a Rotation Quaternion<br />
Notice: right-handed<br />
(need /32768)</p></td>
</tr>
<tr class="even">
<td><p>y</p></td>
<td><p>short</p></td>
</tr>
<tr class="odd">
<td><p>z</p></td>
<td><p>short</p></td>
</tr>
<tr class="even">
<td><p>w</p></td>
<td><p>short</p></td>
</tr>
<tr class="odd">
<td colspan="4"><p>RotateKeyFrame Count = num rotateKeyframes</p></td>
</tr>
<tr class="even">
<td colspan="6"><p>TransformTimeAxis Count = num TimeAxis</p></td>
</tr>
<tr class="odd">
<td><p>animation_name</p></td>
<td><p>String(60 Bytes)</p></td>
<td><p>Animation Name</p></td>
<td rowspan="2"></td>
<td rowspan="5"><p><strong>Animation<br />
</strong>total<strong></strong></p></td>
<td></td>
</tr>
<tr class="even">
<td><p>num_related_bones</p></td>
<td><p>int</p></td>
<td><p>Number of Animation-related Bones</p></td>
<td></td>
</tr>
<tr class="odd">
<td><p>bone_id</p></td>
<td><p>int</p></td>
<td><p>Corresponding Bone ID</p></td>
<td rowspan="2"><p><strong>BoneAnimationEntry</strong><strong></strong></p></td>
<td></td>
</tr>
<tr class="even">
<td><p>transform_time_axis_idx</p></td>
<td><p>int</p></td>
<td><p>Tranform Time Axis for the Bone</p></td>
<td></td>
</tr>
<tr class="odd">
<td colspan="4"><p>BoneAnimationEntry Count = num_related_bones</p></td>
<td></td>
</tr>
<tr class="even">
<td colspan="5"><p>Animation Count = num Animation</p></td>
<td></td>
</tr>
</tbody>
</table>

Key Frame based Bone Animation Object Model:

1.  one TranslateTimeAxis consists of multiple TranslateKeyFrame
2.  one RotateTimeAxis consists of multiple RotateKeyFrame
3.  one TransformTimeAxis consists of one TranslateTimeAxis plus one RotateTimeAxis
4.  one BoneAnimationEntry consists of the corresponding Bone ID and TransformTimeAxis
5.  one Animation consists of multiple BoneAnimationEntry

\*In version 1040, it seems that it stores "TimeStamp (float)+TransformMatrix (4\*4\*short) ",  and a KeyFrame takes 36 Bytes, and there is no distinguishing on Translation Matrix and Rotation Matrix, but this theory fails the validation.

The Animation Chunk consists of  "timestamp(float)+ int(Unknown, with the value of 3 constantly)+(float)\*7(translation+rotation)", takes 36 Bytes per keyframe. ----老顽童5099

There is no BASE action (action that doesn't animate) in version 1060, compared to version 1050. ----老顽童5099

Translation vector is 0 when there is a parent bone. The value is near 0. In version 1050 and 1060, translation is separated, and only the translation of the start and end frame is preserved, making the ABI file smaller. ----老顽童5099

<p>&nbsp;</p>
<p>&nbsp;</p>

Notice:

1\. All numerical data types are in little-endian.

2\. This document includes some information on texture from URF’s document.

3\. This document includes some information deduced from the source codes of the character animation(combined version)(角色动画(合成版)) written by jinshengmao.

4\. int stands for Int32. float stands for Float32(Single). short stands for Int16.

<p>&nbsp;</p>
<p>&nbsp;</p>

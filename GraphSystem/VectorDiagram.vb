'==========================================================================
'
'  File:        VectorDiagram.vb
'  Location:    NDGrapher.GraphSystem <Visual Basic .Net>
'  Description: VectorDiagram
'  Created:     2007.10.18.22:20(GMT+08:00)
'  Version:     0.5 2007.10.18.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Text
Imports System.Diagnostics
Imports Microsoft.VisualBasic

''' <summary>平面向量图</summary>
''' <remarks>具有透视不变性</remarks>
Public Class VectorDiagram
    Inherits NDObject
    Implements IPicObj

    Public C As Vector '中心点
    Public L As Vector '左
    Public R As Vector '右
    Public T As Vector '上
    Public B As Vector '下

    Public Overrides Function Copy() As NDObject
        Dim CopyObject As VectorDiagram
        CopyObject = MyBase.BaseCopy()
        CopyObject.C = +C
        CopyObject.L = +L
        CopyObject.R = +R
        CopyObject.T = +T
        CopyObject.B = +B
        Return CopyObject
    End Function

    Public Overrides Function Complete() As IPicObj()
        If Not HomotheticTransformation Is Nothing Then
            With HomotheticTransformation
                C = .CPos(C)
                L = .CPos(L)
                R = .CPos(R)
                T = .CPos(T)
                B = .CPos(B)
            End With
        End If
        Return New IPicObj() {Me}
    End Function
End Class

'记一点P的参数坐标为U<-[0,1]，三点基准点A,B,C，分别在原直线上0,1/2,1处，则
'交比K=(AB,CP)=(AC*BP)/(BC*AP)=2*(U-1/2)/U=2-1/U
'经过透视变换T，记[a,b,c,p]=T([A,B,C,P])，由透视变换的交比不变性，有
'K=(ac*bp)/(bc*ap)
'记u=ap/ab，有
'u=ab/(ac-K*bc)=U*ab/(bc+U*(ab-bc))
'p=a+ac*u

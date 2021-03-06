'==========================================================================
'
'  File:        Scaler.vb
'  Location:    Scaler <Visual Basic .Net>
'  Description: 进制转换及查看器
'  Version:     2009.12.22.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Compare Text
Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Public Class Scaler

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim i As FileStream
        Try
            i = New FileStream(TextBox1.Text, FileMode.Open)
        Catch
            MsgBox("文件无法读取。")
            Return
        End Try
        If i.Length = 0 Then Return
        If i.Length > 2 ^ 16 Then
            MsgBox("文件太大，最大64KB。")
        End If

        Dim Format As FormatScale
        If ComboBox1.Text = "BIN" Then
            Format = AddressOf FormatBIN
            TextBox2.Font = New Font("宋体", 9)
            Me.Width = 882
        ElseIf ComboBox1.Text = "HEX" Then
            Format = AddressOf FormatHEX
            TextBox2.Font = New Font("宋体", 12)
            Me.Width = 400
        Else
            Format = AddressOf FormatDEC
            TextBox2.Font = New Font("宋体", 12)
            Me.Width = 528
        End If

        Dim s As String = ""
        For n As Integer = 0 To i.Length - 2
            s &= Format(i.ReadByte) & " "
        Next
        s &= Format(i.ReadByte)

        i.Close()
        TextBox2.Text = s
    End Sub
    Private Delegate Function FormatScale(ByVal b As Byte) As String
    Function FormatBIN(ByVal b As Byte) As String
        Dim s As String = ""
        For n As Integer = 0 To 7
            s = (b Mod 2).ToString & s
            b = b >> 1
        Next
        Return s
    End Function
    Function FormatDEC(ByVal b As Byte) As String
        Return b.ToString("D3")
    End Function
    Function FormatHEX(ByVal b As Byte) As String
        Return b.ToString("X2")
    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If TextBox2.Text = "" Then Return

        Dim o As FileStream
        Try
            o = New FileStream(TextBox1.Text, FileMode.Create)
        Catch
            MsgBox("文件无法写入。")
            Return
        End Try

        Dim digit As Integer
        Dim Format As FormatScaleW
        If ComboBox1.Text = "BIN" Then
            Format = AddressOf FormatBINW
            digit = 8
        ElseIf ComboBox1.Text = "HEX" Then
            Format = AddressOf FormatHEXW
            digit = 2
        Else
            Format = AddressOf FormatDECW
            digit = 3
        End If

        Dim a As Char() = TextBox2.Text.ToCharArray
        Dim b As Char() = New Char(a.GetUpperBound(0)) {}
        Dim n As Integer = 0
        For Each c As Char In a
            If (Asc(c) >= &H30 And Asc(c) <= &H39) Or (Asc(c) >= &H41 And Asc(c) <= &H46) Then
                b(n) = c
                n += 1
            End If
        Next
        ReDim Preserve b(n - 1)

        If n Mod digit <> 0 Then
            o.Close()
            MsgBox("数据有误，请立即备份TextBox内的数据。")
            Return
        End If

        Dim s As String
        For i As Integer = 0 To (n - 1) \ digit
            s = ""
            For m As Integer = 0 To digit - 1
                s &= b(i * digit + m)
            Next
            Try
                o.WriteByte(Format(s))
            Catch
                o.Close()
                MsgBox("文件没有完成写入，请立即备份TextBox内的数据。查看目标文件结尾查错。")
                Return
            End Try
        Next

        o.Close()
        MsgBox("文件完成写入。")
    End Sub

    Private Delegate Function FormatScaleW(ByVal s As String) As Byte
    Function FormatBINW(ByVal s As String) As Byte
        Dim b As Byte
        For Each c As Char In s.ToCharArray
            b = b << 1
            b += Val(c)
        Next
        Return b
    End Function
    Function FormatDECW(ByVal s As String) As Byte
        Return CByte(s)
    End Function
    Function FormatHEXW(ByVal s As String) As Byte
        Return Byte.Parse(s, Globalization.NumberStyles.HexNumber)
    End Function

    Private Sub EnterPressed(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyData = Keys.Enter Then Button1_Click(Nothing, Nothing)
    End Sub
    Private Sub SelectAll(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox2.KeyUp
        If e.Control Then
            If e.KeyCode = Keys.A Then TextBox2.SelectAll()
        End If
    End Sub
End Class

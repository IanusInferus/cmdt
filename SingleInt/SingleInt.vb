Imports System

Public Class SingleInt
    Private a As SingleInt32

    Private Sub ToInt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Single.Click
        ErrorProvider.Clear()
        Try
            a.SingleValue = CSng(TextBox_Single.Text)
        Catch ex As Exception
            ErrorProvider.SetError(TextBox_Single, ex.Message)
            Return
        End Try
        TextBox_BigEndian.Text = a.Int32Value.ToString("X8")
        TextBox_LittleEndian.Text = Reverse(a.Int32Value).ToString("X8")
    End Sub

    Private Sub ToSingle_BigEndian_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_BigEndian.Click
        ErrorProvider.Clear()
        Try
            a.Int32Value = CID(Int64.Parse(TextBox_BigEndian.Text.Replace(" ", ""), Globalization.NumberStyles.HexNumber))
        Catch ex As Exception
            ErrorProvider.SetError(TextBox_BigEndian, ex.Message)
            Return
        End Try
        If a.SingleValue.ToString = Single.NaN.ToString AndAlso a.Int32Value <> &HFFC00000 Then
            ErrorProvider.SetError(TextBox_BigEndian, "不正常的值")
            Return
        End If
        TextBox_Single.Text = a.SingleValue
        TextBox_LittleEndian.Text = Reverse(a.Int32Value).ToString("X8")
    End Sub

    Private Sub ToSingle_LittleEndian_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_LittleEndian.Click
        ErrorProvider.Clear()
        Try
            a.Int32Value = Reverse(CID(Int64.Parse(TextBox_LittleEndian.Text.Replace(" ", ""), Globalization.NumberStyles.HexNumber)))
        Catch ex As Exception
            ErrorProvider.SetError(TextBox_LittleEndian, ex.Message)
            Return
        End Try
        If a.SingleValue.ToString = Single.NaN.ToString AndAlso a.Int32Value <> &HFFC00000 Then
            ErrorProvider.SetError(TextBox_LittleEndian, "不正常的值")
            Return
        End If
        TextBox_Single.Text = a.SingleValue
        TextBox_BigEndian.Text = a.Int32Value.ToString("X8")
    End Sub


    <System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)> Private Structure SingleInt32
        <System.Runtime.InteropServices.FieldOffset(0)> Public SingleValue As Single
        <System.Runtime.InteropServices.FieldOffset(0)> Public Int32Value As Int32
    End Structure

    Function Reverse(ByVal i As Int32) As Int32
        Return ((i And &HFF) << 24) Or ((i And &HFF00) << 8) Or ((i And &HFF0000) >> 8) Or (((i And &HFF000000) >> 24) And &HFF)
    End Function

    Shared Function CID(ByVal i As Int64) As Int32
        If CBool(i And &H80000000L) Then
            Return CInt((i And &HFFFFFFFFL) Or &HFFFFFFFF00000000L)
        Else
            Return CInt(i)
        End If
    End Function

End Class

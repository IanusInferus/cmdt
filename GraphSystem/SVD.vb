'==========================================================================
'
'  File:        SVD.vb
'  Location:    NDGrapher.GraphSystem <Visual Basic .Net>
'  Description: 奇异值分解
'  Version:     2023.01.30.
'  Copyright(C) Free
'
'==========================================================================

Imports System
Imports MathNet.Numerics.LinearAlgebra.Double
Imports MathNetDouble = MathNet.Numerics.LinearAlgebra.Double

Partial Class Matrix
    Private Shared Function MatrixToMathNetMatrix(ByVal m As Matrix) As MathNetDouble.Matrix
        Dim n = m.Dimension
        Dim om As New MathNetDouble.DenseMatrix(n, n)
        For y = 0 To n - 1
            For x = 0 To n - 1
                om(x, y) = m.ElementValue(x, y)
            Next
        Next
        Return om
    End Function
    Private Shared Function MathNetMatrixToMatrix(ByVal om As MathNetDouble.Matrix) As Matrix
        If om.RowCount <> om.ColumnCount Then Throw New ArgumentException
        Dim n = om.RowCount
        Dim m As New Matrix(n)
        For y = 0 To n - 1
            For x = 0 To n - 1
                m.ElementValue(x, y) = om(x, y)
            Next
        Next
        Return m
    End Function

    Public Sub SingularValueDecomposition(ByRef U As Matrix, ByRef S As Matrix, ByRef V As Matrix)
        Dim m = MatrixToMathNetMatrix(Me)
        Dim svd = m.Svd(True)
        U = MathNetMatrixToMatrix(svd.U)
        S = MathNetMatrixToMatrix(New MathNetDouble.DiagonalMatrix(svd.S.Count, svd.S.Count, svd.S.ToArray()))
        V = MathNetMatrixToMatrix(svd.VT.Transpose())

        'Dim SingularValueDecomposition As New SingularValueDecomposition(Me)
        'S = SingularValueDecomposition.GetS
        'U = SingularValueDecomposition.GetU
        'V = SingularValueDecomposition.GetV
    End Sub
End Class

''' <summary>Singular Value Decomposition.
'''
''' For an m-by-n matrix A with m >= n, the singular value decomposition is
''' an m-by-n orthogonal matrix U, an n-by-n diagonal matrix S, AndAlso
''' an n-by-n orthogonal matrix V so that A = U*S*V'.
'''
''' The singular values, sigma[k] = S[k][k], are ordered so that
''' sigma[0] >= sigma[1] >= ... >= sigma[n-1].
''' The singular value decompostion always exists, so the constructor will
''' never fail.  The matrix condition number AndAlso the effective numerical
''' rank can be computed from this decomposition.
''' </summary>
Public Class SingularValueDecomposition

    ''' <summary>
    '''  sqrt(a^2 + b^2) without under/overflow.
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Public Shared Function Hypot(ByVal a As Double, ByVal b As Double) As Double
        Dim r As Double
        If (Math.Abs(a) > Math.Abs(b)) Then
            r = b / a
            Return Math.Abs(a) * Math.Sqrt(1 + r * r)
        ElseIf (b <> 0) Then
            r = a / b
            Return Math.Abs(b) * Math.Sqrt(1 + r * r)
        Else
            Return 0.0
        End If
    End Function

    '''<summary>Arrays for internal storage of U AndAlso V.
    ''' @serial internal storage of U.
    ''' @serial internal storage of V.
    ''' </summary>
    Private U, V As Double(,)

    ''' <summary>Array for internal storage of singular values.
    ''' @serial internal storage of singular values.
    ''' </summary>
    Private s As Double()

    ''' <summary>Row AndAlso column dimensions.
    ''' @serial row dimension.
    ''' @serial column dimension.
    ''' </summary>
    Private m, n As Integer

    ''' <summary>Construct the singular value decomposition</summary>
    ''' <param name="Arg">   Rectangular matrix
    ''' </param>
    ''' <remarks>     Structure to access U, S AndAlso V.
    ''' </remarks>
    Public Sub New(ByVal Arg As Matrix)
        ' Derived from LINPACK code.
        ' Initialize.
        Dim A = Arg.Element
        m = Arg.Dimension
        n = Arg.Dimension
        Dim nu As Integer = Math.Min(m, n)
        s = New Double(Math.Min((m + 1), n) - 1) {}
        U = New Double(m - 1, nu - 1) {}
        V = New Double(n - 1, n - 1) {}
        Dim e() As Double = New Double(n - 1) {}
        Dim work() As Double = New Double(m - 1) {}

        ' Reduce A to bidiagonal form, storing the diagonal elements
        ' in s AndAlso the super-diagonal elements in e.

        Dim nct As Integer = Math.Min((m - 1), n)
        Dim nrt As Integer = Math.Max(0, Math.Min(n - 2, m))
        For k = 0 To Math.Max(nct, nrt) - 1
            If k < nct Then
                ' Compute the transformation for the k-th column AndAlso
                ' place the k-th diagonal in s[k].
                ' Compute 2-norm of k-th column without under/overflow.
                s(k) = 0
                For i = k To m - 1
                    s(k) = Hypot(s(k), A(i, k))
                Next
                If (s(k) <> 0) Then
                    If (A(k, k) < 0) Then
                        s(k) = -s(k)
                    End If
                    For i = k To m - 1
                        A(i, k) /= s(k)
                    Next
                    A(k, k) += 1
                End If
                s(k) = -s(k)
            End If
            For j = k + 1 To n - 1
                If (k < nct) AndAlso (s(k) <> 0) Then
                    ' Apply the transformation.
                    Dim t As Double = 0
                    For i = k To m - 1
                        t += A(i, k) * A(i, j)
                    Next
                    t = -t / A(k, k)
                    For i = k To m - 1
                        A(i, j) += t * A(i, k)
                    Next
                End If
                ' Place the k-th row of A into e for the
                ' subsequent calculation of the row transformation.
                e(j) = A(k, j)
            Next

            'wantv
            If k < nct Then
                ' Place the transformation in U for subsequent back
                ' multiplication.
                For i = k To m - 1
                    U(i, k) = A(i, k)
                Next
            End If
            'end wantv

            If k < nrt Then
                ' Compute the k-th row transformation AndAlso place the
                ' k-th super-diagonal in e[k].
                ' Compute 2-norm without under/overflow.
                e(k) = 0
                For i = k + 1 To n - 1
                    e(k) = Hypot(e(k), e(i))
                Next
                If (e(k) <> 0) Then
                    If (e(k + 1) < 0) Then
                        e(k) = -e(k)
                    End If
                    For i = k + 1 To n - 1
                        e(i) /= e(k)
                    Next
                    e(k + 1) += 1
                End If
                e(k) = -e(k)
                If k + 1 < m AndAlso e(k) <> 0 Then
                    ' Apply the transformation.
                    For i = k + 1 To m - 1
                        work(i) = 0
                    Next
                    For j = k + 1 To n - 1
                        For i = k + 1 To m - 1
                            work(i) += e(j) * A(i, j)
                        Next
                    Next
                    For j = k + 1 To n - 1
                        Dim t As Double = -e(j) / e(k + 1)
                        For i = k + 1 To m - 1
                            A(i, j) += t * work(i)
                        Next
                    Next
                End If

                'wantv
                ' Place the transformation in V for subsequent
                ' back multiplication.
                For i = k + 1 To n - 1
                    V(i, k) = e(i)
                Next
                'end wantv

            End If
        Next
        ' Set up the final bidiagonal matrix or order p.
        Dim p As Integer = Math.Min(n, (m + 1))
        If nct < n Then
            s(nct) = A(nct, nct)
        End If
        If m < p Then
            s(p - 1) = 0
        End If
        If nrt + 1 < p Then
            e(nrt) = A(nrt, p - 1)
        End If
        e(p - 1) = 0
        ' If required, generate U.

        'wantu
        For j = nct To nu - 1
            For i = 0 To m - 1
                U(i, j) = 0
            Next
            U(j, j) = 1
        Next
        For k = nct - 1 To 0 Step -1
            If s(k) <> 0 Then
                For j = k + 1 To nu - 1
                    Dim t As Double = 0
                    For i = k To m - 1
                        t += U(i, k) * U(i, j)
                    Next
                    t = -t / U(k, k)
                    For i = k To m - 1
                        U(i, j) += t * U(i, k)
                    Next
                Next
                For i = k To m - 1
                    U(i, k) = -U(i, k)
                Next
                U(k, k) += 1
                For i = 0 To k - 2
                    U(i, k) = 0
                Next
            Else
                For i = 0 To m - 1
                    U(i, k) = 0
                Next
                U(k, k) = 1
            End If
        Next
        'end wantu

        ' If required, generate V.
        'wantv
        For k = n - 1 To 0 Step -1
            If k < nrt AndAlso e(k) <> 0 Then
                For j = k + 1 To nu - 1
                    Dim t As Double = 0
                    For i = k + 1 To n - 1
                        t += V(i, k) * V(i, j)
                    Next
                    t = -t / V(k + 1, k)
                    For i = k + 1 To n - 1
                        V(i, j) += t * V(i, k)
                    Next
                Next
            End If
            For i = 0 To n - 1
                V(i, k) = 0
            Next
            V(k, k) = 1
        Next
        'end wantv

        ' Main iteration loop for the singular values.
        Dim pp As Integer = p - 1
        Dim iter As Integer = 0
        Dim eps As Double = 2 ^ -52

        While p > 0
            Dim kase As Integer
            Dim k As Integer
            ' Here is where a test for too many iterations would go.
            ' This section of the program inspects for
            ' negligible elements in the s AndAlso e arrays.  On
            ' completion the variables kase AndAlso k are set as follows.
            ' kase = 1     if s(p) AndAlso e[k-1] are negligible AndAlso k<p
            ' kase = 2     if s(k) is negligible AndAlso k<p
            ' kase = 3     if e[k-1] is negligible, k<p, AndAlso
            '              s(k), ..., s(p) are not negligible (qr step).
            ' kase = 4     if e(p-1) is negligible (convergence).
            For k = p - 2 To -1 Step -1
                If k = -1 Then Exit For
                If Math.Abs(e(k)) <= eps * (Math.Abs(s(k)) + Math.Abs(s(k + 1))) Then
                    e(k) = 0
                    Exit For
                End If
            Next
            If k = p - 2 Then
                kase = 4
            Else
                Dim ks As Integer
                For ks = p - 1 To k Step -1
                    If ks = k Then Exit For
                    Dim t As Double = 0
                    If ks <> p Then t += Math.Abs(e(ks))
                    If ks <> k + 1 Then t += Math.Abs(e(ks - 1))
                    If Math.Abs(s(ks)) <= eps * t Then
                        s(ks) = 0
                        Exit For
                    End If
                Next
                If ks = k Then
                    kase = 3
                ElseIf ks = p - 1 Then
                    kase = 1
                Else
                    kase = 2
                    k = ks
                End If
            End If
            k += 1

            ' Perform the task indicated by kase.
            Select Case kase
                Case 1 'Deflate negligible s(p).
                    Dim f As Double = e(p - 2)
                    e(p - 2) = 0
                    For j = p - 2 To k Step -1
                        Dim t As Double = Hypot(s(j), f)
                        Dim cs As Double = s(j) / t
                        Dim sn As Double = f / t
                        s(j) = t
                        If j <> k Then
                            f = -sn * e(j - 1)
                            e(j - 1) = cs * e(j - 1)
                        End If

                        'wantv
                        For i = 0 To n - 1
                            t = cs * V(i, j) + sn * V(i, p - 1)
                            V(i, p - 1) = -sn * V(i, j) + cs * V(i, p - 1)
                            V(i, j) = t
                        Next
                        'end wantv

                        j = (j - 1)
                    Next
                Case 2 'Split at negligible s(k).
                    Dim f As Double = e(k - 1)
                    e(k - 1) = 0
                    For j = k To p - 1
                        Dim t As Double = Hypot(s(j), f)
                        Dim cs As Double = s(j) / t
                        Dim sn As Double = f / t
                        s(j) = t
                        f = -sn * e(j)
                        e(j) = cs * e(j)

                        'wantu
                        For i = 0 To m - 1
                            t = cs * U(i, j) + sn * U(i, k - 1)
                            U(i, k - 1) = -sn * U(i, j) + cs * U(i, k - 1)
                            U(i, j) = t
                        Next
                        'end wantu

                    Next
                Case 3 'Perform one qr step.
                    ' Calculate the shift.
                    Dim scale As Double = Max(Math.Abs(s(p - 1)), Math.Abs(s(p - 2)), Math.Abs(e(p - 2)), Math.Abs(s(k)), Math.Abs(e(k)))
                    Dim sp As Double = s(p - 1) / scale
                    Dim spm1 As Double = s(p - 2) / scale
                    Dim epm1 As Double = e(p - 2) / scale
                    Dim sk As Double = s(k) / scale
                    Dim ek As Double = e(k) / scale
                    Dim b As Double = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2
                    Dim c As Double = (sp * epm1) * (sp * epm1)
                    Dim shift As Double = 0
                    If b <> 0 OrElse c <> 0 Then
                        shift = Math.Sqrt(b * b + c)
                        If b < 0 Then shift = -shift
                        shift = c / (b + shift)
                    End If
                    Dim f As Double = (sk + sp) * (sk - sp) + shift
                    Dim g As Double = sk * ek
                    ' Chase zeros.
                    For j = k To p - 2
                        Dim t As Double = Hypot(f, g)
                        Dim cs As Double = f / t
                        Dim sn As Double = g / t
                        If j <> k Then e(j - 1) = t
                        f = cs * s(j) + sn * e(j)
                        e(j) = cs * e(j) - sn * s(j)
                        g = sn * s(j + 1)
                        s(j + 1) = cs * s(j + 1)

                        'wantv
                        For i = 0 To n - 1
                            t = cs * V(i, j) + sn * V(i, j + 1)
                            V(i, j + 1) = -sn * V(i, j) + cs * V(i, j + 1)
                            V(i, j) = t
                        Next
                        'end wantv

                        t = Hypot(f, g)
                        cs = f / t
                        sn = g / t
                        s(j) = t
                        f = cs * e(j) + sn * s(j + 1)
                        s(j + 1) = -sn * e(j) + cs * s(j + 1)
                        g = sn * e(j + 1)
                        e(j + 1) = cs * e(j + 1)

                        'wantu
                        If j < m - 1 Then
                            For i = 0 To m - 1
                                t = cs * U(i, j) + sn * U(i, j + 1)
                                U(i, j + 1) = -sn * U(i, j) + cs * U(i, j + 1)
                                U(i, j) = t
                            Next
                        End If
                        'end wantu

                    Next
                    e(p - 2) = f
                    iter += 1
                Case 4 'Convergence.
                    ' Make the singular values positive.
                    If s(k) <= 0 Then
                        s(k) = Math.Abs(s(k))
                        'wantv
                        For i = 0 To pp
                            V(i, k) = -V(i, k)
                        Next
                        'end wantu
                    End If
                    ' Order the singular values.

                    While (k < pp)
                        If (s(k) >= s(k + 1)) Then Exit While
                        Dim t As Double = s(k)
                        s(k) = s(k + 1)
                        s(k + 1) = t

                        'wantu
                        If k < n - 1 Then
                            For i = 0 To n - 1
                                t = V(i, k + 1)
                                V(i, k + 1) = V(i, k)
                                V(i, k) = t
                            Next
                        End If
                        If k < m - 1 Then
                            For i = 0 To m - 1
                                t = U(i, k + 1)
                                U(i, k + 1) = U(i, k)
                                U(i, k) = t
                            Next
                        End If
                        'end wantu

                        k += 1

                    End While
                    iter = 0
                    p -= 1
            End Select

        End While
    End Sub

    ''' <summary>Return the one-dimensional array of singular values</summary>
    ''' <returns>     diagonal of S.
    ''' </returns>
    Public Overridable ReadOnly Property SingularValues() As Double()
        Get
            Return s
        End Get
    End Property

    ''' <summary>Return the diagonal matrix of singular values</summary>
    ''' <returns>     S
    ''' </returns>
    Public Overridable ReadOnly Property GetS() As Matrix
        Get
            Dim S = New Double(n - 1, n - 1) {}
            For i = 0 To n - 1
                For j = 0 To n - 1
                    S(i, j) = 0
                Next
                S(i, i) = Me.s(i)
            Next
            Return New Matrix(S)
        End Get
    End Property

    ''' <summary>Return the left singular vectors</summary>
    ''' <returns>     U
    ''' </returns>
    Public Overridable Function GetU() As Matrix
        Return New Matrix(U)
    End Function

    ''' <summary>Return the right singular vectors</summary>
    ''' <returns>     V
    ''' </returns>
    Public Overridable Function GetV() As Matrix
        Return New Matrix(V)
    End Function

    ''' <summary>Two norm</summary>
    ''' <returns>     max(S)
    ''' </returns>
    Public Overridable Function Norm2() As Double
        Return s(0)
    End Function

    ''' <summary>Two norm condition number</summary>
    ''' <returns>     max(S)/min(S)
    ''' </returns>
    Public Overridable Function Condition() As Double
        Return s(0) / s(Math.Min(m, n) - 1)
    End Function

    ''' <summary>Effective numerical matrix rank</summary>
    ''' <returns>     Number of nonnegligible singular values.
    ''' </returns>
    Public Overridable Function Rank() As Integer
        Dim eps As Double = 2 ^ -52
        Dim tol As Double = Math.Max(m, n) * s(0) * eps
        Dim r As Integer = 0
        For i = 0 To s.Length - 1
            If s(i) > tol Then
                r += 1
            End If
        Next
        Return r
    End Function
End Class

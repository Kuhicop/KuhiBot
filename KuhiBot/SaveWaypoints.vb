Imports System.IO

Public Class SaveWaypoints
    Public Sub saveTXTwaypoints(ByVal X As ListBox, ByVal Y As ListBox, ByVal Z As ListBox, ByVal textname As String)
        Dim swEscritor As StreamWriter
        Dim txtpath As String = Directory.GetCurrentDirectory() + "\" + textname + ".txt"
        Dim i As Integer
        swEscritor = New StreamWriter(txtpath)
        Try
            For Each item In X.Items
                swEscritor.WriteLine(X.Items(i))
                swEscritor.WriteLine(Y.Items(i))
                swEscritor.WriteLine(Z.Items(i))
                i += 1
            Next
            swEscritor.Close()
            MessageBox.Show("Saved as: " & vbCrLf & txtpath)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class
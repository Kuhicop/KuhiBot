Imports System.IO

Module LoadWaypoints
    Public Sub loadTXTwaypoints(ByRef X As ListBox, ByRef Y As ListBox, ByRef Z As ListBox, ByVal textname As String)
        Dim srReader As StreamReader
        Dim txtpath As String = Directory.GetCurrentDirectory() + "\" + textname + ".txt"
        Dim i As Integer
        srReader = New StreamReader(txtpath)
        Try
            Do Until srReader.EndOfStream
                X.Items.Add(srReader.ReadLine)
                Y.Items.Add(srReader.ReadLine)
                Z.Items.Add(srReader.ReadLine)
            Loop

            srReader.Close()
            MessageBox.Show("Loaded: " & vbCrLf & txtpath)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Module

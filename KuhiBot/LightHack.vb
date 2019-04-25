''**************
''* LIGHT HACK *
''**************
'Module LightHack
'    'Light hack ON
'    Private Sub butLightHackON_Click(sender As Object, e As EventArgs)
'        Dim _myBytes(3) As Byte
'        Dim mytext As Integer

'        'Check Light1 address
'        Try
'            ReadProcessMemory(_targetProcessHandle, Light1, _myBytes, 4, vbNull)
'            mytext = BitConverter.ToInt32(_myBytes, 0)
'            If mytext >= 0 And mytext < 8 Then
'                lightindex = 1
'                WriteValueToAddress(_targetProcessHandle, CInt(Light1), 11)
'                Exit Sub
'            End If
'        Catch ex As Exception
'        End Try

'        'Check Light2 address
'        Try
'            ReadProcessMemory(_targetProcessHandle, Light2, _myBytes, 4, vbNull)
'            mytext = BitConverter.ToInt32(_myBytes, 0)
'            If mytext >= 0 And mytext < 8 Then
'                lightindex = 2
'                WriteValueToAddress(_targetProcessHandle, CInt(Light2), 11)
'                Exit Sub
'            End If
'        Catch ex As Exception
'        End Try

'        'Check Light3 address
'        Try
'            ReadProcessMemory(_targetProcessHandle, Light3, _myBytes, 4, vbNull)
'            mytext = BitConverter.ToInt32(_myBytes, 0)
'            If mytext >= 0 And mytext < 8 Then
'                lightindex = 3
'                WriteValueToAddress(_targetProcessHandle, CInt(Light3), 11)
'                Exit Sub
'            End If
'        Catch ex As Exception
'            Dim result = MessageBox.Show("Restart client and bot if you really want light working!", "Light reset error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
'        End Try

'        'Check Light4 address
'        Try
'            ReadProcessMemory(_targetProcessHandle, Light4, _myBytes, 4, vbNull)
'            mytext = BitConverter.ToInt32(_myBytes, 0)
'            If mytext >= 0 And mytext < 8 Then
'                lightindex = 1
'                WriteValueToAddress(_targetProcessHandle, CInt(Light4), 11)
'                Exit Sub
'            End If
'        Catch ex As Exception
'        End Try
'    End Sub

'    'Light hack OFF
'    Private Sub butLightHackOFF_Click_1(sender As Object, e As EventArgs)
'        Try
'            Select Case lightindex
'                Case 1
'                    WriteValueToAddress(_targetProcessHandle, CInt(Light1), 2)
'                Case 2
'                    WriteValueToAddress(_targetProcessHandle, CInt(Light2), 2)
'                Case 3
'                    WriteValueToAddress(_targetProcessHandle, CInt(Light3), 2)
'                Case Else
'                    Dim result = MessageBox.Show("If doesn't work just make utevo lux!", "Light reset error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
'            End Select
'        Catch ex As Exception
'            MessageBox.Show(ex.Message)
'        End Try
'    End Sub

'End Module

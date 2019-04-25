Imports System.IO

Public Class Alerts
    'Play sound
    Public Sub playsound()
        My.Computer.Audio.Play(Directory.GetCurrentDirectory() + "\danger.wav", AudioPlayMode.WaitToComplete)
    End Sub

    'Logout
    Public Sub Logout()
        NativeMethods.GetFocus()
        My.Computer.Keyboard.SendKeys("^q", True)
    End Sub

    'Close client
    Public Sub CloseClient()
        Dim aProcess As System.Diagnostics.Process
        aProcess = System.Diagnostics.Process.GetProcessById(CInt(NativeMethods.txtPID.Text))
        aProcess.Kill()
    End Sub

    'Close bot
    Public Sub CloseBot()
        Application.Exit()
    End Sub

    'Loop around checkbox
    Public Sub LoopControls()
        '****************
        '* SOUND ALARMS *
        '****************
        With NativeMethods
            'HP above
            If .aS1.Checked Then
                If CInt(.labHPActual.Text) > CInt(.a1txt.Text) Then
                    playsound()
                End If
            End If
            If .aL1.Checked Then
                If CInt(.labHPActual.Text) > CInt(.a1txt.Text) And .labOnOff.BackColor = Color.Green Then
                    Logout()
                End If
            End If
            If .aB1.Checked Then
                If CInt(.labHPActual.Text) > CInt(.a1txt.Text) Then
                    CloseBot()
                End If
            End If
            If .aX1.Checked Then
                If CInt(.labHPActual.Text) > CInt(.a1txt.Text) Then
                    CloseClient()
                End If
            End If


            'HP below
            If .aS2.Checked Then
                If CInt(.labHPActual.Text) < CInt(.a2txt.Text) Then
                    playsound()
                End If
            End If
            If .aL2.Checked Then
                If CInt(.labHPActual.Text) < CInt(.a2txt.Text) Then
                    Logout()
                End If
            End If
            If .aB2.Checked Then
                If CInt(.labHPActual.Text) < CInt(.a2txt.Text) Then
                    CloseBot()
                End If
            End If
            If .aX2.Checked Then
                If CInt(.labHPActual.Text) < CInt(.a2txt.Text) Then
                    CloseClient()
                End If
            End If

            'MP above
            If .aS3.Checked Then
                If CInt(.labMPActual.Text) > CInt(.a3txt.Text) Then
                    playsound()
                End If
            End If
            If .aL3.Checked Then
                If CInt(.labMPActual.Text) > CInt(.a3txt.Text) Then
                    Logout()
                End If
            End If
            If .aB3.Checked Then
                If CInt(.labMPActual.Text) > CInt(.a3txt.Text) Then
                    CloseBot()
                End If
            End If
            If .aX3.Checked Then
                If CInt(.labMPActual.Text) > CInt(.a3txt.Text) Then
                    CloseClient()
                End If
            End If

            'MP below
            If .aS4.Checked Then
                If CInt(.labMPActual.Text) < CInt(.a4txt.Text) Then
                    playsound()
                End If
            End If
            If .aL4.Checked Then
                If CInt(.labMPActual.Text) < CInt(.a4txt.Text) Then
                    Logout()
                End If
            End If
            If .aB4.Checked Then
                If CInt(.labMPActual.Text) < CInt(.a4txt.Text) Then
                    CloseBot()
                End If
            End If
            If .aX4.Checked Then
                If CInt(.labMPActual.Text) < CInt(.a4txt.Text) Then
                    CloseClient()
                End If
            End If

            'PING above
            If .aS5.Checked Then
                If CInt(.labping.Text) > CInt(.a5txt.Text) Then
                    playsound()
                End If
            End If
            If .aL5.Checked Then
                If CInt(.labping.Text) > CInt(.a5txt.Text) Then
                    Logout()
                End If
            End If
            If .aB5.Checked Then
                If CInt(.labping.Text) > CInt(.a5txt.Text) Then
                    CloseBot()
                End If
            End If
            If .aX5.Checked Then
                If CInt(.labping.Text) > CInt(.a5txt.Text) Then
                    CloseClient()
                End If
            End If

            'OFFLINE
            If .aS6.Checked Then
                If .labOnOff.BackColor = Color.Red Then
                    playsound()
                End If
            End If
            If .aL6.Checked Then
                If .labOnOff.BackColor = Color.Red Then
                    Logout()
                End If
            End If
            If .aB6.Checked Then
                If .labOnOff.BackColor = Color.Red Then
                    CloseBot()
                End If
            End If
            If .aX6.Checked Then
                If .labOnOff.BackColor = Color.Red Then
                    CloseClient()
                End If
            End If

            'MOVED IN NATIVE METHODS

            'POISONED
            If .aS8.Checked Then
                If .lablastservermsg.Text = "You are poisoned." Then
                    playsound()
                End If
            End If
            If .aL8.Checked Then
                If .lablastservermsg.Text = "You are poisoned." Then
                    Logout()
                End If
            End If
            If .aB8.Checked Then
                If .lablastservermsg.Text = "You are poisoned." Then
                    CloseBot()
                End If
            End If
            If .aX8.Checked Then
                If .lablastservermsg.Text = "You are poisoned." Then
                    CloseClient()
                End If
            End If

            'VIP ONLINE
            If .aS9.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged in." Then
                    playsound()
                End If
            End If
            If .aL9.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged in." Then
                    Logout()
                End If
            End If
            If .aB9.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged in." Then
                    CloseBot()
                End If
            End If
            If .aX9.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged in." Then
                    CloseClient()
                End If
            End If

            'VIP OFFLINE
            If .aS10.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged out." Then
                    playsound()
                End If
            End If
            If .aL10.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged out." Then
                    Logout()
                End If
            End If
            If .aB10.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged out." Then
                    CloseBot()
                End If
            End If
            If .aX10.Checked Then
                If .lablastservermsg.Text = .txtvip.Text + " has logged out." Then
                    CloseClient()
                End If
            End If

            'AROUND CHANGED IN NATIVE METHODS
            'NOT AROUND IN NATIVE METHODS
        End With
    End Sub
End Class
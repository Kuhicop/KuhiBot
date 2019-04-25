Module Keyboard
    Private Declare Function GetKeyState Lib "user32" (ByVal nVirtKey As IntPtr) As Short

    'Check if insert hold
    Public Function givemetheins() As Boolean
        If GetKeyState(Keys.Insert).ToString <> "-127" And GetKeyState(Keys.Insert).ToString <> "-128" Then
            Return False
        Else
            Return True
        End If
    End Function

    'Check if insert hold
    Public Function givemetheend()
        If GetKeyState(Keys.End).ToString <> "-127" And GetKeyState(Keys.End).ToString <> "-128" Then
            Return False
        Else
            Return True
        End If
    End Function

    'Check if insert hold
    Public Function givemethehome()
        Return GetKeyState(Keys.Home).ToString
        If GetKeyState(Keys.Home).ToString <> "-127" And GetKeyState(Keys.Home).ToString <> "-128" Then
            Return False
        Else
            Return True
        End If
    End Function

    'Check if cntrl hold
    Public Function givemethectrl() As Boolean
        If GetKeyState(Keys.ControlKey).ToString <> "-127" And GetKeyState(Keys.ControlKey).ToString <> "-128" Then
            Return False
        Else
            Return True
        End If
    End Function
End Module

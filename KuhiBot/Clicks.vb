Module Clicks
    Public Declare Sub mouse_event Lib "user32" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)
    Public Declare Function BlockInput Lib "user32" Alias "BlockInput" (ByVal fBlock As Integer) As Integer
    Declare Function GetCursorPos Lib "user32.dll" (ByRef lpPoint As POINT_API) As Boolean
    Public Structure POINT_API
        Public X As Integer
        Public Y As Integer
    End Structure

    'LEFT CLICK
    Public Sub left_click()
        mouse_event(&H2, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(100)
        mouse_event(&H4, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(100)
    End Sub

    'RIGHT CLICK
    Public Sub right_click()
        mouse_event(&H8, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(100)
        mouse_event(&H10, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(100)
    End Sub

    'Down left mouse
    Public Sub Down_Mouse()
        mouse_event(&H2, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(100)
    End Sub

    'Up left mouse
    Public Sub Up_Mouse()
        mouse_event(&H4, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(100)
    End Sub

    'Both click
    Public Sub click_both()
        System.Threading.Thread.Sleep(200)
        mouse_event(&H2, 0, 0, 0, 0)
        mouse_event(&H8, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(200)
        mouse_event(&H4, 0, 0, 0, 0)
        mouse_event(&H10, 0, 0, 0, 0)
        System.Threading.Thread.Sleep(400)
    End Sub
End Module
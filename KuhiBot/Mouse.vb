Module MouseXY
    'Get mouse X/Y
    Public Sub GetMouseXY(ByRef p As POINT_API)
        GetCursorPos(p)
        MessageBox.Show("OK!", "Position found")
    End Sub

    'Silenced get mouse X/Y
    Public Sub GetSMouseXY(ByRef p As POINT_API)
        GetCursorPos(p)
    End Sub
End Module
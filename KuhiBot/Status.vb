Public Class Status
    Public busy As Boolean = False
    Public paused As Boolean = False

    Public Sub setbusy()
        busy = True
    End Sub

    Public Sub setfree()
        busy = False
    End Sub

    Public Sub pausebot()
        paused = True
    End Sub

    Public Sub resumebot()
        paused = False
    End Sub
End Class

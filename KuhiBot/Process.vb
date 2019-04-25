Imports System.Runtime.InteropServices
Imports System.ComponentModel

Module ProcessCalls
    Public Declare Function OpenProcess Lib "kernel32.dll" (ByVal dwDesiredAcess As UInt32, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Int32) As IntPtr
    Public Declare Function ReadProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean
    Public Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As IntPtr) As Boolean
    Public Declare Function WriteProcessMemory Lib "Kernell32" Alias "WriteProcessMemory" (ByVal hprocess As Integer, ByVal Address As Integer, ByRef Value As Integer, ByVal Size As Integer, ByRef BytesWritten As Integer) As Integer
    <DllImport("user32.dll")>
    Public Function SetWindowText(ByVal hwnd As IntPtr, ByVal windowName As String) As Boolean
    End Function


    <DllImport("kernel32.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi)>
    Public Function WriteProcessMemory(ByVal hProcess As System.IntPtr, ByVal lpBaseAddress As System.IntPtr, ByVal lpBuffer() As Byte, ByVal nSize As Integer, <Out()> ByRef lpNumberOfBytesWritten As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Public PROCESS_ALL_ACCESS As UInt32 = &H1F0FFF
    Public PROCESS_VM_READ As UInt32 = &H10
End Module

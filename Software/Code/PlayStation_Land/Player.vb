Imports System.Windows.Threading

Public Class Player
    Public PName As String
    Public Pspan As TimeSpan
    Public Pcredit As Integer
    Public TargetDT As DateTime
    Public pimage As New Image
    Public timer As String
    Public active As Boolean
    Public Always_One As Boolean
    Public alwaycounter As Double
    Dim AlwaysTimer As DispatcherTimer = New DispatcherTimer()

    Public Sub New(ByVal pid As Integer, ByVal PName As String, ByVal Pcredit As Integer, ByVal Target As DateTime)
        PName = PName
        Pcredit = -1
        TargetDT = Target
        active = False
        timer = "00:00:00"
        Always_One = False
        alwaycounter = 0
        AddHandler AlwaysTimer.Tick, AddressOf AlwaysTimer_Tick
        AlwaysTimer.Interval = New TimeSpan(0, 0, 0, 0, 1000)

    End Sub
    Public Function startTimer()
        AlwaysTimer.Start()
    End Function
    Public Function StopTimer()

        AlwaysTimer.Stop()
    End Function
    Public Sub AlwaysTimer_Tick(ByVal sender As Object, e As System.EventArgs)
       
        Me.alwaycounter += 1



        With TimeSpan.FromSeconds(Me.alwaycounter)
            Me.timer = .ToString("hh\:mm\:ss")
        End With
    End Sub

End Class

Imports System.Windows.Threading
Imports System.Data
Imports System.Net.Sockets
Imports System.Text
Imports System.Net
Imports System.IO

Public Class ScreenControl
    Private TargetDT As DateTime
    Private CountDownFrom As TimeSpan
    Dim dt As DispatcherTimer = New DispatcherTimer()

    Public ScreenName As String
    Public POWER_URL As String
    Public LED_URL As String
    Public screen_clicked As Boolean
    Public startupPoint As New Point
    Public relativePoint As New Point
    Dim activescreen As String
    Dim coin_p5_clicked As Boolean
    Dim coin_p10_clicked As Boolean
    Dim reset_clicked As Boolean
    Dim power_clicked As Boolean
    Public id As Integer
    Dim coin_m10_clicked As Boolean
    Dim coin_m5_clicked As Boolean
    Public credit As Integer
    Dim rate As Double
    Public enabled As Boolean
    Dim started As Boolean = True
    Dim time1 As DateTime
    Dim always_clicked As Boolean = False
    Dim time2 As DateTime
    Public players As New List(Of Player)
    Public selected_player As Integer
    Dim was_selected As Integer
    Dim redscreen As Integer
    Public CountOfAlwaysOn As Integer = 0
    Dim isreadon As Boolean = False


    Public Sub New(ByVal idd As Integer, ByVal ScreenNamee As String, ByVal PURL As String, ByVal LURL As String)
        For i As Integer = 1 To 4
            players.Add(New Player(0, "Player " & i, credit, TargetDT))
        Next
        selected_player = 0
        Me.p1 = players(0).pimage
        Dim t As New DataTable
        t = db.query("select rate from rate")
        rate = Double.Parse(t.Rows(0)(0))
        InitializeComponent()
        ScreenName = ScreenNamee
        POWER_URL = PURL
        LED_URL = LURL
        [alias].Text = ScreenNamee
        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 0, 0, 200)
        coin_p5_clicked = False
        coin_p10_clicked = False
        coin_m10_clicked = False
        coin_m5_clicked = False
        reset_clicked = False
        id = idd
        Me.p1.Source = New BitmapImage(New Uri("Resources\Player_ON.png", UriKind.Relative))
        was_selected = -1
        Dim updateclockthread As System.Threading.Thread
        updateclockthread = New System.Threading.Thread(AddressOf updateclock)
        updateclockthread.IsBackground = True
        updateclockthread.Start()
    End Sub
    Public Sub dispatcherTimer_Tick(ByVal sender As Object, e As System.EventArgs)
        Dim totalSeconds As Integer
        For i As Integer = 0 To players.Count - 1
            If players(i).active = True And players(i).Always_One <> True Then

                Dim ts As TimeSpan = players(i).TargetDT.Subtract(DateTime.Now)

                totalSeconds = totalSeconds + Convert.ToInt32(ts.TotalSeconds)
                If Convert.ToInt32(ts.TotalSeconds) <= 300 Then
                    DirectCast(PlayerImages.Children(i), Image).Source = New BitmapImage(New Uri("Resources\Player_Finishing.png", UriKind.Relative))
                    CheckIfRedScreen()
                Else
                    If (isreadon = True) Then
                        Dim parentWindow As MainWindow = DirectCast(Window.GetWindow(Me), MainWindow)
                        If parentWindow.LED_image.ToolTip <> "Disconnected" Then
                            Dim t As DataTable = db.query("select LED_URL from screens where id=" & Me.id & "")
                            Dim url As String = String.Concat(t.Rows(0)(0), "&state=", "0")
                            Dim request As HttpWebRequest = WebRequest.Create(url)
                            request.Timeout = 5000
                            Dim response As HttpWebResponse = request.GetResponse()
                            Dim stream As Stream = response.GetResponseStream()
                            response.Close()
                            isreadon = False
                        Else
                            isreadon = False
                        End If

                    End If

                End If

                If ts.TotalSeconds >= 0 Then
                    players(i).Pcredit = ts.TotalSeconds
                    players(i).timer = ts.ToString("hh\:mm\:ss")

                Else
                    DirectCast(PlayerImages.Children(i), Image).Source = New BitmapImage(New Uri("Resources\Player_Timeout.png", UriKind.Relative))
                    players(i).Pcredit = 0
                    players(i).timer = "00:00:00"
                    players(i).active = False
                    Me.power.IsEnabled = True
                End If

            End If

        Next

        If players(selected_player).Pcredit > 0 Then
            Me.power.IsEnabled = False
        End If
        ' Shuttding Down the Screen in case all the players have less than 10 minutes----------------------------------------------

        If totalSeconds <= 0 And Not CountOfAlwaysOn > 0 Then
            dt.Stop()
            For i As Integer = 0 To players.Count - 1
                If players(i).active = True Then
                    DirectCast(PlayerImages.Children(i), Image).Source = New BitmapImage(New Uri("Resources\Player_Timeout.png", UriKind.Relative))
                    players(i).Pcredit = 0
                    players(i).timer = "00:00:00"
                    players(i).active = False
                End If
            Next
            If Me.Stopp() Then
                Exit Sub

            End If
        End If
        ' Shuttding Down the Screen in case all the players have less than 10 minutes-----------------------------------------------


    End Sub
    Public Function CheckIfRedScreen()

        For Each Player In players
            If Player.Always_One = True Then
                Return True

            End If
        Next
        Dim totalplayers As Integer = 0
        redscreen = 0
        For i = 0 To 3
            Dim ts As TimeSpan
            If players(i).active = True Then
                totalplayers = totalplayers + 1
                ts = players(i).TargetDT.Subtract(DateTime.Now)
                If ts.TotalSeconds < 300 Then
                    redscreen = redscreen + 1
                End If
            End If

        Next

        If redscreen >= totalplayers Then
            Dim image As New Image()
            Dim back As New ImageBrush
            image.Source = New BitmapImage(New Uri("Resources/HDRedScreen.png", UriKind.Relative))

            If isreadon = False Then
                Dim parentWindow As MainWindow = DirectCast(Window.GetWindow(Me), MainWindow)
                If parentWindow.LED_image.ToolTip <> "Disconnected" Then
                    Dim t As DataTable = db.query("select LED_URL from screens where id=" & Me.id & "")
                    Dim webClient As New System.Net.WebClient
                    Dim url As String = String.Concat(t.Rows(0)(0), "&state=", "1")
                    Dim request As HttpWebRequest = WebRequest.Create(url)
                    request.Timeout = 5000
                    Dim response As HttpWebResponse = request.GetResponse()
                    Dim stream As Stream = response.GetResponseStream()
                    response.Close()
                    isreadon = True
                Else
                    isreadon = True
                End If

            End If

            back.ImageSource = image.Source
            MainPanel.Background = back
            Return True
            

        End If


    End Function
    Public Function turnoffledalwayclicked()
        Dim parentWindow As MainWindow = DirectCast(System.Windows.Application.Current.MainWindow, MainWindow)
        If parentWindow.LED_image.ToolTip <> "Disconnected" Then
            Dim t As DataTable = db.query("select LED_URL from screens where id=" & Me.id & "")
            Dim webClient As New System.Net.WebClient
            Dim url As String = String.Concat(t.Rows(0)(0), "&state=", "0")
            Dim result As String = webClient.DownloadString(url)
            isreadon = False
        End If
    End Function
    Public Function addcredit(ByVal credit As Integer)
        CountDownFrom = TimeSpan.FromSeconds(players(selected_player).Pcredit)
        players(selected_player).TargetDT = DateTime.Now.Add(CountDownFrom)
        dt.Start()



    End Function
    Public Function addcreditfromrestore(ByVal credit As Integer, ByVal player As Integer)
        CountDownFrom = TimeSpan.FromSeconds(players(player).Pcredit)
        players(player).TargetDT = DateTime.Now.Add(CountDownFrom)
        dt.Start()

    End Function
    Public Function showpanels()
        Me.toppanel.Visibility = Windows.Visibility.Visible
        Me.bottompanel.Visibility = Windows.Visibility.Visible
        Me.rightpanel.Visibility = Windows.Visibility.Visible
        Me.leftpanel.Visibility = Windows.Visibility.Visible

    End Function
    Public Function hidepanels()
        Me.toppanel.Visibility = Windows.Visibility.Hidden
        Me.bottompanel.Visibility = Windows.Visibility.Hidden
        Me.rightpanel.Visibility = Windows.Visibility.Hidden
        Me.leftpanel.Visibility = Windows.Visibility.Hidden

    End Function
    Public Function Shutdown()
        MessageBox.Show("Shutdown " + LED_URL + "   " + POWER_URL)
    End Function
    Private Sub MainPanel_MouseLeave(sender As Object, e As MouseEventArgs) Handles MainPanel.MouseLeave
        Me.Cursor = Cursors.Arrow
    End Sub
    Private Sub ScreenControl_MouseLeave(sender As Object, e As MouseEventArgs) Handles Me.MouseLeave
        Dim image As New Image()
        Dim back As New ImageBrush
        image.Source = New BitmapImage(New Uri("Resources/HDBlueScreen.png", UriKind.Relative))
        back.ImageSource = image.Source
        MainPanel.Background = back

    End Sub
    Private Sub MainPanel_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles MainPanel.MouseDown
        Me.screen_clicked = True
        Dim parentWindow As MainWindow = DirectCast(Window.GetWindow(Me), MainWindow)
        parentWindow.hideall()
        showpanels()


    End Sub
    Private Sub MainPanel_MouseEnter(sender As Object, e As MouseEventArgs) Handles MainPanel.MouseEnter
        Me.Cursor = Cursors.Hand
        Dim image As New Image()
        Dim back As New ImageBrush
        image.Source = New BitmapImage(New Uri("Resources/HDHoverScreen.png", UriKind.Relative))
        back.ImageSource = image.Source
        MainPanel.Background = back
    End Sub
    Private Sub m5_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles m5.MouseDown
        coin_m5_clicked = True
    End Sub
    Private Sub m10_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles m10.MouseDown
        coin_m10_clicked = True
    End Sub
    Private Sub p5_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles p5.MouseDown
        coin_p5_clicked = True
    End Sub
    Private Sub p10_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles p10.MouseDown
        coin_p10_clicked = True
    End Sub
    Private Sub power_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles power.MouseDown
        power_clicked = True
    End Sub
    Private Sub reset_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles reset.MouseDown
        reset_clicked = True
    End Sub
    Private Sub m5_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles m5.MouseUp
        If players(selected_player).active = False Then
            Exit Sub
        End If
        Dim t As New DataTable
        t = db.query("select rate from rate")
        rate = Double.Parse(t.Rows(0)(0))

        If coin_m5_clicked = True Then
            players(selected_player).Pcredit = players(selected_player).Pcredit - (rate * 60 * 5)

            If players(selected_player).Pcredit <> 0 Then
                db.query("insert into logging values(" & Me.id & ",'" & Me.ScreenName.ToString & "'," & -1 * (rate * 60 * 5) & ",#" & FormatDateTime(Now, DateFormat.ShortDate) & "#)")
            End If
            Me.addcredit(credit)

            coin_m5_clicked = False
        Else
            coin_m5_clicked = False
        End If
    End Sub
    Private Sub p5_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles p5.MouseUp
        turnoffledalwayclicked()
        Dim parentWindow As MainWindow = DirectCast(Window.GetWindow(Me), MainWindow)
        If parentWindow.Power_image.ToolTip = "Disconnected" Then
            MsgBox("Power Module Disconnected")
            Exit Sub
        Else

            If Me.start() = False Then
                Exit Sub
            End If
        End If



        Dim AnyActiveplayer As Boolean = False
        For Each x As Player In players
            If x.active = True Then
                AnyActiveplayer = True
            End If
        Next
        If AnyActiveplayer = False Then
           Else
            If players(selected_player).Pcredit <= 0 Then
                Dim PlayerName As String = InputBox("Player Name:-", "Enter Player Name", " ")
                players(selected_player).PName = PlayerName
                DirectCast(PlayerImages.Children(selected_player), Image).ToolTip = players(selected_player).PName
            End If

        End If
        Dim t As New DataTable
        t = db.query("select rate from rate")
        rate = Double.Parse(t.Rows(0)(0))
        players(selected_player).active = True
        If coin_p5_clicked = True Then
            players(selected_player).Pcredit = players(selected_player).Pcredit + (rate * 60 * 5)
            If players(selected_player).Pcredit > 10 Then
                DirectCast(PlayerImages.Children(selected_player), Image).Source = New BitmapImage(New Uri("Resources\Player_Selected.png", UriKind.Relative))

            End If
            db.query("insert into logging values(" & Me.id & ",'" & Me.ScreenName.ToString & "'," & (rate * 60 * 5) & ",#" & FormatDateTime(Now, DateFormat.ShortDate) & "#)")


            Me.addcredit(credit)
            coin_p5_clicked = False
        Else
            coin_p5_clicked = False
        End If
    End Sub
    Private Sub m10_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles m10.MouseUp

        If players(selected_player).active = False Then
            Exit Sub
        End If
        Dim t As New DataTable
        t = db.query("select rate from rate")
        rate = Double.Parse(t.Rows(0)(0))

        If coin_m10_clicked = True Then
            players(selected_player).Pcredit = players(selected_player).Pcredit - (rate * 60 * 10)
            If players(selected_player).Pcredit <> 0 Then
                db.query("insert into logging values(" & Me.id & ",'" & Me.ScreenName.ToString & "'," & -1 * (rate * 60 * 10) & ",#" & FormatDateTime(Now, DateFormat.ShortDate) & "#)")
            End If
            Me.addcredit(credit)
            coin_m10_clicked = False
        Else
            coin_m10_clicked = False
        End If
    End Sub
    Private Sub p10_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles p10.MouseUp
        turnoffledalwayclicked()
        Dim parentWindow As MainWindow = DirectCast(Window.GetWindow(Me), MainWindow)
        If parentWindow.Power_image.ToolTip = "Disconnected" Then
            MsgBox("Power Module Disconnected")
            Exit Sub
        Else
            If Me.start() = False Then
                Exit Sub
            End If
        End If



        Dim AnyActiveplayer As Boolean = False
        For Each x As Player In players
            If x.active = True Then
                AnyActiveplayer = True
            End If
        Next
        If AnyActiveplayer = False Then
            Else
            If players(selected_player).Pcredit <= 0 Then
                Dim PlayerName As String = InputBox("Player Name:-", "Enter Player Name", " ")
                players(selected_player).PName = PlayerName
                DirectCast(PlayerImages.Children(selected_player), Image).ToolTip = players(selected_player).PName
            End If


        End If

        Dim t As New DataTable
        t = db.query("select rate from rate")
        rate = Double.Parse(t.Rows(0)(0))
        players(selected_player).active = True
        If coin_p10_clicked = True Then
            players(selected_player).Pcredit = players(selected_player).Pcredit + (rate * 60 * 10)
            If players(selected_player).Pcredit > 10 Then
                DirectCast(PlayerImages.Children(selected_player), Image).Source = New BitmapImage(New Uri("Resources\Player_Selected.png", UriKind.Relative))

            End If
            db.execute("insert into logging values(" & Me.id & ",'" & Me.ScreenName.ToString & "'," & (rate * 60 * 10) & ",#" & FormatDateTime(Now, DateFormat.ShortDate) & "#)")


            Me.addcredit(credit)
            coin_p10_clicked = False
        Else
            coin_p10_clicked = False
        End If
    End Sub
    Private Sub reset_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles reset.MouseUp


        Dim t As New DataTable
        t = db.query("select rate from rate")
        rate = Double.Parse(t.Rows(0)(0))


        If reset_clicked = True Then
            If players(selected_player).Always_One = True Then
                CountOfAlwaysOn = CountOfAlwaysOn - 1
                Me.p10.IsEnabled = True
                Me.p5.IsEnabled = True
                Me.m10.IsEnabled = True
                Me.m5.IsEnabled = True
                Me.power.IsEnabled = True
                DirectCast(PlayerImages.Children(selected_player), Image).Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
                players(selected_player).timer = "00:00:00"
                Dim cost As Double
                cost = (players(selected_player).alwaycounter / 60) / rate
                Dim ts As New TimeSpan
                ts = TimeSpan.FromSeconds(players(selected_player).alwaycounter)

                If cost - Math.Floor(cost) >= 0.5 Then


                    Dim msgbox1 As New mymsgbox("Summary", String.Concat("Time Played:   ", ts.ToString("hh\:mm\:ss")), String.Concat("Price :   ", (Math.Floor(cost) + 0.5).ToString))
                Else
                    Dim msgbox1 As New mymsgbox("Summary", String.Concat("Time Played:   ", ts.ToString("hh\:mm\:ss")), String.Concat("Price :   ", Math.Floor(cost).ToString))
                End If
                players(selected_player).active = False
                players(selected_player).Always_One = False
                players(selected_player).StopTimer()
                db.execute("insert into logging values(" & Me.id & ",'" & Me.ScreenName.ToString & "'," & players(selected_player).alwaycounter & ",#" & FormatDateTime(Now, DateFormat.ShortDate) & "#)")

                players(selected_player).alwaycounter = 0
            Else
                players(selected_player).Pcredit = 0
                players(selected_player).timer = "00:00:00"
                players(selected_player).active = False
                Me.power.IsEnabled = True
                DirectCast(PlayerImages.Children(selected_player), Image).Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))

            End If

            Dim AnyActiveplayer As Boolean = False
            For Each x As Player In players
                If x.active = True Then
                    AnyActiveplayer = True
                End If
            Next
            If CountOfAlwaysOn = 0 And AnyActiveplayer = False Then
                dt.Stop()
                If Me.Stopp() Then
                    Exit Sub

                End If
            End If
            reset_clicked = False
        Else
            reset_clicked = False
        End If
    End Sub
    Private Sub power_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles power.MouseUp
        turnoffledalwayclicked()
        Dim parentWindow As MainWindow = DirectCast(Window.GetWindow(Me), MainWindow)
        If parentWindow.Power_image.ToolTip = "Disconnected" Then
            MsgBox("Power Module Disconnected")
            Exit Sub
        Else

            If Me.start() = False Then
                Exit Sub
            End If
        End If


        turnoffledalwayclicked()
        CountOfAlwaysOn = CountOfAlwaysOn + 1
        players(selected_player).alwaycounter = 0
        Dim AnyActiveplayer As Boolean = False
        For Each x As Player In players
            If x.active = True Then
                AnyActiveplayer = True
            End If
        Next
        If AnyActiveplayer = False Then
        Else
            If players(selected_player).Pcredit <= 0 Then
                Dim PlayerName As String = InputBox("Player Name:-", "Enter Player Name", " ")
                players(selected_player).PName = PlayerName
                DirectCast(PlayerImages.Children(selected_player), Image).ToolTip = players(selected_player).PName
            End If


        End If
        Me.p10.IsEnabled = False
        Me.p5.IsEnabled = False
        Me.m10.IsEnabled = False
        Me.m5.IsEnabled = False
        Me.power.IsEnabled = False
        players(selected_player).active = True
        players(selected_player).Always_One = True
        players(selected_player).startTimer()

    End Sub
    Public Function start()
        Try
            Dim t As DataTable = db.query("select POWER_URL from screens where id=" & Me.id & "")
            Dim url As String = String.Concat(t.Rows(0)(0), "&state=", "1")
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Timeout = 5000
            Dim response As HttpWebResponse = request.GetResponse()
            Dim stream As Stream = response.GetResponseStream()
            response.Close()
            Return True



        Catch ex As WebException
            MsgBox(ex.Message)
            Return False
        End Try

    End Function
    Public Function Stopp()
        Try
            Dim t As DataTable = db.query("select POWER_URL from screens where id=" & Me.id & "")
            Dim url As String = String.Concat(t.Rows(0)(0), "&state=", "0")
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Timeout = 5000
            Dim response As HttpWebResponse = request.GetResponse()
            Dim stream As Stream = response.GetResponseStream()
            response.Close()
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try





    End Function
    Private Sub p1_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles p1.MouseUp

        Me.p1.Source = New BitmapImage(New Uri("Resources\Player_On.png", UriKind.Relative))
        Me.p2.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p3.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p4.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        selected_player = 0
        ActiveUser()
        disablecoins(selected_player)

        If players(selected_player).Pcredit <= 0 And players(selected_player).Always_One = False Then
            Me.clock.Text = "00:00:00"
            Me.power.IsEnabled = True
        Else
            Me.power.IsEnabled = False
        End If
    End Sub
    Private Sub p2_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles p2.MouseUp
        disablecoins(selected_player)
        Me.p1.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p2.Source = New BitmapImage(New Uri("Resources\Player_On.png", UriKind.Relative))
        Me.p3.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p4.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        selected_player = 1
        ActiveUser()
        disablecoins(selected_player)
        If players(selected_player).Pcredit <= 0 And players(selected_player).Always_One = False Then
            Me.clock.Text = "00:00:00"
            Me.power.IsEnabled = True
        Else
            Me.power.IsEnabled = False

        End If
    End Sub
    Private Sub p3_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles p3.MouseUp
        disablecoins(selected_player)
        Me.p1.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p2.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p3.Source = New BitmapImage(New Uri("Resources\Player_On.png", UriKind.Relative))
        Me.p4.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))

        selected_player = 2
        ActiveUser()
        disablecoins(selected_player)

        If players(selected_player).Pcredit <= 0 And players(selected_player).Always_One = False Then
            Me.clock.Text = "00:00:00"
            Me.power.IsEnabled = True
        Else
            Me.power.IsEnabled = False
        End If
    End Sub
    Private Sub p4_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles p4.MouseUp
        disablecoins(selected_player)
        Me.p1.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p2.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p3.Source = New BitmapImage(New Uri("Resources\Player_Off.png", UriKind.Relative))
        Me.p4.Source = New BitmapImage(New Uri("Resources\Player_On.png", UriKind.Relative))
        selected_player = 3
        ActiveUser()
        disablecoins(selected_player)

        If players(selected_player).Pcredit <= 0 And players(selected_player).Always_One = False Then
            Me.clock.Text = "00:00:00"
            Me.power.IsEnabled = True
        Else
            Me.power.IsEnabled = False
        End If
    End Sub
    Public Function ActiveUser()

        For i As Integer = 0 To players.Count - 1
            If (players(i).Pcredit > 0 Or players(i).Always_One = True) And i <> selected_player Then

                DirectCast(PlayerImages.Children(i), Image).Source = New BitmapImage(New Uri("Resources\Player_Selected.png", UriKind.Relative))

            End If
        Next
    End Function
    Private Function DeActiveUser(ByVal value As Integer)
        DirectCast(PlayerImages.Children(value), Image).Source = New BitmapImage(New Uri("Resources\Player_Selected.png", UriKind.Relative))
    End Function
    Public Function disablecoins(ByVal Value As Integer)
        If players(Value).Always_One = True Then
            p5.IsEnabled = False
            p10.IsEnabled = False
            m5.IsEnabled = False
            m10.IsEnabled = False
            Me.power.IsEnabled = False
        Else
            p5.IsEnabled = True
            p10.IsEnabled = True
            m5.IsEnabled = True
            m10.IsEnabled = True
            Me.power.IsEnabled = True
        End If
    End Function

    Private Function updateclock() As Task()
        While (1)

            Dispatcher.Invoke(New Action(Function()
                                             Me.clock.Text = players(selected_player).timer
                                         End Function), DispatcherPriority.ContextIdle)
            System.Threading.Thread.Sleep(1000)
        End While
    End Function

End Class

Imports System.Timers
Imports System.Windows.Threading
Imports System.Data
Imports System.Threading
Imports System.IO
Imports System.Windows.Forms
Imports System.Net.Sockets
Imports System.Net

Class MainWindow
    Dim s As ScreenControl
    Dim watchdog As System.Threading.Thread
    Dim Module_Connection As System.Threading.Thread
    Dim listofscreens As New List(Of ScreenControl)
    Public listofscreens2 As New List(Of ScreenControl)
    Public POWERIP As String
    Public LEDIP As String
    Public POWERPORT As String
    Public LEDPORT As String

    Private Function watchdogthread() As Task()
        While (1)
            Thread.Sleep(30000)
            For i As Integer = 0 To listofscreens.Count - 1
                Dispatcher.Invoke(New Action(Function()
                                                 For ii As Integer = 0 To 3
                                                     If listofscreens(i).players(ii).alwaycounter > 0 Then
                                                         Dim status As String = db.execute("update status set credit=" & Integer.Parse(listofscreens(i).players(ii).alwaycounter) * -1 & " where id=" & listofscreens(i).id & " and player=" & ii & "")
                                                         If status <> "True" Then
                                                             System.Windows.MessageBox.Show(status)
                                                         End If
                                                     Else
                                                         Dim status As String = db.execute("update status set credit=" & Integer.Parse(listofscreens(i).players(ii).Pcredit) & " where id=" & listofscreens(i).id & " and player=" & ii & "")
                                                         If status <> "True" Then
                                                             System.Windows.MessageBox.Show(status)
                                                         End If

                                                     End If

                                                 Next
                                             End Function), DispatcherPriority.ContextIdle)
            Next
        End While
    End Function
    Private Sub Window_Loaded_1(sender As Object, e As RoutedEventArgs)
        Application.Current.MainWindow = Me
        Dim t As New DataTable
        Dim SafeClose As Boolean
        t = db.query("select SafeClose from rate")
        SafeClose = Boolean.Parse(t.Rows(0)(0))
        If Not SafeClose Then
            Dim descision As Integer = System.Windows.MessageBox.Show("A System crash has been detected , would you like to restore the latest saved System State?", "Invalid Detected ", MessageBoxButton.YesNo)
            If descision = 6 Then
                t = db.query("Select * from SCREENS where enabled=True order by [id] asc")
                For i As Integer = 0 To t.Rows.Count - 1
                    If t.Rows(i)(4) = "True" Then
                        s = New ScreenControl(t.Rows(i)(0), t.Rows(i)(1), t.Rows(i)(2), t.Rows(i)(2))
                        s.enabled = t.Rows(i)(4)
                        listofscreens.Add(s)
                        s.Margin = New Thickness(0, 0, 0, 20)
                        s.hidepanels()
                        For ii As Integer = 0 To 3

                            Dim t2 As DataTable = db.query("select credit from status where [id]= " & t.Rows(i)(0) & "  and player=" & ii & "")
                            If Integer.Parse(t2.Rows(0)(0)) > 0 Then


                                s.players(ii).active = True
                                s.players(ii).Pcredit = Integer.Parse(t2.Rows(0)(0))
                                s.addcreditfromrestore(Integer.Parse(t2.Rows(0)(0)), ii)
                                s.turnoffledalwayclicked()
                                s.start()
                            ElseIf Integer.Parse(t2.Rows(0)(0)) < 0 Then

                                s.CountOfAlwaysOn = s.CountOfAlwaysOn + 1
                                s.players(ii).alwaycounter = Integer.Parse(t2.Rows(0)(0)) * -1
                                s.players(ii).active = True
                                s.players(ii).Always_One = True
                                s.players(ii).startTimer()
                                s.turnoffledalwayclicked()
                                s.start()

                            Else

                                s.players(ii).Pcredit = 0
                            End If

                        Next

                        WrapPanel.Children.Add(s)

                    End If
                    s.ActiveUser()
                Next

            Else
                t = db.query("Select * from SCREENS where enabled=True order by [id] asc")
                For i As Integer = 0 To t.Rows.Count - 1
                    If t.Rows(i)(4) = "True" Then
                        s = New ScreenControl(t.Rows(i)(0), t.Rows(i)(1), t.Rows(i)(2), t.Rows(i)(2))
                        s.enabled = t.Rows(i)(4)
                        listofscreens.Add(s)
                        s.Margin = New Thickness(0, 0, 0, 20)
                        s.hidepanels()
                        WrapPanel.Children.Add(s)
                    End If
                Next


            End If
            Dim status As String = db.execute("update rate set SafeClose=False")
        Else
            t = db.query("Select * from SCREENS where enabled=True order by [id] asc")
            For i As Integer = 0 To t.Rows.Count - 1
                If t.Rows(i)(4) = "True" Then
                    s = New ScreenControl(t.Rows(i)(0), t.Rows(i)(1), t.Rows(i)(2), t.Rows(i)(2))
                    s.enabled = t.Rows(i)(4)
                    listofscreens.Add(s)
                    s.Margin = New Thickness(0, 0, 0, 20)
                    s.hidepanels()
                    WrapPanel.Children.Add(s)
                End If

            Next
            Dim status As String = db.execute("update rate set SafeClose=False")

        End If
        t = db.query("Select * from SCREENS where enabled=True order by [id] asc")
        Dim sp As New StackPanel
        If t.Rows.Count = 0 Then
            WrapPanel.Children.Clear()
            sp.HorizontalAlignment = Windows.HorizontalAlignment.Center
            sp.VerticalAlignment = Windows.VerticalAlignment.Center
            sp.Name = "MainSp"
            Dim textbox As New TextBlock
            textbox.Text = "No Screens were found in database , navigate to Setting to add new screens."
            textbox.VerticalAlignment = Windows.VerticalAlignment.Center
            textbox.HorizontalAlignment = Windows.HorizontalAlignment.Center
            Dim bc As New BrushConverter
            textbox.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            textbox.FontWeight = FontWeights.ExtraBlack
            textbox.FontSize = "30"
            textbox.TextWrapping = TextWrapping.Wrap
            sp.Uid = "EmptySP"
            sp.Children.Add(textbox)
            WrapPanel.Children.Add(sp)
            Exit Sub
        End If

        listofscreens2 = listofscreens
        watchdog = New Thread(AddressOf watchdogthread)
        watchdog.IsBackground = True
        watchdog.Start()
        Module_Connection = New Thread(AddressOf Connection_listener)
        Module_Connection.IsBackground = True
        Module_Connection.Start()

    End Sub
    Private Function Connection_listener()
        Dim t As New DataTable
        t = db.query("select ip from ip where [Module]='POWER' ")
        POWERIP = t.Rows(0)(0).ToString.Split(":")(0)
        POWERPORT = t.Rows(0)(0).ToString.Split(":")(1)
        t = db.query("select ip from ip where [Module]='LED' ")
        LEDIP = t.Rows(0)(0).ToString.Split(":")(0)
        LEDPORT = t.Rows(0)(0).ToString.Split(":")(1)
        Dim Client As TcpClient = Nothing

        While (1)

            Try
                If My.Computer.Network.Ping(POWERIP) Then
                    Dispatcher.Invoke(New Action(Function()
                                                     Me.Power_image.Source = New BitmapImage(New Uri("Resources\Greenicon.png", UriKind.Relative))
                                                     Me.Power_image.ToolTip = "Connected"
                                                 End Function), DispatcherPriority.ContextIdle)
                End If
            Catch ex As Exception
                Dispatcher.Invoke(New Action(Function()
                                                 Me.Power_image.Source = New BitmapImage(New Uri("Resources\redicon.png", UriKind.Relative))
                                                 Me.Power_image.ToolTip = "Disconnected"
                                             End Function), DispatcherPriority.ContextIdle)

            End Try
      
            Try
                If My.Computer.Network.Ping(LEDIP) Then
                    Dispatcher.Invoke(New Action(Function()
                                                     Me.LED_image.Source = New BitmapImage(New Uri("Resources\Greenicon.png", UriKind.Relative))
                                                     Me.LED_image.ToolTip = "Connected"
                                                 End Function), DispatcherPriority.ContextIdle)
                End If
            Catch ex As Exception
                Dispatcher.Invoke(New Action(Function()
                                                 Me.LED_image.Source = New BitmapImage(New Uri("Resources\redicon.png", UriKind.Relative))
                                                 Me.LED_image.ToolTip = "Disconnected"
                                             End Function), DispatcherPriority.ContextIdle)

            End Try


            Thread.Sleep(2000)


        

        End While


    End Function
    Public Function Init()
        For i As Integer = 0 To WrapPanel.Children.Count - 1
            If WrapPanel.Children(i).Uid = "EmptySP" Then
                WrapPanel.Children.RemoveAt(i)
            End If
        Next
        Dim idtoberemoved As Integer
        Dim t As New DataTable
        t = db.query("Select * from SCREENS where enabled=True order by [id] asc")
        Dim sp As New StackPanel
        If t.Rows.Count = 0 Then
            WrapPanel.Children.Clear()
            sp.HorizontalAlignment = Windows.HorizontalAlignment.Center
            sp.VerticalAlignment = Windows.VerticalAlignment.Center
            sp.Name = "MainSp"
            Dim textbox As New TextBlock
            textbox.Text = "No Screens were found in database , navigate to Setting to add new screens."
            textbox.VerticalAlignment = Windows.VerticalAlignment.Center
            textbox.HorizontalAlignment = Windows.HorizontalAlignment.Center
            Dim bc As New BrushConverter
            textbox.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            textbox.FontWeight = FontWeights.ExtraBlack
            textbox.FontSize = "30"
            textbox.TextWrapping = TextWrapping.Wrap
            sp.Uid = "EmptySP"
            sp.Children.Add(textbox)
            WrapPanel.Children.Add(sp)
            Exit Function
        End If
        Dim disabled_screens As New DataTable
        disabled_screens = db.query("Select * from SCREENS where enabled=False order by [id] asc")
        Dim elementstoberemoved As New List(Of UIElement)
        For i As Integer = 0 To disabled_screens.Rows.Count - 1
            For Each element As UIElement In WrapPanel.Children
                Dim sc As ScreenControl = DirectCast(element, ScreenControl)
                If sc.id = disabled_screens.Rows(i)(0) Then
                    elementstoberemoved.Add(element)
                End If
            Next
        Next
        For i As Integer = 0 To elementstoberemoved.Count - 1
            WrapPanel.Children.Remove(elementstoberemoved(i))
        Next
        Dim elementstobeadded As New List(Of ScreenControl)
        Dim newenabledscreens As New DataTable
        Dim isfound As Boolean = False
        newenabledscreens = db.query("select * from screens where enabled=True order by [id] asc")
        For i As Integer = 0 To newenabledscreens.Rows.Count - 1
            For Each element As UIElement In WrapPanel.Children
                Dim sc As ScreenControl = DirectCast(element, ScreenControl)
                If sc.id = newenabledscreens.Rows(i)(0) Then
                    isfound = True
                End If
            Next
            If Not isfound Then
                s = New ScreenControl(newenabledscreens.Rows(i)(0), newenabledscreens.Rows(i)(1), newenabledscreens.Rows(i)(2), newenabledscreens.Rows(i)(2))
                s.enabled = newenabledscreens.Rows(i)(4)
                listofscreens.Add(s)
                s.Margin = New Thickness(0, 0, 0, 20)
                s.hidepanels()
                elementstobeadded.Add(s)
                isfound = False
            End If
            isfound = False
        Next
        For i As Integer = 0 To elementstobeadded.Count - 1
            Me.WrapPanel.Children.Add(elementstobeadded(i))
        Next

    End Function
    Private Sub MenuItem_Click_1(sender As Object, e As RoutedEventArgs)

    End Sub
    Public Function hideall()
        If Me.WrapPanel.Children.Count > 0 Then
            For Each element As UIElement In WrapPanel.Children
                'process element
                If element.Uid <> "EmptySP" Then
                    Dim sc As ScreenControl = DirectCast(element, ScreenControl)
                    sc.hidepanels()
                End If
            Next
        End If

    End Function
    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed

    End Sub
    Private Sub MainWindow_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim close As New Integer
        close = MsgBox("Are you sure you want to close the application?", MessageBoxButtons.OKCancel)

        If close = 1 Then
            Dim status As String = db.execute("Update rate set SafeClose=True")
            If Me.Power_image.ToolTip = "Disconnected" Then
                MsgBox("Power Module is Disconnected , Exiting without turning off")
                End
            Else

                Dim t As DataTable = db.query("select IP from IP where [MODULE]='POWER' ")
                Dim url As String = String.Concat("http://", t.Rows(0)(0), "/test.php?GPIO=0&state=2")
                Try
                    Dim request As HttpWebRequest = WebRequest.Create(url)
                    request.Timeout = 5000
                    Dim response As HttpWebResponse = request.GetResponse()
                    Dim stream As Stream = response.GetResponseStream()
                    response.Close()

                Catch ex As WebException
                    MsgBox(ex.Message)
                    Exit Sub
                End Try

                End
            End If

        Else
            e.Cancel = True
        End If

    End Sub
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim bc As New BrushConverter
        ScreensView.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        ScreensView.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
        Me.ScreensView.Tag = "Selected"
    End Sub
    Private Sub MainWindow_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseDown

    End Sub
    Private Sub ScreensView_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles ScreensView.MouseDown
        Dim bc As New BrushConverter
        ScreensView.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        ScreensView.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
        SettingsLabel.Background = Brushes.Transparent
        SettingsLabel.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
        Me.SettingsLabel.Tag = ""
        Me.ScreensView.Tag = "Selected"
    End Sub
    Private Sub ScreensView_MouseEnter(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles ScreensView.MouseEnter
        Me.Cursor = System.Windows.Input.Cursors.Hand
        If Not ScreensView.Tag = "Selected" Then
            Dim bc As New BrushConverter
            ScreensView.Background = DirectCast(bc.ConvertFrom("#21FFFFFF"), Brush)
            ScreensView.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)

        End If


    End Sub
    Private Sub SettingsLabel_MouseEnter(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles SettingsLabel.MouseEnter
        Me.Cursor = System.Windows.Input.Cursors.Hand
        If Not SettingsLabel.Tag = "Selected" Then
            Dim bc As New BrushConverter
            SettingsLabel.Background = DirectCast(bc.ConvertFrom("#21FFFFFF"), Brush)
            SettingsLabel.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)

        End If


    End Sub
    Private Sub SettingsLabel_MouseLeave(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles SettingsLabel.MouseLeave

        Me.Cursor = System.Windows.Input.Cursors.Arrow
        If Not Me.SettingsLabel.Tag = "Selected" Then
            Dim bc As New BrushConverter
            SettingsLabel.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            SettingsLabel.Background = Brushes.Transparent

        End If
    End Sub
    Private Sub ScreensView_MouseLeave(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles ScreensView.MouseLeave
        Me.Cursor = System.Windows.Input.Cursors.Arrow
        If Not Me.ScreensView.Tag = "Selected" Then
            Dim bc As New BrushConverter
            ScreensView.Background = Brushes.Transparent
            ScreensView.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)

        End If

    End Sub
    Private Sub SettingsLabel_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles SettingsLabel.MouseDown
        Dim bc As New BrushConverter
        SettingsLabel.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        SettingsLabel.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
        ScreensView.Background = Brushes.Transparent
        ScreensView.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
        Me.ScreensView.Tag = ""
        Me.SettingsLabel.Tag = "Selected"
        Dim message, title, defaultValue As String
        Dim myValue As Object
        ' Set prompt.
        message = "Enter Admin Password"
        ' Set title.
        title = "Password Required"
        Dim pd As New settingspassword
        pd.ShowDialog()
        ' Display message, title, and default value.

    End Sub
    Private Sub MainGrid_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles MainGrid.MouseLeftButtonDown
        Dim t As New DataTable
        t = db.query("select * from screens where enabled= True")
        If t.Rows.Count = 0 Then
            Exit Sub
        End If
        If Not Me.WrapPanel.IsMouseOver Then
            hideall()
        End If

    End Sub
    Private Sub WrapPanel_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles WrapPanel.MouseDown

    End Sub
    Private Sub restore_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles restore.MouseDown
        If Power_image.ToolTip = "Disconnected" Then
            MsgBox("Power Module Disconnected")
            Exit Sub
        End If
        Me.WrapPanel.Children.Clear()
        listofscreens.Clear()
        Dim t As New DataTable
        t = db.query("Select * from SCREENS where enabled=True order by [id] asc")
        For i As Integer = 0 To t.Rows.Count - 1
            If t.Rows(i)(4) = "True" Then
                s = New ScreenControl(t.Rows(i)(0), t.Rows(i)(1), t.Rows(i)(2), t.Rows(i)(2))
                s.enabled = t.Rows(i)(4)
                listofscreens.Add(s)
                s.Margin = New Thickness(0, 0, 0, 20)
                s.hidepanels()
                For ii As Integer = 0 To 3
                    Dim t2 As DataTable = db.query("select credit from status where [id]= " & t.Rows(i)(0) & "  and player=" & ii & "")
                    If Integer.Parse(t2.Rows(0)(0)) > 0 Then
                        s.players(ii).active = True
                        s.players(ii).Pcredit = Integer.Parse(t2.Rows(0)(0))
                        s.addcreditfromrestore(Integer.Parse(t2.Rows(0)(0)), ii)
                        s.turnoffledalwayclicked()
                        s.start()
                    ElseIf Integer.Parse(t2.Rows(0)(0)) < 0 Then
                        s.CountOfAlwaysOn = s.CountOfAlwaysOn + 1
                        s.players(ii).alwaycounter = Integer.Parse(t2.Rows(0)(0)) * -1
                        s.players(ii).active = True
                        s.players(ii).Always_One = True
                        s.players(ii).startTimer()
                        s.turnoffledalwayclicked()
                        s.start()

                    Else

                        s.players(ii).Pcredit = 0
                    End If
                Next

                WrapPanel.Children.Add(s)

            End If
            s.ActiveUser()

        Next


    End Sub
    Private Sub MainWindow_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        Dim t As New DataTable
        t = db.query("select * from screens where enabled= True")
        If t.Rows.Count = 0 Then
            Exit Sub
        End If
        If Not Me.WrapPanel.IsMouseOver Then
            hideall()
        End If

    End Sub

    Private Sub Export_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Export.MouseDown

    End Sub




    Private Sub Export_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Export.MouseUp
        Dim pd As New export
        pd.Show()
    End Sub

    Private Sub Customer_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Customer.MouseUp
        Dim t As New Customers
        t.Show()
    End Sub
End Class

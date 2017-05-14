Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock
Imports System.Threading
Imports System.Windows.Threading

Public Class Customers
    Dim Scrrenlist As New List(Of ScreenControl)
    Dim dt As DispatcherTimer = New DispatcherTimer()
    Dim selected_player As Integer

    Private Sub Customers_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim screenwith As Double = Me.ActualWidth / 5
        Dim screenHeight As Double = Me.ActualHeight / 6


        Scrrenlist = DirectCast(System.Windows.Application.Current.MainWindow, MainWindow).listofscreens2
        Dispatcher.Invoke(New Action(Function()

                                         For i = 0 To Scrrenlist.Count - 1
                                             Dim screen1 As New ScreenControl(Scrrenlist(i).id, Scrrenlist(i).ScreenName, Scrrenlist(i).POWER_URL, Scrrenlist(i).LED_URL)
                                             screen1.players = Scrrenlist(i).players
                                             screen1.Margin = Scrrenlist(i).Margin
                                             screen1.clock.Text = "Ahmad"
                                             screen1.Background = Scrrenlist(i).Background
                                             screen1.hidepanels()

                                             screen1.PlayerImages.Children.Clear()
                                             screen1.clock.Foreground = Scrrenlist(i).clock.Foreground
                                             screen1.clock.FontFamily = Scrrenlist(i).clock.FontFamily
                                             screen1.clock.FontWeight = Scrrenlist(i).clock.FontWeight
                                             screen1.clock.Width = 0.78 * screenwith
                                             screen1.clock.Height = 0.29 * screenHeight
                                             screen1.clock.FontSize = screen1.clock.Height
                                             screen1.clock.Margin = New Thickness(0, screenHeight * 0.0582, 0, 0)


                                             screen1.alias.FontWeight = Scrrenlist(i).alias.FontWeight

                                             screen1.alias.Height = 0.233 * screenHeight
                                             screen1.alias.FontSize = screen1.alias.Height * 0.833333
                                             screen1.alias.Margin = New Thickness(0, screenHeight * 0.058, 0, 0)


                                             screen1.IsEnabled = False
                                             screen1.MainPanel.Width = screenwith
                                             screen1.MainPanel.Height = screenHeight
                                             screen1.Width = screenwith
                                             screen1.Height = screenHeight
                                             screen1.Margin = New Thickness(25, 0, 0, 25)

                                             Me.WrapPanel1.Children.Add(screen1)
                                         Next

                                     End Function), DispatcherPriority.Render)


        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        dt.Interval = New TimeSpan(0, 0, 0, 0, 200)
        dt.Start()
      

    End Sub

    Public Sub dispatcherTimer_Tick(ByVal sender As Object, e As System.EventArgs)
        Dim alwayson As Boolean
        Dim alwaysonplayer As Integer
        Dim y As Integer
        Dim i As Integer
        i = 0
        For Each Sc As ScreenControl In Me.WrapPanel1.Children
            Dim maximumcredit As Integer = 0
            Dim maxplayerindex As Integer = 0
            For y = 0 To 3
                If Sc.players(y).Always_One = True Then
                    alwaysonplayer = y
                    alwayson = True
                Else
                    If Sc.players(y).Pcredit > maximumcredit Then
                        maximumcredit = Sc.players(y).Pcredit
                        maxplayerindex = y
                    End If
                End If
            Next
            Sc.selected_player = maxplayerindex
            If alwayson = True Then
                Sc.selected_player = alwaysonplayer
                Sc.clock.Text = Scrrenlist(i).players(alwaysonplayer).timer
                Sc.MainPanel.Background = Scrrenlist(i).MainPanel.Background
            Else
                Sc.clock.Text = Scrrenlist(i).players(maxplayerindex).timer
                Sc.MainPanel.Background = Scrrenlist(i).MainPanel.Background
            End If
            
            i = i + 1
            y = 0
        Next

    End Sub

    Private Sub Customers_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        Dim screenwith As Double = Me.ActualWidth / 5
        Dim screenHeight As Double = Me.ActualHeight / 6
        For Each screen As ScreenControl In Me.WrapPanel1.Children
            screen.clock.Width = 0.78 * screenwith
            screen.clock.Height = 0.29 * screenHeight
            screen.clock.FontSize = screen.clock.Height
            screen.clock.Margin = New Thickness(0, screenHeight * 0.058, 0, 0)
            screen.alias.Height = 0.233 * screenHeight
            screen.alias.FontSize = screen.alias.Height * 0.833333
            screen.alias.Margin = New Thickness(0, screenHeight * 0.058, 0, 0)
            screen.IsEnabled = False
            screen.MainPanel.Width = screenwith
            screen.MainPanel.Height = screenHeight
            screen.Width = screenwith
            screen.Height = screenHeight
            screen.Margin = New Thickness(25, 0, 0, 25)
       
        Next

    End Sub
End Class

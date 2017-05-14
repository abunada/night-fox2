Imports System.Data
Imports System.Data.OleDb
Imports System.Text.RegularExpressions

Public Class tabsettings
    Dim ipsyntax As New Regex("^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}$")
    Dim nt As System.Windows.Thickness
    Dim et As System.Windows.Thickness
    Dim bb As System.Windows.Media.Brush
    Dim bc As New BrushConverter

    Private Sub Window_Loaded_1(sender As Object, e As RoutedEventArgs)
        Me.NewPassword.CaretBrush = Brushes.White
        Dim bc As New BrushConverter
        relay.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        relay.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
        relay.Tag = "Selected"

    End Sub


    Private Sub tabsettings_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        Try

            MyGrid.Children.Remove(DirectCast(LogicalTreeHelper.FindLogicalNode(MyGrid, "Line"), Line))
            MyGrid.Children.Remove(DirectCast(LogicalTreeHelper.FindLogicalNode(MyGrid, "Line2"), Line))
            seppanel1.Children.Remove(DirectCast(LogicalTreeHelper.FindLogicalNode(MyGrid, "sep1"), Polygon))
            seppanel2.Children.Remove(DirectCast(LogicalTreeHelper.FindLogicalNode(MyGrid, "sep2"), Polygon))
            seppanel3.Children.Remove(DirectCast(LogicalTreeHelper.FindLogicalNode(MyGrid, "sep3"), Polygon))

        Catch ex As Exception

        End Try
        Dim la As New Line

        la.Name = "Line"
        la.X1 = Me.label.Margin.Left + Me.label.Padding.Left
        la.Y1 = Me.label.ActualHeight
        la.X2 = MyGrid.ActualWidth - (Me.label.Margin.Left + Me.label.Padding.Left)
        la.Y2 = Me.label.ActualHeight
        la.Uid = "Line"
        la.StrokeThickness = 5
        Dim bc As New BrushConverter
        la.Stroke = DirectCast(bc.ConvertFrom("#0a3161"), Brush)
        MyGrid.Children.Add(la)


        Dim la2 As New Line
        la2.Name = "Line2"
        la2.X1 = relay.TransformToAncestor(MyGrid).Transform(New Point(0, 0)).X
        la2.Y1 = Banner.ActualHeight + LabelsPanel.ActualHeight + (relay.Padding.Bottom * 2)
        la2.X2 = usersettings.TransformToAncestor(MyGrid).Transform(New Point(0, 0)).X + usersettings.ActualWidth
        la2.Y2 = Banner.ActualHeight + LabelsPanel.ActualHeight + (relay.Padding.Bottom * 2)

        la2.Uid = "Line2"
        la2.Stroke = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        la2.StrokeThickness = 3
        MyGrid.Children.Add(la2)

        Dim t As New Polygon
        t.Name = "sep1"
        t.Uid = "sep1"
        t.Points.Add(New Point((seppanel1.ActualWidth / 2 - 2.5), 15))
        t.Points.Add(New Point((seppanel1.ActualWidth / 2), 9))
        t.Points.Add(New Point((seppanel1.ActualWidth / 2 + 2.5), 15))
        t.Stroke = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        t.Fill = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        seppanel1.Children.Add(t)

        Dim t2 As New Polygon
        t2.Name = "sep2"
        t2.Uid = "sep2"
        t2.Points.Add(New Point((seppanel2.ActualWidth / 2 - 2.5), 15))
        t2.Points.Add(New Point((seppanel2.ActualWidth / 2), 9))
        t2.Points.Add(New Point((seppanel2.ActualWidth / 2 + 2.5), 15))
        t2.Stroke = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        t2.Fill = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        seppanel2.Children.Add(t2)


        Dim t3 As New Polygon
        t3.Name = "sep3"
        t3.Uid = "sep3"
        t3.Points.Add(New Point((seppanel3.ActualWidth / 2 - 2.5), 15))
        t3.Points.Add(New Point((seppanel3.ActualWidth / 2), 9))
        t3.Points.Add(New Point((seppanel3.ActualWidth / 2 + 2.5), 15))
        t3.Stroke = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        t3.Fill = DirectCast(bc.ConvertFrom("#255b96"), Brush)
        seppanel3.Children.Add(t3)


    End Sub

    Private Sub relay_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles relay.MouseDown
        tabctrl.SelectedIndex = 0
        relay.Tag = "Clicked"
    End Sub

    Private Sub Screen_Configuration_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Screen_Configuration.MouseDown
        tabctrl.SelectedIndex = 1
        Screen_Configuration.Tag = "Clicked"
    End Sub

    Private Sub rates_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles rates.MouseDown
        tabctrl.SelectedIndex = 2
        rates.Tag = "Clicked"
    End Sub

    Private Sub tabctrl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles tabctrl.SelectionChanged
        If tabctrl.SelectedIndex = 1 Then
            Dim t As New DataTable

            t = db.query("select * from Screens where id=" & screen_id.Text & " ")
            If t.Rows.Count > 0 Then
                Screen_Alias.Text = t.Rows(0)(1)
                PIP.Text = t.Rows(0)(2)
                LIP.Text = t.Rows(0)(3)
                If t.Rows(0)(4) Then
                    Me.enabled.IsChecked = True
                ElseIf t.Rows(0)(4) = False Then
                    enabled.IsChecked = False
                End If

            End If
        ElseIf tabctrl.SelectedIndex = 0 Then
            Dim t As New DataTable
            t = db.query("select ip from ip ORDER BY [MODULE]  ASC")
            If t.Rows.Count > 0 Then
                Led_Ip.Text = t.Rows(0)(0)
                Power_Ip.Text = t.Rows(1)(0)
            End If
        ElseIf tabctrl.SelectedIndex = 2 Then
            Dim t As New DataTable
            t = db.query("select rate from rate")
            Me.Rate.Text = t.Rows(0)(0)
        End If
    End Sub

    Private Sub CheckBox_Checked_1(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        If PIP.Text <> "" And LIP.Text <> "" And Screen_Alias.Text <> "" Then
            Dim t As New DataTable
            t = db.query("select * from screens where id=" & screen_id.Text & "")
            Dim status As String
            If t.Rows.Count > 0 Then


                status = db.execute("update screens set screen_name='" & Screen_Alias.Text & "', POWER_URL='" & PIP.Text & "'  , LED_URL='" & LIP.Text & "' , Enabled=" & enabled.IsChecked & "  where id=" & screen_id.Text & "")

                If status Is "True" Then
                    MessageBox.Show("Screen " + Screen_Alias.Text + " updated ", "Updated", MessageBoxButton.OK, MessageBoxImage.Information)


                Else
                    MessageBox.Show(status, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End If

            Else
                status = db.execute("insert into SCREENS values (" & screen_id.Text & " , '" & Screen_Alias.Text & "' , '" & PIP.Text & "' , '" & LIP.Text & "', " & enabled.IsChecked & ")")
                Dim status2 As String
                For i As Integer = 0 To 3
                    status2 = db.execute("insert into STATUS values ('" & screen_id.Text & "' , 0," & i & ")")

                Next
                If status Is "True" Then
                    MessageBox.Show("Screen " + Screen_Alias.Text + " inserted ", "Inserted", MessageBoxButton.OK, MessageBoxImage.Information)


                Else
                    MessageBox.Show(status, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End If

                If Not status2 = "True" Then

                    MessageBox.Show(status, "Error", MessageBoxButton.OK, MessageBoxImage.Error)

                End If


            End If
        Else
            MessageBox.Show("Fill In the URL/Scr Fields", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning)
        End If

    End Sub

    Private Sub screen_id_DropDownClosed(sender As Object, e As EventArgs) Handles screen_id.DropDownClosed
        Dim t As New DataTable
        t = db.query("select * from Screens where id=" & screen_id.Text & " ")
        If t.Rows.Count > 0 Then
            Screen_Alias.Text = t.Rows(0)(1)
            PIP.Text = t.Rows(0)(2)
            LIP.Text = t.Rows(0)(3)
            If t.Rows(0)(4) Then
                enabled.IsChecked = True
            ElseIf t.Rows(0)(4) = False Then
                enabled.IsChecked = False
            End If
        Else
            PIP.Text = ""
            LIP.Text = ""
            enabled.IsChecked = False
            Screen_Alias.Text = ""
        End If
    End Sub

    Private Sub screen_id_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles screen_id.SelectionChanged

    End Sub

    Private Sub Save_Click(sender As Object, e As RoutedEventArgs) Handles Save.Click
        If Not ipsyntax.IsMatch(Power_Ip.Text) Or Not ipsyntax.IsMatch(Led_Ip.Text) Then
            MessageBox.Show("Make sure all the fileds have correct values", "Incomplete Information", MessageBoxButton.OK, MessageBoxImage.Warning)
            Exit Sub

        End If
        If Power_Ip.Text <> "" And Led_Ip.Text <> "" Then
            Dim t As New DataTable
            t = db.query("select count(*) from ip")
            Dim transaction As OleDbTransaction
            Dim cmd As New OleDbCommand
            db.con.Open()
            cmd.Connection = db.con
            transaction = db.con.BeginTransaction
            cmd.Transaction = transaction
            Try
                If t.Rows(0)(0) = "0" Then
                    cmd.CommandText = "insert into IP values ('POWER','" & Me.Power_Ip.Text & "')"
                    cmd.ExecuteNonQuery()
                    cmd.CommandText = "insert into IP values ('LED','" & Me.Led_Ip.Text & "')"
                    cmd.ExecuteNonQuery()
                    transaction.Commit()
                    MessageBox.Show("The configuration was submitted successully", "Submitted", MessageBoxButton.OK, MessageBoxImage.Information)
                    For Each window As Window In Application.Current.Windows
                        If window.[GetType]() = GetType(MainWindow) Then
                            TryCast(window, MainWindow).POWERIP = Me.Power_Ip.Text.Split(":")(0)
                            TryCast(window, MainWindow).POWERPORT = Me.Power_Ip.Text.Split(":")(1)
                            TryCast(window, MainWindow).LEDIP = Me.Led_Ip.Text.Split(":")(0)
                            TryCast(window, MainWindow).LEDPORT = Me.Led_Ip.Text.Split(":")(1)
                        End If
                    Next
                Else
                    cmd.CommandText = "update IP set ip='" & Me.Power_Ip.Text & "' where MODULE='POWER'"
                    cmd.ExecuteNonQuery()
                    cmd.CommandText = "update IP set ip='" & Me.Led_Ip.Text & "' where MODULE='LED'"
                    cmd.ExecuteNonQuery()
                    transaction.Commit()
                    MessageBox.Show("The configuration was updated successully", "Updated", MessageBoxButton.OK, MessageBoxImage.Information)


                    For Each window As Window In Application.Current.Windows
                        If window.[GetType]() = GetType(MainWindow) Then
                            TryCast(window, MainWindow).POWERIP = Me.Power_Ip.Text.Split(":")(0)
                            TryCast(window, MainWindow).POWERPORT = Me.Power_Ip.Text.Split(":")(1)
                            TryCast(window, MainWindow).LEDIP = Me.Led_Ip.Text.Split(":")(0)
                            TryCast(window, MainWindow).LEDPORT = Me.Led_Ip.Text.Split(":")(1)
                        End If
                    Next

                End If
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("No action was taken.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Finally
                db.con.Close()
            End Try
        Else
            MessageBox.Show("Make sure POWER IP ,LED IP and rate are filled", "Incomplete Information", MessageBoxButton.OK, MessageBoxImage.Warning)
        End If
    End Sub

    Private Sub Led_Ip_KeyDown(sender As Object, e As KeyEventArgs) Handles Led_Ip.KeyDown
        If Led_Ip.Text.Length > 0 Then
            If e.Key = Key.Tab Then
                Dim ipsyntax As New Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$")
                If Not ipsyntax.IsMatch(Led_Ip.Text) Then
                    Led_Ip.BorderBrush = Brushes.Red
                    err2.Visibility = Windows.Visibility.Visible
                    err2.ToolTip = "Invalid Ip Adress Format"



                Else
                    err2.Visibility = Windows.Visibility.Hidden
                    
                End If

            End If
        End If
    End Sub

    Private Sub Led_Ip_LostFocus(sender As Object, e As RoutedEventArgs) Handles Led_Ip.LostFocus
        If Led_Ip.Text.Length > 0 Then

            If Not ipsyntax.IsMatch(Led_Ip.Text) Then

                err2.Visibility = Windows.Visibility.Visible
                err2.ToolTip = "Rate must be Positive Number"



            Else
                err2.Visibility = Windows.Visibility.Hidden
                
            End If

        End If
    End Sub

    Private Sub Led_Ip_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Led_Ip.MouseDown

    End Sub

    Private Sub Power_Ip_KeyDown(sender As Object, e As KeyEventArgs) Handles Power_Ip.KeyDown
        If Power_Ip.Text.Length > 0 Then
            If e.Key = Key.Tab Then
                Dim ipsyntax As New Regex("^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}$")
                If Not ipsyntax.IsMatch(Power_Ip.Text) Then
                    Power_Ip.BorderBrush = Brushes.Red
                    err1.Visibility = Windows.Visibility.Visible
                    err1.ToolTip = "Invalid Ip Adress Format"



                Else
                    err1.Visibility = Windows.Visibility.Hidden
                    
                End If

            End If
        End If
    End Sub

    Private Sub Power_Ip_LostFocus(sender As Object, e As RoutedEventArgs) Handles Power_Ip.LostFocus
        If Power_Ip.Text.Length > 0 Then
            Dim ipsyntax As New Regex("^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}$")
            If Not ipsyntax.IsMatch(Power_Ip.Text) Then

                err1.Visibility = Windows.Visibility.Visible
                err1.ToolTip = "Invalid Ip Adress Format"

            Else
                err1.Visibility = Windows.Visibility.Hidden
                

            End If
        End If
    End Sub

    
    Private Sub relay_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles relay.MouseUp
        If relay.Tag = "Clicked" Then
            Dim bc As New BrushConverter
            relay.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
            relay.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
            Screen_Configuration.Background = Brushes.Transparent
            Screen_Configuration.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            rates.Background = Brushes.Transparent
            rates.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            usersettings.Background = Brushes.Transparent
            usersettings.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            relay.Tag = "Selected"

        End If
    End Sub

    Private Sub Screen_Configuration_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Screen_Configuration.MouseUp
        If Screen_Configuration.Tag = "Clicked" Then
            Dim bc As New BrushConverter
            Screen_Configuration.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
            Screen_Configuration.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
            relay.Background = Brushes.Transparent
            relay.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            rates.Background = Brushes.Transparent
            rates.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            usersettings.Background = Brushes.Transparent
            usersettings.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            Screen_Configuration.Tag = "Selected"
        End If
    End Sub

    Private Sub rates_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles rates.MouseUp
        If rates.Tag = "Clicked" Then
            Dim bc As New BrushConverter
            rates.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
            rates.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
            Screen_Configuration.Background = Brushes.Transparent
            Screen_Configuration.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            relay.Background = Brushes.Transparent
            relay.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            usersettings.Background = Brushes.Transparent
            usersettings.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            rates.Tag = "Selected"
        End If
    End Sub

    Private Sub usersettings_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles usersettings.MouseDown
        tabctrl.SelectedIndex = 3
        usersettings.Tag = "Clicked"
    End Sub

    Private Sub usersettings_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles usersettings.MouseUp
        If usersettings.Tag = "Clicked" Then
            Dim bc As New BrushConverter
            usersettings.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
            usersettings.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
            Screen_Configuration.Background = Brushes.Transparent
            Screen_Configuration.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            relay.Background = Brushes.Transparent
            relay.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            rates.Background = Brushes.Transparent
            rates.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            usersettings.Tag = "Selected"
        End If
    End Sub

    Private Sub relay_MouseEnter(sender As Object, e As MouseEventArgs) Handles relay.MouseEnter
        Me.Cursor = Cursors.Hand
        If tabctrl.SelectedIndex <> 0 Then
            relay.Foreground = DirectCast(bc.ConvertFrom("#F2F8FF"), Brush)
            relay.Background = DirectCast(bc.ConvertFrom("#21FFFFFF"), Brush)
        End If
    End Sub

    Private Sub relay_MouseLeave(sender As Object, e As MouseEventArgs) Handles relay.MouseLeave
        Me.Cursor = Cursors.Arrow
        If tabctrl.SelectedIndex <> 0 Then
            relay.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            relay.Background = Brushes.Transparent
        End If
    End Sub

    Private Sub Screen_Configuration_MouseEnter(sender As Object, e As MouseEventArgs) Handles Screen_Configuration.MouseEnter
        Me.Cursor = Cursors.Hand
        If tabctrl.SelectedIndex <> 1 Then
            Screen_Configuration.Foreground = DirectCast(bc.ConvertFrom("#F2F8FF"), Brush)
            Screen_Configuration.Background = DirectCast(bc.ConvertFrom("#21FFFFFF"), Brush)
        End If
    End Sub

    Private Sub Screen_Configuration_MouseLeave(sender As Object, e As MouseEventArgs) Handles Screen_Configuration.MouseLeave
        Me.Cursor = Cursors.Arrow
        If tabctrl.SelectedIndex <> 1 Then
            Screen_Configuration.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            Screen_Configuration.Background = Brushes.Transparent
        End If
    End Sub

    Private Sub rates_MouseEnter(sender As Object, e As MouseEventArgs) Handles rates.MouseEnter
        Me.Cursor = Cursors.Hand
        If tabctrl.SelectedIndex <> 2 Then
            rates.Foreground = DirectCast(bc.ConvertFrom("#F2F8FF"), Brush)
            rates.Background = DirectCast(bc.ConvertFrom("#21FFFFFF"), Brush)
        End If
    End Sub

    Private Sub rates_MouseLeave(sender As Object, e As MouseEventArgs) Handles rates.MouseLeave
        Me.Cursor = Cursors.Arrow
        If tabctrl.SelectedIndex <> 2 Then
            rates.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            rates.Background = Brushes.Transparent
        End If


    End Sub
    Private Sub usersettings_MouseEnter(sender As Object, e As MouseEventArgs) Handles usersettings.MouseEnter
        Me.Cursor = Cursors.Hand
        If tabctrl.SelectedIndex <> 3 Then
            usersettings.Foreground = DirectCast(bc.ConvertFrom("#F2F8FF"), Brush)
            usersettings.Background = DirectCast(bc.ConvertFrom("#21FFFFFF"), Brush)
        End If
    End Sub

    Private Sub usersettings_MouseLeave(sender As Object, e As MouseEventArgs) Handles usersettings.MouseLeave
        Me.Cursor = Cursors.Arrow
        If tabctrl.SelectedIndex <> 3 Then
            usersettings.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
            usersettings.Background = Brushes.Transparent
        End If


    End Sub

    Private Sub SaveRate_Click(sender As Object, e As RoutedEventArgs) Handles SaveRate.Click
        If Not IsNumeric(Rate.Text) Then
            MessageBox.Show("Rate Value is not in correct format", "Incomplete Information", MessageBoxButton.OK, MessageBoxImage.Warning)
            rate_err1.Visibility = Windows.Visibility.Visible
            Exit Sub
        End If
        Dim cmd As New OleDbCommand
        db.con.Open()
        cmd.Connection = db.con
        cmd.CommandText = "update rate set rate=" & Me.Rate.Text & ""
        cmd.ExecuteNonQuery()
        rate_err1.Visibility = Windows.Visibility.Hidden
        db.con.Close()
        MessageBox.Show("The configuration was updated successully", "Updated", MessageBoxButton.OK, MessageBoxImage.Information)

    End Sub

    Private Sub UpdatePassowrd_Click(sender As Object, e As RoutedEventArgs) Handles UpdatePassowrd.Click
        If OldPassowrd.Password <> "" And NewPassword.Password <> "" And ConfirmPassword.Password <> "" Then
            If NewPassword.Password <> ConfirmPassword.Password Then
                MessageBox.Show("Password does not match the confirm password ", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Else
                Dim t As New DataTable
                t = db.query("select PASSWORD from USERS")
                If OldPassowrd.Password <> t.Rows(0)(0) Then
                    MessageBox.Show("The old password is incorrect", "Unable to Change Password", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                ElseIf db.execute("Update [USERS] set [password]='" & Me.NewPassword.Password & "'") = "True" Then
                    MessageBox.Show("Password Updated Succefully", "Password Updated", MessageBoxButton.OK, MessageBoxImage.None)
                Else
                    MessageBox.Show("Something went wrong ", "Sorry for that", MessageBoxButton.OK, MessageBoxImage.Error)

                End If
            End If
        Else
            MessageBox.Show("Make sure to fill all the fields", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        End If
    End Sub

    Private Sub tabsettings_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
        Dim bc As New BrushConverter
        For Each window As Window In Application.Current.Windows
            If window.[GetType]() = GetType(MainWindow) Then
                TryCast(window, MainWindow).ScreensView.Background = DirectCast(bc.ConvertFrom("#255b96"), Brush)
                TryCast(window, MainWindow).ScreensView.Foreground = DirectCast(bc.ConvertFrom("#f2f8ff"), Brush)
                TryCast(window, MainWindow).ScreensView.Tag = "Selected"
                TryCast(window, MainWindow).SettingsLabel.Background = Brushes.Transparent
                TryCast(window, MainWindow).SettingsLabel.Foreground = DirectCast(bc.ConvertFrom("#FF0A3161"), Brush)
                TryCast(window, MainWindow).SettingsLabel.Tag = ""
            End If
        Next
        For Each window As Window In Application.Current.Windows
            If window.[GetType]() = GetType(MainWindow) Then
                TryCast(window, MainWindow).Init()

            End If
        Next
    End Sub
End Class


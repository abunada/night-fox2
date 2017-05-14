Imports System.Data

Public Class settingspassword

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Dim t As New DataTable
        t = db.query("select password from users")
        If passwordtext.Password.ToString = t.Rows(0)(0).ToString Then


            Dim tabsetting As New tabsettings

            tabsetting.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            Me.Close()
            tabsetting.ShowDialog()

        Else
            MsgBox("Wrong Password")


        End If


    End Sub

    Private Sub settingspassword_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
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

    End Sub
End Class

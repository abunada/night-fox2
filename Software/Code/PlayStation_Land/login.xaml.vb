Imports System.Data
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock.Time


Public Class login
    Dim attempt As Boolean
    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)

        Dim d As New DataTable
        d = db.query("select * from users where  USER_NAME= '" & Me.UserName.Text & "' and password= '" & Me.Password.Password & "'  ")
        If d.Rows.Count > 0 Then
            Me.Hide()
            Dim main As New MainWindow
            main.ShowDialog()
        Else
            Me.WrongLabel.Visibility = Windows.Visibility.Visible

        End If
    End Sub
End Class

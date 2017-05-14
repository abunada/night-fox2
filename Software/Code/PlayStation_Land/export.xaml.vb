Imports System.IO
Imports System.Windows.Forms
Imports System.Data

Public Class export

    Private Sub export_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim curApp As Application = Application.Current
        Dim mainWindow As Window = curApp.MainWindow
        Me.Left = mainWindow.Left + (mainWindow.ActualWidth - Me.ActualWidth) / 2
        Me.Top = mainWindow.Top + (mainWindow.ActualHeight - Me.ActualHeight) / 2
    End Sub

    Private Sub Export_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles MyBase.MouseUp, MyBase.MouseUp

    End Sub

    Private Sub ExportButton_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles ExportButton.MouseUp
        If FromDate.SelectedDate.HasValue = False Then
            MsgBox("Select From Date", MsgBoxStyle.Information, "Missing information")
        ElseIf ToDate.SelectedDate.HasValue = False Then
            MsgBox("Select To Date", MsgBoxStyle.Information, "Missing information")

        Else
            Dim t As New DataTable
            t = db.query("select * from logging where timestamp between #" & FormatDateTime(FromDate.SelectedDate, DateFormat.ShortDate) & "# and #" & FormatDateTime(ToDate.SelectedDate, DateFormat.ShortDate) & "# ")
            If t.Rows.Count = 0 Then
                MsgBox("No Logs for this date", MsgBoxStyle.Information)
            Else
                Dim saveFileDialog1 As New SaveFileDialog()
                saveFileDialog1.Title = "Save Screens Log File As"
                saveFileDialog1.Filter = "Comma Seperated Value|*.csv"

                saveFileDialog1.ShowDialog()
                If saveFileDialog1.FileName <> "" Then
                    Using writer As StreamWriter = New StreamWriter(Path.GetTempPath + "\datalog.csv")
                        csvwriter.WriteDataTable(t, writer, True)
                    End Using
                    Try
                        System.IO.File.Copy(Path.GetTempPath + "datalog.csv", saveFileDialog1.FileName, True)
                        MsgBox("Data Exported Succesfully")
                    Catch ex As IOException
                        MsgBox(ex.ToString)
                    End Try

                    System.IO.File.Delete(Path.GetTempPath + "datalog.csv")
                End If

            End If
            Me.Close()

        End If

    End Sub
End Class

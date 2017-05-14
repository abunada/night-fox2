Imports System.Data.OleDb
Imports System.Data

Module db
    'Public con As New OleDbConnection("Provider=microsoft.Jet.oledb.4.0;Data Source=D:\Users\ahmadq\Documents\Visual Studio 2012\Projects\PlayStation_Land\PlayStation_Land\Data\PlayStation_Land.mdb;")
    Public con As New OleDbConnection("Provider=microsoft.Jet.oledb.4.0;Data Source=" + AppDomain.CurrentDomain.BaseDirectory.ToString + "\Data\PlayStation_Land.mdb;")
    Public Function query(querystring As String)
        Dim ds As New DataSet()
        Dim cmd As New OleDbCommand(querystring, con)
        Dim da As New OleDbDataAdapter(cmd)
        da.Fill(ds, "table1")
        Dim t1 As DataTable = ds.Tables("table1")
        con.Close()
        Return t1




    End Function
    Public Function execute(command As String) As String
        Try
            Dim cmd As New OleDbCommand
            If con.State = ConnectionState.Open Then
                
            End If
            con.Open()
            cmd.Connection = con
            cmd.CommandText = command
            cmd.CommandType = CommandType.Text
            cmd.ExecuteNonQuery()
            Return "True"
        Catch ex As Exception
            Return ex.Message
        Finally
            con.Close()
        End Try

    End Function

End Module

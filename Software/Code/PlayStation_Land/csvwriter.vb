Public Class csvwriter

    Public Shared Sub WriteDataTable(ByVal sourceTable As System.Data.DataTable, _
        ByVal writer As IO.TextWriter, ByVal includeHeaders As Boolean)
        If (includeHeaders) Then
            Dim headerValues As IEnumerable(Of String) = sourceTable.Columns.OfType(Of System.Data.DataColumn)().Select(Function(column) QuoteValue(column.ColumnName))

            writer.WriteLine(String.Join(",", headerValues))
        End If

        Dim items As IEnumerable(Of String) = Nothing
        For Each row As System.Data.DataRow In sourceTable.Rows
            items = row.ItemArray.Select(Function(obj) QuoteValue(obj.ToString()))
            writer.WriteLine(String.Join(",", items))
        Next

        writer.Flush()
    End Sub

    Private Shared Function QuoteValue(ByVal value As String) As String
        Return String.Concat("""", value.Replace("""", """"""), """")
    End Function
End Class


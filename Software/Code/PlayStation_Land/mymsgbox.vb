Public Class mymsgbox

    Private Sub mymsgbox_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Public Sub New(ByVal Title As String, ByVal msg1 As String, ByVal msg2 As String)
        InitializeComponent() ' This call is required by the Windows Form Designer.
        Me.Text = Title
        Me.Label1.Text = msg1
        Me.Label2.Text = msg2
        Me.Show()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class
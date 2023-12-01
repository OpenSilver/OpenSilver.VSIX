Imports System.Windows
Imports System.Windows.Controls

Partial Public Class ErrorWindow
    Inherits ChildWindow

    Public Overloads Shared Sub Show(ex As Exception)
        Dim errorWindow = New ErrorWindow(ex)
        errorWindow.Show()
    End Sub

    Public Overloads Shared Sub Show(uri As Uri)
        Dim errorWindow = New ErrorWindow(uri)
        errorWindow.Show()
    End Sub

    Public Overloads Shared Sub Show(message As String, Optional details As String = "")
        Dim errorWindow = New ErrorWindow(message, details)
        errorWindow.Show()
    End Sub

    Private Sub New(e As Exception)
        InitializeComponent()
        If e IsNot Nothing Then
            ErrorTextBox.Text = e.Message + vbCrLf + vbCrLf + e.StackTrace
        End If
    End Sub

    Private Sub New(uri As Uri)
        InitializeComponent()
        If uri IsNot Nothing Then
            ErrorTextBox.Text = "Page not found: '" + uri.ToString() + "'"
        End If
    End Sub

    Private Sub New(message As String, details As String)
        InitializeComponent()
        ErrorTextBox.Text = message + vbCrLf + vbCrLf + details
    End Sub

    Private Sub OKButton_Click(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = True
    End Sub

End Class
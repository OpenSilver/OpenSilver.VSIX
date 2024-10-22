Imports System.Windows
Imports System.Windows.Controls

Partial Public Class ErrorWindow
    Inherits ChildWindow

    Public Overloads Shared Sub Show(ex As Exception)
        Dim errorWindow = New ErrorWindow(ex)
        errorWindow.Show()
    End Sub

    Public Overloads Shared Sub Show(uri As Uri, Optional ex As Exception = Nothing)
        Dim errorWindow = New ErrorWindow(uri, ex)
        errorWindow.Show()
    End Sub

    Public Overloads Shared Sub Show(message As String, Optional details As String = "")
        Dim errorWindow = New ErrorWindow(message, details)
        errorWindow.Show()
    End Sub

    Private Sub New(ex As Exception)
        InitializeComponent()
        If ex IsNot Nothing Then
            ErrorTextBox.Text = ex.ToString()
        End If
    End Sub

    Private Sub New(uri As Uri, Optional ex As Exception = Nothing)
        InitializeComponent()
        If uri IsNot Nothing Then
            ErrorTextBox.Text = $"Problem loading page '{uri}'{vbCrLf}{ex}"
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
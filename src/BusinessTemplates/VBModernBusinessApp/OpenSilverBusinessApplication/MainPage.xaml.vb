Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Navigation

Partial Public Class MainPage
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ContentFrame_Navigated(sender As Object, e As NavigationEventArgs)

        For Each child As UIElement In LinksStackPanel.Children

            Dim hb As HyperlinkButton = TryCast(child, HyperlinkButton)
            If hb IsNot Nothing AndAlso hb.NavigateUri IsNot Nothing Then

                If hb.NavigateUri.ToString().Equals(e.Uri.ToString()) Then
                    VisualStateManager.GoToState(hb, "ActiveLink", True)
                Else
                    VisualStateManager.GoToState(hb, "InactiveLink", True)
                End If

            End If

        Next

    End Sub

    Private Sub ContentFrame_NavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        e.Handled = True
        ErrorWindow.Show(e.Uri, e.Exception)
    End Sub

End Class
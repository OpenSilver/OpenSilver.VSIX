Imports System.ComponentModel

Namespace Models

    Partial Public Class User

        Protected Overrides Sub OnPropertyChanged(propertyName As String)

            MyBase.OnPropertyChanged(propertyName)

            If propertyName = "Name" Or propertyName = "FriendlyName" Then
                Me.RaisePropertyChanged("DisplayName")
            End If

        End Sub

    End Class

End Namespace
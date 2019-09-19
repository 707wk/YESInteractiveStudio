''' <summary>
''' 4行*2列传感器布局的长方形箱体
''' </summary>
Public Class YesTechMG15
    Inherits YesTechBaseBox

    Public Sub New(value As NovaStarScanBoard)
        MyBase.New(value)
    End Sub

    Friend Overrides Sub BoxRotateAngle360()
        Throw New NotImplementedException()
    End Sub

    Friend Overrides Sub BoxRotateAngle90()
        Throw New NotImplementedException()
    End Sub

    Friend Overrides Sub BoxRotateAngle180()
        Throw New NotImplementedException()
    End Sub

    Friend Overrides Sub BoxRotateAngle270()
        Throw New NotImplementedException()
    End Sub

End Class

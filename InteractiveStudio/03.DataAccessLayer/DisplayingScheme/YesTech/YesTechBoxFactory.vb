''' <summary>
''' 箱体工厂类
''' </summary>
Public NotInheritable Class YesTechBoxFactory
    Private Sub New()
    End Sub

    ''' <summary>
    ''' 根据箱体尺寸创建不同类型箱体实例
    ''' </summary>
    ''' <param name="scanBoard"></param>
    ''' <returns></returns>
    Public Shared Function Create(scanBoard As NovaStarScanBoard) As YesTechBaseBox
        With scanBoard.SizeOfOriginal

            If YesTechMG7.IsEquals(.Width, .Height) Then
                Return New YesTechMG7(scanBoard)

            ElseIf YesTechMG11.IsEquals(.Width, .Height) Then
                Return New YesTechMG11(scanBoard)

            ElseIf YesTechMG15.IsEquals(.Width, .Height) Then
                Return New YesTechMG15(scanBoard)

            Else
                Throw New Exception($"未知箱体:{ .ToString}")
            End If

        End With

    End Function
End Class

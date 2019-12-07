''' <summary>
''' 4行*2列传感器布局的长方形箱体
''' </summary>
Public Class YesTechMG15
    Inherits YesTechBaseBox

    Public Sub New(value As NovaStarScanBoard)
        MyBase.New(value)
    End Sub

    Public Shared Function IsEquals(width As Integer, height As Integer) As Boolean
        If width = height \ 2 OrElse
            width \ 2 = height Then
            Return True
        End If

        Return False
    End Function

    Friend Overrides Sub BoxRotateAngle360()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height \ 2,
            ScanBoard.SizeOfZoom.Width \ 2)
        Dim sensorKeyArray() As Integer = {
            0, 1,
            4, 5,
            8, 9,
            12, 13
        }

        For rowID = 0 To 4 - 1
            For columnID = 0 To 2 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 2 + columnID)
                    .Size = sensorWidth
                    .Location = New Point(ScanBoard.LocationInDisplayingWindow.X + columnID * sensorWidth,
                                          ScanBoard.LocationInDisplayingWindow.Y + rowID * sensorWidth)
                    .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                                  .Location.Y + sensorWidth \ 2)
                End With

                SensorList.Add(addSensor)
            Next
        Next
    End Sub

    Friend Overrides Sub BoxRotateAngle90()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height \ 2,
            ScanBoard.SizeOfZoom.Width \ 2)
        Dim sensorKeyArray() As Integer = {
            12, 8, 4, 0,
            13, 9, 5, 1
        }

        For rowID = 0 To 2 - 1
            For columnID = 0 To 4 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 4 + columnID)
                    .Size = sensorWidth
                    .Location = New Point(ScanBoard.LocationInDisplayingWindow.X + columnID * sensorWidth,
                                          ScanBoard.LocationInDisplayingWindow.Y + rowID * sensorWidth)
                    .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                                  .Location.Y + sensorWidth \ 2)
                End With

                SensorList.Add(addSensor)
            Next
        Next
    End Sub

    Friend Overrides Sub BoxRotateAngle180()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height \ 2,
            ScanBoard.SizeOfZoom.Width \ 2)
        Dim sensorKeyArray() As Integer = {
            13, 12,
            9, 8,
            5, 4,
            1, 0
        }

        For rowID = 0 To 4 - 1
            For columnID = 0 To 2 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 2 + columnID)
                    .Size = sensorWidth
                    .Location = New Point(ScanBoard.LocationInDisplayingWindow.X + columnID * sensorWidth,
                                          ScanBoard.LocationInDisplayingWindow.Y + rowID * sensorWidth)
                    .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                                  .Location.Y + sensorWidth \ 2)
                End With

                SensorList.Add(addSensor)
            Next
        Next
    End Sub

    Friend Overrides Sub BoxRotateAngle270()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height \ 2,
            ScanBoard.SizeOfZoom.Width \ 2)
        Dim sensorKeyArray() As Integer = {
            1, 5, 9, 13,
            0, 4, 8, 12
        }

        For rowID = 0 To 2 - 1
            For columnID = 0 To 4 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 4 + columnID)
                    .Size = sensorWidth
                    .Location = New Point(ScanBoard.LocationInDisplayingWindow.X + columnID * sensorWidth,
                                          ScanBoard.LocationInDisplayingWindow.Y + rowID * sensorWidth)
                    .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                                  .Location.Y + sensorWidth \ 2)
                End With

                SensorList.Add(addSensor)
            Next
        Next
    End Sub

End Class

''' <summary>
''' 4行*4列传感器布局的正方形箱体,兼容 扇形/三角形箱体
''' </summary>
Public Class YesTechMG7
    Inherits YesTechBaseBox

    Public Sub New(value As NovaStarScanBoard)
        MyBase.New(value)
    End Sub

    Public Shared Function IsEquals(width As Integer, height As Integer) As Boolean
        If width = height Then
            Return True
        End If

        Return False
    End Function

    Friend Overrides Sub BoxRotateAngle360()
        Dim sensorWidth As Integer = ScanBoard.SizeOfZoom.Width \ 4
        Dim sensorKeyArray() As Integer = {
            0, 1, 2, 3,
            4, 5, 6, 7,
            8, 9, 10, 11,
            12, 13, 14, 15
        }

        For rowID = 0 To 4 - 1
            For columnID = 0 To 4 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 4 + columnID)
                    .DisplayingWindowID = ScanBoard.DisplayingWindowID
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
        Dim sensorWidth As Integer = ScanBoard.SizeOfZoom.Width \ 4
        Dim sensorKeyArray() As Integer = {
            12, 8, 4, 0,
            13, 9, 5, 1,
            14, 10, 6, 2,
            15, 11, 7, 3
        }

        For rowID = 0 To 4 - 1
            For columnID = 0 To 4 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 4 + columnID)
                    .DisplayingWindowID = ScanBoard.DisplayingWindowID
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
        Dim sensorWidth As Integer = ScanBoard.SizeOfZoom.Width \ 4
        Dim sensorKeyArray() As Integer = {
            15, 14, 13, 12,
            11, 10, 9, 8,
            7, 6, 5, 4,
            3, 2, 1, 0
        }

        For rowID = 0 To 4 - 1
            For columnID = 0 To 4 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 4 + columnID)
                    .DisplayingWindowID = ScanBoard.DisplayingWindowID
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
        Dim sensorWidth As Integer = ScanBoard.SizeOfZoom.Width \ 4
        Dim sensorKeyArray() As Integer = {
            3, 7, 11, 15,
            2, 6, 10, 14,
            1, 5, 9, 13,
            0, 4, 8, 12
        }

        For rowID = 0 To 4 - 1
            For columnID = 0 To 4 - 1

                Dim addSensor As New Sensor
                With addSensor
                    .Key = sensorKeyArray(rowID * 4 + columnID)
                    .DisplayingWindowID = ScanBoard.DisplayingWindowID
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

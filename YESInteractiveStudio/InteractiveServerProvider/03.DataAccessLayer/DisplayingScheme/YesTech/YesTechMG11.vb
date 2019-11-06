''' <summary>
''' 4行*1列传感器布局的长方形箱体
''' </summary>
Public Class YesTechMG11
    Inherits YesTechBaseBox

    Public Sub New(value As NovaStarScanBoard)
        MyBase.New(value)
    End Sub

    Public Shared Function IsEquals(width As Integer, height As Integer) As Boolean
        If width = height \ 4 OrElse
            width \ 4 = height Then
            Return True
        End If

        Return False
    End Function

    Friend Overrides Sub BoxRotateAngle360()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Width)
        Dim sensorKeyArray() As Integer = {
            0,
            4,
            8,
            12
        }

        For rowID = 0 To 4 - 1

            Dim addSensor As New Sensor
            With addSensor
                .Key = ScanBoard.PortID * 100000 + ScanBoard.ScannerID * 100 + sensorKeyArray(rowID)
                .Size = sensorWidth
                .Location = New Point(ScanBoard.LocationInDisplayingWindow.X,
                                      ScanBoard.LocationInDisplayingWindow.Y + rowID * sensorWidth)
                .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                              .Location.Y + sensorWidth \ 2)
            End With

            SensorItems.Add(addSensor)
        Next
    End Sub

    Friend Overrides Sub BoxRotateAngle90()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Width)
        Dim sensorKeyArray() As Integer = {
            12, 8, 4, 0
        }

        For columnID = 0 To 4 - 1

            Dim addSensor As New Sensor
            With addSensor
                .Key = ScanBoard.PortID * 100000 + ScanBoard.ScannerID * 100 + sensorKeyArray(columnID)
                .Size = sensorWidth
                .Location = New Point(ScanBoard.LocationInDisplayingWindow.X + columnID * sensorWidth,
                                      ScanBoard.LocationInDisplayingWindow.Y)
                .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                              .Location.Y + sensorWidth \ 2)
            End With

            SensorItems.Add(addSensor)
        Next
    End Sub

    Friend Overrides Sub BoxRotateAngle180()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Width)
        Dim sensorKeyArray() As Integer = {
            12,
            8,
            4,
            0
        }

        For rowID = 0 To 4 - 1

            Dim addSensor As New Sensor
            With addSensor
                .Key = ScanBoard.PortID * 100000 + ScanBoard.ScannerID * 100 + sensorKeyArray(rowID)
                .Size = sensorWidth
                .Location = New Point(ScanBoard.LocationInDisplayingWindow.X,
                                      ScanBoard.LocationInDisplayingWindow.Y + rowID * sensorWidth)
                .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                              .Location.Y + sensorWidth \ 2)
            End With

            SensorItems.Add(addSensor)
        Next
    End Sub

    Friend Overrides Sub BoxRotateAngle270()
        Dim sensorWidth As Integer =
            If(ScanBoard.SizeOfZoom.Width > ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Height,
            ScanBoard.SizeOfZoom.Width)
        Dim sensorKeyArray() As Integer = {
            0, 4, 8, 12
        }

        For columnID = 0 To 4 - 1

            Dim addSensor As New Sensor
            With addSensor
                .Key = ScanBoard.PortID * 100000 + ScanBoard.ScannerID * 100 + sensorKeyArray(columnID)
                .Size = sensorWidth
                .Location = New Point(ScanBoard.LocationInDisplayingWindow.X + columnID * sensorWidth,
                                      ScanBoard.LocationInDisplayingWindow.Y)
                .LocationOfCenter = New Point(.Location.X + sensorWidth \ 2,
                                              .Location.Y + sensorWidth \ 2)
            End With

            SensorItems.Add(addSensor)
        Next
    End Sub

End Class

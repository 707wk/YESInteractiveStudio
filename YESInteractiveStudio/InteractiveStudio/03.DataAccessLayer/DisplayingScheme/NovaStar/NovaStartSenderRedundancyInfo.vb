''' <summary>
''' 发送卡冗余信息
''' </summary>
Public Class NovaStartSenderRedundancyInfo

    Public Sub New()

    End Sub

    Public Sub New(masterSenderIDValue As Integer,
                   masterPortIDValue As Integer,
                   slaveSenderIDValue As Integer,
                   slavePortIDValue As Integer)

        MasterSenderID = masterSenderIDValue
        MasterPortID = masterPortIDValue
        SlaveSenderID = slaveSenderIDValue
        SlavePortID = slavePortIDValue

    End Sub

    ''' <summary>
    ''' 主发送卡
    ''' </summary>
    Public MasterSenderID As Integer
    ''' <summary>
    ''' 主端口
    ''' </summary>
    Public MasterPortID As Integer
    ''' <summary>
    ''' 冗余发送卡
    ''' </summary>
    Public SlaveSenderID As Integer
    ''' <summary>
    ''' 冗余端口
    ''' </summary>
    Public SlavePortID As Integer

End Class

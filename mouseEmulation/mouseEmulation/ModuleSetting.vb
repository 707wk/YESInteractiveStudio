Imports System.IO
Imports System.Net.Sockets
Imports Nova.Mars.SDK

Module ModuleSetting
    ''' <summary>
    ''' 接收卡信息
    ''' </summary>
    Public Structure ScanBoardInfo
        ''' <summary>
        ''' 屏幕索引
        ''' </summary>
        Dim ScreenIndex As Integer
        ''' <summary>
        ''' 在屏幕数组的下标
        ''' </summary>
        Dim linkIndex As Integer
        '控制器索引
        Dim SenderIndex As Integer
        ''' <summary>
        ''' 网口索引
        ''' </summary>
        Dim PortIndex As Integer
        ''' <summary>
        ''' 连接序号
        ''' </summary>
        Dim ConnectIndex As Integer

        ''' <summary>
        ''' X轴索引
        ''' </summary>
        Dim X As Integer
        ''' <summary>
        ''' Y轴索引
        ''' </summary>
        Dim Y As Integer
    End Structure
    Public InteractMode As Integer
    ''' <summary>
    ''' 1： 普通模式 2：地砖踩踏模式 3：地砖四合一模式
    ''' </summary>
    Public TempI As Integer
    '''' <summary>
    '''' 统计单张接收 特定模式 单触发块 开关量和
    '''' </summary>
    'Public TempI2 As Integer
    ''' <summary>
    ''' 统计单张接收  开关量和
    ''' </summary>
    Public TempS As Integer
    ''' <summary>
    ''' 复位类型 0软复位 1硬复位
    ''' </summary>
    Public resetType As Integer
    ''' <summary>
    ''' 屏幕灵敏度设置
    ''' </summary>
    Public ScreenSensitivity As Integer
    ''' <summary>
    ''' 踩踏模式下 历史点击记录 只适合点击事件
    ''' </summary>
    Public ScreenM2clickH As Integer
    ''' <summary>
    ''' 踩踏模式下 点击记录 只适合点击事件
    ''' </summary>
    Public ScreenM2click As Integer

    'Public ReceiveData(,) As Integer
    'Public ReceiveDataH(,) As Integer
    ''' <summary>
    ''' 屏幕特定模式 单触发块 抗干扰参数   （统计接收卡 单触发块 开关量和）
    ''' </summary>
    Public ScreenAnti As Integer
    ''' <summary>
    ''' 屏幕总抗干扰参数 （统计接收卡开关量和）
    ''' </summary>
    Public ScreenAntiS As Integer
    ''' <summary>
    ''' 屏幕定时复位增量温度 K
    ''' </summary>
    Public ResetTemp As Integer
    ''' <summary>
    ''' 屏幕定时复位时间  min
    ''' </summary>
    Public ResetTimeSec As Integer

    ''' <summary>
    ''' 接收卡信息索引表
    ''' </summary>
    Public ScanBoardTable As Hashtable

    ''' <summary>
    ''' 显示屏信息
    ''' </summary>
    Public Structure screenInfo
        '显示屏索引
        'Dim index As Integer
        ''' <summary>
        ''' 备注
        ''' </summary>
        Dim remark As String
        ''' <summary>
        ''' 是否显示
        ''' </summary>
        Dim showFlage As Boolean
        ''' <summary>
        ''' 播放文件路径
        ''' </summary>
        Dim filePath As String

        ''' <summary>
        ''' 显示屏 X 偏移(单位像素)
        ''' </summary>
        Dim x As Integer
        ''' <summary>
        ''' 显示屏 Y 偏移(单位像素)
        ''' </summary>
        Dim y As Integer

        ''' <summary>
        ''' 显示屏宽度(单位像素)
        ''' </summary>
        Dim width As Integer
        ''' <summary>
        ''' 显示屏高度(单位像素)
        ''' </summary>
        Dim height As Integer

        ''' <summary>
        ''' 屏幕单元宽度(单位像素)
        ''' </summary>
        Dim ScanBoardWidth As Integer
        ''' <summary>
        ''' 屏幕单元高度(单位像素)
        ''' </summary>
        Dim ScanBoardHeight As Integer
        ''' <summary>
        ''' 上次点击状态
        ''' </summary>
        Dim clickHistoryArray(,) As Integer

        ''' <summary>
        ''' 屏幕所在的发送卡序列
        ''' </summary>
        Dim SenderList As List(Of Integer)

        ''' <summary>
        ''' 播放窗体
        ''' </summary>
        Dim playDialog As FormPlay
    End Structure
    ''' <summary>
    ''' 显示屏数组
    ''' </summary>
    Public screenMain As screenInfo()

    ''' <summary>
    ''' 发送卡(控制器)信息
    ''' </summary>
    Public Structure senderInfo
        '控制器索引
        'Dim index As Integer
        ''' <summary>
        ''' 是否需要连接
        ''' </summary>
        Dim link As Boolean
        ''' <summary>
        ''' IP信息
        ''' </summary>
        Dim ipDate As Byte()
        ''' <summary>
        ''' 临时保存修改后的IP信息
        ''' </summary>
        Dim tmpIpData As Byte()
        ''' <summary>
        ''' 连接变量
        ''' </summary>
        Dim cliSocket As Socket
    End Structure
    ''' <summary>
    ''' 发送卡数组
    ''' </summary>
    Public senderArray As senderInfo()

    ''' <summary>
    ''' 屏幕播放信息
    ''' </summary>
    <Serializable()>
    Public Structure screenPlayInfo
        '屏幕索引
        'Dim screenIndex As Integer
        ''' <summary>
        ''' 备注
        ''' </summary>
        Dim remark As String
        ''' <summary>
        ''' 是否显示
        ''' </summary>
        Dim showFlage As Boolean
        '播放文件路径
        'Dim filePath As String
    End Structure

    ''' <summary>
    ''' 播放相关信息
    ''' </summary>
    <Serializable()>
    Public Structure playInfoSave
        ''' <summary>
        ''' 屏幕播放信息列表
        ''' </summary>
        Dim playList As screenPlayInfo()

        ''' <summary>
        ''' 播放列表
        ''' </summary>
        Dim filesList As Hashtable
    End Structure
    ''' <summary>
    ''' 系统设置变量
    ''' </summary>
    Public systeminfo As playInfoSave

    '播放列表[废弃]
    'Public playFilesList As Hashtable

    ''' <summary>
    ''' 查询时间间隔
    ''' </summary>
    Public checkTime As Integer
    ''' <summary>
    ''' 运行模式
    ''' 0点击
    ''' 1测试
    ''' 2黑屏
    ''' 3忽略
    ''' 4测试(显示电容)
    ''' </summary>
    Public runMode As Integer

    ''' <summary>
    ''' 记录数据标记
    ''' </summary>
    Public recordDataFlage As Boolean
    ''' <summary>
    ''' 记录数据文件变量
    ''' </summary>
    Public recordDataFile As StreamWriter

    ''' <summary>
    ''' 语言类型 0中文 1English
    ''' </summary>
    Public selectLanguageId As Integer

    ''' <summary>
    ''' Nova服务变量
    ''' </summary>
    Public rootClass As MarsHardwareEnumerator
    Public mainClass As MarsControlSystem

    ''' <summary>
    ''' 输出日志
    ''' </summary>
    ''' <param name="str"></param>
    Public Sub putlog(str As String)
        Dim tmp As StreamWriter = New StreamWriter("log.txt", True)
        tmp.WriteLine(Format(Now(), "[yyyy-MM-dd HH:mm:ss] ") & str)
        tmp.Close()
    End Sub

    ''' <summary>
    ''' 设置温度复位幅度
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Function SetresetTemp(value As Integer) As Boolean
        Dim sendByte2(5 - 1) As Byte
        Dim sendstr As String = "aadb0103"
        For i As Integer = 0 To 3
            sendByte2(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte2(4) = value
        'For i As Integer = 0 To senderArray.Length - 1

        mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte2)
        SetresetTemp = True
    End Function

    ''' <summary>
    ''' 设置定时复位时间间隔
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Function SetResetTimeSec(value As Integer) As Boolean
        Dim sendByte2(5 - 1) As Byte
        Dim sendstr As String = "aadb0102"
        For i As Integer = 0 To 3
            sendByte2(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte2(4) = value
        mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte2)
        SetResetTimeSec = True
    End Function
End Module

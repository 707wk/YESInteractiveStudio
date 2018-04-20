Imports System.Net.NetworkInformation

Module ModuleSmartTimeApp
    ''' <summary>
    ''' 获取错误码
    ''' </summary>
    Declare Function SmartTimeGetLastError Lib ".\data\LiveUpdate.dll" () As Integer
    ''' <summary>
    ''' 查找加密锁
    ''' </summary>
    Declare Function SmartTimeFind Lib ".\data\LiveUpdate.dll" (ByVal nAppID As String, ByRef KeyHandle As Integer, ByRef KeyNumber As Integer) As Integer
    ''' <summary>
    ''' 打开加密锁
    ''' </summary>
    Declare Function SmartTimeOpen Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByVal uPin1 As Integer, ByVal uPin2 As Integer, ByVal uPin3 As Integer, ByVal uPin4 As Integer) As Integer
    ''' <summary>
    ''' 关闭加密锁
    ''' </summary>
    Declare Function SmartTimeClose Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer) As Integer
    ''' <summary>
    ''' 检查加密锁是否存在
    ''' </summary>
    Declare Function SmartTimeCheckExist Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer) As Integer
    ''' <summary>
    ''' 获取加密锁硬件ID
    ''' </summary>
    Declare Function SmartTimeGetUid Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef UID As Byte) As Integer
    ''' <summary>
    ''' 读内存区
    ''' </summary>
    Declare Function SmartTimeReadMemory Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByVal start As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    ''' <summary>
    ''' 写内存区
    ''' </summary>
    Declare Function SmartTimeWriteMemory Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByVal start As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    ''' <summary>
    ''' 读分页存储区
    ''' </summary>
    Declare Function SmartTimeReadPageFile Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByVal pageNo As Integer, ByVal startAddr As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    ''' <summary>
    ''' 写分页存储区
    ''' </summary>
    Declare Function SmartTimeWritePageFile Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByVal pageNo As Integer, ByVal startAddr As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    ''' <summary>
    ''' 3DES数据加密
    ''' </summary>
    Declare Function SmartTimeTriDesEncrypt Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef pBuffer As Byte, ByVal buffSize As Integer) As Integer
    ''' <summary>
    ''' 3DES数据解密
    ''' </summary>
    Declare Function SmartTimeTriDesDecrypt Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef pBuffer As Byte, ByVal buffSize As Integer) As Integer
    ''' <summary>
    ''' 获取到期时间
    ''' </summary>
    Declare Function SmartTimeGetExpiryDateTime Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef endYear As Integer, ByRef endMonth As Integer, ByRef endDay As Integer, ByRef endHour As Integer, ByRef endMin As Integer, ByRef endSec As Integer) As Integer
    ''' <summary>
    ''' 获取锁内时间
    ''' </summary>
    Declare Function SmartTimeGetCurrentDateTime Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef currYear As Integer, ByRef currMonth As Integer, ByRef currDay As Integer, ByRef currHour As Integer, ByRef currMin As Integer, ByRef currSec As Integer) As Integer
    ''' <summary>
    ''' 获取剩余次数
    ''' </summary>
    Declare Function SmartTimeGetCount Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef Count As Integer) As Integer
    ''' <summary>
    ''' 获取版本号
    ''' </summary>
    Declare Function SmartTimeGetSoftVersion Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef version As Integer) As Integer
    ''' <summary>
    ''' 获取请求码
    ''' </summary>
    Declare Function SmartTimeRemoteRequest Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByRef Request As Byte) As Integer
    ''' <summary>
    ''' 注册
    ''' </summary>
    Declare Function SmartTimeRemoteRegister Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByVal Register As String) As Integer
    ''' <summary>
    ''' 升级
    ''' </summary>
    Declare Function SmartTimeUpgrade Lib ".\data\LiveUpdate.dll" (ByVal KeyHandle As Integer, ByVal FilePath As String) As Integer

    ''' <summary>
    ''' 将硬件信息写入加密狗
    ''' </summary>
    Public Sub register(KeyHandle As Integer, hashcode As String)
        Dim pageNo As Integer = 0
        Dim startAdd As Integer = 0
        Dim pBuffer() As Byte = System.Text.Encoding.Unicode.GetBytes(hashcode)

        If SmartTimeWritePageFile(KeyHandle, 0, 0, 64, pBuffer(0)) <> 0 Then
            'putlog("注册失败")
            'TextBox5.AppendText("写该分页失败,错误代码：" & SmartTimeGetLastError())
            End
        End If

    End Sub

    ''' <summary>
    ''' 检测加密狗是否配对
    ''' </summary>
    Public Sub checkdog()
        If System.IO.File.Exists(".\data\LiveUpdate.dll") = False Then
            End
        End If

        Dim KeyHandle(8) As Integer
        Dim KeyNum As Integer
        Dim GUID(32) As Byte
        Dim sGUID As String = Nothing

        If SmartTimeFind("ME触摸地砖屏控制系统", KeyHandle(0), KeyNum) <> 0 Then
            'putlog("查找加密锁失败,错误码是：" & SmartTimeGetLastError())
            End
        End If
        'putlog("查找到加密锁的个数是：" & KeyNum & vbCrLf)

        If SmartTimeGetUid(KeyHandle(0), GUID(0)) <> 0 Then
            'putlog("获取加密锁的硬件ID失败,错误代码是：" & SmartTimeGetLastError())
            End
        End If

        For i = 0 To 32 - 1
            sGUID = sGUID & Chr(GUID(i))
        Next

        Dim uPin1 As Integer = &H5025479E
        Dim uPin2 As Integer = &HDE5E769B
        Dim uPin3 As Integer = &HA5D52993
        Dim uPin4 As Integer = &H84878D64

        If SmartTimeOpen(KeyHandle(0), uPin1, uPin2, uPin3, uPin4) <> 0 Then
            'putlog("打开加密锁失败,错误代码是：" & SmartTimeGetLastError())
            End
        End If

        Dim pageNo As Integer = 0
        Dim startAdd As Integer = 0
        Dim pBuffer(64) As Byte
        Dim hashcode As String
        'Dim descBytes() As Byte = System.Text.Encoding.Unicode.GetBytes("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")

        If SmartTimeReadPageFile(KeyHandle(0), pageNo, startAdd, 64, pBuffer(0)) <> 0 Then
            'putlog("读该分页失败,错误代码：" & SmartTimeGetLastError())
            End
        End If

        Dim zeroNum As Integer = 0
        For i As Integer = 0 To 64 - 1
            If pBuffer(i) = 0 Then
                zeroNum += 1
            End If
        Next

        If zeroNum = 64 Then
            register(KeyHandle(0), Wangk.Hash.GetStr128MD5(sGUID & "YESTECH"))
        Else
            hashcode = System.Text.Encoding.Unicode.GetString(pBuffer, 0, 64)
            If hashcode.Equals(Wangk.Hash.GetStr128MD5(sGUID & "YESTECH")) = False Then
                End
            End If
        End If

        SmartTimeClose(KeyHandle(0))
    End Sub
End Module
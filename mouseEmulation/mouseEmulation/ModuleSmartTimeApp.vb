Module ModuleSmartTimeApp
    '获取错误码
    Declare Function SmartTimeGetLastError Lib "SmartTimeApp.dll" () As Integer
    '查找加密锁
    Declare Function SmartTimeFind Lib "SmartTimeApp.dll" (ByVal nAppID As String, ByRef KeyHandle As Integer, ByRef KeyNumber As Integer) As Integer
    '打开加密锁
    Declare Function SmartTimeOpen Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByVal uPin1 As Integer, ByVal uPin2 As Integer, ByVal uPin3 As Integer, ByVal uPin4 As Integer) As Integer
    '关闭加密锁
    Declare Function SmartTimeClose Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer) As Integer
    '检查加密锁是否存在
    Declare Function SmartTimeCheckExist Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer) As Integer
    '获取加密锁硬件ID
    Declare Function SmartTimeGetUid Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef UID As Byte) As Integer
    '读内存区
    Declare Function SmartTimeReadMemory Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByVal start As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    '写内存区
    Declare Function SmartTimeWriteMemory Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByVal start As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    '读分页存储区
    Declare Function SmartTimeReadPageFile Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByVal pageNo As Integer, ByVal startAddr As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    '写分页存储区
    Declare Function SmartTimeWritePageFile Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByVal pageNo As Integer, ByVal startAddr As Integer, ByVal length As Integer, ByRef pBuffer As Byte) As Integer
    '3DES数据加密
    Declare Function SmartTimeTriDesEncrypt Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef pBuffer As Byte, ByVal buffSize As Integer) As Integer
    '3DES数据解密
    Declare Function SmartTimeTriDesDecrypt Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef pBuffer As Byte, ByVal buffSize As Integer) As Integer
    '获取到期时间
    Declare Function SmartTimeGetExpiryDateTime Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef endYear As Integer, ByRef endMonth As Integer, ByRef endDay As Integer, ByRef endHour As Integer, ByRef endMin As Integer, ByRef endSec As Integer) As Integer
    '获取锁内时间
    Declare Function SmartTimeGetCurrentDateTime Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef currYear As Integer, ByRef currMonth As Integer, ByRef currDay As Integer, ByRef currHour As Integer, ByRef currMin As Integer, ByRef currSec As Integer) As Integer
    '获取剩余次数
    Declare Function SmartTimeGetCount Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef Count As Integer) As Integer
    '获取版本号
    Declare Function SmartTimeGetSoftVersion Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef version As Integer) As Integer
    '获取请求码
    Declare Function SmartTimeRemoteRequest Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByRef Request As Byte) As Integer
    '注册
    Declare Function SmartTimeRemoteRegister Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByVal Register As String) As Integer
    '升级
    Declare Function SmartTimeUpgrade Lib "SmartTimeApp.dll" (ByVal KeyHandle As Integer, ByVal FilePath As String) As Integer
End Module

Imports System.IO
Imports System.IO.Ports
Imports System.Net
Imports System.Text
Imports IniParser
Imports IniParser.Model


Module Module1

    Public URLs As String
    Public conf As New FileIniDataParser
    Public sp As New SerialPort
    Public confdata As IniData
    Public NL As String
    Dim endchar() As Char
    Sub Main()

        Console.BackgroundColor = ConsoleColor.White
        Console.ForegroundColor = ConsoleColor.Black
        Console.Clear()

        Console.WriteLine("→#串口消息转发工具-企业微信&钉钉#")
        Console.WriteLine("→开始运行，按""ESC""退出..." & " @ " & Now)
        Console.WriteLine("→请仔细阅读并确认配置文件""Conf.ini""的内容！")
        Try
            confdata = conf.ReadFile("conf.ini")

            URLs = confdata.Sections("WeChat_DingTalk").GetKeyData("URL").Value

            sp.PortName = confdata.Sections("SerialPort").GetKeyData("PortName").Value
            sp.BaudRate = confdata.Sections("SerialPort").GetKeyData("BaudRate").Value
            sp.Parity = confdata.Sections("SerialPort").GetKeyData("Parity").Value
            sp.DataBits = confdata.Sections("SerialPort").GetKeyData("DataBits").Value
            sp.StopBits = confdata.Sections("SerialPort").GetKeyData("StopBits").Value
            sp.Handshake = confdata.Sections("SerialPort").GetKeyData("Handshake").Value
            sp.ReadTimeout = confdata.Sections("SerialPort").GetKeyData("Timeout").Value
            sp.WriteTimeout = confdata.Sections("SerialPort").GetKeyData("Timeout").Value
            NL = confdata.Sections("SerialPort").GetKeyData("NL").Value.ToString

            Select Case NL
                Case "vbLf"
                    sp.NewLine = vbLf
                    endchar = vbLf.ToCharArray
                Case "vbCrLf"
                    sp.NewLine = vbCrLf
                    endchar = vbCrLf.ToCharArray
                Case "vbCr"
                    sp.NewLine = vbCr
                    endchar = vbCr.ToCharArray
                Case Else
                    sp.NewLine = NL
                    endchar = NL.ToCharArray
            End Select
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine("→配置文件加载成功！" & " @ " & Now)
            Console.ForegroundColor = ConsoleColor.Black
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("→配置文件打开失败，5s后退出..." & " @ " & Now)
            Console.ForegroundColor = ConsoleColor.Black
            Threading.Thread.Sleep(5000)
            Exit Sub
        End Try
        sp.Encoding = System.Text.Encoding.GetEncoding("gb2312")

        Console.WriteLine("→群机器人Webhook地址：")
        Console.WriteLine("→" & URLs & " @ " & Now)

        Try
            sp.Open()
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine("→串口打开成功,串口号：" & sp.PortName & "；波特率：" & sp.BaudRate & "；奇偶校验：" & sp.Parity & "；数据位：" & sp.DataBits & "；停止位：" & sp.StopBits & "；流控制：" & sp.Handshake & "；读超时:" & sp.ReadTimeout & "ms" & " @ " & Now)
            Console.ForegroundColor = ConsoleColor.Black
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("→串口打开失败，5s后退出..." & " @ " & Now)
            Console.ForegroundColor = ConsoleColor.Black
            Threading.Thread.Sleep(5000)
            Exit Sub
        End Try

        AddHandler sp.DataReceived, AddressOf DataReceivedHandler
        If Console.ReadKey().Key = ConsoleKey.Escape Then
            sp.Close()
            Exit Sub
        End If

    End Sub


    Private Sub DataReceivedHandler(sender As Object, e As SerialDataReceivedEventArgs)
        Dim spc As SerialPort
        Dim indata As String = ""
        spc = CType(sender, SerialPort)
        Try
            indata = spc.ReadLine()
            sp.DiscardInBuffer()
            sp.DiscardOutBuffer()
        Catch ex As Exception
            sp.DiscardInBuffer()
            sp.DiscardOutBuffer()
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("→接收串口消息出错！" & " @ " & Now)
            Console.ForegroundColor = ConsoleColor.Black
        End Try
        If indata <> "" Then
            indata = indata.TrimEnd(endchar)
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine("→接收到串口消息:[" & indata & "]" & " @ " & Now)
            Console.ForegroundColor = ConsoleColor.Black

            indata = "{""msgtype"":""text"",""text"":{""content"":""" & indata & """}}"
            ServicePointManager.Expect100Continue = False
            Dim request As WebRequest = WebRequest.Create(URLs)
            request.Method = "POST"
            request.Timeout = 5000
            Dim encoding As New UTF8Encoding()
            Dim bys As Byte() = encoding.GetBytes(indata)
            request.ContentLength = bys.Length
            request.ContentType = "application/json;charset=UTF-8"
            Try
                Dim datastream As Stream = request.GetRequestStream()
                datastream.Write(bys, 0, bys.Length)
                datastream.Close()
                Dim sr As StreamReader = New StreamReader(request.GetResponse.GetResponseStream)
                Console.ForegroundColor = ConsoleColor.DarkMagenta
                Console.WriteLine("→服务器返回信息：[" & sr.ReadToEnd & "]" & " @ " & Now)
                Console.ForegroundColor = ConsoleColor.Black
            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("→信息发送出错！" & " @ " & Now)
                Console.ForegroundColor = ConsoleColor.Black
            End Try
        End If
    End Sub

End Module

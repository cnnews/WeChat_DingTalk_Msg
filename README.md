#串口消息转发工具-企业微信&钉钉#

Conf.ini，放在可执行文件同一个目录下

---------------------------------------------------------------

;在你深入了解本文件的内容之前,请不要修改本文件内容！
;修改文件内容后需要重新打开软件生效

[WeChat_DingTalk]
;企业微信或钉钉群机器人Webhook地址
URL=https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=xxxxxx
;URL=https://oapi.dingtalk.com/robot/send?access_token=xxxxxxxx

;串口参数,应当与发送端设置匹配
[SerialPort]
;接收信息的串口号,COM1,COM2,COM3...
PortName=COM2
;波特率,默认9600
BaudRate=9600
;奇偶校验,可选0(无),1(奇校验),2(偶校验)
Parity=0
;数据位长度，可选5-8,默认8
DataBits=8
;停止位,可选1(1bit),2(2bit),3(1.5bit),默认1
StopBits=1
;流控制,可选0(无),1(软件流控),2(硬件流控),3(软硬件流控),默认0
Handshake=0
;读写超时(ms),默认500
Timeout=500
;消息结束标志,每一组消息中位于该标志以后的字符会被忽略.
;可选vbCr(换行)、vbLf(回车)、vbCrLf(回车+换行)，或者其他英文字串如"aabbcc",默认vbCr
NL=vbCr

---------------------------------------------------------------

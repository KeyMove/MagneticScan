uart.setup(0,115200,8,0,1,0)
wifi.setmode(3)
wifi.ap.config({ssid="Dev-"..node.chipid(),pwd="12345678"})
TimeOut=0
MaxTimeOut=30
PreRecv=function() return false end
UDPServer=nil
function init()
if UDPServer==nil then
UDPServer=net.createServer(net.UDP,5)
UDPServer:listen(2333,nil)
UDPServer:on("receive",function(c,d)
user=c
TimeOut=0
if PreRecv(d) then return end  
local str={}
local op=print
print=function(v) table.insert(str,(v or "nil")) end
local f,v=pcall(loadstring(d))
if not f then table.insert(str,"\r\n") table.insert(str,v) end
c:send(table.concat(str))
print=op
end)
end
wifi.sta.connect()
tmr.alarm(6, 1000, tmr.ALARM_AUTO, function() TimeOut=TimeOut+1 if TimeOut>MaxTimeOut then wifi.sta.connect() TimeOut=0 end end)
end

init()
if file.open("app.lua") and adc.read(0)==1024 then
dofile("app.lua")
end
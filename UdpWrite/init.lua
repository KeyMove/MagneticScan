function init()
uart.setup(0,115200,8,0,1,0)
wifi.setmode(3)
wifi.ap.config({ssid="Car-"..node.chipid(),pwd="12345678"})
if file.open("cfg") then
local ssid=file.readline()
local pwd=file.readline()
if not ssid or not pwd then return end
wifi.sta.config(ssid,pwd)
return 1
end
end

function saveConfig()
local n,p=wifi.sta.getconfig()
file.remove("cfg")
file.open("cfg","w+")
file.writeline(n)
file.writeline(p)
file.flush()
file.close()
end

PreRecv=function() return false end
UDPServer=net.createServer(net.UDP,5)
UDPServer:listen(2333,nil)
UDPServer:on("receive",function(c,d)
user=c
if PreRecv(d) then return end  
local str={}
local op=print
print=function(v) table.insert(str,(v or "nil")) end
local f,v=pcall(loadstring(d))
if not f then table.insert(str,"\r\n") table.insert(str,v) end
c:send(table.concat(str))
print=op
end)

init()
if file.open("app.lua") then
dofile("app.lua")
end
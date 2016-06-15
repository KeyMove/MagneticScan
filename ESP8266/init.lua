function init()
print("set bps:115200")
uart.setup(0,115200,8,0,1,0)
wifi.setmode(3)
wifi.ap.config({ssid="Nwifi"..node.chipid(),pwd="12345678"})
if file.open("cfg") then
msg=cjson.decode(file.readline())
if not msg.ssid or not msg.pwd then return end
wifi.sta.config(msg.ssid,msg.pwd)
wifi.sta.autoconnect(1)
wifi.sta.connect()
end
end

function saveConfig()
local n,p=wifi.sta.getconfig()
local v=cjson.encode({ssid=n,pwd=p})
file.open("cfg","w+")
file.writeline(v)
file.close()
uart.write(0,"ok");
end

init()
dofile("app.lua")
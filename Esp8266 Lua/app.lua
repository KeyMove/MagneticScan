local cmd={alive=0,getdataWifi=1,setdataUart=2,getipid=3}
local s=net.createServer(net.UDP,5)--
local user=nil
local Esp8266={}

function t2s(t) for i=1,#t do t[i]=string.char(t[i]) end return table.concat(t) end

local op = print

function uartExp(data)

	if Decode(data)==false then
	local str={}
	print=function(v) table.insert(str,(v or "nil")) end
	local f,v=pcall(loadstring(data))
	if not f then table.insert(str,"\r\n") table.insert(str,v)  end
	print=op
	print(table.concat(str))	
	end
end

s:listen(2334,nil)
s:on("receive",function(c,d)
user=c
	if Decode(d)==false then
	local str={}
	print=function(v) table.insert(str,(v or "nil")) end
	local f,v=pcall(loadstring(d))
	if not f then table.insert(str,"\r\n") table.insert(str,v) end
	c:send(table.concat(str))
	print=op
	end
end)

m=0
l=0
id=0
sum=0
data=nil
function Decode(dat)--解码收到的数据并发送  0 解码失败
local v=string.byte 
local t=nil

for i=1,#dat do
t=v(dat,i)
if m==0 then
if t==85 then 
m=1
table.insert(data,t)
end
elseif m==1 then
l=t
table.insert(data,t)
l=l*256
m=2
elseif m==2 then
l=l+t
table.insert(data,t)
m=3
elseif m==3 then
id=t
table.insert(data,t)
m=4
sum=0
elseif m==4 then
if l~=0 then 
sum=sum+t table.insert(data,t) l=l-1 if l==0 then m=5 sum=sum%256 end 
else m=5
end
elseif m==5 then
if t==sum then table.insert(data,t) m=6 else m=0 data={} end
elseif m==6 then
if Esp8266[id]~=nil and t==170 then m=0 table.insert(data,t) Esp8266[id](data) data={} return true end

end
end
data={}
return false
end

function appInit()
Esp8266[cmd.getdataWifi] = function (t) if user==nil then return end user:send(t2s(t)) tmr.alarm(0,30000,1,function() wifi.sta.connect() end) end--发送数据到wifi
Esp8266[cmd.setdataUart] = function (t) t=t or {0} for i=1,#t do  uart.write(0,t[i]) end end--发送数据到uart
Esp8266[cmd.getipid] = function(t) print(t) if user==nil then return end user:send(t) tmr.alarm(0,30000,1,function() wifi.sta.connect() end)  end
uart.on("data",uartExp,0)
end

function GetIPID()
Esp8266[cmd.getipid]("ID:"..node.chipid())
end

function sendWifi(t)
Esp8266[cmd.getdataWifi](t)

end

function sendUart(t)
Esp8266[cmd.setdataUart](t)

end

appInit()

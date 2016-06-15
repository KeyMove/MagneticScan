local init = false
local idinit = false
local cmd={alive=0,getdata=1,setdata=2,getipid=3}
local s=net.createServer(net.UDP,5)
local uartcall={}
local user=nil
s:listen(2333,nil)
s:on("receive",function(c,d)
user=c
local str={}
local op=print
print=function(v) table.insert(str,(v or "nil")) end
local f,v=pcall(loadstring(d))
if not f then table.insert(str,"\r\n") table.insert(str,v) end
c:send(table.concat(str))
print=op
end)

function t2s(t) for i=1,#t do t[i]=string.char(t[i]) end return table.concat(t) end

function uartpacket(id,data)
local sum=0;
data=data or {0}
uart.write(0,85)
uart.write(0,#data/256)
uart.write(0,#data%256)
uart.write(0,id)
if data~=nil then
for i=1,#data do
sum=sum+data[i]
uart.write(0,data[i])
end
else
uart.write(0,0)
end
uart.write(0,sum%256)
uart.write(0,170)
end



m=0
l=0
id=0
sum=0
data=nil
function uartdecode(dat)
local v=string.byte 
local t=nil
for i=1,#dat do
t=v(dat,i)
if m==0 then
if t==85 then m=1 end
elseif m==1 then
l=t
l=l*256
m=2
elseif m==2 then
l=l+t
m=3
elseif m==3 then
id=t
data={}
m=4
sum=0
elseif m==4 then
if l~=0 then sum=sum+t table.insert(data,t) l=l-1 if l==0 then m=5 sum=sum%256 end end
elseif m==5 then
if t==sum then m=6 else m=0 end
elseif m==6 then
if uartcall[id]~=nil then uartcall[id](data) end
m=0
end
end
end

function UpdateData()
uartpacket(cmd.getdata)
end

function GetData()
if not init then 
init=true
uart.on("data",uartdecode,0)
uartpacket(cmd.getdata,nil)
uartcall[cmd.getdata]=function (t) if user==nil then return end user:send("Data:"..t2s(t)) end
end
uartpacket(cmd.getdata)
end

function GetIPID()
if not idinit then
idinit=true
uartcall[cmd.getipid]=function (t) if user==nil then 
return end user:send("ID:"..t) end
end
uartcall[cmd.getipid](node.chipid())
end

Dim NTPserver
NTPserver=127.0.0.1

On Error Resume Next 
Set objFSO =CreateObject("Scripting FilesystemObject")
Set objFile=objFSO.OpenTextFile("ComputerIPList.txt")

strInstallFile="           "
StrInstallCMD="            "

Do until objFile.AtEndOfStream

strComputer=objFile.readLine

'------Checking If PC is On -------'
Set WshShell=WScript.CreateObject("WScript.Shell")
Set WshExec=WshShell.Exec("ping -n 1 -w 1000 "& strComputer) 'send 3 Echo Requests ,waiting 2 secs each 
strPingResults=LCase(WshExec.StdOut.ReadAll)

Dim objWMI,osInfo
Set objWMI=GetObject("wingmts:"&"{impersonationLevel=impersonate}!\\.\root\cimv2")
Set osInfo = objWMI.ExecQuery("SELECT * FROM Win32_OperatingSystem")

Dim Flag 
 Flag=False

For each os in osInfo
If Left(os.Version, 3) >= 6.0 Then
		flag = True
	End If
Next

Dim objShell
Set objShell=CreateObject("Shell.Application")

Dim fileName,fileNameXP
Dim dispreg

fileName   = "w32tm /config /update /manualpeerlist:" & NTPserver & " /syncfromflags:manual & sc config w32time start= delayed-auto & net start w32time & w32tm /resync"
fileNameXP = "w32tm /config /update /manualpeerlist:" & NTPserver & " /syncfromflags:manual & sc config w32time start= auto         & net start w32time & w32tm /resync"

dispreg = "& reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\DateTime\Servers /v 0 /t REG_SZ /d " & NTPserver & " /f & reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\DateTime\Servers /t REG_SZ /d 0 /f"

If flag Then ' Vista or later
	objShell.ShellExecute "cmd.exe", "/q /c """ & fileName   & dispreg & """","","runas",0
Else ' XP
	objShell.ShellExecute "cmd.exe", "/q /c """ & fileNameXP & dispreg & """","","",0
End If


If InStr(strPingResults,"reply from")Then 

'-------Sucesssful Ping then to run the commands-----'
Set objWMIService = GetObject("winmgmts:" _ & "{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2")

WshShell.Exec"%COMSPEC% /C COPY"& strInstallFile & "\\ "& strComputer &" \C$\Windows\TEMP",0,TRUE
WshShell.Exec"%COMSPEC% /C psexec \\"& strComputer &" "& StrInstallCMD &"",0,TRUE 

else 
'----------Unsuccessful Ping will leave the computer and proceed on with other computer -----'
strNewContents=strNewContents & strComputer & vbCrLf

End If
Loop

objFile.Close 

Set objFile = objFSO.OpenTextFile("ComputerIPList.txt", 2)
objFile.Write strNewContents

objFile.Close
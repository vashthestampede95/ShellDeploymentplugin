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
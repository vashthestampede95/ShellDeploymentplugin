on error resume next

Set objFSI=CreateObject("Scripting FileSystemObject")
Set objSFile=objFSI.OpenTextFile("ComputerIPList.txt")

strInstallFile="     "
strcmdline="   "
'---------------------Copying the file onto the remote computer on new directory ------'
Set objWMIService = GetObject("winmgmts:" & "{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2") 
 
Set colFolders = objWMIService.ExecQuery("Select * from Win32_Directory where Name = 'c:\\Scripts'") 
 
For Each objFolder in colFolders 
    errResults  = objFolder.Copy("D:\Archive") 
Next 

'---------------------Moving the file onto the backup directory folder and moving the new file onto new directory -------'
Set objWMIService = GetObject("winmgmts:" _ 
    & "{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2") 
 
Set colFolders = objWMIService.ExecQuery _ 
    ("Select * from Win32_Directory where name = 'c:\\Scripts'") 
 
For Each objFolder in colFolders 
    errResults = objFolder.Rename("C:\Admins\Documents\Archive\VBScript") 
Next
objSFile.Close

This is a small console application that add/remove an application from an IIS Website.

This is written in C#

## How use the application

1. Copy the IISSiteGenerator.exe from \bin\Release
2. Run the from command line 
	### Add an Application:
     IISSiteGenerator add < Web site name > < application name to be created > < directory >

	### Remove an Application:
     IISSiteGenerator remove < Web site name > < application name to be created > 

# Issues

1. There might be an issue on permission. Grant a read/write access on the file C:\Windows\System32\inetsrv\config\redirection.config to IUSR user account.


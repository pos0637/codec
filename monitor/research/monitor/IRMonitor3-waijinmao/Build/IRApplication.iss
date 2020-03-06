; -- 64Bit.iss --
; Demonstrates installation of a program built for the x64 (a.k.a. AMD64)
; architecture.
; To successfully run this installation and the program it installs,
; you must have a "x64" edition of Windows.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=外经贸大厦体温监测系统
AppVersion=1.5
AppVerName=外经贸大厦体温监测系统v1.5
AppPublisher=外经贸大厦
AppPublisherURL=http://www.gh-i.com
AppSupportURL=http://www.gh-i.com
AppUpdatesURL=http://www.gh-i.com
DefaultDirName={pf}\IRApplication
DefaultGroupName=外经贸大厦体温监测系统v1.5
UninstallDisplayIcon={app}\IRApplication.exe
Compression=lzma2
SolidCompression=yes
OutputDir=../Build
OutputBaseFilename=外经贸大厦体温监测系统v1.5
SetupIconFile=./Logo.ico
; "ArchitecturesAllowed=x64" specifies that Setup cannot run on
; anything but x64.
ArchitecturesAllowed=x64
; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
; done in "64-bit mode" on x64, meaning it should use the native
; 64-bit Program Files directory and the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64
AllowNoIcons=yes

[Files]
Source: "../Release/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\外经贸大厦体温监测系统"; Filename: "{app}\IRApplication.exe"

# FauxRansomware 

<br>A proof-of-concept minimal ransomware project.<br>

*Features*: 
- Completely written in C#
- Tested on Windows 7 and Windows 10
- Contains two sub-projects - '<b>DoIt</b>' to perform pseudo-ransomware activities and '<b>UndoIt</b>' to undo the changes made
- Randomized sleep times to provide a sense of timed-operation

*What does it do?*
- Uses PInvoke to change system's wallpaper (embedded in Resources section) 
- Finds files with certain extensions (.xlsx, .pdf & .docx) in the User's Desktop folder
- Overwrites the first few bytes of the files with null byte to make them inaccessible
- Changes the file extension
- Deletes the shadow volume using 'vssadmin' 
- Drops a ransom note
- Pops up a Windows message notification 

# Dependencies

This project depends on .NET framework 3.5. Please download it before compiling the project.

Download .NET Framework 3.5 SP1 - https://dotnet.microsoft.com/en-us/download/dotnet-framework/net35-sp1

# Compilation

Open the project in Visual Studio and build the projects in 'Debug' or 'Release' mode. The compiled binaries will be found in the 'bin' folder of the respective sub-project folders - ".\FauxRansomware\DoIt\bin" and ".\FauxRansomware\UndoIt\bin"

# Usage

DoIt (Ransomware)
```
DoIt.exe
```

UndoIt (Ransomware Recovery)
```
UndoIt.exe
```

# Recommended Systems
- [X] Windows 7 (x64)
- [X] Windows 10 (x64)

# Disclaimer

I, am not responsible for any actions, and or damages, caused by this project. This project was created for educational purposes only and it is NOT to be used maliciously on any system that is not owned by the user.

You bear the full responsibility of your actions and by using this project, you automatically agree to the above.

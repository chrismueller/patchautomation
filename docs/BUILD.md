# Build

Logon to an ![SMP](smp.png) SMP server.

Pull or copy the repo to a folder.

Run [build.bat](..\build.bat) from an elevated cmd prompt (As Administrator)

```
[Drive]:\Temp\Patch\patchautomation>build.bat
```

Check for any logs.

You may receive the following

```
Microsoft (R) Visual C# Compiler version 4.6.1586.0
for C# 5
Copyright (C) Microsoft Corporation. All rights reserved.

This compiler is provided as part of the Microsoft (R) .NET Framework, but only supports language versions up to C# 5, which is no longer the latest version. For compilers that support newer versions of the C# programming language, see http://go.microsoft.com/fwlink/?LinkID=533240
```

If everything was successful there should now be 3 new exes created where # is the current version you are building.

- PatchAutomation-*#*.exe
- PatchExclusion-*#*.exe
- ZeroDayPatch-*#*.exe
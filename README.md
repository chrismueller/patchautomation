# ![Band Aid Symantec](docs/band_aid_Symantec.png) PatchAutomation and ZeroDayPatch ![Band Aid Protirus](docs/band_aid_Protirus.png)

![](https://img.shields.io/badge/language-c%23-green.svg)
![](https://img.shields.io/badge/tag-smp-yellow.svg)
![](https://img.shields.io/badge/tag-symantec-yellow.svg)
![](https://img.shields.io/badge/tag-patch-yellow.svg)

Patch Automation and ZeroDayPatch tool kit for ![SMP](docs/smp.png) Symantec Management Platform

---

> Important Forewords: This tool will allow you to stage and distribute all bulletins that match the critical severity (or more if you use the /severity switch). I and Symantec in general do not advise to do this on test, validation or production systems _unless_ the MetaData Import Task is configured to import bulletins that you want to distribute or test.

---

There are also some **Docs**

- [Build](/docs/BUILD.md)
- [Build Errors](/docs/BUILDERRORS.md)
- [New Version](/docs/NEWVERSION.md)
- [Logs](/docs/LOGS.md)
- [Switches](/docs/SWITCHES.md)

---

Command Line Help

```
ZeroDayPatch (version #) command line usage:

    /vulnerable
            Use this command line switch to install and run a custom stored
            procedure to retrieve candidate bulletins. The procedure will be
            installed is and named ZeroDayPatch_GetVulnerableMachines-0003.

    /targetguid=
            Use this option to set the target guid to be used with newly 
            created policies. This will over-write the default target defined
            globally.
			
            Note that you can specify more than 1 target guid. Just add more
            /targetguid= to you command line or config file. This
            is most useful if you are delegating computer targetting to other
            team (such as server , workstation administrators).

    /config=
        Reads the file at the provided path and parses each line for com-
        -mand line options. Here is a sample config file content:
            /severity=critical
            /custom-sp=CWoC_GetAllBulletins
            /vendor=google
            /dryrun
            /debug

    /test   
        Run the automate in test mode only. A maximum of 10 policies will
        be created in this mode.

    /dryrun 
        Run the automate in dry run mode. No changes will be made to the 
        system, but expected operation will be printed to the console.

    /severity=|*
        Set the severity used to select bulletins that will be handle by 
        the automate. The * wildcard can be use to match all severities.

    /patchall
        Use this command line if you want to manage bulletins from all
        vendors in the database. By default we only handle Microsoft bul-
        -letins.

    /released-before=
        Configure a date filter that will include bulletin released before
        the specified date. It is set by default to the current date.

    /released-after=
        Configure a date filter that will include bulletin released after
        the specified date. It is set by default to (current date -1 year).

    /custom-sp=
        This option allows the user to specify a custom stored procedure to
        be called during the execution. The stored procedure may be present
        on the database (if not the automate will return with no errors) and
        must contains the following columns that are used and needed:
            * _resourceguid [Software bulletin guid]
            * released [Software bulletin release date]
            * bulletin [Bulletin name]
            * severity [Bulletin Severity]
        You can also add a vendor column if you want to filter bulletins by
        vendor (see option /vendor)

    /vendor=|*
        Configure a vendor filter to only return bulletins that match the
        vendor string from a custom procedure. This is because the vendor
        field doesn't exist in default Patch Procedures used by this tool.

        If /vendor is specified with a custom-sp that doesn't contain the
        vendor field the setting will be ignored (all bulletins will be
        returned).

    /debug
        Output extra information on the command line to allow debugging or
        reporting problems to Symantec Connect.

    /duplicates
        Use this command if you want the tool to generate duplicate
        policies. This is useful if you want, for example, to migrate
        policies from a parent to a child SMP without disruption.

        Note! Duplicated and new entries will be added to the exclusion 
        table in the database for safety reasons.

    /exclude-on-fail
        Use this command to add bulletins to the excluded table if it fails
        3 times during the stagging or policy creation phases. If not uses
        the failing bulletin will only be skipped.

    /retarget
        Use this command if you want to switch existing policies to use a
        new target. The target guid should be provided with /targetguid=...

    /version
        Print out the current version of the tool.

    /?
        Print this help message to the console (stdout).
```

---

Original [PatchAutomation](https://github.com/somewhatsomewhere/patchautomation) from [Ludovic Ferre
](https://www.symantec.com/connect/user/ludovic-ferre)

>[END OF "SUPPORT" NOTICE]

>Hello everyone, after close to 5 years maintaining various tools around Symantec Connect this legacy is turning to be more of a burden than anything else.
>It's still is a great set of tool and they all have their use, but as such I'm not going to maintain them anymore.
>The source code for this tool may still change over time, and can be found on Github: https://github.com/somewhatsomewhere?tab=repositories

>[/END OF "SUPPORT" NOTICE]

---

Check the [WIKI](https://github.com/Protirus/patchautomation/wiki) or the following Symantec Connect Articles

- [{CWoC} PatchAutomation and ZeroDayPatch builds for 8.5](https://www.symantec.com/connect/articles/cwoc-patchautomation-and-zerodaypatch-builds-85)

- [{CWoC} PatchAutomation Toolkit - Documentation and Guides](https://www.symantec.com/connect/articles/cwoc-patchautomation-toolkit-documentation-and-guides)

- [{CWoC} PatchAutomation and ZeroDayPatch builds for 8.1](https://www.symantec.com/connect/downloads/cwoc-patchautomation-and-zerodaypatch-builds-81)

- [{CWoC} PatchAutomation and ZeroDayPatch builds for 7.6](https://www.symantec.com/connect/downloads/cwoc-patchautomation-and-zerodaypatch-builds-76)

- [{CWoC} PatchAutomation and ZeroDayPatch builds for 7.5 SP1](https://www.symantec.com/connect/downloads/cwoc-patchautomation-and-zerodaypatch-builds-75-sp1)

- [{CWoC} ZeroDayPatch: Patch Automation Tool for PMS 7.1 SP2](https://www.symantec.com/connect/downloads/cwoc-zerodaypatch-patch-automation-tool-pms-71-sp2)
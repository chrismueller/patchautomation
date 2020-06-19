# Switches

---

Replace # in `ZeroDayPatch-#.exe` to the current exe version number i.e. `ZeroDayPatch-8.5.exe`

---

Command **version**

`ZeroDayPatch-#.exe /version`

Output 

`{CWoC} ZeroDayPatch version 12`

---

Command **dryrun**

`ZeroDayPatch-#.exe /dryrun`

Output

```
Runtime Configuration data:
        Debug = False
        Dry run = True
        Help needed = False
        Patch all vendors = False
        Released after = 07/11/2008 10:08:05
        Released before = 07/11/2018 10:08:05
        Severity = critical
        Test run = False
        Vendor name = *
        Custom stored procedure =
        Vulnerable = False

ZeroDayPatch 12 starting.
Making sure legacy stored procedure spBulletinsWithVulnerableClients is not present.
Making sure legacy stored procedure ZeroDayPatch_GetVulnerableMachines-0001 is not present.
Making sure legacy stored procedure ZeroDayPatch_GetVulnerableMachines-0002 is not present.
Making sure legacy stored procedure ZeroDayPatch_GetVulnerableMachines-0003 is not present.
# Using "exec spPMCoreReport_AllSoftwareBulletins" to get bulletin candidates.
# 5074 bulletins returned by the stored procedure execution.
# 0 bulletin names found in the exclusion table.
1034 bulletins match the critical severity and will be checked for policies.

######## THIS IS A DRY RUN ########

Processing bulletin MS18-10-O365-DEFERRED (87efd180-d73d-4bd2-91a6-93ca8437e5a1) now.
        ... bulletin will be staged now.
        Bulletin is now staged.
        Checking if we need to create a new policy now.
        ... create a policy for the bulletin now.
        Software update policy created!

...

0 software update policy creation tasks were started.
ZeroDayPatch execution completed now. See you soon...
```

---

Command **test**

`ZeroDayPatch-#.exe /test`

Output

```
As above

But only 10 Policies are created.
```

---

**Command Line Help**

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
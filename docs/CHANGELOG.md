# Change Log

See below the versions and notes on each.

---

**Version 13**

*Nov 2018*

- SMP 8.5
- Created exe to work with 8.5 dlls
- Updated code for error with `GetNonstagedUpdates` on build.

---

**Version 12**

*10 Jul 2017*

- SMP 8.1
- The build available here works on 8.1, 8.1 RU1 and 8.1 RU2 (english OS).

---

**Version 11**

*14 Mar 2016*

- SMP 8.0
- This release doesn't have much else - but keeping up to date is good enough as the tool is now quite mature.

---

**Version 10**

*19 Feb 2015*

- SMP 7.6
- The major change that version 10 brings in (across all supported ITMS versions - 7.1 SP2 MP1.1, 7.5 SP1 and 7.6) is that we are no longer tied to the Altiris.PatchManagementCore.dll. The tools will work directly from any location. Also with 7.6 we moved to .Net 4.0 so the build linking actually points to the 4.0 GAC.

*16 Jun 2014*

- SMP 7.5 SP1
- Version 10 brings about a major change in the tool: it no longer requires the Altiris.PatchManagementCore.Web.dll to work. This means the tool is completely standalone and can run from any folders.

---

Original Notes from [Connect](https://www.symantec.com/connect/downloads/patch-automation-tool-pms-71-sp2)


Document version **10**:

- Attached version 10 for 7.1
- Updated the command line message to reflect changes

---

Document version **9**:

- Attached version 9 for 7.1
- Updated the command line message to reflect changes
- Added feature `/exclude-on-fail`
- Added feature `/retarget`

---

Document version **8**:

- Attached version 8
- Updated the command line message to reflect changes
- Added feature `/duplicates`
- Added automatic creation of the "patchautomation_excluded" table.
- Changed naming scheme to be simpler
- Aligned doc and release versions

---

Document version **7b**:

- Attached version 0.7.0 release (from revision 8755b369fd0e)
- Attached custom-procedure sample `CWoC_GetAllBulletins.sql`
- Updated the command line message on this download to reflect changes
- Added feature `/custom-sp=<store procedure>`
- Added feature `/vendor=<vendor name>|*`
- Added feature `/config=<config filename>`

---

Document version **7**:

- Attached version 0.6.7 release (from revision 777)
- Attached version 0.6.7 debug (from revision 777)
- Updated the command line message on this download to reflect changes
- Added feature `/severity=*`

---

Document version **6**:

- Attached version 0.6.6 (from revision 770)
- Made previous versions visible (so you are not forced to get the latest only ;)

---

Document version **5**:

- Added stored procedure schema versioning (and auto-update)
- Corrected stored procedure field to match "Released" date
- Corrected a typo in the console messages
- Attached file version 0.5.7

---

Document version **4**:

- Added command line `switch /released-before`
- Added command line `switch /released-after`
- Added command line `switch /patchall`
- Converted the command line switch description to be a copy of `/?` output
- Attached file version 0.5.6b which contains the Altiris.PatchManagementCore.Web.dll to allow the tool to run on 7.1 SP2 versions up to MP1.

---

Document version **3**:

- Added foreword section
- Attached file with version 0.5.4
- Remove the hardcoded target for new policies. This will now use the system default target (normally user configured)
- Added `/?` handler and help message for the console

---

Document version **2**:

- Replaced the file with version 0.5.3 (ZeroDayPatch-0.5.3.zip)
- Added /vulnerable switch to the tool and documentation
Implemented custom target guid during policy creation (missing from the API)

---

## Patch Automation

[{CWoC} PatchAutomation - Automated patching with Full Test Life-cycle](https://www.symantec.com/connect/downloads/cwoc-patch-automation-full-test-life-cycle)

*Document changes:*

**Version 1.1**: Changed the attached file to PatchAutomation-0.6.3 (build from revision 747).

**Version 1.2**: Changed the image link to the full size version.

**Version 1.3**: Updated the command line help message, workflow and uploaded release 0.6.4 (built from revision 750).

**Version 1.4**: Added patch exclusion using a database table in build 0.6.6 (built from revision 770).

**Version 1.5**: Added `/custom-sp` feature to allow users to call-in their own stored procedure and better control the bulletins handled by the program (built from revision 777)

**Version 1.6**: Added `/fr` switch and `/version` and updated the documentation accordingly and uploaded release 0.6.9  (built from revision 787). Also moved the data from the original blog post to a proper download page which is better suited.

**Version 1.7**: Added `/vendor` and `/config` command line options. Uploaded custom stored procedure to provide the missing vendor field (needed with /vendor). Uploaded version 0.7.0 (built from revision 1499b791f1eb)

**Version 1.8**: Pulled a bug fix and some spelling corrections (58f95c9cd476 and ade2195063cc) from Brian Nelson and bumped the version to 0.7.1.

**Version 1.9**: Changed the numbering scheme, so we are now at version 8 and added a "/duplicates" switch. This allows you to generate duplicate policies if you need them, or brand new ones. Any policy created will then be added to the "patchautomation_excluded" table that we generate automatically now. This is useful if you want to transition existing policies to a new target, or as in my case, from hierarchy based to locally generated. Amended the command line /? print out to match those changes.

**Version 2.0**: Added Patch Automation version 10 (Built for 7.1 SP2 MP1.1) and documentation on the `/exclude-on-fail` switch.

---

You may see links to the original code being hosted on Google, this has now been migrated to GitHub.

- [altiris-ns-tooling](https://code.google.com/archive/p/altiris-ns-tooling/source)
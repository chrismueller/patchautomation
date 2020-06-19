# Logs

Logs are created in the root folder where the exe resided.

- `journal_datetime.log`

For example `journal_20181115143405.log`

They will output

`15/11/2018 14:59:03: Staged bulletin MS18-10-O365-DEFERRED (guid=87efd180-d73d-4bd2-91a6-93ca8437e5a1).`

You can also check the **Altiris Log Viewer**

- "[Install Drive]:\Program Files\Altiris\Diagnostics\LogViewer2.exe"

This will have a Module column value of the exe i.e. `ZeroDayPatch-8.5.exe` then then whatever it is interacting with i.e. `Altiris.PathcManagementCore.dll` etc.
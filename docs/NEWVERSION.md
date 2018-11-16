# New Version

When a new ![SMP](smp.png) SMP version is released you will need to update the [build.bat](..\build.bat) file.

You will likely only need to up `#` in the following code but it's best practice to search the GAC for the following dlls to make sure the folder name is correct.

The `GAC` (`C:\Windows\Microsoft.NET\assembly\GAC_MSIL`)

In [build.bat](..\build.bat) we have some variables

`set acm=Altiris.Common`

`acm` is used on

`set atrscm=%acm%\%ver1%\%acm%`

which is using `ver1` and this is the value that needs to be checked/updated.

`set ver1=v4.0_8.5.3073.0__d516cb311cfb6e4f`

So

- Altiris.Common

The full path would be `C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Altiris.Common\v4.0_8.5.3073.0__d516cb311cfb6e4f\Altiris.Common.dll`

---

Add a new line at the top

`if "%1"=="#" goto build-#`

then add a new section

```bat
:build-#

set ver1=v4.0_#.3073.0__d516cb311cfb6e4f
set atrscm=%acm%\%ver1%\%acm%
set atrsns=%ans%\%ver1%\%ans%
set atrsrx=%ars%\%ver1%\%ars%
set atrssi=%asi%\%ver1%\%asi%

set ver2=v4.0_#.3035.0__99b1e4cc0d03f223
set atrstm=%ats%\%ver2%\%ats%
set atrstc=%atc%\%ver2%\%atc%
set atrsdn=%adn%\%ver2%\%adn%

set ver3=v4.0_#.3049.0__d516cb311cfb6e4f
set atrspm=%apm%\%ver3%\%apm%

set ver4=v4.0_#.3008.0__d516cb311cfb6e4f
set invrm=%airm%\%ver4%\%airm%

set ver5=v4.0_#.3018.0__d516cb311cfb6e4f
set softm=%asm%\%ver5%\%asm%

set fullref=/reference:%gac%\%softm%.dll /reference:%gac%\%invrm%.dll /reference:%gac%\%atrscm%.dll /reference:%gac%\%atrsns%.dll /reference:%gac%\%atrsrx%.dll /reference:%gac%\%adb%\%ver1%\%adb%.dll /reference:%gac%\%atrssi%.dll /reference:%gac%\%atrstm%.dll /reference:%gac%\%atrspm%.dll /reference:%gac%\%atrstc%.dll /reference:%gac%\%atrsdn%.dll

set id=#

cmd /c %csc% %fullref% /out:ZeroDayPatch-%id%.exe ZeroDayPatch.cs Constant.cs APIWrapper.cs CLIConfig.cs CLIInit.cs | %no_obs% | %no_pol% | %no_prv%
cmd /c %csc% %fullref% /out:PatchAutomation-%id%.exe PatchAutomation.cs Constant.cs APIWrapper.cs CLIConfig.cs CLIInit.cs | %no_obs% | %no_pol% | %no_prv%
cmd /c %csc% %fullref% /out:PatchExclusion-%id%.exe patchexclusion.cs APIWrapper.cs Constant.cs | %no_obs% | %no_pol% | %no_prv%

goto end
```

---

Next update the Version Number in [Constant.cs](https://github.com/Protirus/patchautomation/blob/139aac64d52f82ee73bdc00e83283fd000b0e956/Constant.cs#L9)

`public const string VERSION = "#";`

Now you can run the [BUILD](BUILD.md) to create your new versions.
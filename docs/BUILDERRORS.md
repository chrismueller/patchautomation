# Build Errors

For example when we created the new build for 8.5 we received the following error.

> APIWrapper.cs(65,5): error CS1662: Cannot convert anonymous method to delegate type 'System.Func<Altiris.Common.GuidCollection>' because some of the return types in the block are not implicitly convertible to the delegate return type
APIWrapper.cs(65,12): error CS0029: Cannot implicitly convert type 'Altiris.Common.Collections.Generic.Set<System.Guid>' to 'Altiris.Common.GuidCollection'

There have been some changes with `GuidCollections` and `List<Guid>`.

It's useful to have a .NET Decompiler to check what's going on.

- [dotPeek](https://www.jetbrains.com/decompiler/)
- [JustDecompile](https://www.telerik.com/products/decompiler.aspx)
- [ILSpy](https://github.com/icsharpcode/ILSpy)

Original

[APIWrapper.cs](https://github.com/Protirus/patchautomation/blob/139aac64d52f82ee73bdc00e83283fd000b0e956/APIWrapper.cs#L63)

```csharp
public static GuidCollection GetNonstagedUpdates(IList<Guid> gcUpdates){
    return new GuidCollection(PMDal.PerformWithDlr<GuidCollection>(delegate {
        return Altiris.NS.DataAccessLayer.DataAccessLayer<PatchManagementCoreResourcesDAL>.Instance.spPMCore_SoftwareUpdateListIsNotDownloaded(new GuidCollection(gcUpdates));
    }));
```

Updated

```csharp
GuidCollection gc = new GuidCollection();
foreach (Guid g in Altiris.NS.DataAccessLayer.DataAccessLayer<PatchManagementCoreResourcesDAL>.Instance.spPMCore_SoftwareUpdateListIsNotDownloaded(new GuidCollection(gcUpdates))) {
    gc.Add(g);
}
return gc;
```
public static class ModuleInit
{
    #region enable

    [ModuleInitializer]
    public static void Init() =>
        VerifyImageSharp.Initialize();

    #endregion

    [ModuleInitializer]
    public static void InitOther() =>
        VerifyImageSharp.Initialize();
}
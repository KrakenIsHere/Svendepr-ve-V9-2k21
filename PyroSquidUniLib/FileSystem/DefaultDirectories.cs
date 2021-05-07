using System;
using System.IO;



namespace PyroSquidUniLib.FileSystem
{
    public static class DefaultDirectories
    {
        public static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static readonly string ProgramFiles86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static readonly string CurrentUserDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static readonly string AllUsersDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        public static readonly string StartMenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
        public static readonly string Programs = Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);
        public static readonly string Startup = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        public static readonly string System32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
        public static readonly string SysWow64 = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        public static readonly string LocalDisk = Path.GetPathRoot(Environment.SystemDirectory);
        public static readonly string Documents = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        public static readonly string ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string Fonts = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        public static readonly string Resources = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
        public static readonly string AdminTools = Environment.GetFolderPath(Environment.SpecialFolder.CommonAdminTools);
        public static readonly string Windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
    }
}

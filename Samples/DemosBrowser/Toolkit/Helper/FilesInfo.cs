using System;

namespace DemosBrowser.Toolkit.Helper
{
    public static class FilesInfo
    {
        public static string FormatSize(this long dblFileSize)
        {
            return FormatSize((double)dblFileSize);
        }

        public static string FormatSize(this double dblFileSize)
        {
            if (dblFileSize < 1024)
                return String.Format("{0:N0} B", dblFileSize);
            if (dblFileSize < 1024 * 1024)
                return String.Format("{0:N2} KB", dblFileSize / 1024);
            if (dblFileSize < 1024 * 1024 * 1024)
                return String.Format("{0:N2} MB", dblFileSize / (1024 * 1024));
            if (dblFileSize >= 1024 * 1024 * 1024)
                return String.Format("{0:N2} GB", dblFileSize / (1024 * 1024 * 1024));

            return dblFileSize.ToString();
        }
    }
}

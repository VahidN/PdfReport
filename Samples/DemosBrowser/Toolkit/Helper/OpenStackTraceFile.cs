using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using EnvDTE;

namespace DemosBrowser.Toolkit.Helper
{
    public class OpenStackTraceFile
    {
        public string FullFilename { set; get; }
        public int Line { set; get; }
        public int Column { set; get; }

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private DTE FindVSInstance(string fileName)
        {
            IList<DTE> list = FindVSInstances();
            foreach (DTE dte in list)
            {
                if (dte.get_IsOpenFile("{FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF}", fileName))
                {
                    return dte;
                }
            }

            foreach (DTE dte in list)
            {
                if (dte != null)
                {
                    return dte;
                }
            }

            if (list.Count > 0)
            {
                return list[0];
            }

            return null;
        }

        private IList<DTE> FindVSInstances()
        {
            IBindCtx ctx;
            IRunningObjectTable table;
            IEnumMoniker moniker;
            IMoniker[] rgelt = new IMoniker[1];
            IntPtr zero = IntPtr.Zero;
            IList<DTE> list = new List<DTE>();
            CreateBindCtx(0, out ctx);
            ctx.GetRunningObjectTable(out table);
            table.EnumRunning(out moniker);
            moniker.Reset();
            while (moniker.Next(1, rgelt, zero) == 0)
            {
                string str;
                object obj2;
                rgelt[0].GetDisplayName(ctx, null, out str);
                table.GetObject(rgelt[0], out obj2);
                DTE item = obj2 as DTE;
                if ((item != null) && str.StartsWith("!VisualStudio.DTE"))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        private void ShowSelection(Window win)
        {
            ((TextSelection)win.Document.Selection).MoveTo(Line, Column, false);
            win.DTE.MainWindow.Activate();
            System.Threading.Thread.Sleep(500);
            if (GetForegroundWindow().ToInt32() != win.HWnd)
            {
                bool flag = SetForegroundWindow(new IntPtr(win.HWnd));
            }
        }

        public void ShowToUser()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                ShowToUserBackground();
            });
        }

        private void ShowToUserBackground()
        {
            if (!string.IsNullOrWhiteSpace(FullFilename) && File.Exists(FullFilename))
            {
                DTE dte = FindVSInstance(FullFilename);
                if (dte == null)
                {
                    return;
                }
                Window win = dte.ItemOperations.OpenFile(FullFilename, "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}");
                TryShowSelection(win, 0);
            }
        }

        private void TryShowSelection(Window win, int tries)
        {
            try
            {
                ShowSelection(win);
            }
            catch (COMException exception)
            {
                if ((tries >= 4) || !exception.Message.Contains("RPC_E_SERVERCALL_RETRYLATER"))
                {
                    throw;
                }
                System.Threading.Thread.Sleep(500);
                TryShowSelection(win, tries + 1);
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DemosBrowser.Toolkit.AcrobatReader
{
    public class ShellThumbnail : IDisposable
    {
        [Flags]
        public enum Estrret
        {
            StrretWstr = 0,
            StrretOffset = 1,
            StrretCstr = 2
        }

        [Flags]
        public enum Eshcontf
        {
            ShcontfFolders = 32,
            ShcontfNonfolders = 64,
            ShcontfIncludehidden = 128,
        }

        [Flags]
        public enum Eshgdn
        {
            ShgdnNormal = 0,
            ShgdnInfolder = 1,
            ShgdnForaddressbar = 16384,
            ShgdnForparsing = 32768
        }

        [Flags]
        public enum Esfgao
        {
            SfgaoCancopy = 1,
            SfgaoCanmove = 2,
            SfgaoCanlink = 4,
            SfgaoCanrename = 16,
            SfgaoCandelete = 32,
            SfgaoHaspropsheet = 64,
            SfgaoDroptarget = 256,
            SfgaoCapabilitymask = 375,
            SfgaoLink = 65536,
            SfgaoShare = 131072,
            SfgaoReadonly = 262144,
            SfgaoGhosted = 524288,
            SfgaoDisplayattrmask = 983040,
            SfgaoFilesysancestor = 268435456,
            SfgaoFolder = 536870912,
            SfgaoFilesystem = 1073741824,
            SfgaoHassubfolder = -2147483648,
            SfgaoContentsmask = -2147483648,
            SfgaoValidate = 16777216,
            SfgaoRemovable = 33554432,
            SfgaoCompressed = 67108864,
        }

        [Flags]
        public enum EIEIFLAG
        {
            IeiflagAsync = 1,
            IeiflagCache = 2,
            IeiflagAspect = 4,
            IeiflagOffline = 8,
            IeiflagGleam = 16,
            IeiflagScreen = 32,
            IeiflagOrigsize = 64,
            IeiflagNostamp = 128,
            IeiflagNoborder = 256,
            IeiflagQuality = 512
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0, CharSet = CharSet.Auto)]
        public struct StrretCstr
        {
            public Estrret uType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 520)]
            public byte[] cStr;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        public struct StrretAny
        {
            [FieldOffset(0)]
            public Estrret uType;
            [FieldOffset(4)]
            public IntPtr pOLEString;
        }
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Size
        {
            public int cx;
            public int cy;
        }

        [ComImport, Guid("00000000-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IUnknown
        {

            [PreserveSig]
            IntPtr QueryInterface(ref Guid riid, ref IntPtr pVoid);

            [PreserveSig]
            IntPtr AddRef();

            [PreserveSig]
            IntPtr Release();
        }

        [ComImportAttribute]
        [GuidAttribute("00000002-0000-0000-C000-000000000046")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMalloc
        {
            [PreserveSig]
            IntPtr Alloc(int cb);

            [PreserveSig]
            IntPtr Realloc(IntPtr pv, int cb);

            [PreserveSig]
            void Free(IntPtr pv);

            [PreserveSig]
            int GetSize(IntPtr pv);

            [PreserveSig]
            int DidAlloc(IntPtr pv);

            [PreserveSig]
            void HeapMinimize();
        }

        [ComImportAttribute]
        [GuidAttribute("000214F2-0000-0000-C000-000000000046")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumIDList
        {

            [PreserveSig]
            int Next(int celt, ref IntPtr rgelt, ref int pceltFetched);

            void Skip(int celt);

            void Reset();

            void Clone(ref IEnumIDList ppenum);
        }

        [ComImportAttribute]
        [GuidAttribute("000214E6-0000-0000-C000-000000000046")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellFolder
        {

            void ParseDisplayName(IntPtr hwndOwner, IntPtr pbcReserved,
                                  [MarshalAs(UnmanagedType.LPWStr)]string lpszDisplayName,
                                  ref int pchEaten, ref IntPtr ppidl, ref int pdwAttributes);

            void EnumObjects(IntPtr hwndOwner,
                             [MarshalAs(UnmanagedType.U4)]Eshcontf grfFlags,
                             ref IEnumIDList ppenumIDList);

            void BindToObject(IntPtr pidl, IntPtr pbcReserved, ref Guid riid,
                              ref IShellFolder ppvOut);

            void BindToStorage(IntPtr pidl, IntPtr pbcReserved, ref Guid riid, IntPtr ppvObj);

            [PreserveSig]
            int CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);

            void CreateViewObject(IntPtr hwndOwner, ref Guid riid,
                                  IntPtr ppvOut);

            void GetAttributesOf(int cidl, IntPtr apidl,
                                 [MarshalAs(UnmanagedType.U4)]ref Esfgao rgfInOut);

            void GetUIObjectOf(IntPtr hwndOwner, int cidl, ref IntPtr apidl, ref Guid riid, ref int prgfInOut, ref IUnknown ppvOut);

            void GetDisplayNameOf(IntPtr pidl,
                                  [MarshalAs(UnmanagedType.U4)]Eshgdn uFlags,
                                  ref StrretCstr lpName);

            void SetNameOf(IntPtr hwndOwner, IntPtr pidl,
                           [MarshalAs(UnmanagedType.LPWStr)]string lpszName,
                           [MarshalAs(UnmanagedType.U4)] Eshcontf uFlags,
                           ref IntPtr ppidlOut);
        }

        [ComImportAttribute, GuidAttribute("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IExtractImage
        {
            void GetLocation([Out, MarshalAs(UnmanagedType.LPWStr)]
                                 StringBuilder pszPathBuffer, int cch, ref int pdwPriority, ref Size prgSize, int dwRecClrDepth, ref int pdwFlags);

            void Extract(ref IntPtr phBmpThumbnail);
        }

        public class UnmanagedMethods
        {
            [DllImport("shell32", CharSet = CharSet.Auto)]
            internal extern static int SHGetMalloc(ref IMalloc ppMalloc);

            [DllImport("shell32", CharSet = CharSet.Auto)]
            internal extern static int SHGetDesktopFolder(ref IShellFolder ppshf);

            [DllImport("shell32", CharSet = CharSet.Auto)]
            internal extern static int SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);

            [DllImport("gdi32", CharSet = CharSet.Auto)]
            internal extern static int DeleteObject(IntPtr hObject);
        }

        ~ShellThumbnail()
        {
            Dispose();
        }

        private IMalloc _alloc;
        private bool _disposed;

        public ShellThumbnail()
        {
            DesiredSize = new System.Drawing.Size(100, 100);
        }

        public Bitmap ThumbNail { get; private set; }

        public System.Drawing.Size DesiredSize { get; set; }

        private IMalloc allocator
        {
            get
            {
                if (!_disposed)
                {
                    if (_alloc == null)
                    {
                        UnmanagedMethods.SHGetMalloc(ref _alloc);
                    }
                }
                else
                {
                    Debug.Assert(false, "Object has been disposed.");
                }
                return _alloc;
            }
        }

        public Bitmap GetThumbnail(string fileName)
        {
            if (!File.Exists(fileName) && !Directory.Exists(fileName))
            {
                throw new FileNotFoundException(string.Format("The file '{0}' does not exist", fileName), fileName);
            }

            if (ThumbNail != null)
            {
                ThumbNail.Dispose();
                ThumbNail = null;
            }

            var folder = getDesktopFolder;
            if (folder == null) return ThumbNail;

            var pidlMain = IntPtr.Zero;
            try
            {
                var cParsed = 0;
                var pdwAttrib = 0;
                var filePath = Path.GetDirectoryName(fileName);
                folder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, filePath, ref cParsed, ref pidlMain, ref pdwAttrib);
            }
            catch (Exception)
            {
                freeResources(pidlMain, folder, null, null);
                throw;
            }

            if (pidlMain == IntPtr.Zero)
            {
                freeResources(pidlMain, folder, null, null);
                return null;
            }


            var iidShellFolder = new Guid("000214E6-0000-0000-C000-000000000046");
            IShellFolder item = null;
            try
            {
                folder.BindToObject(pidlMain, IntPtr.Zero, ref iidShellFolder, ref item);
            }
            catch (Exception)
            {
                freeResources(pidlMain, folder, null, item);
                throw;
            }

            if (item == null)
            {
                freeResources(pidlMain, folder, null, null);
                return null;
            }

            IEnumIDList idEnum = null;
            try
            {
                item.EnumObjects(IntPtr.Zero, (Eshcontf.ShcontfFolders | Eshcontf.ShcontfNonfolders), ref idEnum);
            }
            catch (Exception)
            {
                freeResources(pidlMain, folder, idEnum, item);
                throw;
            }

            if (idEnum == null)
            {
                freeResources(pidlMain, folder, null, item);
                return null;
            }

            var pidl = IntPtr.Zero;
            var fetched = 0;
            var complete = false;
            while (!complete)
            {
                var hRes = idEnum.Next(1, ref pidl, ref fetched);
                if (hRes != 0)
                {
                    pidl = IntPtr.Zero;
                    complete = true;
                }
                else
                {
                    if (getThumbNail(fileName, pidl, item))
                    {
                        complete = true;
                    }
                }

                if (pidl != IntPtr.Zero)
                {
                    allocator.Free(pidl);
                }
            }

            freeResources(pidlMain, folder, idEnum, item);
            return ThumbNail;
        }

        private void freeResources(IntPtr pidlMain, IShellFolder folder, IEnumIDList idEnum, IShellFolder item)
        {
            if (idEnum != null) Marshal.ReleaseComObject(idEnum);
            if (item != null) Marshal.ReleaseComObject(item);
            if (pidlMain != IntPtr.Zero) allocator.Free(pidlMain);
            if (folder != null) Marshal.ReleaseComObject(folder);
        }

        private bool getThumbNail(string file, IntPtr pidl, IShellFolder item)
        {
            IntPtr hBmp = IntPtr.Zero;
            IExtractImage extractImage = null;
            try
            {
                var pidlPath = pathFromPidl(pidl);
                if (!Path.GetFileName(pidlPath).ToUpper().Equals(Path.GetFileName(file).ToUpper()))
                {
                    return false;
                }

                IUnknown iunk = null;
                var prgf = 0;
                var iidExtractImage = new Guid("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1");
                item.GetUIObjectOf(IntPtr.Zero, 1, ref pidl, ref iidExtractImage, ref prgf, ref iunk);
                extractImage = (IExtractImage)iunk;
                if (extractImage == null)
                {
                    return true;
                }

                var sz = new Size { cx = DesiredSize.Width, cy = DesiredSize.Height };
                var location = new StringBuilder(260, 260);
                var priority = 0;
                const int requestedColourDepth = 32;
                const EIEIFLAG flags =
                    EIEIFLAG.IeiflagScreen | EIEIFLAG.IeiflagAsync | EIEIFLAG.IeiflagQuality | EIEIFLAG.IeiflagCache;
                var uFlags = (int)flags;
                try
                {
                    extractImage.GetLocation(
                        location, location.Capacity, ref priority, ref sz, requestedColourDepth, ref uFlags);
                }
                catch { }

                extractImage.Extract(ref hBmp);

                if (hBmp != IntPtr.Zero)
                {
                    ThumbNail = Image.FromHbitmap(hBmp);
                }

                Marshal.ReleaseComObject(extractImage);
                extractImage = null;
                return true;
            }
            finally
            {
                if (hBmp != IntPtr.Zero)
                {
                    UnmanagedMethods.DeleteObject(hBmp);
                }
                if (extractImage != null)
                {
                    Marshal.ReleaseComObject(extractImage);
                }
            }
        }

        private static string pathFromPidl(IntPtr pidl)
        {
            var path = new StringBuilder(260, 260);
            var result = UnmanagedMethods.SHGetPathFromIDList(pidl, path);
            return result == 0 ? string.Empty : path.ToString();
        }

        private static IShellFolder getDesktopFolder
        {
            get
            {
                IShellFolder ppshf = null;
                UnmanagedMethods.SHGetDesktopFolder(ref ppshf);
                return ppshf;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_alloc != null)
            {
                Marshal.ReleaseComObject(_alloc);
            }
            _alloc = null;
            if (ThumbNail != null)
            {
                ThumbNail.Dispose();
            }
            _disposed = true;
        }
    }
}

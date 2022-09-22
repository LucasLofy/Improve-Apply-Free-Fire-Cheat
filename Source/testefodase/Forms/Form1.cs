using Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xanax;
using Xanax.Class;
using static Memory.Mem;

namespace testefodase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Func
        public Mem MemLib = new Mem();
        [DllImport("KERNEL32.DLL")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);
        [DllImport("KERNEL32.DLL")]
        public static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);
        [DllImport("KERNEL32.DLL")]
        public static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        private void SuspendProcess()
        {
            var process = Process.GetProcessById(Convert.ToInt32(PID.Text)); // throws exception if process does not exist
            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                if (pOpenThread == IntPtr.Zero)
                    continue;
                SuspendThread(pOpenThread);
                CloseHandle(pOpenThread);
            }
        }
        public void ResumeProcess()
        {
            var process = Process.GetProcessById(Convert.ToInt32(PID.Text));
            if (process.ProcessName == string.Empty)
                return;
            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                if (pOpenThread == IntPtr.Zero)
                    continue;
                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);
                CloseHandle(pOpenThread);
            }
        }

        public string GetProcID(int index)
        {
            string result = "";
            checked
            {
                if (index == 1 || index == 0)
                {
                    IntPtr intPtr = IntPtr.Zero;
                    uint num = 0U;
                    IntPtr intPtr2 = CreateToolhelp32Snapshot(2U, 0U);
                    if ((int)intPtr2 > 0)
                    {
                        ProcessEntry32 processEntry = default(ProcessEntry32);
                        processEntry.dwSize = (uint)Marshal.SizeOf<ProcessEntry32>(processEntry);
                        for (int num2 = Process32First(intPtr2, ref processEntry); num2 == 1; num2 = Process32Next(intPtr2, ref processEntry))
                        {
                            IntPtr intPtr3 = Marshal.AllocHGlobal((int)processEntry.dwSize);
                            Marshal.StructureToPtr<ProcessEntry32>(processEntry, intPtr3, true);
                            object obj = Marshal.PtrToStructure(intPtr3, typeof(ProcessEntry32));
                            ProcessEntry32 processEntry2 = (obj != null) ? ((ProcessEntry32)obj) : default(ProcessEntry32);
                            Marshal.FreeHGlobal(intPtr3);

                            if (processEntry2.szExeFile.Contains("HD-Player") && processEntry2.cntThreads > num)
                            {
                                num = processEntry2.cntThreads;
                                intPtr = (IntPtr)((long)(unchecked((ulong)processEntry2.th32ProcessID)));
                            }

                            if (processEntry2.szExeFile.Contains("AndroidProcess") && processEntry2.cntThreads > num)
                            {
                                num = processEntry2.cntThreads;
                                intPtr = (IntPtr)((long)(unchecked((ulong)processEntry2.th32ProcessID)));
                            }

                            if (processEntry2.szExeFile.Contains("LdVBoxHeadless") && processEntry2.cntThreads > num)
                            {
                                num = processEntry2.cntThreads;
                                intPtr = (IntPtr)((long)(unchecked((ulong)processEntry2.th32ProcessID)));
                            }

                            if (processEntry2.szExeFile.Contains("MEmuHeadless") && processEntry2.cntThreads > num)
                            {
                                num = processEntry2.cntThreads;
                                intPtr = (IntPtr)((long)(unchecked((ulong)processEntry2.th32ProcessID)));
                            }

                            if (processEntry2.szExeFile.Contains("NoxVMHandle") && processEntry2.cntThreads > num)
                            {
                                num = processEntry2.cntThreads;
                                intPtr = (IntPtr)((long)(unchecked((ulong)processEntry2.th32ProcessID)));
                            }

                            if (processEntry2.szExeFile.Contains("aow_exe") && processEntry2.cntThreads > num)
                            {
                                num = processEntry2.cntThreads;
                                intPtr = (IntPtr)((long)(unchecked((ulong)processEntry2.th32ProcessID)));
                            }
                        }
                    }
                    result = Convert.ToString(intPtr);
                    PID.Text = Convert.ToString(intPtr);
                }
                return result;
            }


        }

        public async void Rep(string original, string replace)
        {
            try
            {
                SuspendProcess();
                this.MemLib.OpenProcess(Convert.ToInt32(PID.Text));
                IEnumerable<long> scanmem = await this.MemLib.AoBScan(0L, 0x00007fffffffffff, original, true, true);
                long FirstScan = scanmem.FirstOrDefault();
                if (FirstScan == 0)
                {
                    Alert("Open Emulator First", XanaxAlert.enmType.Error);
                    ResumeProcess();
                }
                else
                {
                    Alert("Applying...", XanaxAlert.enmType.Applying);
                }
                foreach (long num in scanmem)
                {
                    this.MemLib.ChangeProtection(num.ToString("X"), Mem.MemoryProtection.ReadWrite, out Mem.MemoryProtection _);
                    this.MemLib.WriteMemory(num.ToString("X"), "bytes", replace);

                }
                if (FirstScan == 0)
                {
                    Alert("Cant Search, But Dont Worry", XanaxAlert.enmType.Error);
                    ResumeProcess();
                }
                else
                {
                    scanmem = (IEnumerable<long>)null;
                    Alert("Applied", XanaxAlert.enmType.Applied);
                    ResumeProcess();
                }
            }
            catch
            {
            }
        }

        public struct ProcessEntry32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
        #endregion

        public void Alert(string msg, XanaxAlert.enmType type)
        {
                XanaxAlert frm = new XanaxAlert();
                frm.showAlert(msg, type);
        }


        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            if (siticoneButton1.Checked)
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton1.Text = "Disable";
            }
            else
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton1.Text = "Enable";
            }
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            if (siticoneButton2.Checked)
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton2.Text = "Disable";
            }
            else
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton2.Text = "Enable";
            }
        }

        private void siticoneButton4_Click(object sender, EventArgs e)
        {
            if (siticoneButton4.Checked)
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton4.Text = "Disable";
            }
            else
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton4.Text = "Enable";
            }
        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            if (siticoneButton3.Checked)
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton3.Text = "Disable";
            }
            else
            {
                GetProcID(1);
                Rep("search", "replace");
                siticoneButton3.Text = "Enable";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);
            foreach (Process proc in processes)
            {
                proc.PriorityClass = ProcessPriorityClass.High;
            }

            string location = Assembly.GetExecutingAssembly().Location;
            if (location == "" || location == null)
            {
                location = Assembly.GetEntryAssembly().Location;
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C ren \"" + location + "\" " + Undetected.RandomString() + ".exe",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }
    }
}

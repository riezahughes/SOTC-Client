using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SotcArchipelago.Helpers
{
    // Core's macOS Mach-O lookup can't find PCSX2's EEmem symbol (it treats the symbol's absolute address as an offset
    // and ignores the load slide), so this does the lookup here. Needs root (the client relaunches itself under sudo).
    public static class MacPcsx2Helpers
    {
        private const string ProcessName = "PCSX2";
        private const string EEmemSymbol = "_EEmem";

        private const ulong FirstUserAddress = 0x100000000;
        private const uint MH_MAGIC_64 = 0xFEEDFACF;
        private const uint LC_SEGMENT_64 = 0x19;
        private const uint LC_SYMTAB = 0x2;
        private const int VM_REGION_BASIC_INFO_64 = 9;
        private const uint VM_REGION_BASIC_INFO_COUNT_64 = 9;
        private const int VM_PROT_READ = 1;

        [DllImport("libSystem.dylib")]
        private static extern int mach_task_self();

        [DllImport("libSystem.dylib")]
        private static extern int task_for_pid(int targetPort, int pid, out int task);

        [DllImport("libSystem.dylib")]
        private static extern int mach_vm_read_overwrite(int task, ulong address, ulong size, IntPtr data, out ulong outSize);

        [DllImport("libSystem.dylib")]
        private static extern int mach_vm_region(int task, ref ulong address, ref ulong size, int flavor, IntPtr info, ref uint infoCount, out int objectName);

        // Returns the address of the PS2's EE RAM inside the PCSX2 process (what Core calls the global offset).
        public static ulong GetEEmemOffset()
        {
            var process = Process.GetProcessesByName(ProcessName).FirstOrDefault()
                ?? throw new InvalidOperationException($"Could not find a running '{ProcessName}' process.");

            int result = task_for_pid(mach_task_self(), process.Id, out int task);
            if (result != 0)
                throw new InvalidOperationException($"task_for_pid failed ({result}). The client has to run as root.");

            ulong loadAddress = FindMainImage(task)
                ?? throw new InvalidOperationException("Could not find the PCSX2 executable in memory.");

            byte[] header = Read(task, loadAddress, 32);
            uint ncmds = BitConverter.ToUInt32(header, 16);
            uint sizeofcmds = BitConverter.ToUInt32(header, 20);
            byte[] commands = Read(task, loadAddress + 32, sizeofcmds);

            ulong textVmAddr = 0, linkeditVmAddr = 0, linkeditFileOff = 0;
            uint symOff = 0, nSyms = 0, strOff = 0, strSize = 0;
            bool foundText = false, foundLinkedit = false, foundSymtab = false;

            int offset = 0;
            for (uint i = 0; i < ncmds; i++)
            {
                uint cmd = BitConverter.ToUInt32(commands, offset);
                uint cmdSize = BitConverter.ToUInt32(commands, offset + 4);

                if (cmd == LC_SEGMENT_64)
                {
                    string name = Encoding.ASCII.GetString(commands, offset + 8, 16).TrimEnd('\0');
                    if (name == "__TEXT")
                    {
                        textVmAddr = BitConverter.ToUInt64(commands, offset + 24);
                        foundText = true;
                    }
                    else if (name == "__LINKEDIT")
                    {
                        linkeditVmAddr = BitConverter.ToUInt64(commands, offset + 24);
                        linkeditFileOff = BitConverter.ToUInt64(commands, offset + 40);
                        foundLinkedit = true;
                    }
                }
                else if (cmd == LC_SYMTAB)
                {
                    symOff = BitConverter.ToUInt32(commands, offset + 8);
                    nSyms = BitConverter.ToUInt32(commands, offset + 12);
                    strOff = BitConverter.ToUInt32(commands, offset + 16);
                    strSize = BitConverter.ToUInt32(commands, offset + 20);
                    foundSymtab = true;
                }

                offset += (int)cmdSize;
            }

            if (!foundText || !foundLinkedit || !foundSymtab)
                throw new InvalidOperationException("The PCSX2 executable has no usable symbol table.");

            // addresses in the file are absolute for the unslid image, so shift them by how far the image actually moved
            ulong slide = loadAddress - textVmAddr;
            ulong linkeditBase = linkeditVmAddr + slide - linkeditFileOff;

            byte[] symbols = Read(task, linkeditBase + symOff, nSyms * 16UL);
            byte[] strings = Read(task, linkeditBase + strOff, strSize);
            byte[] wanted = Encoding.ASCII.GetBytes(EEmemSymbol + "\0");

            for (uint i = 0; i < nSyms; i++)
            {
                int entry = (int)(i * 16);
                uint nStrx = BitConverter.ToUInt32(symbols, entry);
                byte nType = symbols[entry + 4];
                if ((nType & 0x01) == 0 || nStrx + wanted.Length > strings.Length)
                    continue;

                if (!strings.AsSpan((int)nStrx, wanted.Length).SequenceEqual(wanted))
                    continue;

                ulong pointerLocation = BitConverter.ToUInt64(symbols, entry + 8) + slide;
                ulong eeMem = BitConverter.ToUInt64(Read(task, pointerLocation, 8), 0);
                if (eeMem == 0)
                    throw new InvalidOperationException("PCSX2 hasn't allocated the PS2's memory yet. Start the game first.");

                return eeMem;
            }

            throw new InvalidOperationException($"Could not find the '{EEmemSymbol}' symbol in PCSX2.");
        }

        // the main executable is the first mapped image above the 4GB zero page
        private static ulong? FindMainImage(int task)
        {
            IntPtr info = Marshal.AllocHGlobal(64);
            try
            {
                ulong address = FirstUserAddress;
                for (int i = 0; i < 4096; i++)
                {
                    ulong size = 0;
                    uint infoCount = VM_REGION_BASIC_INFO_COUNT_64;
                    if (mach_vm_region(task, ref address, ref size, VM_REGION_BASIC_INFO_64, info, ref infoCount, out _) != 0)
                        return null;

                    if ((Marshal.ReadInt32(info) & VM_PROT_READ) != 0
                        && TryRead(task, address, 4, out byte[] magic)
                        && BitConverter.ToUInt32(magic, 0) == MH_MAGIC_64)
                        return address;

                    address += size;
                }
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(info);
            }
        }

        private static byte[] Read(int task, ulong address, ulong size)
        {
            if (!TryRead(task, address, size, out byte[] data))
                throw new InvalidOperationException($"Could not read 0x{size:X} bytes from PCSX2 at 0x{address:X}.");
            return data;
        }

        private static bool TryRead(int task, ulong address, ulong size, out byte[] data)
        {
            data = new byte[size];
            IntPtr buffer = Marshal.AllocHGlobal((int)size);
            try
            {
                if (mach_vm_read_overwrite(task, address, size, buffer, out ulong read) != 0 || read != size)
                    return false;
                Marshal.Copy(buffer, data, 0, (int)size);
                return true;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}

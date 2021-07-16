using System;
using System.IO;
using System.Text;

namespace unpnesmmw
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("unpnesmmw V1.1 - Unpack the PocketNES Menu Maker Roms");
            Console.WriteLine("BY R-YaTian - github.com/R-YaTian/unpnesmmw");
            if (args != null && args.Length > 0)
            {
                if (args.Length > 1)
                {
                    Console.WriteLine("Error: too many arguments.");
                }
                else
                {
                    if (File.Exists(args[0]))
                        Unpack(args[0]);
                    else
                        Console.WriteLine("Error: File not exists.");
                }
            }
            else
            {
                Console.WriteLine("Please offer the file path or a file name.\nPress any key to exit.");
                //Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
        private static void Unpack(String file_name)
        {
            if (!Directory.Exists("./output"))
                Directory.CreateDirectory("./output");
            byte[] buf;
            byte[] newbuf;
            byte[] fname;
            byte[] header = { 0x4E, 0x45, 0x53, 0x1A };
            var fs = new FileStream(file_name, FileMode.Open);
            buf = new byte[fs.Length];
            fs.Read(buf, 0, (int)fs.Length);
            int offset = 0;
            bool flag = false;
            do
            {
                int fStart = FIndexOf(buf, header, offset);
                int fEnd = FIndexOf(buf, header, fStart + header.Length);
                fEnd -= 0x30;
                if (fStart != -1 && fEnd == -49)
                    fEnd = buf.Length;
                //Console.WriteLine("{0},{1}", fStart, fEnd); Debug out
                if (fStart == -1 || fEnd == -1)
                {
                    flag = true;
                    break;
                }
                newbuf = new byte[fEnd - fStart];
                fname = new byte[0x30];
                Array.Copy(buf, fStart, newbuf, 0, fEnd - fStart);
                Array.Copy(buf, fStart - 0x30, fname, 0, 0x30);
                string fstr = Encoding.Default.GetString(fname);
                int endIndex = fstr.IndexOf("\0");
                fstr = fstr.Substring(0, endIndex);
                Console.WriteLine("Writing \"" + fstr + ".nes\"");
                BinaryWriter bw;
                bw = new BinaryWriter(new FileStream("./output/" + fstr + ".nes", FileMode.Create));
                bw.Write(newbuf);
                bw.Flush();
                bw.Close();
                offset = fEnd;
                offset += 0x30;
            }
            while (offset < buf.Length);
            if (!flag)
                Console.WriteLine("Done! Press any key to exit.");
            else
                Console.WriteLine("Error: wrong file type.\nPress any key to exit.");
            Console.ReadKey();
        }
        private static int FIndexOf(byte[] srcBytes, byte[] searchBytes, int offset = 0)
        {
            if (offset == -1)
                return -1;
            if (srcBytes == null || srcBytes.Length == 0)
                return -1;
            if (searchBytes == null || searchBytes.Length == 0)
                return -1;
            if (srcBytes.Length < searchBytes.Length)
                return -1;
            for (int i = offset; i < srcBytes.Length - searchBytes.Length; i++)
            {
                if (srcBytes[i] != searchBytes[0])
                    continue;
                if (searchBytes.Length == 1)
                    return i;
                bool not_found = false;
                for (var j = 1; j < searchBytes.Length; j++)
                {
                    if (srcBytes[i + j] != searchBytes[j])
                    {
                        not_found = true;
                        break;
                    }
                }
                if (!not_found)
                    return i;
            }
            return -1;
        }
    }
}

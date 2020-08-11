/*
 * 
 * 测试程序，直接Build完成后打开多个文件即可
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestShareMemory
{
    public struct MessageCode
    {
        public int CmdID;
        public int CmdSize;
    }
    class Program
    {
        static void Main(string[] args)
        {
            MemoryData<BinaryReader>.LogAction += (data, type) =>
            {
                Console.WriteLine(data);
            };

            string cmd = "";
            do
            {
                cmd = Console.ReadLine();
                string content = null;
                switch (cmd.ToLower())
                {
                    case "clear":
                        Console.Clear();
                        break;
                    case "write":
                        /*                        Console.Write("Content:");
                                                content = Console.ReadLine();
                                                ShareMemory.Write("Test", content);*/
                        MessageCode codeWrite = new MessageCode() { CmdID = 666, CmdSize = 1024 };
                        ShareMemory.WriteObject("Test", codeWrite);
                        break;
                    case "read":
                        /*                        content = ShareMemory.ReadString("Test");
                                                Console.WriteLine(content);*/
                        MessageCode codeRead = ShareMemory.ReadObject<MessageCode>("Test");
                        Console.WriteLine("codeRead=" + codeRead.CmdID);
                        break;
                }
            } while (cmd.ToLower() != "exit");
        }
    }
}

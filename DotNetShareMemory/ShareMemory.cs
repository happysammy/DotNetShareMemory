using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.Collections;

namespace System.IO
{
    public class ShareMemory
    {

        #region Static Setting

        //部分默认设置


        /// <summary>
        /// 新建映射文件时的默认大小（4096）
        /// </summary>
        public static long MEMORY_DEFAULT_CAPACITY = 4096;

        #endregion

        #region 日志打印相关的内容

        public static Action<object,int> LogAction = null;
        public static string LasetError = string.Empty;

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="data"></param>
        /// <param name="logType">
        /// <para>0 - Log</para>
        /// <para>1 - Error</para>
        /// <para>2 - Warning</para>
        /// </param>
        public static void Log(object data,int logType=0)
        {
            try
            {
                LogAction?.Invoke(data, logType);
            }catch(Exception e)
            {
                LasetError = $"ShareMemory.Log({data},{logType})Fail:{e.Message}";
            }
        }

        #endregion

        /// <summary>
        /// 管理已经创建的映射文件
        /// </summary>
        protected static Dictionary<string, MemoryMappedFile> _MemoryManager = new Dictionary<string, MemoryMappedFile>();

        /// <summary>
        /// 获取一个内存映射
        /// </summary>
        /// <param name="memName"></param>
        /// <returns></returns>
        protected static MemoryMappedFile GetMemeory(string memName)
        {
            MemoryMappedFile mmf = null;
            if (!_MemoryManager.ContainsKey(memName))
            {
                mmf = MemoryMappedFile.CreateNew(memName, MEMORY_DEFAULT_CAPACITY);
                _MemoryManager.Add(memName, mmf);
            }

            return _MemoryManager[memName];
        }

        #region Write

        /// <summary>
        /// 将字符串写入到指定名称的内存映射中
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="info"></param>
        public static void WriteToMemory(string memName,string data)
        {

            MemoryMappedFile mmf = GetMemeory(memName);
            try
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    StreamWriter writer = new StreamWriter(stream);

                    writer.BaseStream.Position = 0;

                    //写入预备类型
                    writer.Write((Int32)0);
                    //写入预备长度
                    writer.Write((long)0);
                    //写入内容
                    writer.Write(data);
                    //获取长度
                    long size = writer.BaseStream.Position;
                    //写入长度
                    writer.BaseStream.Position = 4;
                    writer.Write((long)size);


                    writer.Close();
                }
            }catch (Exception e)
            {
                Log($"ShareMemory.WriteToMemory({data.GetType().Name}):{e.Message}",1);
            }
        }

        /// <summary>
        /// 将byte数据写入到共享区域
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="info"></param>
        public static void WriteToMemory(string memName,byte[] data)
        {
            MemoryMappedFile mmf  = GetMemeory(memName);

            try
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    
                    StreamWriter writer = new StreamWriter(stream);

                    writer.BaseStream.Position = 0;

                    writer.Write((Int32)0);
                    writer.Write((long)0);
                    writer.Write(data);
                    long size = writer.BaseStream.Position;
                    writer.BaseStream.Position = 4;
                    writer.Write((long)size);

                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Log($"ShareMemory.WriteToMemory({data.GetType().Name}):{e.Message}", 1);
            }
        }

        #endregion

        #region Read

        public static string Read(string memName)
        {
            MemoryMappedFile mmf = GetMemeory(memName);

            try
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {

                    StreamReader reader = new StreamReader(stream);
                    char[] Int32Bytes = new char[4];
                    char[] Int64Bytes = new char[8];

                    reader.BaseStream.Position = 4;

                    reader.Read(Int64Bytes, 0, 8);


                    reader.Write((Int32)0);
                    writer.Write((long)0);
                    writer.Write(data);
                    long size = writer.BaseStream.Position;
                    writer.BaseStream.Position = 4;
                    writer.Write((long)size);

                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Log($"ShareMemory.WriteToMemory({data.GetType().Name}):{e.Message}", 1);
            }
        }

        #endregion
    }
}

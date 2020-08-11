using System.Collections.Generic;

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
                if (logType == 1)
                {
                    LasetError = data.ToString();
                }
            }
            catch(Exception e)
            {
                LasetError = $"ShareMemory.Log({data},{logType})Fail:{e.Message}";
            }
        }

        #endregion

        /// <summary>
        /// 管理已经创建的映射文件
        /// </summary>
        protected static Dictionary<string, AdvanceBinaryMemoryData> _MemoryManager = new Dictionary<string, AdvanceBinaryMemoryData>();

        /// <summary>
        /// 获取一个内存映射
        /// </summary>
        /// <param name="memName"></param>
        /// <returns></returns>
        protected static AdvanceBinaryMemoryData GetMemory(string memName)
        {
            AdvanceBinaryMemoryData data = null;
            if (!_MemoryManager.ContainsKey(memName))
            {
                data = new AdvanceBinaryMemoryData(memName, MEMORY_DEFAULT_CAPACITY);
                _MemoryManager.Add(memName, data);
            }

            return _MemoryManager[memName];
        }

        /// <summary>
        /// 删除已保存的内存映射文件
        /// </summary>
        /// <param name="memName"></param>
        protected static void RemoveMemory(string memName)
        {
            if (_MemoryManager.ContainsKey(memName))
            {
                try
                {
                    AdvanceBinaryMemoryData data = _MemoryManager[memName];
                    data.Dispose();
                }
                catch (Exception e)
                {
                    Log($"ShareMemory.RemoveMemory({memName}):{e.Message}", 1);
                }

                _MemoryManager.Remove(memName);
            }
        }

        #region Write

        /// <summary>
        /// 将字符串写入到指定名称的内存映射中
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="info"></param>
        public static void Write(string memName,string data)
        {
            AdvanceBinaryMemoryData brm = GetMemory(memName);
            brm.Write(memName, data);
        }

        /// <summary>
        /// 将一个对象写入指定共享内存
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="obj"></param>
        public static void WriteObject(string memName,object obj)
        {
            AdvanceBinaryMemoryData brm = GetMemory(memName);
            brm.WriteObject(memName, obj);
        }

        /// <summary>
        /// 将byte数据写入到共享区域
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="info"></param>
        public static bool Write(string memName,byte[] data,int dataType = 0)
        {
            AdvanceBinaryMemoryData brm = GetMemory(memName);
            return brm.Write(memName, data,dataType);
        }

        #endregion

        #region Read

        /// <summary>
        /// 读取二进制数据组
        /// </summary>
        /// <param name="memName"></param>
        /// <returns></returns>
        public static byte[] Read(string memName)
        {
            AdvanceBinaryMemoryData brm = GetMemory(memName);
            return brm.Read(memName);
        }

        /// <summary>
        /// 从共享内从中读取字符串
        /// </summary>
        /// <param name="memName"></param>
        /// <returns></returns>
        public static string ReadString(string memName)
        {
            AdvanceBinaryMemoryData brm = GetMemory(memName);
            return brm.ReadString(memName);
        }

        /// <summary>
        /// 读取一个对象
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ReadObject<T>(string memName)
        {
            AdvanceBinaryMemoryData brm = GetMemory(memName);
            return brm.ReadObject<T>(memName);
        }

        #endregion
    }
}

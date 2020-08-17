using System.Text;
using System.Runtime.InteropServices;

namespace System.IO
{
    public class AdvanceBinaryMemoryData : BinaryMemoryData
    {
        protected Encoding _DataEncoding = Encoding.UTF8;

        /// <summary>
        /// 数据编码模式
        /// </summary>
        public Encoding DataEncoding
        {
            get
            {
                return _DataEncoding;
            }
        }

        public AdvanceBinaryMemoryData(string name, long size) : this(name, size, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 通过文字编码构造一个共享内存区域
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="encoding"></param>
        public AdvanceBinaryMemoryData(string name, long size, Encoding encoding) : base(name, size)
        {
            _DataEncoding = encoding;
        }


        #region Write

        /// <summary>
        /// 将字符串写入到指定名称的内存映射中
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="info"></param>
        public virtual void Write(string memName, string data)
        {
            try
            {
                byte[] bs = DataEncoding.GetBytes(data);
                Write(memName, bs);
            }
            catch (Exception e)
            {
                Log($"{_Name}({GetType()}).Write({data.GetType().Name}):{e.Message}", 1);
            }
        }

        /// <summary>
        /// 将一个对象写入指定共享内存
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="obj"></param>
        public virtual bool WriteObject(string memName, object obj)
        {
            int size = Marshal.SizeOf(obj);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(obj, ptr, false);
                Marshal.FreeHGlobal(ptr);
                byte[] data = new byte[size];
                Marshal.Copy(ptr, data, 0, size);
                Write(memName, data);
                return true;
            }
            catch (Exception e)
            {
                Log($"{_Name}({GetType()}).Write({obj.GetType().Name}):{e.Message}", 1);
                return false;
            }
        }

        #endregion

        #region Read

        /// <summary>
        /// 从共享内从中读取一个字符串
        /// </summary>
        /// <param name="memName"></param>
        /// <returns></returns>
        public virtual string ReadString(string memName)
        {
            byte[] data = Read(memName);
            if (data != null && data.Length > 0)
            {
                return DataEncoding.GetString(data);
            }
            else
            {
                Log($"{_Name}({GetType()}).ReadString({memName}):data is null", 2);
            }

            return string.Empty;
        }

        /// <summary>
        /// 读取一个对象
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual T ReadObject<T>(string memName)
        {
            try
            {
                byte[] data = Read(memName);
                IntPtr ptr = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                T t = (T)Marshal.PtrToStructure(ptr, typeof(T));
                Marshal.FreeHGlobal(ptr);
                return t;
            }
            catch (Exception e)
            {
                Log($"{_Name}({GetType()}).ReadObject({typeof(T).Name}):{e.Message}", 1);
                return default(T);
            }
        }

        #endregion


    }
}

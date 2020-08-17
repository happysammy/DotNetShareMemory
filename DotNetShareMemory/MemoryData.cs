using System.IO.MemoryMappedFiles;

namespace System.IO
{
    /// <summary>
    /// 记录共享内存的各类信息
    /// </summary>
    public abstract class MemoryData<T>:IDisposable
    {

        #region Static Setting

        //部分默认设置


        /// <summary>
        /// 新建映射文件时的默认大小（4096）
        /// </summary>
        public static long MEMORY_DEFAULT_CAPACITY = 4096;

        #endregion

        #region 日志打印相关的内容

        public static Action<object, int> LogAction = null;
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
        public static void Log(object data, int logType = 0)
        {
            try
            {
                LogAction?.Invoke(data, logType);
                LasetError = data.ToString();
            }
            catch (Exception e)
            {
                LasetError = $"ShareMemory.Log({data},{logType})Fail:{e.Message}";
            }
        }

        #endregion

        /// <summary>
        /// 当前管理的内存映射文件
        /// </summary>
        protected MemoryMappedFile _MemFile;

        /// <summary>
        /// 当前内存映射文件的最大容量
        /// </summary>
        protected long _Size = MEMORY_DEFAULT_CAPACITY;

        /// <summary>
        /// 内存映射文件的名称
        /// </summary>
        protected string _Name = string.Empty;

        /// <summary>
        /// 当前内存映射文件的最大容量
        /// </summary>
        public long Size
        {
            get
            {
                return _Size;
            }
        }

        /// <summary>
        /// 内存映射文件的名称
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        /// <summary>
        /// 构造一个新的共享内存映射文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        public MemoryData(string name, long size)
        {
            _Size = size;
            _Name = name;
            CreateNew(name, size);
        }

        /// <summary>
        /// 构造一个新的共享内存映射文件（用于更新共享区大小）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        public virtual void CreateNew(string name, long size)
        {
            _MemFile = MemoryMappedFile.CreateOrOpen(name, size);
        }

        /// <summary>
        /// 更新映射文件大小
        /// </summary>
        /// <param name="newSize"></param>
        public virtual void UpdateSize(int newSize)
        {
            try
            {
                lock (_MemFile)
                {
                    Dispose();
                    _Size = newSize;
                    CreateNew(_Name, _Size);
                }
            }
            catch (Exception e)
            {
                Log($"{_Name}({GetType().Name}).UpdateSize({newSize}):{e.Message}", 1);
            }
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public virtual void Dispose()
        {
            if (_MemFile == null || _MemFile.SafeMemoryMappedFileHandle.IsClosed || _MemFile.SafeMemoryMappedFileHandle.IsInvalid)
            {
                Log($"{_Name}({GetType().Name}).Dispose:_MemFile is Closed or Invalid", 1);
            }
            else
            {
                _MemFile.Dispose();
            }

            _MemFile = null;
        }

        #region Write

        /// <summary>
        /// 将数据写入到共享区域
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="info"></param>
        public virtual bool Write(string memName, T data, int dataType = 0)
        {
            return true;
        }

        #endregion

        #region Read

        public virtual T Read(string memName)
        {
            return default(T);
        }

        #endregion

    }
}

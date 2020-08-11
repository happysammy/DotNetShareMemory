using System.Text;
using System.IO.MemoryMappedFiles;

namespace System.IO
{
    public class BinaryMemoryData : MemoryData<byte[]>
    {
        public BinaryMemoryData(string name,long size):base(name,size)
        {
        }

        #region Write

        /// <summary>
        /// 将byte数据写入到共享区域
        /// </summary>
        /// <param name="memName"></param>
        /// <param name="info"></param>
        public override bool Write(string memName, byte[] data, int dataType = 0)
        {
            if(data.Length>_Size)
            {
                UpdateSize(data.Length);
            }

            MemoryMappedFile mmf = _MemFile;
            try
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {

                    BinaryWriter writer = new BinaryWriter(stream);

                    writer.BaseStream.Position = 0;

                    //写入数据类型
                    writer.Write((Int32)dataType);
                    //写入数据准备长度
                    writer.Write((long)0);
                    //写入长度
                    writer.Write(data);
                    //获取数据长度
                    long size = writer.BaseStream.Position - 12;
                    //写入数据长度
                    writer.BaseStream.Position = 4;
                    writer.Write((long)size);

                    writer.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log($"{_Name}({GetType()}).Write({data.GetType().Name}):{e.Message}", 1);
                return false;
            }
        }

        #endregion

        #region Read
        public override byte[] Read(string memName)
        {
            MemoryMappedFile mmf = _MemFile;

            byte[] data = null;
            try
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    reader.BaseStream.Position = 4;
                    long size = reader.ReadInt64();
                    if (size > 0)
                    {
                        data = new byte[size];
                        reader.Read(data, 0, (int)size);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Log($"{_Name}({GetType()}).Read({memName}):{e.Message}", 1);
            }

            return data;
        }
        #endregion


    }
}

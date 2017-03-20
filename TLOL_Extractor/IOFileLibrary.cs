using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLOL_Extractor
{
    public static class IOFileLibrary
    {
        #region Write unpack
        public static void WriteNAME(this MSGFile file, BinaryWriter dataWriter)
        {
            if (file.HasNAME)
            {
                dataWriter.Write(file.NAME);
                dataWriter.Write(file.NAME_BlockSize);
                dataWriter.Write(file.NAME_Bytes, 0, file.NAME_Bytes.Length);
            }
        }

        public static void WriteNPCT(this MSGFile file, BinaryWriter dataWriter)
        {
            if (file.HasNPCT)
            {
                dataWriter.Write(file.NPCT);
                dataWriter.Write(file.NPCT_BlockSize);
                dataWriter.Write(file.NPCT_Bytes, 0, file.NPCT_Bytes.Length);
            }
        }

        public static void WriteTCRC(this MSGFile file, BinaryWriter dataWriter)
        {
            if (file.HasTCRC)
            {
                dataWriter.Write(file.TCRC);
                dataWriter.Write(file.TCRC_BlockSize);
                foreach (var value in file.CRCs.Values.OrderBy(o => o.Checksum))
                {
                    if (value.Checksum == MSGFile.TEXTVALUE)
                        continue;
                    dataWriter.Write(value.Checksum);
                    dataWriter.Write(value.Pointer);
                }
                //dataWriter.Write(file.TEXT);
                //dataWriter.Write(file.TEXT_BlockSize);
            }
        }
        #endregion

        #region Read unpack
        public static bool ReadTCRC(this MSGFile file, BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return false;
            file.TCRC = reader.ReadInt32();
            if (file.TCRC != MSGFile.TCRCVALUE)
            {
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
                return false;
            }
            file.TCRC_BlockSize = reader.ReadInt32();
            for (int i = 0; i < file.TCRC_BlockSize / 8; i++)
            {
                uint checksum = reader.ReadUInt32();
                int pointer = reader.ReadInt32();
                file.CRCs.Add(pointer + 16 + file.TCRC_BlockSize, new TLOL_Extractor.MSGFile.CRC() { Checksum = checksum, Pointer = pointer });
            }
            return true;
        }

        public static bool ReadNPCT(this MSGFile file, BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return false;
            file.NPCT = reader.ReadInt32();
            if (file.NPCT != MSGFile.NPCTVALUE)
            {
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
                return false;
            }
            file.NPCT_BlockSize = reader.ReadInt32();
            file.NPCT_Bytes = reader.ReadBytes(file.NPCT_BlockSize);
            return true;
        }

        public static bool ReadNAME(this MSGFile file, BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return false;
            file.NAME = reader.ReadInt32();
            if (file.NAME != MSGFile.NAMEVALUE)
            {
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
                return false;
            }
            file.NAME_BlockSize = reader.ReadInt32();
            file.NAME_Bytes = reader.ReadBytes(file.NAME_BlockSize);
            return true;
        }
        #endregion

    }
}

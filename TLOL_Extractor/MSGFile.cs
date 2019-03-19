using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLOL_Extractor
{
    public class MSGFile
    {
        public struct CRC
        {
            public uint Checksum;
            public int Pointer;
        };

        public const int TCRCVALUE = 1129464660;
        public const int TEXTVALUE = 1415071060;
        public const int NPCTVALUE = 1413697614;
        public const int NAMEVALUE = 1162690894;

        public bool HasTCRC { get; set; }
        public int TCRC { get; set; }
        public int TCRC_BlockSize { get; set; }
        public Dictionary<int, CRC> CRCs { get; set; }
        public bool HasTEXT { get; set; }
        public int TEXT { get; set; }
        public int TEXT_BlockSize { get; set; }
        public Dictionary<int, char[]> Texts { get; set; }
        public bool HasNPCT { get; set; }
        public int NPCT { get; set; }
        public int NPCT_BlockSize { get; set; }
        public byte[] NPCT_Bytes { get; set; }
        public bool HasNAME { get; set; }
        public int NAME { get; set; }
        public int NAME_BlockSize { get; set; }
        public byte[] NAME_Bytes { get; set; }
        public string Path { get; private set; }

        public MSGFile()
        {
            CRCs = new Dictionary<int, CRC>();
        }

        public static MSGFile ReadMSGFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("The file {0} not exists.", path);
                return null;
            }

            var msgFile = new MSGFile();

            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));

            /************ READ TCTC ************/
            msgFile.HasTCRC = msgFile.ReadTCRC(reader);

            /************ READ TEXT ************/
            if (msgFile.HasTCRC)
            {
                msgFile.TEXT = reader.ReadInt32();
                msgFile.TEXT_BlockSize = reader.ReadInt32();
                msgFile.CRCs.Add(16 + msgFile.TCRC_BlockSize + msgFile.TEXT_BlockSize,
                    new CRC()
                    {
                        Checksum = (uint)msgFile.TEXT,
                        Pointer = msgFile.TEXT_BlockSize
                    });

                msgFile.Texts = new Dictionary<int, char[]>();
                int lastOffset = -1;
                foreach (var pointer in msgFile.CRCs.Keys.OrderBy(o => o))
                {
                    if (lastOffset == -1)
                    {
                        lastOffset = pointer;
                        continue;
                    }
                    reader.BaseStream.Seek(lastOffset, SeekOrigin.Begin);
                    // 15-03-16
                    //char[] text = reader.ReadChars(pointer - lastOffset);
                    List<char> characters = new List<char>();
                    int n = 0;
                    while(n < 2)
                    {
                        char c = reader.ReadChar();
                        if (c == '\0')
                        {
                            characters.Add(c);
                            n++;
                            continue;
                        }
                        n = 0;
                        characters.Add(c);
                    }
                    char[] text = characters.ToArray();
                    // FIN 15-03-16
                    lastOffset = pointer;
                    msgFile.Texts.Add(lastOffset, text);
                }
                msgFile.HasTEXT = true;
            }

            /************ READ NPCT ************/
            msgFile.HasNPCT = msgFile.ReadNPCT(reader);

            /************ READ NAME ************/
            msgFile.HasNAME = msgFile.ReadNAME(reader);

            reader.Close();

            msgFile.Path = path;
            return msgFile;
        }

        public void Unpack()
        {
            string path = this.Path;
            string dataPath = path.Replace(".msg", ".data"),
                textPath = path.Replace(".msg", ".text");
            
            BinaryWriter dataWriter = new BinaryWriter(File.Create(dataPath));
            /************ SAVE TCRC ************/
            this.WriteTCRC(dataWriter);

            /************ SAVE REXT ************/
            if (HasTEXT)
            {
                BinaryWriter textWriter = new BinaryWriter(File.Create(textPath));
                foreach (var key in Texts.Keys.OrderBy(o => o))
                {
                    var aux1 = new string(Texts[key]);
                    aux1 = aux1.Replace("\0", "\n<end>\n") + '\n';
                    var aux2 = aux1.ToCharArray();
                    textWriter.Write(aux2, 0, aux2.Length);
                }
                textWriter.Close();
            }

            /************ SAVE NPCT ************/
            this.WriteNPCT(dataWriter);

            /************ SAVE NAME ************/
            this.WriteNAME(dataWriter);

            dataWriter.Close();
        }

        public static MSGFile ReadRepackFiles(string dataPath, string textPath)
        {
            Console.WriteLine($"Repack {textPath}> Checking .text and .data files...");
            if (!File.Exists(dataPath))
            {
                Console.WriteLine("The file {0} not exists.", dataPath);
                return null;
            }
            if (!File.Exists(textPath))
            {
                Console.WriteLine("The file {0} not exists.", textPath);
                return null;
            }

            MSGFile msgFile = new MSGFile();

            /************ READ TCRC ************/
            Console.WriteLine($"Repack {textPath}> Reading TCRC block...");
            BinaryReader reader = new BinaryReader(File.Open(dataPath, FileMode.Open));
            msgFile.HasTCRC = msgFile.ReadTCRC(reader);

            /************ READ TEXT ************/
            Console.WriteLine($"Repack {textPath}> Reading TEXT block...");
            if (msgFile.HasTCRC)
            {
                msgFile.TEXT = TEXTVALUE;
                // JRH 2018-08-17
                //BinaryReader textReader = new BinaryReader(File.Open(textPath, FileMode.Open), Encoding.Default);
                BinaryReader textReader = new BinaryReader(File.Open(textPath, FileMode.Open), EncodingExtension.GetUTF8WithoutBOM());
                // FIN JRH 
                char[] chars = textReader.ReadChars((int)textReader.BaseStream.Length);
                string text = new string(chars);
                var texts = text.Split(new string[] { "\n<end>\n\n<end>\n\n" }, StringSplitOptions.None);
                var charArrays = texts.Select(s => s.Replace("\n<end>\n", "\0").ToCharArray().Concat(new char[] { '\0', '\0' }).ToArray())
                    .ToArray();

                msgFile.Texts = new Dictionary<int, char[]>();
                CRC[] oldPointers = msgFile.CRCs.OrderBy(o => o.Key).Select(s => s.Value).ToArray();
                msgFile.CRCs.Clear();
                // JRH 2018-08-17
                int newPointers = charArrays.Length - 1;
                if (oldPointers.Length != newPointers)
                    Console.WriteLine($"\tPointer's quanity has changed from {oldPointers.Length} to {newPointers}.");
                msgFile.TCRC_BlockSize = newPointers * 8;
                // FIN JRH 
                int n = 0;
                for (int i = 0; i < newPointers; i++)
                {
                    var txt = charArrays[i];
                    int key = n + 16 + msgFile.TCRC_BlockSize;
                    msgFile.Texts.Add(key, txt);
                    msgFile.CRCs.Add(key, new CRC()
                        {
                            Checksum = oldPointers[i].Checksum,
                            Pointer = n
                        });
                    // JRH 2018-08-17
                    //n += txt.Length;
                    n += txt.Length + txt.Count(c => c == 'ñ' || c == 'á' || c == 'é' || c == 'í' || c == 'ó' || c == 'ú' || c == 'Ñ' || c == 'Á' || c == 'É' || c == 'Í' || c == 'Ó' || c == 'Ú' || c == '¿' || c == '¡');
                    // FIN JRH 
                }
                msgFile.TEXT_BlockSize = n;
                textReader.Close();
                msgFile.HasTEXT = true;
            }

            /************ READ NPCT ************/
            Console.WriteLine($"Repack {textPath}> Reading NPCT block...");
            msgFile.HasNPCT = msgFile.ReadNPCT(reader);

            /************ READ NAME ************/
            Console.WriteLine($"Repack {textPath}> Reading NAME block...");
            msgFile.HasNAME = msgFile.ReadNAME(reader);
            reader.Close();

            msgFile.Path = textPath.Replace(".text", ".msg");
            return msgFile;
        }

        public void Repack()
        {
            string path = this.Path;
            Console.WriteLine($"Repack {path}> Packing MSG file...");
            // JRH 2018-08-17
            //BinaryWriter writer = new BinaryWriter(File.Create(path), Encoding.Default);
            BinaryWriter writer = new BinaryWriter(File.Create(path), Encoding.UTF8);
            // FIN JRH 
            /************ SAVE TCRC ************/
            this.WriteTCRC(writer);

            /************ SAVE REXT ************/
            if (HasTEXT)
            {
                writer.Write(TEXT);
                writer.Write(TEXT_BlockSize);
                foreach (var key in Texts.Keys.OrderBy(o => o))
                {
                    writer.Write(Texts[key], 0, Texts[key].Length);
                }
            }

            /************ SAVE NPCT ************/
            this.WriteNPCT(writer);

            /************ SAVE NAME ************/
            this.WriteNAME(writer);

            writer.Close();
        }
    }
}

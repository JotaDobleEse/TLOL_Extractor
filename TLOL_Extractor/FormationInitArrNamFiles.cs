using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TLOL_Extractor
{
    public class FormationInitArrNamFiles
    {
        public List<char[]> Texts { get; private set; }
        public List<byte> Pointers { get; private set; }
        public List<byte[]> Blocks { get; private set; }
        public string Path { get; private set; }

        public FormationInitArrNamFiles()
        {
            Texts = new List<char[]>();
            Pointers = new List<byte>();
            Blocks = new List<byte[]>();
        }

        public static FormationInitArrNamFiles ReadFiles(string arrPath, string namPath)
        {
            FormationInitArrNamFiles arrNamFile = new FormationInitArrNamFiles();

            if (!File.Exists(arrPath))
            {
                Console.WriteLine("The file {0} not exists.", arrPath);
                return null;
            }
            if (!File.Exists(namPath))
            {
                Console.WriteLine("The file {0} not exists.", namPath);
                return null;
            }

            using (BinaryReader arrReader = new BinaryReader(File.Open(arrPath, FileMode.Open)))
            {

                for (int i = 0; i < arrReader.BaseStream.Length; i += 0x6)
                {
                    byte pointer = arrReader.ReadByte();
                    arrNamFile.Pointers.Add(pointer);

                    arrNamFile.Blocks.Add(arrReader.ReadBytes(0x5));
                }

            }

            using (BinaryReader namReader = new BinaryReader(File.Open(namPath, FileMode.Open), Encoding.Default))
            {
                arrNamFile.Texts.Add(new char[] { '\0' });
                for (int i = 0; i < arrNamFile.Pointers.Count; i++)
                {
                    byte pointer = arrNamFile.Pointers[i];
                    List<char> charArray = new List<char>();
                    int p;
                    if (pointer == 0)
                    {
                        charArray.AddRange("<ENDBLOCK>".ToCharArray());
                    }
                    else if ((p = arrNamFile.Pointers.IndexOf(pointer)) < i)
                    {
                        charArray.AddRange(string.Format("<{0}>", p).ToCharArray());
                    }
                    else
                    {
                        namReader.BaseStream.Seek(pointer, SeekOrigin.Begin);
                        char c;
                        while ((c = namReader.ReadChar()) != '\0')
                        {
                            charArray.Add(c);
                        }
                    }
                    charArray.Add('\0');
                    arrNamFile.Texts.Add(charArray.ToArray());
                }
            }

            arrNamFile.Path = arrPath;
            return arrNamFile;
        }

        public void Extract()
        {
            string path = Path.Replace(".arr", ".text"),
                dataPath = Path.Replace(".arr", ".data");

            using (BinaryWriter writer = new BinaryWriter(File.Create(path), EncodingExtension.GetUTF8WithoutBOM()))
            {
                foreach (var text in Texts)
                {
                    var aux1 = new string(text);
                    aux1 = aux1.Replace("\0", "\n<end>\n") + '\n';
                    var aux2 = aux1.ToCharArray();
                    writer.Write(aux2, 0, aux2.Length);
                }
            }

            using (BinaryWriter writer = new BinaryWriter(File.Create(dataPath)))
            {
                foreach (var block in Blocks)
                {
                    writer.Write((byte)block.Length);
                    writer.Write(block, 0, block.Length);
                }
            }
        }

        public static FormationInitArrNamFiles ReadDataTextFiles(string dataPath, string textPath)
        {
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

            FormationInitArrNamFiles arrNamFile = new FormationInitArrNamFiles();

            using (BinaryReader dataReader = new BinaryReader(File.Open(dataPath, FileMode.Open)))
            {
                while (dataReader.BaseStream.Position < dataReader.BaseStream.Length)
                {
                    byte length = dataReader.ReadByte();
                    arrNamFile.Blocks.Add(dataReader.ReadBytes(length));
                }
            }

            using (BinaryReader textReader = new BinaryReader(File.Open(textPath, FileMode.Open), Encoding.Default))
            {
                char[] chars = textReader.ReadChars((int)textReader.BaseStream.Length);
                string text = new string(chars).ReplaceSpanishChars();
                var texts = text.Split(new string[] { "\n<end>\n\n" }, StringSplitOptions.None).Reverse()
                    .Skip(1).Reverse().ToArray();

                byte length = 0;
                foreach (var txt in texts)
                {
                    if (txt == "<ENDBLOCK>")
                    {
                        arrNamFile.Pointers.Add(0);
                    }
                    else if (Regex.IsMatch(txt, "<\\d+>"))
                    {
                        var index = ushort.Parse(txt.Replace("<", "").Replace(">", ""));
                        arrNamFile.Pointers.Add(arrNamFile.Pointers[index + 1]);
                    }
                    else
                    {
                        arrNamFile.Pointers.Add(length);
                        arrNamFile.Texts.Add(txt.ToCharArray().Concat(new char[] { '\0' }).ToArray());
                        length += (byte)(txt.Length + 1);
                    }
                }
            }
            arrNamFile.Path = textPath;
            return arrNamFile;
        }

        public void WriteArrNamFiles()
        {
            string arrPath = Path.Replace(".text", ".arr"),
                namPath = Path.Replace(".text", ".nam");

            using (BinaryWriter writer = new BinaryWriter(File.Create(arrPath)))
            {
                int i = 1;
                foreach (var block in Blocks)
                {
                    writer.Write(Pointers[i++]);
                    writer.Write(block, 0, block.Length);
                }
            }

            using (BinaryWriter writer = new BinaryWriter(File.Create(namPath), Encoding.Default))
            {
                foreach (var text in Texts)
                {
                    writer.Write(text, 0, text.Length);
                }
            }
        }
    }
}

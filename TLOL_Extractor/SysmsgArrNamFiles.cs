using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TLOL_Extractor
{
    public class SysmsgArrNamFiles
    {
        public List<char[]> Texts { get; private set; }
        public List<ushort> Pointers { get; private set; }
        public string Path { get; private set; }

        public SysmsgArrNamFiles()
        {
            Texts = new List<char[]>();
            Pointers = new List<ushort>();
        }

        public static SysmsgArrNamFiles ReadFiles(string arrPath, string namPath)
        {
            SysmsgArrNamFiles arrNamFile = new SysmsgArrNamFiles();

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
                using (BinaryReader namReader = new BinaryReader(File.Open(namPath, FileMode.Open)))
                {
                    //ushort lastPointer = 0;
                    arrNamFile.Texts.Add(new char[] { '\0' });
                    do
                    {
                        var pointer = arrReader.ReadUInt16();
                        if (pointer == 0)
                            arrNamFile.Texts.Add("<ENDBLOCK>\0".ToCharArray());
                        else if (arrNamFile.Pointers.Contains(pointer))
                            arrNamFile.Texts.Add(string.Format("<{0}>\0", arrNamFile.Pointers.IndexOf(pointer)).ToCharArray());
                        else
                        {
                            if (pointer < namReader.BaseStream.Length)
                            {
                                namReader.BaseStream.Seek(pointer, SeekOrigin.Begin);
                                List<char> block = new List<char>();
                                char c;
                                while ((c = namReader.ReadChar()) != '\0')
                                {
                                    block.Add(c);
                                    if (namReader.BaseStream.Length <= namReader.BaseStream.Position)
                                        break;
                                }
                                block.Add('\0');
                                arrNamFile.Texts.Add(block.ToArray());
                            }
                        }
                        arrNamFile.Pointers.Add(pointer);

                    } while (arrReader.BaseStream.Position != arrReader.BaseStream.Length);
                }
            }

            arrNamFile.Path = arrPath;
            return arrNamFile;
        }

        public void Extract()
        {
            string path = Path.Replace(".arr", ".text");

            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                foreach (var text in Texts)
                {
                    var aux1 = new string(text);
                    aux1 = aux1.Replace("\0", "\n<end>\n") + '\n';
                    var aux2 = aux1.ToCharArray();
                    writer.Write(aux2, 0, aux2.Length);
                }
            }
        }

        public static SysmsgArrNamFiles ReadTextFile(string path)
        {
            SysmsgArrNamFiles arrNamFile = new SysmsgArrNamFiles();

            if (!File.Exists(path))
            {
                Console.WriteLine("The file {0} not exists.", path);
                return null;
            }

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open), Encoding.Default))
            {
                char[] chars = reader.ReadChars((int)reader.BaseStream.Length);
                string text = new string(chars).ReplaceSpanishChars();
                var texts = text.Split(new string[] { "\n<end>\n\n" }, StringSplitOptions.None).Reverse()
                    .Skip(1).Reverse().ToArray();

                ushort length = 0;
                arrNamFile.Texts.Add(texts[0].ToCharArray().Concat(new char[] { '\0' }).ToArray());
                length += (ushort)(texts[0].Length + 1);
                foreach (var txt in texts.Skip(1))
                {
                    if (txt.Contains('<'))
                    {
                        if (Regex.IsMatch(txt, "<\\d+>"))
                        {
                            var index = ushort.Parse(txt.Replace("<", "").Replace(">", ""));
                            arrNamFile.Pointers.Add(arrNamFile.Pointers[index]);
                            //arrNamFile.Texts.Add(texts[index].ToCharArray().Concat(new char[] { '\0' }).ToArray());
                        }
                        else if (txt == "<ENDBLOCK>")
                        {
                            arrNamFile.Pointers.Add(0);
                        }
                    }
                    else
                    {
                        if (txt != texts.Last())
                            arrNamFile.Pointers.Add(length);
                        arrNamFile.Texts.Add(txt.ToCharArray().Concat(new char[] { '\0' }).ToArray());
                        length += (ushort)(txt.Length + 1);
                    }
                }
                //var charArrays = texts.Select(s => s.ToCharArray().Concat(new char[] { '\0' }).ToArray())
                //    .ToArray();
            }

            arrNamFile.Path = path;
            return arrNamFile;
        }

        public void WriteArrNamFiles()
        {
            string arrPath = Path.Replace(".text", ".arr"),
                namPath = Path.Replace(".text", ".nam");

            using (BinaryWriter writer = new BinaryWriter(File.Create(arrPath)))
            {
                foreach (var pointer in Pointers)
                {
                    writer.Write(pointer);
                }
            }

            using (BinaryWriter writer = new BinaryWriter(File.Create(namPath),Encoding.Default))
            {
                foreach (var text in Texts)
                {
                    writer.Write(text,0,text.Length);
                }
            }
        }
    }
}

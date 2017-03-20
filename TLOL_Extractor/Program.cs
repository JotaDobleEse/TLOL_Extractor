using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLOL_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            //MSGFile.ReadRepackFiles("_d01_gardn.data", "_d01_gardn.text").Repack();
            //MapInfoArrNamFiles.ReadFiles("map_info.arr", "map_info.nam").Extract();
            //MapInfoArrNamFiles.ReadDataTextFiles("_map_info.data", "_map_info.text").WriteArrNamFiles();

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("----------- THE LEGEND OF LEGACY -----------");
            Console.WriteLine("-----------     TEXT EDITOR      -----------");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("1 - Unpack .msg file");
            Console.WriteLine("2 - Repack .msg file");
            Console.WriteLine("3 - Unpack .arr .nam files (sysmsg_message)");
            Console.WriteLine("4 - Repack .arr .nam files (sysmsg_message)");
            Console.WriteLine("5 - Unpack .arr .nam files (itemlist)");
            Console.WriteLine("6 - Repack .arr .nam files (itemlist)");
            Console.WriteLine("7 - Unpack .arr .nam files (arts_data)");
            Console.WriteLine("8 - Repack .arr .nam files (arts_data)");
            Console.WriteLine("9 - Unpack .arr .nam files (ene_monster and ene_monster_setumei)");
            Console.WriteLine("10 - Repack .arr .nam files (ene_monster and ene_monster_setumei)");
            Console.WriteLine("11 - Unpack .arr .nam files (formation_init)");
            Console.WriteLine("12 - Repack .arr .nam files (formation_init)");
            Console.WriteLine("13 - Unpack .arr .nam files (map_pos)");
            Console.WriteLine("14 - Repack .arr .nam files (map_pos)");
            Console.WriteLine("15 - Unpack .arr .nam files (map_info)");
            Console.WriteLine("16 - Repack .arr .nam files (map_info)");
            Console.WriteLine("17 - Unpack all .msg files (execute into xls directory or subdirectories)");
            Console.WriteLine();
            Console.Write("Option: ");
            try
            {
                int option = int.Parse(Console.ReadLine());
                switch(option)
                {
                    case 1:
                        UnpackMsgFile();
                        break;
                    case 2:
                        RepackMsgFile();
                        break;
                    case 3:
                        UnpackSysmsgArrNamFiles();
                        break;
                    case 4:
                        RepackSysmsgArrNamFiles();
                        break;
                    case 5:
                        UnpackItemlistArrNamFiles();
                        break;
                    case 6:
                        RepackItemlistArrNamFiles();
                        break;
                    case 7:
                        UnpackArtsdataArrNamFiles();
                        break;
                    case 8:
                        RepackArtsdataArrNamFiles();
                        break;
                    case 9:
                        UnpackEneMonsterArrNamFiles();
                        break;
                    case 10:
                        RepackEneMonsterArrNamFiles();
                        break;
                    case 11:
                        UnpackFormationInitArrNamFiles();
                        break;
                    case 12:
                        RepackFormationInitArrNamFiles();
                        break;
                    case 13:
                        UnpackMapPosArrNamFiles();
                        break;
                    case 14:
                        RepackMapPosArrNamFiles();
                        break;
                    case 15:
                        UnpackMapInfoArrNamFiles();
                        break;
                    case 16:
                        RepackMapInfoArrNamFiles();
                        break;
                    case 17:
                        UnpackAllMsgFiles();
                        break;
                }
            } 
            catch (Exception)
            {
                Console.WriteLine("Invalid option");
            }
            Console.Write("Press enter to continue...");
            Console.ReadLine();

        }

        private static void RepackMapInfoArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of map_info .data and .text files (without filenames): ");
            string textPath = Console.ReadLine();
            if (string.IsNullOrEmpty(textPath))
                textPath = ".";
            string dataPath = textPath + "\\map_info.data";
            textPath += "\\map_info.text";
            MapInfoArrNamFiles.ReadDataTextFiles(dataPath, textPath).WriteArrNamFiles();
        }

        private static void UnpackMapInfoArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of map_info .arr and .nam files (without filenames): ");
            string arrPath = Console.ReadLine();
            if (string.IsNullOrEmpty(arrPath))
                arrPath = ".";
            string namPath = arrPath + "\\map_info.nam";
            arrPath += "\\map_info.arr";
            MapInfoArrNamFiles.ReadFiles(arrPath, namPath).Extract();
        }

        private static void RepackMapPosArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of map_pos .data and .text files (without filenames): ");
            string textPath = Console.ReadLine();
            if (string.IsNullOrEmpty(textPath))
                textPath = ".";
            string dataPath = textPath + "\\map_pos.data";
            textPath += "\\map_pos.text";
            MapPosArrNamFiles.ReadDataTextFiles(dataPath, textPath).WriteArrNamFiles();
        }

        private static void UnpackMapPosArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of map_pos .arr and .nam files (without filenames): ");
            string arrPath = Console.ReadLine();
            if (string.IsNullOrEmpty(arrPath))
                arrPath = ".";
            string namPath = arrPath + "\\map_pos.nam";
            arrPath += "\\map_pos.arr";
            MapPosArrNamFiles.ReadFiles(arrPath, namPath).Extract();
        }

        private static void RepackFormationInitArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of formation_init .data and .text files (without filenames): ");
            string textPath = Console.ReadLine();
            if (string.IsNullOrEmpty(textPath))
                textPath = ".";
            string dataPath = textPath + "\\formation_init.data";
            textPath += "\\formation_init.text";
            FormationInitArrNamFiles.ReadDataTextFiles(dataPath, textPath).WriteArrNamFiles();
        }

        private static void UnpackFormationInitArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of formation_init .arr and .nam files (without filenames): ");
            string arrPath = Console.ReadLine();
            if (string.IsNullOrEmpty(arrPath))
                arrPath = ".";
            string namPath = arrPath + "\\formation_init.nam";
            arrPath += "\\formation_init.arr";
            FormationInitArrNamFiles.ReadFiles(arrPath, namPath).Extract();
        }

        private static void RepackEneMonsterArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of ene_monster .data and .text files (without filenames): ");
            string textPath = Console.ReadLine();
            if (string.IsNullOrEmpty(textPath))
                textPath = ".";
            string dataPath = textPath + "\\ene_monster.data";
            textPath += "\\ene_monster.text";
            EneMonsterArrNamFiles.ReadDataTextFiles(dataPath, textPath).WriteArrNamFiles();
        }

        private static void UnpackEneMonsterArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of ene_monster .arr and .nam files, and ene_monster_setumei file (without filenames): ");
            string arrPath = Console.ReadLine();
            if (string.IsNullOrEmpty(arrPath))
                arrPath = ".";
            string namPath = arrPath + "\\ene_monster.nam";
            string namSetumeiPath = arrPath + "\\ene_monster_setumei.nam";
            arrPath += "\\ene_monster.arr";
            EneMonsterArrNamFiles.ReadFiles(arrPath, namPath, namSetumeiPath).Extract();
        }

        private static void RepackArtsdataArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of itemlist .data and .text files (without filenames): ");
            string textPath = Console.ReadLine();
            if (string.IsNullOrEmpty(textPath))
                textPath = ".";
            string dataPath = textPath + "\\arts_data.data";
            textPath += "\\arts_data.text";
            ArtsdataArrNamFiles.ReadDataTextFiles(dataPath, textPath).WriteArrNamFiles();
        }

        private static void UnpackArtsdataArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of itemlist .arr and .nam files (without filenames): ");
            string arrPath = Console.ReadLine();
            if (string.IsNullOrEmpty(arrPath))
                arrPath = ".";
            string namPath = arrPath + "\\arts_data.nam";
            arrPath += "\\arts_data.arr";
            ArtsdataArrNamFiles.ReadFiles(arrPath, namPath).Extract();
        }

        private static void RepackItemlistArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of itemlist .data and .text files (without filenames): ");
            string textPath = Console.ReadLine();
            if (string.IsNullOrEmpty(textPath))
                textPath = ".";
            string dataPath = textPath + "\\itemlist.data";
            textPath += "\\itemlist.text";
            ItemlistArrNamFiles.ReadDataTextFiles(dataPath, textPath).WriteArrNamFiles();
        }

        private static void UnpackItemlistArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of itemlist .arr and .nam files (without filenames): ");
            string arrPath = Console.ReadLine();
            if (string.IsNullOrEmpty(arrPath))
                arrPath = ".";
            string namPath = arrPath + "\\itemlist.nam";
            arrPath += "\\itemlist.arr";
            ItemlistArrNamFiles.ReadFiles(arrPath, namPath).Extract();
        }

        private static void RepackSysmsgArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of .text file (without filename): ");
            string textPath = Console.ReadLine();
            if (string.IsNullOrEmpty(textPath))
                textPath = ".";
            textPath += "\\sysmsg_message.text";
            SysmsgArrNamFiles.ReadTextFile(textPath).WriteArrNamFiles();
        }

        private static void UnpackSysmsgArrNamFiles()
        {
            Console.Clear();
            Console.Write("Insert path of sysmsg_message .arr and .nam files (without filenames): ");
            string arrPath = Console.ReadLine();
            if (string.IsNullOrEmpty(arrPath))
                arrPath = ".";
            string namPath = arrPath + "\\sysmsg_message.nam";
            arrPath += "\\sysmsg_message.arr";
            SysmsgArrNamFiles.ReadFiles(arrPath, namPath).Extract();
        }

        private static void RepackMsgFile()
        {
            Console.Clear();
            Console.Write("Insert path of .data and .text (without file extension): ");
            string dataPath = Console.ReadLine();
            //Console.Write("Insert path of .text: ");
            //string textPath = Console.ReadLine();
            string textPath = dataPath + ".text";
            dataPath += ".data";
            MSGFile.ReadRepackFiles(dataPath, textPath).Repack();
        }

        private static void UnpackMsgFile()
        {
            Console.Clear();
            Console.Write("Insert path of .msg file: ");
            string path = Console.ReadLine();
            MSGFile.ReadMSGFile(path).Unpack();
        }

        private static void UnpackAllMsgFiles()
        {
            Console.Clear();
            var files = Directory.EnumerateFiles(".", "*.msg", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                MSGFile.ReadMSGFile(file).Unpack();
                Console.WriteLine("{0} file unpacked", file);
            }
        }
    }
}

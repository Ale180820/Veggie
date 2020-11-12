using System;
using System.Collections.Generic;
using System.Text;

namespace VeggieBack.Controllers {
    public class LZWCompressor: ICompressor {

        /// <summary>
        /// Method that will call the interface method
        /// </summary>
        /// <param name="file"> File sent (.txt) </param>
        /// <param name="routeDirectory"> Current directory path </param>
        public void CompressFile(IFormFile file, string routeDirectory)
        {
            Compress(file, routeDirectory);
        }

        /// <summary>
        /// Method that will call the interface method
        /// </summary>
        /// <param name="file"> File sent (.lzw)</param>
        /// <param name="routeDirectory">Current directory path</param>
        /// <returns> Returns compressed element </returns>
        public string DecompressFile(IFormFile file, string routeDirectory)
        {
            return Decompress(file, routeDirectory);
        }

        /// <summary>
        /// Method for compressing the file
        /// </summary>
        /// <param name="file"> File sent (.txt) </param>
        /// <param name="routeDirectory"> Current directory path </param>
        public void Compress(IFormFile file, string routeDirectory)
        {

            var bufferLenght = 1000;
            var dictionaryOfLetters = new Dictionary<string, string>();
            var bbfWriting = new List<byte>();
            var characterList = new List<string>();
            var byteBuffer = new byte[bufferLenght];
            var aux = string.Empty;
            var letter = string.Empty;
            var lastLetter = string.Empty;

            if (!Directory.Exists(Path.Combine(routeDirectory, "compress")))
            {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "compress"));
            }

            using (var readerFile = new BinaryReader(file.OpenReadStream()))
            {
                using (var stream = new FileStream(Path.Combine(routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.lzw"), FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        while (readerFile.BaseStream.Position != readerFile.BaseStream.Length)
                        {
                            byteBuffer = readerFile.ReadBytes(bufferLenght);
                            for (int i = 0; i < byteBuffer.Count(); i++)
                            {
                                letter = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                                if (!dictionaryOfLetters.ContainsKey(letter))
                                {
                                    var number = Convert.ToString(dictionaryOfLetters.Count() + 1, 2);
                                    dictionaryOfLetters.Add(letter, number);
                                    letter = string.Empty;
                                }
                            }
                        }

                        writer.Write(Encoding.ASCII.GetBytes(Convert.ToString(dictionaryOfLetters.Count).PadLeft(8, '0').ToCharArray()));

                        foreach (var rowList in dictionaryOfLetters)
                        {
                            writer.Write(Convert.ToByte(Convert.ToChar(rowList.Key[0])));
                        }

                        readerFile.BaseStream.Position = 0;
                        letter = string.Empty;

                        while (readerFile.BaseStream.Position != readerFile.BaseStream.Length)
                        {
                            byteBuffer = readerFile.ReadBytes(bufferLenght);
                            for (int i = 0; i < byteBuffer.Count(); i++)
                            {

                                letter += Convert.ToString(Convert.ToChar(byteBuffer[i]));
                                if (!dictionaryOfLetters.ContainsKey(letter))
                                {
                                    var num = Convert.ToString(dictionaryOfLetters.Count() + 1, 2);
                                    dictionaryOfLetters.Add(letter, num);
                                    characterList.Add(dictionaryOfLetters[lastLetter]);
                                    lastLetter = string.Empty;
                                    lastLetter += letter.Last();
                                    letter = lastLetter;

                                }
                                else
                                {
                                    lastLetter = letter;
                                }
                            }
                        }

                        characterList.Add(dictionaryOfLetters[letter]);

                        var mBites = Math.Log2((float)dictionaryOfLetters.Count);
                        mBites = mBites % 1 >= 0.5 ? Convert.ToInt32(mBites) : Convert.ToInt32(mBites) + 1;

                        writer.Write(Convert.ToByte(mBites));

                        for (int i = 0; i < characterList.Count; i++)
                        {
                            characterList[i] = characterList[i].PadLeft(Convert.ToInt32(mBites), '0');
                        }

                        foreach (var character in characterList)
                        {
                            aux += character;
                            if (aux.Length >= 8)
                            {
                                var max = aux.Length / 8;
                                for (int i = 0; i < max; i++)
                                {
                                    bbfWriting.Add(Convert.ToByte(Convert.ToInt32(aux.Substring(0, 8), 2)));
                                    aux = aux.Substring(8);
                                }
                            }
                        }

                        if (aux.Length != 0)
                        {
                            bbfWriting.Add(Convert.ToByte(Convert.ToInt32(aux.PadRight(8, '0'), 2)));
                        }

                        writer.Write(bbfWriting.ToArray());

                    }
                }
            }
        }

        /// <summary>
        /// Método for descompressing the file
        /// </summary>
        /// <param name="file"> File sent (.huff)</param>
        /// <param name="routeDirectory"> Current directory path </param>
        /// <returns></returns>
        public string Decompress(IFormFile file, string routeDirectory)
        {

            var dictionaryOfLetters = new Dictionary<int, string>();
            var bbfLenght = 10000;
            var byteBff = new byte[bbfLenght];
            var bbfWriting = new List<byte>();
            var auxPrevious = string.Empty;
            var auxPrevio = string.Empty;
            var aux = string.Empty;
            var first = true;


            if (!Directory.Exists(Path.Combine(routeDirectory, "decompress")))
            {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "decompress"));
            }

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "decompress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(streamWriter))
                    {

                        byteBff = reader.ReadBytes(8);
                        var CantDiccionario = Convert.ToInt32(Encoding.UTF8.GetString(byteBff));
                        for (int i = 0; i < CantDiccionario; i++)
                        {
                            byteBff = reader.ReadBytes(1);
                            var letter = Convert.ToChar(byteBff[0]).ToString();
                            dictionaryOfLetters.Add(dictionaryOfLetters.Count() + 1, letter);
                        }

                        byteBff = reader.ReadBytes(1);
                        var numberOfBits = Convert.ToInt32(byteBff[0]);

                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBff = reader.ReadBytes(bbfLenght);
                            foreach (var item in byteBff)
                            {
                                aux += Convert.ToString(item, 2).PadLeft(8, '0'); ;
                                while (aux.Length >= numberOfBits)
                                {
                                    var number = Convert.ToInt32(aux.Substring(0, numberOfBits), 2);
                                    if (number != 0)
                                    {
                                        if (first)
                                        {
                                            first = false;
                                            auxPrevio = dictionaryOfLetters[number];
                                            bbfWriting.Add(Convert.ToByte(Convert.ToChar(auxPrevio)));
                                        }
                                        else
                                        {
                                            if (number > dictionaryOfLetters.Count)
                                            {
                                                auxPrevious = auxPrevio + auxPrevio.First();
                                                dictionaryOfLetters.Add(dictionaryOfLetters.Count + 1, auxPrevious);
                                            }
                                            else
                                            {
                                                auxPrevious = dictionaryOfLetters[number];
                                                dictionaryOfLetters.Add(dictionaryOfLetters.Count + 1, $"{auxPrevio}{auxPrevious.First()}");
                                            }

                                            foreach (var letter in auxPrevious)
                                            {
                                                bbfWriting.Add(Convert.ToByte(letter));
                                            }

                                            auxPrevio = auxPrevious;
                                        }
                                    }
                                    aux = aux.Substring(numberOfBits);
                                }
                            }
                        }
                        writer.Write(bbfWriting.ToArray());
                    }
                }
            }
            return Path.Combine(routeDirectory, "decompress", Path.GetFileNameWithoutExtension(file.FileName));
        }
    }
}

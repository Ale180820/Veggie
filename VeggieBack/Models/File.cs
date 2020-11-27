using System;

namespace VeggieBack.Models {
    public class File {

        public static int codeFile = 0;
        public int _id { get; set; }
        public string fileName { get; set; }
        public string compressedFilePath { get; set; }

        public File() {
            var rand = new Random();
            codeFile = codeFile + 100 + rand.Next(0, 100000);
            this._id = codeFile;
        }
    }
}

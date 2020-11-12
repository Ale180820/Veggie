using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeggieBack.Interfaces {

    /// <summary>
    /// Compressor interface
    /// </summary>
    public interface ICompressor {

        /// <summary>
        /// Method for compresssing the file
        /// </summary>
        /// <param name="file"> File sent (.txt) </param>
        /// <param name="routeDirectory"> Current directory path </param>
        void Compress(IFormFile file, string routeDirectory);

        /// <summary>
        /// Method for decompressing the file
        /// </summary>
        /// <param name="file"> File sent (.huff) </param>
        /// <param name="routeDirectory"> Current directory path </param>
        /// <returns></returns>
        string Decompress(IFormFile file, string routeDirectory);

    }
}

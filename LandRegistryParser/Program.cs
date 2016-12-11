using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using LandRegistryParser;


namespace HtmlParser
{
    class Program
    {
        private const decimal KeyOffset = 59.53M;
        private const decimal ValueOffset = 201.26M;
        private const decimal DelimeterOffset = 168.5M;

        static void Main(string[] args)
        {
            var model = Converter.Convert("..\\..\\src.pdf");
            var result = Parser.Parse(model,KeyOffset,ValueOffset,DelimeterOffset);
        }
    }
}

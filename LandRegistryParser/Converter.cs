using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using LandRegistryParser.Models;
using pdftron;
using pdftron.PDF;

namespace LandRegistryParser
{
    public class Converter
    {
        public static Pdf Convert(string path, bool savePdfStructureAsXml = false, string xmlName = "pdf.xml")
        {
            PDFNet.Initialize();
            using (var doc = new PDFDoc(path))
            {
                var result = new XDocument(new XElement("Pdf"));

                for (int i = 1; i <= doc.GetPageCount(); i++)
                {
                    var page = doc.GetPage(i);
                    using (var txt = new TextExtractor())
                    {
                        txt.Begin(page);
                        var text = txt.GetAsXML(TextExtractor.XMLOutputFlags.e_words_as_elements | TextExtractor.XMLOutputFlags.e_output_bbox);

                        //combine words within a line (we don't need their position)
                        var xml = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(text)));
                        var lines = xml.Root.DescendantsAndSelf().Where(s => s.Name == "Line").ToArray();
                        foreach (var line in lines)
                        {
                            var t = String.Join(" ", line.DescendantsAndSelf("Word").Select(s => s.Value));
                            line.RemoveNodes();
                            line.SetValue(t);
                        }
                        result.Root.Add(xml.Root);
                    }
                }

                //save the temporary xml just for debug purposes
                if (savePdfStructureAsXml)
                {
                    var destinationPath = Path.GetDirectoryName(path) + xmlName;
                    using (var writer = new XmlTextWriter(destinationPath, null))
                    {
                        writer.Formatting = Formatting.Indented;
                        result.Save(writer);
                    }
                }

                using (var ms = new MemoryStream())
                {
                    result.Save(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    var serializer = new XmlSerializer(typeof(Pdf));
                    return (Pdf)serializer.Deserialize(ms);
                }
            }
        }
    }
}

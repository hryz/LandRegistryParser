using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace LandRegistryParser.Models
{
    [XmlRoot("Pdf")]
    public class Pdf
    {
        [XmlElement("Page")]
        public List<Page> Pages { get; set; }

        public void Init()
        {
            foreach (var page in Pages)
            {
                page.Init();
            }
        }
    }

    public class Page
    {
        [XmlAttribute("num")]
        public int Number { get; set; }

        [XmlAttribute("crop_box")]
        public string CropBox { get; set; }

        [XmlAttribute("media_box")]
        public string MediaBox { get; set; }

        [XmlAttribute("rotate")]
        public int Rotate { get; set; }

        [XmlElement("Flow")]
        public List<Flow> Flows { get; set; }

        public Rectangle CropRect { get; set; }

        public Rectangle MediaRect { get; set; }

        public void Init()
        {
            CropRect = new Rectangle(CropBox);
            MediaRect = new Rectangle(MediaBox);
            foreach (var flow in Flows)
            {
                flow.Init();
                flow.ParentPage = this;
            }
        }
    }

    public class Flow
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("Para")]
        public List<Paragraph> Paragraphs { get; set; }

        public Page ParentPage { get; set; }

        public void Init()
        {
            foreach (var paragraph in Paragraphs)
            {
                paragraph.Init();
                paragraph.ParentFlow = this;
            }
        }
    }

    public class Paragraph
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("Line")]
        public List<Line> Lines { get; set; }

        public Flow ParentFlow { get; set; }

        public void Init()
        {
            foreach (var line in Lines)
            {
                line.Init();
                line.ParentParagraph = this;
            }
        }

        public override string ToString() => String.Join("|", Lines);
    }

    public class Line
    {
        [XmlAttribute("box")]
        public string Box { get; set; }

        [XmlText]
        public string Text { get; set; }

        public Rectangle Rect { get; set; }

        public Paragraph ParentParagraph { get; set; }

        public void Init()
        {
            Rect = new Rectangle(Box);
        }

        public override string ToString() => Text;
    }

    public class Rectangle
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        public Rectangle(){ }

        public Rectangle(string src)
        {
            var numbers = src.Split(',')
                .Select(s => decimal.Parse(s, CultureInfo.InvariantCulture))
                .ToArray();

            X = numbers[0];
            Y = numbers[1];
            Width = numbers[2];
            Height = numbers[3];
        }

        public override string ToString() => $"{X}, {Y}, {Width}, {Height}";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using HtmlParser;
using LandRegistryParser.Models;

namespace LandRegistryParser
{
    public class Parser
    {
        public static List<List<KeyValuePair<string, string>>> Parse(Pdf model, 
            decimal keyOffset, decimal valueOffset, decimal delimeterOffset)
        {
            model.Init();
            //denormalize
            var allLines = new List<Line>();
            foreach (var page in model.Pages)
            {
                var offset = (model.Pages.Count - page.Number) * page.MediaRect.Height;
                var lines = page.Flows.SelectMany(f => f.Paragraphs.SelectMany(p => p.Lines));
                foreach (var line in lines)
                {
                    line.Rect.Y += offset;
                    allLines.Add(line);
                }
            }
            allLines = allLines.OrderByDescending(o => o.Rect.Y).ToList();

            //get text aligned as key values
            var keys = allLines
                .Where(s => s.Rect.X == keyOffset)
                .OrderByDescending(o => o.Rect.Y)
                .ToList();

            var values = allLines
                    .Where(s => s.Rect.X == valueOffset)
                    .OrderByDescending(o => o.Rect.Y)
                    .ToList();

            var delimetersProperty = allLines
                    .Where(s => s.Rect.X == delimeterOffset)
                    .OrderByDescending(o => o.Rect.Y)
                    .Select(s => s.Rect.Y)
                    .ToList();

            //concat multiline keys
            var newKeys = new List<Line>();
            Line currentKey = null;
            foreach (Line key in keys)
            {
                if (key.Text.StartsWithUpper())
                {
                    currentKey = key;
                    newKeys.Add(key);
                }
                else
                {
                    currentKey.Text += " " + key.Text;
                }
            }
            //get corresponding values
            var kv = new List<Tuple<string, string, decimal>>();
            for (int i = 0; i < newKeys.Count; i++)
            {
                var key = newKeys[i];
                if (i == newKeys.Count - 1)
                {
                    var vals = values.Where(x => x.Rect.Y < key.Rect.Y + 5M).Select(s => s.Text);
                    kv.Add(new Tuple<string, string, decimal>(key.Text, String.Join(" ", vals), key.Rect.Y));
                }
                else
                {
                    var nextKey = newKeys[i + 1];
                    var vals = values.Where(s => s.Rect.Y < key.Rect.Y + 5M && s.Rect.Y > nextKey.Rect.Y + 5M).Select(s => s.Text);
                    kv.Add(new Tuple<string, string, decimal>(key.Text, String.Join(" ", vals), key.Rect.Y));
                }
            }

            //group by apartments
            var properties = new List<List<KeyValuePair<string, string>>>();
            for (int i = 0; i < delimetersProperty.Count; i++)
            {
                var del = delimetersProperty[i];
                if (i == delimetersProperty.Count - 1)
                {
                    var vals = kv.Where(x => x.Item3 < del)
                        .Select(s => new KeyValuePair<string, string>(s.Item1, s.Item2))
                        .ToList();

                    properties.Add(vals);
                }
                else
                {
                    var nextDel = delimetersProperty[i + 1];
                    var vals = kv.Where(x => x.Item3 < del && x.Item3 > nextDel)
                        .Select(s => new KeyValuePair<string, string>(s.Item1, s.Item2))
                        .ToList();

                    properties.Add(vals);
                }
            }
            return properties;
        }
    }
}

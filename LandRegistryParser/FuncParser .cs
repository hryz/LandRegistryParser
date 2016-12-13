using System;
using System.Collections.Generic;
using System.Linq;
using LandRegistryParser.Models;

namespace LandRegistryParser
{
    public class FuncParser
    {
        public static IEnumerable<List<KeyValuePair<string, string>>> Parse(Pdf model, 
            decimal keyOffset, decimal valueOffset, decimal delimeterOffset)
        {
            model.Init();
            //denormalize
            var allLines = new List<Line>();
            foreach (var page in model.Pages)
            {
                var offset = (page.Number - 1) * page.MediaRect.Height;
                var lines = page.Flows.SelectMany(f => f.Paragraphs.SelectMany(p => p.Lines));
                foreach (var line in lines)
                {
                    line.Rect.Y = page.MediaRect.Height - line.Rect.Y + offset;
                    allLines.Add(line);
                }
            }
            //filter out the useless info
            allLines = allLines
                .Where(s => s.Rect.X == keyOffset 
                            || s.Rect.X == valueOffset 
                            || s.Rect.X == delimeterOffset)
                .OrderBy(o => o.Rect.Y)
                .ToList();

            //split by owners
            var delimetersProperty = allLines
                    .Where(s => s.Rect.X == delimeterOffset)
                    .ToList();

            var owners = allLines.Buffer(delimetersProperty, s => s.Rect.Y, d => d.Rect.Y);

            foreach (var owner in owners)
            {
                //combine multiline keys
                var keys = owner.Where(s => s.Rect.X == keyOffset);
                var upperKeys = keys.Where(s => s.Text.StartsWithUpper()).ToList();
                
                var compositeKeys = keys.Buffer(upperKeys, s => s.Rect.Y, d => d.Rect.Y)
                    .Where(s => s.Any())
                    .Select(s => new Line
                    {
                        Text = String.Join(" ", s.Select(x => x.Text)),
                        Rect = s.First().Rect
                    }).ToList();

                //combine multiline values
                var values = owner.Where(s => s.Rect.X == valueOffset);
                var upperValues = values.Where(s => compositeKeys.Any(k => k.Rect.Y.IsAlmostEqualTo(s.Rect.Y))).ToList();

                var vals = values.Buffer(upperValues, s => s.Rect.Y, d => d.Rect.Y)
                    .Where(s => s.Any())
                    .Select(s => new Line
                    { 
                        Text = String.Join(" ", s.Select(x => x.Text)),
                        Rect = s.First().Rect
                    })
                    .ToList();

                //match keys and values
                yield return compositeKeys
                    .Select(k => new KeyValuePair<string,string>(
                        k.Text, 
                        vals.FirstOrDefault(v => v.Rect.Y.IsAlmostEqualTo(k.Rect.Y))?.Text))
                    .ToList();

            }
        }

        
    }
}

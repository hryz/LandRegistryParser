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
                var keys = owner.Where(s => s.Rect.X == keyOffset);
                var upperKeys = keys.Where(s => s.Text.StartsWithUpper()).ToList();
                
                var compositeKeys = keys.Buffer(upperKeys, s => s.Rect.Y, d => d.Rect.Y)
                    .Where(s => s.Any())
                    .Select(s => new Line
                    {
                        Text = String.Join(" ", s.Select(x => x.Text)),
                        Rect = s.First().Rect
                    }).ToList();

                var values = owner.Where(s => s.Rect.X == valueOffset)
                    .Select(s => new
                    {
                        Value = s,
                        Key = compositeKeys.FirstOrDefault(k => AlmostEqual(k.Rect.Y, s.Rect.Y))
                    });
                var upperValues = values.Where(s => s.Key != null).ToList();

                var vals = values.Buffer(upperValues, s => s.Value.Rect.Y, d => d.Value.Rect.Y)
                    .Where(s => s.Any())
                    .Select(s => new KeyValuePair<string, string>(
                        s.First().Key.Text, 
                        String.Join(" ", s.Select(x => x.Value.Text))))
                    .ToList();


                //yield return vals;

            }
            throw new Exception();
        }

        private static bool AlmostEqual(decimal a, decimal b, decimal maxDifference = 10M)
        {
            return Math.Abs(a - b) < maxDifference;
        }
    }
}

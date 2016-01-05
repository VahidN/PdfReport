using System;
using System.Collections;
using System.Linq;

namespace PdfReportSamples.HexDump
{
    public static class PrintHex
    {
        public static char ToSafeAscii(this int b)
        {
            if (b >= 32 && b <= 126)
            {
                return (char)b;
            }
            return '_';
        }

        public static IEnumerable HexDump(this byte[] data)
        {
            int bytesPerLine = 16;
            return data
                        .Select((c, i) => new { Char = c, Chunk = i / bytesPerLine })
                        .GroupBy(c => c.Chunk)
                        .Select(g =>
                                  new
                                  {
                                      Hex = g.Select(c => String.Format("{0:X2} ", c.Char)).Aggregate((s, i) => s + i),
                                      Chars = g.Select(c => ToSafeAscii(c.Char).ToString()).Aggregate((s, i) => s + i)
                                  })
                        .Select((s, i) =>
                                        new
                                        {
                                            Offset = String.Format("{0:d6}", i * bytesPerLine),
                                            Hex = s.Hex,
                                            Chars = s.Chars
                                        });
        }
    }
}

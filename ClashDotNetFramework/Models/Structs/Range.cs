using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Models.Structs
{
    public readonly struct Range
    {
        public int Start { get; }

        public int End { get; }

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        public bool InRange(int num)
        {
            return Start <= num && num <= End;
        }
    }
}

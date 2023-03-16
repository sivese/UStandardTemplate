using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script._TEST
{
    public static class ForRange
    {
        public static RangeEnumerator FromTo(int from, int to) => new RangeEnumerator(from, to);
    }

    public class RangeEnumerator : IEnumerator
    {
        private int _start = 0;
        private int _last = 0;

        private int _now = 0;

        public RangeEnumerator(int start, int last)
        {
            _start = start;
            _last = last;
        }

        object IEnumerator.Current => Current;
        public int Current => _now;

        public bool MoveNext()
        {
            _now++;
            return (_now < _last);
        }

        public void Reset() => _now = _start;
    }
}

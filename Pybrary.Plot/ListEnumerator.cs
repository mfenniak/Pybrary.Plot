using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pybrary.Plot
{
    public class ListEnumerator<T> : IEnumerator<T>
    {
        private int index = -1;
        private IList<T> back;

        public ListEnumerator(IList<T> back)
        {
            this.back = back;
        }

        public T Current
        {
            get
            {
                if (index < 0 || index >= this.back.Count)
                    throw new InvalidOperationException();
                return this.back[index];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (index < 0 || index >= this.back.Count)
                    throw new InvalidOperationException();
                return this.back[index];
            }
        }

        public bool MoveNext()
        {
            index++;
            return index < this.back.Count;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {
        }
    }
}

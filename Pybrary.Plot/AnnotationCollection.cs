using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Pybrary.Plot
{
    public class AnnotationCollection : EventObject, ICollection<Annotation>
    {
        private List<Annotation> annotations = new List<Annotation>();

        public AnnotationCollection()
        {
        }

        public void Draw(Graphics g, AdvancedRect dataArea)
        {
            foreach (Annotation ann in annotations)
                ann.Draw(g, dataArea);
        }

        public int Count
        {
            get
            {
                return annotations.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(Annotation item)
        {
            annotations.Add(item);
            raiseEvent();
        }

        public void Clear()
        {
            annotations.Clear();
            raiseEvent();
        }

        public bool Remove(Annotation item)
        {
            if (annotations.Remove(item))
            {
                raiseEvent();
                return true;
            }

            return false;
        }

        public bool Contains(Annotation item)
        {
            return annotations.Contains(item);
        }

        public void CopyTo(Annotation[] arr, int arrIdx)
        {
            for (int i = 0; i < annotations.Count; i++)
                arr[i + arrIdx] = annotations[i];
        }

        public IEnumerator<Annotation> GetEnumerator()
        {
            return new ListEnumerator<Annotation>(annotations);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

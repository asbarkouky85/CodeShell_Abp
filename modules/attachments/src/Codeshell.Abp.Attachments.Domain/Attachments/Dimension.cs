using Codeshell.Abp.Files;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Values;

namespace Codeshell.Abp.Attachments
{
    public class Dimension : ValueObject, IDimension
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Dimension(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int CompareTo(IDimension other)
        {
            if (Width > other.Width || Height > other.Height)
                return -1;
            else if (Width == other.Width && Height == other.Height)
                return 0;
            else
                return 1;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Width;
            yield return Height;
        }
    }
}

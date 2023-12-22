namespace Codeshell.Abp.Files
{
    public class FileDimesion : IDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int CompareTo(IDimension other)
        {
            if (Width > other.Width || Height > other.Height)
                return -1;
            else if (Width == other.Width && Height == other.Height)
                return 0;
            else
                return 1;
        }
    }
}

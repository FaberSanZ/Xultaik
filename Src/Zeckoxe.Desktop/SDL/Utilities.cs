using System.Text;

namespace Zeckoxe.Sdl2
{
    internal static class Utilities
    {
        public static unsafe string GetString(byte* stringStart)
        {
            if (stringStart == null) { return null; }

            int characters = 0;
            while (stringStart[characters] != 0)
            {
                characters++;
            }

            return Encoding.UTF8.GetString(stringStart, characters);
        }
    }
}

using System.IO;
using System.Linq;

namespace fotoorg
{
    public static class FileExtensions
    {
        public static bool IsPhoto(this string fileName)
            => new[] { ".jpg", ".png", ".gif", ".heic" }.Any(x => x == Path.GetExtension(fileName).ToLower());

        public static bool IsVideo(this string fileName)
            => new[] { ".mov", ".avi", ".mp4", ".mpg" }.Any(x => x == Path.GetExtension(fileName).ToLower());
    }
}

using photo.exif;
using System;
using System.IO;
using System.Linq;

namespace fotoorg
{
    class FileItem : IDistributable
    {
        private Parser _parser = new Parser();

        public DateTime FileDate { get; private set; }
        public FileType FileType { get; private set; }
        public string Filename { get; private set; }
        public string FilePath { get; private set; }

        public string SourceLocation
            => Path.Combine(FilePath, Filename);

        public FileItem(FileInfo file)
        {
            Filename = file.Name;
            FilePath = file.FullName.Replace(Filename, string.Empty).TrimEnd('\\');
            FileDate = GetFileDateTime(file);

            FileType = GetFileType(Filename);
        }

        /// <summary>
        /// Returns the file's type: { Photo, Video, Other}
        /// </summary>
        private FileType GetFileType(string fileName)
        {
            if (fileName.IsPhoto())
                return FileType.Photo;
            else if (fileName.IsVideo())
                return FileType.Video;
            else
                return FileType.Other;
        }

        /// <summary>
        /// Gets the file DateTime based on the following logic:
        /// If it's a photo, use the Date Taken value straight from the Exif
        /// If Exif is unavailable (and for all other cases), the file's creation date will be used
        /// </summary>
        private DateTime GetFileDateTime(FileInfo fi)
        {
            if (fi.Name.IsPhoto())
            {
                try
                {
                    return _parser.Parse(fi.FullName)
                        .Where(x => x.Title == "ExifDTOrig")
                        .Select(x => Convert.ToDateTime(x.Value.ToString().Substring(0, 10).Replace(":", "-")))
                        .Single();
                }
                catch (InvalidOperationException)
                {
                    // exif data may be missing
                }
            }

            return fi.CreationTime < fi.LastWriteTime 
                    ? fi.CreationTime 
                    : fi.LastWriteTime;
        }
    }
}

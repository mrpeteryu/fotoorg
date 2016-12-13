using Palmer;
using photo.exif;
using System;
using System.IO;
using System.Linq;

namespace fotoorg
{
    public class fotoorg
    {
        string source;
        string target;
        bool moveFiles;
        Parser parser = new Parser();

        public fotoorg(string source, string target, bool moveFiles = false)
        {
            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException(source);

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            this.source = source;
            this.target = target;
            this.moveFiles = moveFiles;
        }

        public void Start()
        {
            var files = Directory
                            .EnumerateFiles(source, "*.*", SearchOption.AllDirectories)
                            .Select(fileName => new FileInfo(fileName));

            int counter = 1;
            int totalFiles = files.Count();
            
            foreach (var file in files)
            {
                Console.Write($"Copying File {file.Name} ({counter}/{totalFiles})");
                Distribute(file);
                counter++;
            }
        }

        public void Distribute(FileInfo fi)
        {
            string target = GetTargetFullName(fi);
            bool isCopied = false;

            Retry.On<FileNotFoundException>().For(5).With((context) =>
            {
                File.Copy(fi.FullName, target, true);
                Console.WriteLine($" to {target}");
                isCopied = true;    
            });

            if (moveFiles)
            {
                if (isCopied)
                {
                    File.Delete(fi.FullName);
                }
                else
                {
                    Console.WriteLine($"Unable to delete: {fi.Name}");
                }
            }
        }

        public string GetTargetFullName(FileInfo fi)
        {
            var createDate = GetFileTimestamp(fi).ToString("yyyy-MM-dd");
            var copyToFolder = Path.Combine(target, createDate);

            if (fi.Name.IsVideo())
                copyToFolder = Path.Combine(copyToFolder, "Videos");
            else if (!fi.Name.IsPhoto())
                copyToFolder = Path.Combine(copyToFolder, "Others");

            if (!Directory.Exists(copyToFolder))
                Directory.CreateDirectory(copyToFolder);

            return Path.Combine(copyToFolder, fi.Name);
        }

        /// <summary>
        /// Gets a value that represents the file's timestamp.
        /// If file is a photo, the Exif information will be used. 
        /// For all other cases, the Created Date will be used.
        /// </summary>
        public DateTime GetFileTimestamp(FileInfo fi)
        {
            if (fi.Name.IsPhoto())
            {
                try
                {
                    return parser.Parse(fi.FullName)
                        .Where(x => x.Title == "ExifDTOrig")
                        .Select(x => Convert.ToDateTime(x.Value.ToString().Substring(0, 10).Replace(":", "-")))
                        .First();
                }
                catch(InvalidOperationException)
                {
                    Console.WriteLine($"No ExifDTOrig Info for {fi.Name} ; Using Creation Date");
                }
            }
            return fi.CreationTime;
        }
    }
}

using Palmer;
using photo.exif;
using System;
using System.IO;
using System.Linq;

namespace fotoorg
{
    public class fotoorg
    {
        public string Source { get; private set; }
        public string Target { get; private set; }
        private bool _moveFiles = false;
        private Parser _parser = new Parser();

        public event EventHandler OnBeforeFileCopy;
        public event EventHandler OnAfterFileCopy;
        public event EventHandler OnError;

        /// <summary>
        /// Constructor
        /// </summary>        
        public fotoorg(string source, string target)
        {
            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException(source);

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            Source = source;
            Target = target;
        }

        public void Start(bool moveFiles = false)
        {
            _moveFiles = moveFiles;

            var files = Directory
                            .EnumerateFiles(Source, "*.*", SearchOption.AllDirectories)
                            .Select(fileName => new FileInfo(fileName));

            int counter = 1;
            int totalFiles = files.Count();

            foreach (var file in files)
            {
                NotifyOnBeforeFileCopy($"Copying File {file.Name} ({counter}/{totalFiles})");
                Distribute(file);
                counter++;
            }
        }

        #region Private Methods
        private void NotifyOnBeforeFileCopy(string msg)
        {
            if (OnBeforeFileCopy != null)
                OnBeforeFileCopy(msg, EventArgs.Empty);
        }

        private void NotifyOnAfterFileCopy(string msg)
        {
            if (OnAfterFileCopy != null)
                OnAfterFileCopy(msg, EventArgs.Empty);
        }

        private void NotifyOnError(string error)
        {
            if (OnError != null)
                OnError(error, EventArgs.Empty);
        }

        private void Distribute(FileInfo fi)
        {
            string target = GetTargetFullName(fi);
            bool isCopied = false;

            Retry.On<FileNotFoundException>().For(5).With((context) =>
            {
                File.Copy(fi.FullName, target, true);
                Console.WriteLine($" to {target}");
                isCopied = true;
            });

            if (_moveFiles)
            {
                if (isCopied)
                {
                    File.Delete(fi.FullName);
                }
                else
                {
                    NotifyOnError($"Unable to delete: {fi.Name}");
                }
            }
        }

        private string GetTargetFullName(FileInfo fi)
        {
            var createDate = GetFileTimestamp(fi).ToString("yyyy-MM-dd");
            var copyToFolder = Path.Combine(Target, createDate);

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
        private DateTime GetFileTimestamp(FileInfo fi)
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
                    NotifyOnError($"No ExifDTOrig Info for {fi.Name} ; Using Creation Date");
                }
            }
            return fi.CreationTime;
        }
        #endregion
    }
}

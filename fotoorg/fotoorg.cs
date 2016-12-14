using fotoorg.Utils;
using Palmer;
using photo.exif;
using System;
using System.IO;
using System.Linq;

namespace fotoorg
{
    public class fotoorg
    {
        private Parser _parser = new Parser();
        private bool _moveFiles = false;

        public string Source { get; private set; }
        public string Target { get; private set; }

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
                NotifyOnBeforeFileCopy($"Processing file: {file.Name} ({counter}/{totalFiles})");
                Distribute(new FileItem(file));
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

        private void Distribute(FileItem file)
        {
            string target = GetTargetFullPath(file);
            bool isCopied = false;

            Retry.On<FileNotFoundException>().For(5).With((context) =>
            {
                FileUtil.PreserveCopy(file.SourceLocation, target, true, true);
                Console.WriteLine($" to {target}");
                isCopied = true;
            });

            if (_moveFiles)
            {
                if (isCopied)
                {
                    File.Delete(file.SourceLocation);
                }
                else
                {
                    NotifyOnError($"Unable to delete: {file.SourceLocation}");
                }
            }
        }

        private string GetTargetFullPath(FileItem file)
        {
            var fileDate = file.FileDate.ToString("yyyy-MM-dd");
            var targetFolderPath = Path.Combine(Target, fileDate);

            if (file.FileType == FileType.Video)
                targetFolderPath = Path.Combine(targetFolderPath, "Videos");
            else if (file.FileType == FileType.Other)
                targetFolderPath = Path.Combine(targetFolderPath, "Others");

            if (!Directory.Exists(targetFolderPath))
                Directory.CreateDirectory(targetFolderPath);

            return Path.Combine(targetFolderPath, file.Filename);
        }
        
        #endregion
    }
}

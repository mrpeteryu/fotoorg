using System.IO;

namespace fotoorg.Utils
{
    class FileUtil
    {
        public static void PreserveCopy(string copyFromPath, string copyToPath, bool overwrite = false, bool applyDateFix = false)
        {
            var origin = new FileInfo(copyFromPath);
            origin.CopyTo(copyToPath, overwrite);

            var destination = new FileInfo(copyToPath);
            if (destination.IsReadOnly)
                destination.IsReadOnly = false;

            // get the earlier timestamp of Created Date vs. Last Write Date.
            var originalFileTime = origin.CreationTime < origin.LastWriteTime
                ? origin.CreationTime
                : origin.LastWriteTime;

            // Apply Timestamp Fix to Created Date?
            destination.CreationTime = applyDateFix
                ? originalFileTime
                : origin.CreationTime;

            // Apply Timestamp Fix to Last Write Date?
            destination.LastWriteTime = applyDateFix 
                ? originalFileTime
                : origin.LastWriteTime;

            destination.LastAccessTime = origin.LastAccessTime;
            
            if (origin.IsReadOnly)
                destination.IsReadOnly = true;
        }
    }
}

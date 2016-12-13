using System;

namespace fotoorg
{
    public enum FileType { Photo, Video, Other }

    interface IDistributable
    {
        string Filename { get; }
        string FilePath { get; }
        string SourceLocation { get; }

        DateTime FileDate { get; }
        FileType FileType { get; }
    }
}

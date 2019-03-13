# fotoorg
fotoorg is a lightweight utility to assist with organizing your photos and videos on local disk.

## Usage
Run *fotoorg.exe*

#### Foto Org Options

#### Required Arguments:

The source path containing the photos/videos to process.   
        *-s*, *-source* {SourcePath}  
  
The target path to put the processed files into.  
        *-t*, *-target* {TargetPath}  
  
#### Optional Arguments:  
Move Files - The files path in the source path will be removed after being it is processed.  
        *-m*, *-move*  
  
Date Fix - The earlier date of the two (Created Date or Last Write Date) will be use as the date for the all target files.  
        *-d*, *-datefix*  
        


## Examples:

*Process photos/videos from C:\DCIM and copies over an organized version to C:\Photos*
> fotoorg.exe -s "C:\DCIM" -t "C:\Photos"  

*Process photos/videos from C:\DCIM and moves over an organized version to C:\Photos.   
All the files in C:\Photos will have their date corrected.*  
> fotoorg.exe -s "C:\DCIM" -t "C:\Photos" -m -d

// /*
// JamesM
// 2013 06 14 10:38 AM
// 2013 06 14 12:09 PM
// IFileSystem.cs
// PictureHandler
// PictureHandler
// */

namespace PictureHandlerLibrary {
	public interface IFileSystem {
		bool FileExists(string pathName);
		string[] GetFilesInDirectory(string directory);
		void MoveFile(string fileToMove, string newLocation);
		bool DirectoryExists(string directoryName);
		void DeleteDirectory(string directoryName);
		void CreateDirectory(string directoryName);
		void DeleteFile(string fileName);
		void CopyFile(string fromFileName, string toFileName);
		void DeleteDirectoryAndAllFiles(string directoryName);
		long GetFileLength(string fileName);
		void CopyFiles(string sourceDirectory, string targetDirectory);
	}
}
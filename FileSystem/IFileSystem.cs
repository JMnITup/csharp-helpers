namespace FileSystem {
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
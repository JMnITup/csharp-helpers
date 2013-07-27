#region

using System;
using System.IO;
using FileSystemLibrary;

#endregion

namespace FileSystem {
	public class FileSystem : IFileSystem {
		public bool FileExists(string pathName) {
			return (File.Exists(pathName));
		}

		public string[] GetFilesInDirectory(string directory) {
			return Directory.GetFiles(directory);
		}

		public long GetFileLength(string fileName) {
			var info = new FileInfo(fileName);
			return info.Length;
		}

		public void MoveFile(string fileToMove, string newLocation) {
			File.Move(fileToMove, newLocation);
		}

		public bool DirectoryExists(string directoryName) {
			return Directory.Exists(directoryName);
		}

		public void DeleteDirectory(string directoryName) {
			Directory.Delete(directoryName);
		}

		public void CreateDirectory(string directoryName) {
			Directory.CreateDirectory(directoryName);
		}

		public void DeleteFile(string fileName) {
			File.Delete(fileName);
		}

		public void CopyFile(string fromFileName, string toFileName) {
			File.Copy(fromFileName, toFileName);
		}

		public void DeleteDirectoryAndAllFiles(string directoryName) {
			if (directoryName.Length < 10) {
				throw new Exception("deleting directory " + directoryName + " is ok?");
			}
			if (DirectoryExists(directoryName)) {
				foreach (string fileName in Directory.GetFiles(directoryName)) {
					File.Delete(fileName);
				}
				Directory.Delete(directoryName);
			}
		}

		public void CopyFiles(string sourceDirectory, string targetDirectory) {
			if (DirectoryExists(sourceDirectory) && DirectoryExists(targetDirectory)) {
				string[] filesInDirectory = GetFilesInDirectory(sourceDirectory);
				foreach (var file in filesInDirectory) {
					var newFileSplit = file.Split('\\');
					var newFile = newFileSplit[newFileSplit.Length - 1];

					
					CopyFile(file, targetDirectory + "\\" + newFile);
				}
			}
		}
	}
}
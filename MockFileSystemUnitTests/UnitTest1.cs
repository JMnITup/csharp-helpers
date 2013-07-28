#region

using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockFileSystemLibrary;

#endregion

namespace MockFileSystemUnitTests {
	[TestClass]
	public class UnitTest1 {
		[TestMethod]
		public void InstantiateMockFileSystem() {
			// Arrange

			// Act
			var fs = new MockFileSystem();

			// Assert
			Assert.IsNotNull(fs);
		}

		[TestMethod]
		public void InstantiateMockFileSystemHasValidCurrentDirectorySet() {
			// Arrange

			// Act
			var fs = new MockFileSystem();

			// Assert
			Assert.IsNotNull(fs.GetCurrentDirectory());
			Assert.IsTrue(fs.GetCurrentDirectory().Length >= 3);
			Assert.IsTrue(Regex.IsMatch(fs.GetCurrentDirectory(), "[A-Za-z]:\\.*", RegexOptions.IgnoreCase));
		}

		[TestMethod]
		public void CreateSimpleDirectoryAfterInstantiate() {
			// Arrange
			var fs = new MockFileSystem();
			const string newdir = "newdir";

			// Act
			fs.CreateDirectory(newdir);

			// Assert
			Assert.IsTrue(fs.DirectoryExists(newdir));
		}

		[TestMethod]
		public void DirectoryExistsReturnsTrueForExistingDirectory() {
			// Arrange
			var fs = new MockFileSystem();
			const string newdir = "newdir";
			fs.CreateDirectory(newdir);

			// Act

			// Assert
			Assert.IsTrue(fs.DirectoryExists(newdir));
		}

		[TestMethod]
		public void DirectoryExistsReturnsTrueForExistingDirectoryWithTrailingSlash() {
			// Arrange
			var fs = new MockFileSystem();
			const string newdir = "newdir";
			fs.CreateDirectory(newdir);

			// Act
			bool result = fs.DirectoryExists(newdir + "\\");

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void DirectoryExistsReturnsFalseForNonexistingDirectory() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			bool result = fs.DirectoryExists("doesnotexist");

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void CreateDirectoryFromCurrent() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.CreateDirectory("test");

			// Assert
			Assert.IsTrue(fs.DirectoryExists("c:\\test"));
		}

		[TestMethod]
		[ExpectedException(typeof (IOException))]
		public void CreateExistingDirectoryThrowsIoException() {
			// Arrange
			var fs = new MockFileSystem();
			fs.CreateDirectory("test");

			// Act
			fs.CreateDirectory("test");

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void CreateDirectoryFromCurrentWithTrailingSlash() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.CreateDirectory("test\\");

			// Assert
			Assert.IsTrue(fs.DirectoryExists("c:\\test"));
		}

		[TestMethod]
		public void CreateAbsoluteDirectoryFromExistingDrive() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.CreateDirectory("c:\\test");

			// Assert
			Assert.IsTrue(fs.DirectoryExists("c:\\test"));
		}

		[TestMethod]
		public void CreateAbsoluteDirectoryFromExistingDriveWithTrailingSlash() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.CreateDirectory("c:\\test\\");

			// Assert
			Assert.IsTrue(fs.DirectoryExists("c:\\test"));
		}

		[TestMethod]
		public void CreateNestedDirectoryFromCurrent() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.CreateDirectory("test\\test2");

			// Assert
			Assert.IsTrue(fs.DirectoryExists("c:\\test\\test2"));
		}

		[TestMethod]
		public void CreateNestedDirectoryFromCurrentWithTrailingSlash() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.CreateDirectory("test\\test2\\");

			// Assert
			Assert.IsTrue(fs.DirectoryExists("c:\\test\\test2"));
		}

		[TestMethod]
		public void DirectoryExistsIsCaseInsensitive() {
			// Arrange
			var fs = new MockFileSystem();
			fs.CreateDirectory("test\\test2");

			// Act

			// Assert
			Assert.IsTrue(fs.DirectoryExists("C:\\TEST\\test2"));
		}

		[TestMethod]
		public void CreateFileFromCurrent() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.AddFile("testFile.txt", 352);

			// Assert
			Assert.IsTrue(fs.FileExists("c:\\testFile.txt"));
		}

		[TestMethod]
		public void CreateFileFromAbsolute() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.AddFile("c:\\testFile.txt", 352);

			// Assert
			Assert.IsTrue(fs.FileExists("c:\\testFile.txt"));
		}

		[TestMethod]
		public void GetFileSizeReturnsCorrectValue() {
			// Arrange
			var fs = new MockFileSystem();
			const int size = 352;
			const string file = "c:\\testFile.txt";
			fs.AddFile(file, size);

			// Act
			long value = fs.GetFileLength(file);

			// Assert
			Assert.AreEqual(size, value);
		}

		[TestMethod]
		public void CreateNestedFileFromAbsolute() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.AddFile("c:\\test\\test2\\testFile.txt", 352);

			// Assert
			Assert.IsTrue(fs.FileExists("c:\\test\\test2\\testFile.txt"));
		}

		[TestMethod]
		public void CreateNestedFileFromCurrentDirectory() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			fs.AddFile("test\\test2\\testFile.txt", 352);

			// Assert
			Assert.IsTrue(fs.FileExists("c:\\test\\test2\\testFile.txt"));
		}

		[TestMethod]
		public void CopyFileCreatesNewFile() {
			// Arrange
			var fs = new MockFileSystem();
			const string oldFilePath = "c:\\old\\file1.txt";
			const long oldSize = 100;
			const string newFilePath = "c:\\new\\file2.txt";
			fs.AddFile(oldFilePath, oldSize);
			fs.CreateDirectory("c:\\new");

			// Act
			fs.CopyFile(oldFilePath, newFilePath);

			// Assert
			Assert.IsTrue(fs.FileExists(newFilePath));
			Assert.AreEqual(oldSize, fs.GetFileLength(newFilePath));
		}

		[TestMethod]
		public void CopyFileLeavesOldFile() {
			// Arrange
			var fs = new MockFileSystem();
			const string oldFilePath = "c:\\old\\file1.txt";
			const long oldSize = 100;
			const string newFilePath = "c:\\new\\file2.txt";
			fs.AddFile(oldFilePath, oldSize);
			fs.CreateDirectory("c:\\new");

			// Act
			fs.CopyFile(oldFilePath, newFilePath);

			// Assert
			Assert.IsTrue(fs.FileExists(oldFilePath));
			Assert.AreEqual(oldSize, fs.GetFileLength(oldFilePath));
		}

		[TestMethod]
		[ExpectedException(typeof (IOException))]
		public void CopyFileToExistingFileThrowsIoException() {
			// Arrange
			var fs = new MockFileSystem();
			const string oldFilePath = "c:\\old\\file1.txt";
			const long oldSize = 100;
			const string newFilePath = "c:\\new\\file2.txt";
			const long newSize = 300;
			fs.AddFile(oldFilePath, oldSize);
			fs.AddFile(newFilePath, newSize);

			// Act
			fs.CopyFile(oldFilePath, newFilePath);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (FileNotFoundException))]
		public void CopyFile_NonExistantFromFileThrowsFileNotFoundException() {
			// Arrange
			var fs = new MockFileSystem();
			const string oldFilePath = "c:\\old\\file1.txt";
			//long oldSize = 100;
			const string newFilePath = "c:\\new\\file2.txt";
			//long newSize = 300;

			// Act
			fs.CopyFile(oldFilePath, newFilePath);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentException))]
		public void CopyFile_FromFileIsDirectoryThrowsArgumentException() {
			// Arrange
			var fs = new MockFileSystem();
			fs.CreateDirectory("c:\\old");
			const string oldFilePath = "c:\\old";
			//long oldSize = 100;
			const string newFilePath = "c:\\new\\file2.txt";
			//long newSize = 300;

			// Act
			fs.CopyFile(oldFilePath, newFilePath);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentException))]
		public void CopyFile_ToFileIsDirectoryThrowsArgumentException() {
			// Arrange
			var fs = new MockFileSystem();
			fs.CreateDirectory("c:\\new");
			const string oldFilePath = "c:\\old\\file1.txt";
			const long oldSize = 100;
			const string newFilePath = "c:\\new";
			//long newSize = 300;
			fs.AddFile(oldFilePath, oldSize);

			// Act
			fs.CopyFile(oldFilePath, newFilePath);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentNullException))]
		public void CopyFile_FromFileIsNullThrowsArgumentNullException() {
			// Arrange
			var fs = new MockFileSystem();
			fs.CreateDirectory("c:\\new");
			const string newFilePath = "c:\\new\\file2.txt";
			//long newSize = 300;

			// Act
			fs.CopyFile(null, newFilePath);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof (ArgumentNullException))]
		public void CopyFile_ToFileIsNullThrowsArgumentNullException() {
			// Arrange
			var fs = new MockFileSystem();
			fs.CreateDirectory("c:\\new");
			const string oldFilePath = "c:\\old\\file1.txt";
			const long oldSize = 100;
			fs.AddFile(oldFilePath, oldSize);

			// Act
			fs.CopyFile(oldFilePath, null);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void FileDelete_RemovesFile() {
			// Arrange
			var fs = new MockFileSystem();
			const string fileName = "c:\\test\\file1.txt";
			fs.AddFile(fileName, 255);

			// Act
			fs.DeleteFile(fileName);

			// Assert
			Assert.IsFalse(fs.FileExists(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof (UnauthorizedAccessException))]
		public void FileDelete_OnDirectoryThrowsUnauthorizedAccessException() {
			// Arrange
			var fs = new MockFileSystem();
			const string fileName = "c:\\test\\";
			fs.CreateDirectory(fileName);

			// Act
			fs.DeleteFile(fileName);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void FileDelete_DeletingNonExistantFileDoesNotFail() {
			// Arrange
			var fs = new MockFileSystem();
			const string fileName = "c:\\test\\Horker.txt";

			// Act
			fs.DeleteFile(fileName);

			// Assert
		}

		[TestMethod]
		public void DeleteDirectory_RemovesEmptyDirectory() {
			// Arrange
			var fs = new MockFileSystem();
			const string dirName = "c:\\test";
			fs.CreateDirectory(dirName);

			// Act
			fs.DeleteDirectory(dirName);

			// Assert
			Assert.IsFalse(fs.DirectoryExists(dirName));
		}

		[TestMethod]
		[ExpectedException(typeof (IOException))]
		public void DeleteDirectory_RemoveNonEmptyDirectoryThrowsIoException() {
			// Arrange
			var fs = new MockFileSystem();
			string dirName = "c:\\test";
			fs.CreateDirectory(dirName);
			fs.AddFile(dirName + "\\file1.txt", 100);

			// Act
			fs.DeleteDirectory(dirName);

			// Assert
			Assert.Fail();
		}
	}
}
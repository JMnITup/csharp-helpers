#region

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
			string newdir = "newdir";

			// Act
			fs.CreateDirectory(newdir);

			// Assert
			Assert.IsTrue(fs.DirectoryExists(newdir));
		}

		[TestMethod]
		public void DirectoryExistsReturnsTrueForExistingDirectory() {
			// Arrange
			var fs = new MockFileSystem();
			string newdir = "newdir";
			fs.CreateDirectory(newdir);

			// Act

			// Assert
			Assert.IsTrue(fs.DirectoryExists(newdir));
		}

		[TestMethod]
		public void DirectoryExistsReturnsTrueForExistingDirectoryWithTrailingSlash() {
			// Arrange
			var fs = new MockFileSystem();
			string newdir = "newdir";
			fs.CreateDirectory("newdir");

			// Act
			var result = fs.DirectoryExists("newdir\\");

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void DirectoryExistsReturnsFalseForNonexistingDirectory() {
			// Arrange
			var fs = new MockFileSystem();

			// Act
			var result = fs.DirectoryExists("doesnotexist");

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
		[ExpectedException(typeof(IOException))]
		public void CreateExistingDirectoryThrowsIOException() {
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

	}
}
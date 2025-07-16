using System;
using System.IO;
using FluentAssertions;
using Moq;
using MockFileSystemLibrary;
using FileSystemLibrary;
using Xunit;

namespace MockFileSystemUnitTests;

public class MockFileSystemTests
{
    [Fact]
    public void InstantiateMockFileSystem()
    {
        // Arrange & Act
        var fs = new MockFileSystem();

        // Assert
        fs.Should().NotBeNull();
    }

    [Fact]
    public void CreateDirectoryFromCurrentCreatesDirectory()
    {
        // Arrange
        var fs = new MockFileSystem();
        var newDir = "newdir";

        // Act
        fs.CreateDirectory(newDir);

        // Assert
        fs.DirectoryExists(newDir).Should().BeTrue();
    }

    [Fact]
    public void CopyFileCreatesTargetFile()
    {
        // Arrange
        var fs = new MockFileSystem();
        const string oldFilePath = "c:\\old\\file1.txt";
        const string newFilePath = "c:\\new\\file2.txt";
        fs.AddFile(oldFilePath, 100);
        fs.CreateDirectory("c:\\new");

        // Act
        fs.CopyFile(oldFilePath, newFilePath);

        // Assert
        fs.FileExists(newFilePath).Should().BeTrue();
    }

    [Fact]
    public void CopyFileToExistingFileThrowsIoException()
    {
        // Arrange
        var fs = new MockFileSystem();
        const string oldFilePath = "c:\\old\\file1.txt";
        const string newFilePath = "c:\\new\\file2.txt";
        fs.AddFile(oldFilePath, 100);
        fs.AddFile(newFilePath, 200);

        // Act
        Action act = () => fs.CopyFile(oldFilePath, newFilePath);

        // Assert
        act.Should().Throw<IOException>();
    }

    private class FileMover
    {
        private readonly IFileSystem _fs;
        public FileMover(IFileSystem fs) => _fs = fs;
        public void Move(string src, string dest)
        {
            if (!_fs.FileExists(src)) throw new FileNotFoundException();
            _fs.MoveFile(src, dest);
        }
    }

    [Fact]
    public void FileMoverUsesFileSystem()
    {
        // Arrange
        var mock = new Mock<IFileSystem>();
        mock.Setup(m => m.FileExists("a.txt")).Returns(true);
        var mover = new FileMover(mock.Object);

        // Act
        mover.Move("a.txt", "b.txt");

        // Assert
        mock.Verify(m => m.MoveFile("a.txt", "b.txt"), Times.Once);
    }
}

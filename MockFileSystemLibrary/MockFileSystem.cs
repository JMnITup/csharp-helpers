#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FileSystem;
using FileSystemLibrary;
using JMExtensions;

#endregion

namespace MockFileSystemLibrary {
	public class MockFileSystem : IFileSystem {
		public enum NodeType {
			Drive,
			Directory,
			File,
		}

		private readonly Node _currentDrive;

		private readonly List<Node> _driveList;
		private Node _currentDirectory;

		public MockFileSystem() {
			_driveList = new List<Node>();
			Node newNode = Node.CreateDriveNode("C");
			_driveList.Add(newNode);
			_currentDirectory = newNode;
			_currentDrive = newNode;
		}

		private void SetCurrentDirectory(Node newDirectory) {
			_currentDirectory = newDirectory;
			// TODO: throw DirectoryNotFoundException if doesn't exist
		}

		public void SetCurrentDirectory(string newDirectory) {
			Node x = ConvertPathToNodeReference(newDirectory);
			_currentDirectory = x;
			// TODO: throw DirectoryNotFoundException if doesn't exist
		}

		public string GetCurrentDirectory() {
			return _currentDirectory.GetFullPath();
		}

		public void AddFile(string fileName, long fileSize, DateTime? exifDateTimeOriginal = null) {
			string[] x = ConvertPathToParentAndTargetPaths(fileName);
			string parentPath = x[0];
			string childPath = x[1];

			if (!DirectoryExists(parentPath)) {
				CreateDirectory(parentPath);
			}


			var newFile = new File(childPath, fileSize) {ExifDateTimeOriginal = exifDateTimeOriginal};
			ConvertPathToNodeReference(parentPath).AddFile(newFile);
		}

		public void DeleteDirectory(string directoryName) {
			throw new NotImplementedException();
		}

		public void DeleteFile(string fileName) {
			throw new NotImplementedException();
		}

		public void CopyFile(string fromFileName, string toFileName) {
			throw new NotImplementedException();
		}

		public void DeleteDirectoryAndAllFiles(string directoryName) {
			throw new NotImplementedException();
		}

		public long GetFileLength(string fileName) {
			throw new NotImplementedException();
		}

		public void CopyFiles(string sourceDirectory, string targetDirectory) {
			throw new NotImplementedException();
		}

		public void CreateDirectory(string path) {
			while (path.Right(1) == "\\") {
				path = path.Substring(0, path.Length - 1);
			}
			if (ConvertPathToNodeReference(path) != null) { throw new IOException("Directory of file with same name already exists");}
			string parentPath = _currentDirectory.GetFullPath();
			string childPath = path;
			int indexOfLastSlash = path.LastIndexOf('\\');
			if (indexOfLastSlash >= 0) {
				childPath = path.Substring(indexOfLastSlash + 1);
				parentPath = path.Substring(0, indexOfLastSlash + 1);

				if (!DirectoryExists(parentPath)) {
					CreateDirectory(parentPath);
				}
			}

			ConvertPathToNodeReference(parentPath).CreateDirectoryNode(childPath);
			//return path;
		}

		public void MoveFile(string fileToMove, string newLocation) {
			Node fileNode = ConvertPathToNodeReference(fileToMove);

			string[] x = ConvertPathToParentAndTargetPaths(newLocation);
			string newParentString = x[0];
			Node newParentNode = ConvertPathToNodeReference(newParentString);
			string newTargetString = x[1];
			if (newParentNode == null) {
				throw new Exception("Something bad happened");
			}
			Node oldParentNode = fileNode.Parent;

			if (newParentNode.Children.Find(y => y.Name.ToUpper() == newTargetString.ToUpper()) != null) {
				throw new IOException("New file location already contains a file of the same name");
			}

			oldParentNode.Children.Remove(fileNode);
			newParentNode.Children.Add(fileNode);
			fileNode.Parent = newParentNode;
			fileNode.Name = newTargetString;
		}

		public bool DirectoryExists(string path) {
			Node x = ConvertPathToNodeReference(path);
			if (x == null) {
				return false;
			}
			if (x.Type == NodeType.Directory || x.Type == NodeType.Drive) {
				return true;
			}
			return false;
		}

		private string[] ConvertPathToParentAndTargetPaths(string path) {
			while (path.Contains("\\\\")) {
				path = path.Replace("\\\\", "\\");
			}
			while (path.Right(1) == "\\") {
				path = path.Substring(0, path.Length - 1);
			}
			string parentPath = _currentDirectory.GetFullPath();
			string childPath = path;
			int indexOfLastSlash = path.LastIndexOf('\\');
			if (indexOfLastSlash >= 0) {
				childPath = path.Substring(indexOfLastSlash + 1);
				parentPath = path.Substring(0, indexOfLastSlash + 1);
			}
			return new[] {parentPath, childPath};
		}

		private Node ConvertPathToNodeReference(string path) {
			while (path.Contains("\\\\")) {
				path = path.Replace("\\\\", "\\");
			}
			string[] driveSplit = path.Split(':');
			if (driveSplit.Length > 2) {
				throw new Exception("Too many Colons in path");
			}
			if (driveSplit.Length <= 0) {
				throw new Exception("Path too short?");
			}
			Node drive = null;
			string path2 = "";
			if (driveSplit.Length == 2) {
				path2 = driveSplit[1];
				if (driveSplit[0] == "") {
					drive = _currentDrive;
				} else {
					drive = GetDrive(driveSplit[0]);
				}
			}
			if (driveSplit.Length == 1) {
				drive = _currentDrive;
				path2 = driveSplit[0];
			}
			Node currentLocation;
			if (path2.Length > 0 && path2.Substring(0, 1) == "\\") {
				currentLocation = drive;
				path2 = path2.Substring(1);
			} else {
				currentLocation = _currentDirectory;
			}
			string[] pathSplit = path2.Split('\\');
			foreach (string entry in pathSplit) {
				if (entry == "") {
					break;
				}
				if (currentLocation == null) {
					return null;
				}
				Node newLoc = currentLocation.Children.Find(x => x.GetName().ToUpper() == entry.ToUpper());
				if (newLoc == null) {
					return null;
				}
				currentLocation = newLoc;
			}

			return currentLocation;
		}

		private Node GetDrive(string driveLetter) {
			if (_driveList == null || _driveList.Count <= 0) {
				throw new Exception(string.Format("No drives defined in mock filesystem, drive {0} not found", driveLetter));
			}
			return _driveList.Find(x => x.GetName().ToUpper() == driveLetter.ToUpper());
		}

		public bool FileExists(string path) {
			Node x = ConvertPathToNodeReference(path);
			if (x == null) {
				return false;
			}
			if (x.Type == NodeType.File) {
				return true;
			}
			return false;
		}

		public string[] GetFilesInDirectory(string directory) {
			throw new NotImplementedException();
		}

		public DateTime? GetMockExifData(string fileName) {
			var fileNode = (File) ConvertPathToNodeReference(fileName);
			if (fileNode.Type != NodeType.File) {
				return null;
			}
			return fileNode.ExifDateTimeOriginal;
		}

		public void ResizeFile(string fileName, long fileSize) {
			var fileNode = (File) ConvertPathToNodeReference(fileName);
			fileNode.FileSize = fileSize;
		}

		private class File : Node {
			public File(string fileName, long fileSize) : base(fileName, NodeType.File) {
				FileSize = fileSize;
			}

			public string FileName {
				get { return Name; }
			}

			public DateTime? ExifDateTimeOriginal { get; set; }

			public long FileSize { get; set; }
		}

		[DebuggerDisplay("{_name} ({_type})")]
		private class Node {
			internal readonly List<Node> Children = new List<Node>();
			internal readonly NodeType Type;
			internal string Name;
			internal Node Parent;

			private Node() {}

			protected Node(string name, NodeType type) {
				Name = name;
				Type = type;
			}

			private Node(string name, NodeType type, Node parent)
				: this(name, type) {
				Parent = parent;
			}

			public string GetName() {
				return Name;
			}

			public string GetFullPath() {
				if (Parent != null) {
					if (Type == NodeType.Directory) {
						return Parent.GetFullPath() + Name + "\\";
					}
					return Parent.GetFullPath() + Name;
				}
				return Name + ":\\";
			}

			public static Node CreateDriveNode(string name) {
				if (name.Length > 1) {
					throw new Exception("Drive names must be single characters");
				}
				return new Node(name, NodeType.Drive);
			}

			public Node CreateDirectoryNode(string name) {
				if (Type == NodeType.File) {
					throw new Exception("Cannot create directory within a file");
				}
				var newNode = new Node(name, NodeType.Directory, this);
				Children.Add(newNode);
				return newNode;
			}

			public Node CreateFileNode(string name) {
				if (Type == NodeType.File) {
					throw new Exception("Cannot create file within a file");
				}
				var newNode = new Node(name, NodeType.File, this);
				Children.Add(newNode);
				return newNode;
			}

			public void AddFile(File newFile) {
				if (Type == NodeType.File) {
					throw new Exception("Cannot create file within a file");
				}
				Children.Add(newFile);
				newFile.Parent = this;
			}
		}
	}
}
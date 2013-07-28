#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FileSystemLibrary;
using JMExtensions;

#endregion

namespace MockFileSystemLibrary {
	public partial class MockFileSystem {
		public enum NodeType {
			Drive,
			Directory,
			File,
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
#region

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace MockFileSystemLibrary {
	[DebuggerDisplay("{DisplayFileStructure()}")]
	public partial class MockFileSystem {
		public enum NodeType {
			Drive,
			Directory,
			File,
		}

		public string DisplayFileStructure() {
			string rv = "";
			foreach (Node drive in _driveList) {
				rv += drive.DisplayNode();
			}
			return rv;
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

		[DebuggerDisplay("{Name} ({Type})")]
		private class Node : ICloneable {
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

			public string DisplayNode(int depth = 0) {
				var prefix = new string('=', depth);
				string rv = prefix + Name + "\n";
				foreach (Node child in Children) {
					rv += child.DisplayNode(depth + 1);
				}
				return rv;
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

			public virtual void Rename(string newName) {
				// TODO: disallow special characters like slashes
				Name = newName;
			}

			#region Implementation of ICloneable

			/// <summary>
			///   Creates a new object that is a copy of the current instance.
			/// </summary>
			/// <returns>
			///   A new object that is a copy of this instance.
			/// </returns>
			public object Clone() {
				return MemberwiseClone();
			}

			#endregion
		}
	}
}
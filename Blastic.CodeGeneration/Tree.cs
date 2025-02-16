using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Blastic.CodeGeneration
{
	public class Tree<T>
	{
		[DebuggerDisplay("{" + nameof(Id) + "}")]
		public class Node
		{
			public T Id { get; }
			public bool HasValue { get; }

			public Node? Parent { get; }
			public List<Node> Children { get; }

			public bool HasParent => Parent != null;
			public bool HasChildren => Children.Any();

			public Node(T value, Node? parent, bool hasValue)
			{
				Id = value;
				Parent = parent;
				HasValue = hasValue;
				Children = [];
			}

			public Node AddChild(T value, bool hasValue)
			{
				Node node = new(value, this, hasValue);
				Children.Add(node);

				return node;
			}
		}

		public Node Root { get; }

		public Tree(T root)
		{
			Root = new Node(root, null, false);
		}
	}
}
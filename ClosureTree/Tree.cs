using System.Collections.Generic;
using System.Linq;

namespace ClosureTree
{
	/// <summary>
	/// Builds a tree structure from closure data where each node has one parent and zero or more children. The root node will have a null parent node.
	/// </summary>
	public class Tree
	{
		/// <summary>
		/// Gets the root node of the tree.
		/// </summary>
		public Node Root { get; set; }

		/// <summary>
		/// Gets all of the nodes in the tree, in a flattened view indexed by ID.
		/// </summary>
		public Dictionary<int, Node> Nodes { get; } = new Dictionary<int, Node>();

		/// <summary>
		/// Builds a tree from a collection of closures. Assumes there will be a single root node, i.e., one tree, in the closure data.
		/// </summary>
		public Tree(IEnumerable<(int ParentId, int ChildId, int Depth)> closures)
		{
			foreach (var closure in closures)
			{
				var child = GetOrAddNode(closure.ChildId);

				if (closure.Depth == 1)
				{
					var parent = GetOrAddNode(closure.ParentId);

					parent.Children.Add(child.Id, child);
					child.Parent = parent;
				}
			}

			Root = Nodes.Values.Single(n => n.Parent == null);
		}

		Node GetOrAddNode(int id)
		{
			if (!Nodes.ContainsKey(id))
				Nodes.Add(id, new Node(id));

			return Nodes[id];
		}
	}
}

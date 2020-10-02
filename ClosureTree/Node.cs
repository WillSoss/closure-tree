using System.Collections.Generic;

namespace ClosureTree
{
	/// <summary>
	/// A single node in a <see cref="Tree"/>.
	/// </summary>
	public class Node
	{
		/// <summary>
		/// Gets the ID of this node.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets the parent <see cref="Node"/> of this node. If this node is the root node, <see cref="Parent"/> will be null.
		/// </summary>
		public Node Parent { get; set; }

		/// <summary>
		/// Gets a dictionary of child nodes, indexed by ID.
		/// </summary>
		public Dictionary<int, Node> Children { get; } = new Dictionary<int, Node>();

		public Node(int id) => Id = id;
		public override string ToString() => $"Node {Id}";
		public override bool Equals(object obj) => obj is int id ? id == Id : obj is Node node ? node.Id == Id : base.Equals(obj);
		public override int GetHashCode() => (typeof(Node).GetHashCode(), Id.GetHashCode()).GetHashCode();
	}
}

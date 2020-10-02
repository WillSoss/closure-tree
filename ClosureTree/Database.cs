using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ClosureTree
{
	public class Database
	{
		public string ConnectionString { get; }

		public Database(string connectionString)
		{
			ConnectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
		}

		async Task<SqlConnection> OpenConnection()
		{
			var con = new SqlConnection(ConnectionString);
			await con.OpenAsync();

			return con;
		}

		/// <summary>
		/// Adds a new node to the node with ID <paramref name="parentId"/>, or creates 
		/// a new tree with a single node if <paramref name="parentId"/> is null.
		/// </summary>
		/// <returns>The ID of the newly added node.</returns>
		public async Task<int> AddNode(int? parentId)
		{
			using var con = await OpenConnection();

			return await con.QueryFirstOrDefaultAsync<int>("dbo.AddNode", new { parentId }, commandType: CommandType.StoredProcedure);
		}

		/// <summary>
		/// Deletes the node with ID <paramref name="nodeId"/> and all its children.
		/// </summary>
		/// <returns>The IDs of all deleted nodes.</returns>
		public async Task<IEnumerable<int>> DeleteNode(int nodeId)
		{
			using var con = await OpenConnection();

			return await con.QueryAsync<int>("dbo.DeleteNode", new { nodeId }, commandType: CommandType.StoredProcedure);
		}

		/// <summary>
		/// Copies the tree at node <paramref name="nodeId"/> to the node at <paramref name="parentId"/>. New nodes with unique IDs
		/// are created in this operation, matching the structure of the copied tree.
		/// </summary>
		/// <returns>The IDs of the newly added nodes.</returns>
		public async Task<IEnumerable<int>> CopyNode(int nodeId, int? parentId)
		{
			using var con = await OpenConnection();
			var tx = await con.BeginTransactionAsync();

			var tree = new Tree(await GetTree(nodeId));

			var copied = await AddTree(tree.Root, parentId);

			tx.Commit();

			return copied;

			async Task<IEnumerable<int>> AddTree(Node root, int? parentId)
			{
				var nodes = new List<int>();

				nodes.Add(await AddNode(parentId));

				foreach (var child in root.Children.Values)
					nodes.AddRange(await AddTree(child, nodes[0]));

				return nodes;
			}
		}

		/// <summary>
		/// Moves the tree at node <paramref name="nodeId"/> to the node at <paramref name="parentId"/>. If <paramref name="parentId"/>
		/// is null, a separate tree is created.
		/// </summary>
		/// <returns>The IDs of the moved nodes.</returns>
		public async Task<IEnumerable<int>> MoveNode(int nodeId, int? parentId)
		{
			using var con = await OpenConnection();

			if (parentId.HasValue && await IsChildOf(con, parentId.Value, nodeId))
				throw new ArgumentOutOfRangeException(nameof(nodeId), "A node cannot be moved to one of its descendants.");

			return await con.QueryAsync<int>("dbo.MoveNode", new { nodeId, parentId }, commandType: CommandType.StoredProcedure);
		}

		/// <summary>
		/// Gets the tree at node <paramref name="nodeId"/>, to the depth specified in <paramref name="depth"/>.
		/// </summary>
		/// <returns>A tree in the form of closure table data.</returns>
		public async Task<IEnumerable<(int ParentId, int ChildId, int Depth)>> GetTree(int nodeId, int? depth = null)
		{
			using var con = await OpenConnection();

			return await con.QueryAsync<(int ParentId, int ChildId, int Depth)>("dbo.GetTree", new { nodeId, depth }, commandType: CommandType.StoredProcedure);
		}

		async Task<bool> IsChildOf(SqlConnection con, int nodeId, int parentId) =>
			(await con.QueryFirstOrDefaultAsync<int?>("dbo.IsChildOf", new { nodeId, parentId }, commandType: CommandType.StoredProcedure)).HasValue;

		/// <summary>
		/// Clears all nodes and closure data from the database.
		/// </summary>
		public async Task Clear()
		{
			using var con = await OpenConnection();

			await con.ExecuteAsync("dbo.Clear", commandType: CommandType.StoredProcedure);
		}
	}
}

using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace ClosureTree.Tests
{
	[Collection("Closure Tests")]
	public class Getting_a_tree : Test
	{
		[Fact]
		public async Task Should_get_a_single_node_tree()
		{
			await db.Clear();

			var a = await db.AddNode(null);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0)
			});
		}

		[Fact]
		public async Task Should_get_a_tree()
		{
			//        a
			//        |
			//        b
			//        | 
			//        c         
			//        |         
			//        d         

			await db.Clear();

			var a = await db.AddNode(null);
			var b = await db.AddNode(a);
			var c = await db.AddNode(b);
			var d = await db.AddNode(c);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(c, c, 0),
				(d, d, 0),
				(a, b, 1),
				(b, c, 1),
				(c, d, 1),
				(a, c, 2),
				(b, d, 2),
				(a, d, 3)
			});
		}

		[Fact]
		public async Task Should_get_a_sub_tree()
		{
			// Get sub-tree c:
			//        a
			//        |
			//        b
			//        | 
			//        c        c 
			//        |        |
			//        d        d

			await db.Clear();

			var a = await db.AddNode(null);
			var b = await db.AddNode(a);
			var c = await db.AddNode(b);
			var d = await db.AddNode(c);

			(await db.GetTree(c)).Should().BeEquivalentTo(new[]
			{
				(c, c, 0),
				(d, d, 0),
				(c, d, 1)
			});
		}

		[Fact]
		public async Task Should_get_a_tree_to_depth()
		{
			// Get sub-tree b to depth 1:
			//        a
			//        |
			//        b        b
			//        |        |
			//        c        c 
			//        |
			//        d

			await db.Clear();

			var a = await db.AddNode(null);
			var b = await db.AddNode(a);
			var c = await db.AddNode(b);
			var d = await db.AddNode(c);

			(await db.GetTree(b, 1)).Should().BeEquivalentTo(new[]
			{
				(b, b, 0),
				(c, c, 0),
				(b, c, 1)
			});
		}

	}
}

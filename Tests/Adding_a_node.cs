using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace ClosureTree.Tests
{
	[Collection("Closure Tests")]
	public class Adding_a_node : Test
	{
		[Fact]
		public async Task Should_create_new_tree()
		{
			await db.Clear();

			var a = await db.AddNode(null);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0)
			});
		}

		[Fact]
		public async Task Should_add_node_to_single_node_tree()
		{
			// Add node b:
			//        a             a
			//						|
			//						b

			await db.Clear();

			var a = await db.AddNode(null);
			var b = await db.AddNode(a);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(a, b, 1)
			});
		}

		[Fact]
		public async Task Should_add_node_to_tree()
		{
			// Add node e:
			//        a             a
			//       / \           / \
			//      b   c         b   c
			//           \           / \
			//            d         e   d

			await db.Clear();

			var a = await db.AddNode(null);
			var b = await db.AddNode(a);
			var c = await db.AddNode(a);
			var d = await db.AddNode(c);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(c, c, 0),
				(d, d, 0),
				(a, b, 1),
				(a, c, 1),
				(c, d, 1),
				(a, d, 2)
			});

			var e = await db.AddNode(c);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(c, c, 0),
				(d, d, 0),
				(e, e, 0),
				(a, b, 1),
				(a, c, 1),
				(c, d, 1),
				(c, e, 1),
				(a, d, 2),
				(a, e, 2)
			});
		}
	}
}

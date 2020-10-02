using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace ClosureTree.Tests
{
	[Collection("Closure Tests")]
	public class Deleting_a_node : Test
	{
		[Fact]
		public async Task Should_delete_tree()
		{
			await db.Clear();

			var nodeId = await db.AddNode(null);

			var deleted = await db.DeleteNode(nodeId);

			deleted.Should().HaveCount(1).And.BeEquivalentTo(nodeId);

			(await db.GetTree(nodeId)).Should().BeEmpty();
		}

		[Fact]
		public async Task Should_delete_node_and_children()
		{           
			// Delete node c:
			//        a         a
			//        |         |
			//        b         b
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

			var deleted = await db.DeleteNode(c);

			deleted.Should().HaveCount(2).And.BeEquivalentTo(new[] { c, d });

			var closures = await db.GetTree(a);
			
			closures.Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(a, b, 1)
			});
		}
	}
}

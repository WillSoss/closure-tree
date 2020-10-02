using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ClosureTree.Tests
{
	[Collection("Closure Tests")]
	public class Moving_a_node : Test
	{
		[Fact]
		public async Task Should_move_node()
		{
			// Move node c from b to a:
			//        a             a 
			//        |            / \ 
			//        b           b   c 
			//        |                \ 
			//        c                 d
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

			var moved = await db.MoveNode(c, a);

			moved.Should().BeEquivalentTo(new[] { c, d });

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
		}

		[Fact]
		public async Task Should_sever_a_node()
		{
			// Sever node c:
			//        a         a 
			//        |         | 
			//        b         b 
			//        |         
			//        c         c 
			//        |         | 
			//        d         d     

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

			var moved = await db.MoveNode(c, null);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(a, b, 1)
			});

			(await db.GetTree(c)).Should().BeEquivalentTo(new[]
{
				(c, c, 0),
				(d, d, 0),
				(c, d, 1)
			});
		}

		[Fact]
		public async Task Should_join_a_node()
		{
			// Join node c to b:
			//        a         a 
			//        |         | 
			//        b         b 
			//                  |
			//        c         c 
			//        |         | 
			//        d         d     

			await db.Clear();

			var a = await db.AddNode(null);
			var b = await db.AddNode(a);
			var c = await db.AddNode(null);
			var d = await db.AddNode(c);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
{
				(a, a, 0),
				(b, b, 0),
				(a, b, 1)
			});

			(await db.GetTree(c)).Should().BeEquivalentTo(new[]
{
				(c, c, 0),
				(d, d, 0),
				(c, d, 1)
			});

			var moved = await db.MoveNode(c, b);

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
		public async Task Should_not_move_node_to_child()
		{
			// Attempt to move node b to c should fail:
			//        a
			//        |  
			//        b  
			//        |  
			//        c  
       
			await db.Clear();

			var a = await db.AddNode(null);
			var b = await db.AddNode(a);
			var c = await db.AddNode(b);

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(c, c, 0),
				(a, b, 1),
				(b, c, 1),
				(a, c, 2),
			});

			await db.Invoking(db => db.MoveNode(b, c))
				.Should().ThrowAsync<ArgumentOutOfRangeException>();
		}
	}
}

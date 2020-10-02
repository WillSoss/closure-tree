using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClosureTree.Tests
{
	[Collection("Closure Tests")]
	public class Copying_a_node : Test
	{
		[Fact]
		public async Task Should_copy_node()
		{
			// Copy node c to a:
			//        a             a 
			//        |            / \ 
			//        b           b   e (c)
			//        |           |    \ 
			//        c           c     f (d)
			//        |           |
			//        d           d
			//

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

			var copied = await db.CopyNode(c, a);

			copied.Should().HaveCount(2).And.NotContain(new[] { a, b, c, d });

			var e = copied.First();
			var f = copied.Last();

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(c, c, 0),
				(d, d, 0),
				(e, e, 0),
				(f, f, 0),
				(a, b, 1),
				(a, e, 1),
				(b, c, 1),
				(c, d, 1),
				(e, f, 1),
				(a, c, 2),
				(a, f, 2),
				(b, d, 2),
				(a, d, 3),
			});
		}

		[Fact]
		public async Task Should_copy_node_to_new_tree()
		{
			// Copy node c to a:
			//        a           a    e (c)
			//        |           |    |
			//        b           b    f (d)
			//        |           |   
			//        c           c   
			//        |           |
			//        d           d
			//

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

			var copied = await db.CopyNode(c, null);

			copied.Should().HaveCount(2).And.NotContain(new[] { a, b, c, d });

			var e = copied.First();
			var f = copied.Last();

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

			(await db.GetTree(e)).Should().BeEquivalentTo(new[]
			{
				(e, e, 0),
				(f, f, 0),
				(e, f, 1)
			});
		}

		[Fact]
		public async Task Should_copy_node_to_descendant()
		{
			// Copy node b to c:
			//        a             a
			//        |             |
			//        b             b
			//        |             |
			//        c             c
			//                      |
			//                      d (b)
			//                      |
			//                      e (c)

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

			var copied = await db.CopyNode(b, c);

			copied.Should().HaveCount(2).And.NotContain(new[] { a, b, c });

			var d = copied.First();
			var e = copied.Last();

			(await db.GetTree(a)).Should().BeEquivalentTo(new[]
			{
				(a, a, 0),
				(b, b, 0),
				(c, c, 0),
				(d, d, 0),
				(e, e, 0),
				(a, b, 1),
				(b, c, 1),
				(c, d, 1),
				(d, e, 1),
				(a, c, 2),
				(b, d, 2),
				(c, e, 2),
				(a, d, 3),
				(b, e, 3),
				(a, e, 4)
			});
		}
	}
}

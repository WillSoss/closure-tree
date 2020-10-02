namespace ClosureTree.Tests
{
	public class Test
	{
		protected readonly Database db;

		public Test()
		{
			db = new Database("server=.;database=closure-tree;integrated security=true;");
		}
	}
}

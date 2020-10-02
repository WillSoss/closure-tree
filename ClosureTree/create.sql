SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Node](
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Node] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Closure](
	[ParentId] [int] NOT NULL,
	[ChildId] [int] NOT NULL,
	[Depth] [int] NOT NULL,
 CONSTRAINT [PK_Closure] PRIMARY KEY CLUSTERED 
(
	[ParentId] ASC,
	[ChildId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Closure]  WITH CHECK ADD  CONSTRAINT [FK_Child_Node] FOREIGN KEY([ChildId])
REFERENCES [dbo].[Node] ([Id])
GO

ALTER TABLE [dbo].[Closure] CHECK CONSTRAINT [FK_Child_Node]
GO

ALTER TABLE [dbo].[Closure]  WITH CHECK ADD  CONSTRAINT [FK_Parent_Node] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Node] ([Id])
GO

ALTER TABLE [dbo].[Closure] CHECK CONSTRAINT [FK_Parent_Node]
GO

CREATE PROCEDURE [dbo].[GetTree]
(
	@NodeId int,
	@Depth int
)
AS
BEGIN
	SET NOCOUNT ON;

	select	*
	from	[Closure]
	where	ParentId in (select ChildId from [Closure] where ParentId = @NodeId and	(@Depth is null or Depth <= @Depth)) and
			ChildId in (select ChildId from [Closure] where ParentId = @NodeId and (@Depth is null or Depth <= @Depth))
	order by Depth, ParentId, ChildId
END
GO

CREATE PROCEDURE [dbo].[IsChildOf]
(
	@NodeId int,
	@ParentId int
)
AS
BEGIN
	SET NOCOUNT ON;

	select	ParentId
	from	[Closure]
	where	ChildId = @NodeId and ParentId = @ParentId

END
GO

CREATE PROCEDURE [dbo].[AddNode]
(
	@ParentId int
)
AS
BEGIN
	SET NOCOUNT ON;
	declare @Node table (Id int)

	-- Add the node and capture the Id
    insert	[Node]
	output	Inserted.Id into @Node
	default values

	-- Add a closure to the new node, for each of the n nodes in the path 
	-- from the root to the immediate parent. If this is the root node 
	-- (@ParentId = null), this won't add anything.
	insert	[Closure] (ParentId, ChildId, Depth)
	select	c.ParentId, n.Id, Depth + 1
	from	[Closure] c, @Node n
	where	c.ChildId = @ParentId

	-- Add a self-referencing closure
	insert  [Closure]
	select	n.Id, n.Id, 0
	from	@Node n

	select	Id
	from	@Node
END
GO

CREATE PROCEDURE [dbo].[DeleteNode]
(
	@NodeId int
)
AS
BEGIN
	SET NOCOUNT ON;
	declare @Node table (Id int)

	-- Delete the closures from the node's children to their parents
	delete	c
	from	[Closure] n join [Closure] c
				on n.ChildId = c.ChildId
	where	n.ParentId = @NodeId

	-- Delete the orphaned nodes
	delete	[Node]
	output	Deleted.Id into @Node
	where	Id not in (select ParentId from [Closure])

	select	Id
	from	@Node
END
GO

CREATE PROCEDURE [dbo].[MoveNode]
(
	@NodeId int,
	@ParentId int
)
AS
BEGIN
	SET NOCOUNT ON;
	declare @Node table (Id int)

	-- Delete closures that cross the link we're breaking
	delete c
	from	[Closure] c
	where	c.ParentId in (select ParentId from [Closure] where ChildId = @NodeId and ParentId != @NodeId) and
			c.ChildId in (select ChildId from [Closure] where ParentId = @NodeId)

	-- Add closures for each node up the tree for the new parent
	-- and down the tree from the node being moved
	insert	[Closure] (ParentId, ChildId, Depth)
	output	Inserted.ChildId into @Node
	select	p.ParentId, c.ChildId, p.Depth + c.Depth + 1
	from	[Closure] p, [Closure] c
	where	p.ChildId = @ParentId and 
			c.ParentId = @NodeId

	select	distinct *
	from	@Node
END
GO

CREATE PROCEDURE [dbo].[Clear]
AS
BEGIN

	delete from [Closure]
	delete from [Node]
	
END
GO
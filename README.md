# Closure Tree
A simple C#/MSSQL closure table implementation for fun and learning.

## Get Started

1. Create an MSSQL database named `closure-tree`, or whatever you'd like to name it. 
1. Open the file `create.sql` in the `ClosureTree` folder and run the script in the newly created database to add the tables and stored procedures needed to run the code.
3. Edit the connection string in `Test.cs` to target your database. Run the tests from Visual Studio or by running `dotnet test` from the `Tests` folder.

## What's a Closure Table?

A closure table is a technique for storing hierarchical data in a database table. A table stores the relationship between each ancestor/descendant pair of nodes in the tree, as well as the number of nodes between them, known as depth. This enables tree-based operations, like getting, adding, deleting, or moving nodes or sub-trees, in a manner that is easy to implement and fast to execute.

For example, the following tree:

```
    a
   / \
  b   c
     / \
    d   e
```

Would be stored in a closure table like this:

|Parent|Child|Depth|
|------|-----|-----|
|a|a|0|
|b|b|0|
|c|c|0|
|d|d|0|
|e|e|0|
|a|b|1|
|a|c|1|
|c|d|1|
|c|e|1|
|a|d|2|
|a|e|2|

See the tests for examples of what a closure table can do!

## Who's this for?

Anyone, really! Whether you're a student learning data structures or a developer starting to build an accounting system, hopefully this will be useful. It's a project that was just a little fun, something pure and academic between real world work.

If you find it useful, let me know! If you improve it, open a PR so other's can see your handiwork!

## How do I make use of this?

The code in this repo just stores the table structure, but no information about a node beyond an ID. Columns could be added to `dbo.Node` to store data or a foreign key to an existing table. `dbo.Node` could also be dropped and the IDs in `dbo.Closure` could point directly to another table. This code is just a starting point for building something real on top of it.
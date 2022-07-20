# GodotExt - Extension methods for the Godot C# API

This library contains a set of useful extension methods that augment Godot's C# API to make it easier to use and more type-safe. The extension methods are defined for a handful of types. 

## Installation

GodotExt is published on [NuGet](https://www.nuget.org/packages/GodotExt). To add it use this command line command (or the NuGet facilities of your IDE):

```bash
dotnet add package GodotExt --version 0.2.0
```

If you are targeting `netstandard2.1` also add the following lines to your `.csproj` file to make it work with Godot:

```xml
<PropertyGroup>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

## What's inside
### Finding nodes

You can use the `AtPath` and `WithName`  extension methods as a type-safe and verified replacement for Godot's `GetNode` and `FindNode` methods. Both replacements will verify that the node exists and throw an exception if it doesn't.

```csharp
var button = GetNode<Button>("Some/Path/To/Button");

// becomes
var button = this.AtPath<Button>("Some/Path/to/Button");
```

```csharp
var button = FindNode<Button>("Button", true, false);

// becomes
var button = this.WithName<Button>("Button");
```

You can use `FindAllDescendants` and `FindClosestDescendants` to find all nodes in a subtree that are of a certain type and match additional criteria.

```csharp
// find the red buttons below the current node
var allRedButtons = FindAllDescendants<Button>(b => b.GetColor() == Color.Red);

// find the outermost containers that are in the "Alpha" group
var allContainersInGroupAlpha = FindClosestDescendants<Container>(c => c.IsInGroup("Alpha"));
```

The difference between `FindAllDescendants` and `FindClosestDescendants` is that `FindAllDescendants` will return all nodes in the whole sub-tree that match the criteria, while `FindClosestDescendants` will return the first node in each branch of the sub-tree that matches the criteria (e.g. `FindClosestDescendants`  will stop searching a branch when it has found a match). 

Also, be aware that these functions traverse the node tree so they can be expensive and should not be used in tight loops. Their main use case is for saving games where they allow you to quickly find interesting nodes in the tree that need saving and where it is ok to wait a few milliseconds for the result.

### Parent-child relationships

`GetChildNodes<T>` is a type-safe replacement for `GetChildren`. It will only return nodes of the given type back:

```csharp
foreach(var child in this.GetChildren()) {
   if (child is Button button) {
      button.Disabled = true;
   }
}

// becomes   

foreach(var button in this.GetChildNodes<Button>()) {
    button.Disabled = true;
}
```

`MoveToNewParent` moves a node to a new parent:

```csharp

if (player.GetParent() != null) {
    player.GetParent().RemoveChild(player);
}
airship.AddChild(player);

// becomes

player.MoveToNewParent(airship);
```

`MoveToNewParentKeepPosition` moves a `Node2D` to a new parent, keeping its global position and rotation

```csharp

var playerPosition = player.GlobalPosition;
var playerRotation = player.GlobalRotation;

player.GetParent().RemoveChild(player);

airship.AddChild(player);

player.GlobalPosition = playerPosition;
player.GlobalRotation = playerRotation;

// becomes

player.MoveToNewParentKeepPosition(airship);
```

`RemoveFromParent` removes a node from its parent:

```csharp
if (node.GetParent() != null) {
    node.GetParent().RemoveChild(node);
}

// becomes

node.RemoveFromParent();
```

`RemoveAndFree` is a safer alternative to `QueueFree`. It ensures the node is actually removed from the tree before `QueueFree`  is called, which helps avoiding strange behaviour when the node is still in the tree for the remainder of the current frame:

```csharp
if (node.GetParent() != null) {
    node.GetParent().RemoveChild(node);
}
node.QueueFree();

// becomes

node.RemoveAndFree();
```

### Nodes in groups
`FindBelowInGroup` finds all nodes below a given node that are in a given group:

```csharp
var myNode = ...; // this is the node below which we want to find all nodes that are in a certain group

var nodesInGroup = GetTree().GetNodesInGroup("SomeGroup");
var result = new List<Node>();
foreach (var node in nodesInGroup) {
    if (myNode.IsAParentOf(node)) {
        result.Add(node);
    }
}

// becomes

var nodesInGroup = myNode.FindBelowInGroup("SomeGroup");
```

`GetNodesInGroup` gets an override that will filter the group nodes to only return nodes of the given type:

```csharp

var enemies = GetTree().GetNodesInGroup("SomeGroup")
    .OfType<Enemy>();
    
// becomes

var enemies = GetTree().GetNodesInGroup<Enemy>("SomeGroup");
```    

### Signals
`FiresSignal` provides a better-readable alternative to the built-in `ToSignal`  when you want to wait for a certain signal with `await`.

```csharp
await ToSignal(timer,"timeout");

// becomes

await timer.FiresSignal("timeout");
```

#### Timers and Tweens
For timers and tweens there are additional methods that make using them easier:

```csharp
// make a one-off 500ms timer and wait on it
await ToSignal(GetTree().CreateTimer(0.5), "timeout");

// becomes
await this.Sleep(0.5);
```
```csharp
await ToSignal(timer, "timeout");

// becomes
await timer.Timeout();
```


```csharp
await ToSignal(tween, "tween_completed");

// becomes
await tween.IsCompleted();
```

#### Fluent API for signal connections

`Connect` gets an override which allows you to fluently create signal connections:

```csharp
button
    .Connect(
        "pressed",  
        this, nameof(OnButtonPressed)
    );

// becomes
button
    .Connect("pressed")
    .To(this, nameof(OnButtonPressed));
```
In addition to the vanilla API it also directly checks for errors during binding and throws an exception if the binding fails instead of silently failing. Also, this API allows for easier additional binds and binding flags, which can use the actual enum's instead of `uint`s and don't require you to manually create Godot's `Array` instances:

```csharp
button.Connect(
    "pressed", 
    this, nameof(OnButtonPressed),
    new Array(button), 
    ((uint) ConnectFlags.Deferred) | ((uint) ConnectFlags.OneShot)));

// becomes
button
    .Connect("pressed")
    .WithBinds(button)
    .WithFlags(ConnectFlags.Deferred, ConnectFlags.OneShot)
    .To(this, nameof(OnButtonPressed))
```

The fluent API also returns a binding object which allows you to easily disconnect the binding again without having to write all the parameters again:

```csharp
button.Connect("pressed", this, nameof(OnButtonPressed));
// later...
button.Disconnect("pressed", this, nameof(OnButtonPressed));

// becomes

var connection = button
    .Connect("pressed")
    .To(this, nameof(OnButtonPressed));
// later...    
connection.Disconnect();    
```

### Miscellaneous

#### Type-safe cloning of nodes

`Clone` is a type-safe variant of `Duplicate`:

```csharp
var monsterClone = monster.Duplicate() as Monster;

// becomes
var monsterClone = monster.Clone(); // no casting necessary

```

#### Snake-case conversion for `CallDeferred`

When calling Godot-API through `CallDeferred` you need to translate the method name into snake-case. This can easily introduce typos. Therefore `string` gets an extension method that converts the string to snake-case:

```csharp
someNode.CallDeferred("someMethod", new Array(1, 2, 3));

// becomes
someNode.CallDeferred(nameof(SomeNode.SomeMethod).ToSnakeCase(), new Array(1, 2, 3));
```

As an additional bonus the deferred call will now also appear in the usage search of your IDE.

#### 2D World-Screen transformations on `ViewPort`

`Viewport` has some extensions that allow you to quickly transform screen coordinates to a world coordinates:

```csharp
var worldCoordinates = GetGlobalMousePosition() * viewport.CanvasTransform;

// becomes

var worldCoordinates = viewport.ScreenToWorld(GetGlobalMousePosition());
```

and the reverse is also possible:


```csharp
var playerPositionOnScreen = viewport.CanvasTransform * player.position;

// becomes

var playerPositionOnScreen = viewport.WorldToScreen(player.position);
```





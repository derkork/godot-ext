# GodotExt - Extension methods for the Godot C# API

This library contains a set of useful extension methods that augment Godot's C# API to make it easier to use and more type-safe. The extension methods are defined for a handful of types. 

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

### Parent-child relationships

`GetChildNodes<T>` is a type-safe replacement for `GetChildren`. It will only return nodes of the given type back:

```csharp

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
await ToSignal(GetTree().CreateTimer(500), "timeout");

// becomes
await this.Sleep(500);
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

This API allows for easier additional binds and binding flags, which can use the actual enum's instead of `uint`s and don't require you to manually create Godot's `Array` instances:

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
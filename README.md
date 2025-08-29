# Extend RPC Library

A library for **s&box** that makes it easy to send RPC calls and receive **asynchronous callbacks**.  
It simplifies **client ↔ server** communication while taking full advantage of `async/await`.

---

## 🚀 Features
- Asynchronous RPC calls
- Receive callbacks using `async/await`
- Simplified error and timeout handling
- Supported return types:  
  ✅ **Primitives** (`int`, `float`, `bool`, `string`, etc.)  
  ✅ **structs**  
  ✅ **records**
  ✅ **List<>**  

---

## 📦 Installation
Clone or download this repository and add its content directly into your **s&box** project at `Libraries/extend.extend_rpc` folder.  

---

## 🛠 Basic Usage Example

```csharp
protected override void OnStart()
{
    Task.RunInThreadAsync(async () =>
    {
        var result = await Compute(1, 4);
        Log.Info("Result is: " + result);
    });
}

[RpcCallback]
public async Task<int> Compute(int a, int b)
{
    return a + b;
}
```
## 🛠 List Usage Example

```csharp
protected override void OnStart()
{
    Task.RunInThreadAsync(async () =>
    {
        var results = await Compute(1, 4);
        Log.Info("Result is: " + string.Join(", ", results));
    });
}

[RpcCallback]
public async Task<List<int>> Compute(int a, int b)
{
    return [a + b];
}
```
## 🛠 Using a struct as return type
```csharp
public struct PlayerStats
{
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public float Accuracy { get; set; }
}

[RpcCallback]
public async Task<PlayerStats> GetPlayerStats(string playerId)
{
    // Simulate database fetch
    await Task.Delay(100); 

    return new PlayerStats
    {
        Kills = 10,
        Deaths = 3,
        Accuracy = 0.75f
    };
}

protected override void OnStart()
{
    Task.RunInThreadAsync(async () =>
    {
        var stats = await GetPlayerStats("Player_42");
        Log.Info($"Kills: {stats.Kills}, Deaths: {stats.Deaths}, Accuracy: {stats.Accuracy}");
    });
}
```
## 🛠 Using a record as return type
```csharp
// Primary constructors are not supported yet when serializing / deserializing in s&box
public record InventoryItem( string Name, int Quantity );

// Instead, use this way
public record InventoryItem
{
    public string Name { get; set; }
    public int Quantity { get; set; } 

    public InventoryItem( string name, int quantity )
    {
        Name = name;
        Quantity = quantity;
    }
}

[RpcCallback]
public async Task<InventoryItem> GetInventoryItem(string itemId)
{
    await Task.Delay(50);

    return new InventoryItem("Health Potion", 5);
}

protected override void OnStart()
{
    Task.RunInThreadAsync(async () =>
    {
        var item = await GetInventoryItem("potion_health");
        Log.Info($"Item: {item.Name}, Quantity: {item.Quantity}");
    });
}
```
## ⚙️ How it works

Functions marked with the `[RpcCallback]` attribute are **wrapped automatically**.  
When you call them:

1. The wrapper sends an **RPC to the server**.  
2. The server executes the **original method**.  
3. The return value is captured.  
4. Another **RPC is sent back to the caller (client)** with the result.  

This makes it possible to `await` the call just like a local async function.

The caller receives the result as a `Task<T>` (async/await).

Any return type is supported as long as it’s a primitive, a struct, or a record that can be serialized.

Collections (`List<T>`, arrays, etc.) are also supported if `T` is a valid type.

The library handles serialization, network transport, and automatic value return.

Configurable timeout (default: `5s`).

## 📖 Quick Reference

`[RpcCallback]` → Marks a method as an asynchronous RPC.

`Task.RunInThreadAsync(...)` → Runs a task outside the main game thread.

Exceptions are automatically propagated back to the caller.

## 🤝 Contributing

Pull Requests are welcome!
Please follow C# best practices. Unit tests is optional.

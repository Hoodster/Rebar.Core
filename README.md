![license](https://img.shields.io/github/license/Hoodster/Rebar.Core)   ![nuget](https://img.shields.io/nuget/v/rebar.core) ![downloads](https://img.shields.io/nuget/dt/rebar.core)

# rebar.core
[NuGet library](https://www.nuget.org/packages/rebar.core/)  to support CQS pattern based project with Autofac implementation

## Note
Library is created for personal purposes but I decided that it can get useful for someone else so feel free to use.

## Features
Library allows to provide simple command query separation inside project using AutoFac library.

## Examples
### Commands
Commands are executed without results in return.
#### Namespace
``` csharp
using Rebar.Core.Command
```
#### Execute
```csharp
private ICommandDispatcher _dispatcher;

public Execute(ICommandDispatcher dispatcher)
{
    _dispatcher = dispatcher;
}

...

var command = new SampleCommand("John");
_dispatcher.Execute(command);

```
#### SampleCommand.cs
```csharp
public class SampleCommand : ICommand
{
    public string Name { get; set; }
  
    public SampleCommand(string name)
    {
        this.Name = name;
    }
}
```

#### SampleCommandHandler.cs
```csharp
public class SampleCommandHandler : ICommandHandler<SampleCommand>
{
    public void Execute(SampleCommand command)
    {
        command.Name // John
        ...
    }
}
```
### Queries
Queries are executed and return defined class object
#### Namespace
```csharp
using Rebar.Core.Query
```
#### Execute
```csharp
private IQueryDispatcher _dispatcher
public Execute(IQueryDispatcher dispatcher)
{
    _dispatcher = dispatcher;
}

...
var query = new SampleQuery(10);
var result = _dispatcher.Execute(query); // result = { SubstractionResult: 4 }
```
#### SampleQuery.cs
```csharp
public class SampleQuery : IQuery<SampleQueryResponse>
{
    public int BaseNumber { get; set; }
  
    public SampleQuery(int number)
    {
        this.BaseNumber = number;
    }
}
```
#### SampleQueryResponse.cs
```csharp
public class SampleQueryResponse : IQueryResponse
{
    public int SubstractionResult { get; set; }
  
    public SampleQueryResponse(int result) 
    {
        this.SubstractionResult = result;
    }
}
```

#### SampleQueryResponseHandler.cs
```csharp
public class SampleQueryResponseHandler : IQueryHandler<SampleQuery, SampleQueryResponse>

public SampleQueryResponse Execute(SampleQuery query)
{
    var substraction = query.BaseNumber - 6;
    return new SampleQueryResponse(substraction);
}
```

## Async operations
### Commands
```
ICommandHandler<ICommand> => IAsyncCommandHandler<ICommand>
```
```csharp
public class SampleCommandHandler : IAsyncCommandHandler<SampleCommand>
{
    public void ExecuteAsync(SampleCommand command, CancellationToken token) {}
}

...

_dispatcher.ExecuteAsync(command);
```

### Queries
```
IQueryHandler<IQuery, IQueryResponse> => IAsyncQueryHandler<IQuery, IQueryResponse>
```
```csharp
public class SampleQueryResponseHandler : IAsyncQueryHandler<SampleQuery, SampleQueryResponse>
{
    public Task<SampleQueryResponse> ExecuteAsync(SampleQuery query, CancellationToken token) {}
}

...

_dispatcher.ExecuteAsync(query);
```

## Configuration
In `startup.cs` include following line to include Rebar.Core in service collection.
```csharp
using Rebar.Core.Extensions;
...
builder.AddRebarCore();
```
Next in AutoFac module files declare registering commands and/or queries.
> ❗ **Important:** Handlers are name sensitive. They should end with "QueryHandler" or "CommandHandler" otherwise they won't be recognized.
```csharp
using Rebar.Extensions;

public class AppModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var executingAssembly = GetExecutingAssembly();
        
        // register commands within assembly
        builder.RegisterCommandHandlers(executingAssembly);
        
        // register queries within assembly
        builder.RegisterQueryHandlers(executingAssembly);
        
        // register both queries and commands within assembly
        builder.RegisterAll(executingAssembly);
    }
 }
```
### Methods
```csharp
ContainerBuilder RegisterCommandHandlers(self ContainerBuilder builder, ExecutingAssembly assembly, InstanceTypes instanceType, object[] lifeTimeScopeTags)
```
Registers commands within assembly.

#### Parameters

- **`builder` ContainerBuilder** <br> *description*: Extension for AutoFac `ContainerBuilder` <br><br>
- **`assembly` ExecutingAssembly** <br> *description*: Assembly that contains commands definitions <br><br>
- **(optional) `instanceType` InstanceTypes** <br> *description*: Instance lifecycle type for commands. <br> *default*: `InstanceTypes.LifetimeScope` <br> <br>
- **(optional) ``lifeTimeScopeTags`` object[]** <br> *description*: Tag applied to matching lifetime scopes. Optional for request scope, required for matching lifetime scope. <br> *default*: `null` <br><br>


```csharp
ContainerBuilder RegisterQueryHandlers(self ContainerBuilder builder, ExecutingAssembly assembly, InstanceTypes instanceType, object[] lifeTimeScopeTags)
```
Registers queries within assembly.
#### Parameters
- **`builder` ContainerBuilder** <br> *description*: Extension for AutoFac `ContainerBuilder` <br><br>
- **`assembly` ExecutingAssembly** <br> *description*: Assembly that contains queries definitions <br><br>
- **(optional) `instanceType` InstanceTypes** <br> *description*: Instance lifecycle type for queries. <br> *default*: `InstanceTypes.LifetimeScope` <br> <br>
- **(optional) ``lifeTimeScopeTags`` object[]** <br> *description*: Tag applied to matching lifetime scopes. Optional for request scope, required for matching lifetime scope. <br> *default*: `null` <br><br>

```csharp
ContainerBuilder RegisterAll(self ContainerBuilder builder, ExecutingAssembly assembly, InstanceTypes instanceType, object[] lifeTimeScopeTags)
```
Registers both commands and queries within assembly.
#### Parameters
- **`builder` ContainerBuilder** <br> *description*: Extension for AutoFac `ContainerBuilder` <br><br>
- **`assembly` ExecutingAssembly** <br> *description*: Assembly that contains commands and queries definitions <br><br>
- **(optional) `instanceType` InstanceTypes** <br> *description*: Instance lifecycle type for command and queries. <br> *default*: `InstanceTypes.LifetimeScope` <br> <br>
- **(optional) ``lifeTimeScopeTags`` object[]** <br> *description*: Tag applied to matching lifetime scopes. Optional for request scope, required for matching lifetime scope. <br> *default*: `null` <br><br>



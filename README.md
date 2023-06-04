![license](https://img.shields.io/github/license/Hoodster/rebar.core)   ![nuget](https://img.shields.io/nuget/v/rebar.core) ![downloads](https://img.shields.io/nuget/dt/rebar.core)

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
public class SampleCommandHandler : ICommandHandler<SampleCommand>
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
...

using Marten;
using Marten.Events.Projections;
using Newtonsoft.Json;
using Shouldly;
using static MartenDotNetTestTemplate.Tests.TestDatabase;

namespace MartenDotNetTestTemplate.Tests;

public record SomethingHappened(DateTimeOffset On);

public class Something
{
  public Guid Id { get; set; }
  public DateTimeOffset On { get; }

  [JsonConstructor]
  private Something()
  {
  }

  private Something(DateTimeOffset on)
  {
    On = on;
  }

  public static Something Create(SomethingHappened happened)
  {
    return new Something(happened.On);
  }
}

public class When_snapshot_is_configured_inline
{
  public class When_event_is_persisted : IAsyncLifetime
  {
    private Guid _streamId;
    private DocumentStore? _store;

    public async Task InitializeAsync()
    {
      _store = DocumentStore.For(_ =>
      {
        _.Connection(GetTestDbConnectionString);
        _.Projections.Snapshot<Something>(SnapshotLifecycle.Inline);
      });

      var on = DateTimeOffset.Now;
      var happened = new SomethingHappened(on);
      _streamId = Guid.NewGuid();

      await using var session = _store.LightweightSession();
      session.Events.Append(_streamId, happened);
      await session.SaveChangesAsync();
    }

    [Fact]
    public async Task should_write_snapshot_in_same_transaction()
    {
      await using var session = _store.QuerySession();
      var something = session.Load<Something>(_streamId);
      something.ShouldNotBeNull();
    }


    public Task DisposeAsync()
    {
      _store.Dispose();
      return Task.CompletedTask;
    }
  }
}

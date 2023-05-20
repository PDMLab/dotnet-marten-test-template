namespace MartenDotNetTestTemplate.Tests;

public class When_snapshot_is_configured_inline
{
  public class When_event_is_persisted : IAsyncLifetime
  {
    [Fact]
    public void should_write_snapshot_in_same_transaction()
    {
    }

    public Task InitializeAsync()
    {
      throw new NotImplementedException();
    }

    public Task DisposeAsync()
    {
      throw new NotImplementedException();
    }
  }

}


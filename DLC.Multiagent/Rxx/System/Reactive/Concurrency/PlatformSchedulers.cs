using System.Diagnostics.Contracts;

namespace System.Reactive.Concurrency
{
  internal static class PlatformSchedulers
  {
    public static IScheduler Concurrent
    {
      get
      {
        Contract.Ensures(Contract.Result<IScheduler>() != null);

#if !PORT_45 && !PORT_40 && !UNIVERSAL
        return ThreadPoolScheduler.Instance;
#else
        return TaskPoolScheduler.Default;
#endif
      }
    }
  }
}
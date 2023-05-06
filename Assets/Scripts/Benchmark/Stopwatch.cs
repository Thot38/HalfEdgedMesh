using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Benchmark
{
  public class Stopwatch : IDisposable
  {
    private DateTime _startTime;
    private List<string> _times;

    public Stopwatch(List<string> times)
    {
      _times = times;
      _startTime = DateTime.Now;
    }

    public void Dispose()
    {
      var timeSpan = DateTime.Now - _startTime;
      _times.Add(timeSpan.ToString());
    }
  }
}

namespace Auth.Infrastructure.Cache
{
    public class CacheHealthState
    {
        private readonly object _lock = new();
        private volatile bool _isMemoryPressureHigh;
      

        public bool IsMemoryPressureHigh
        {
            get { lock (_lock) return _isMemoryPressureHigh; }
            set { lock (_lock) _isMemoryPressureHigh = value; }
        }
        //public bool IsMemoryPressureHigh { get; set; }
    }
}

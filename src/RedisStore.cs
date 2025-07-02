namespace codecrafters_redis;

public class RedisStore
{
    private readonly Dictionary<string, string> _storage = new();
    private readonly object _lock = new();

    public void Set(string key, string value)
    {
        lock (_lock)
        {
            _storage[key] = value;
        }
    }

    public bool TryGet(string key, out string? value)
    {
        lock (_lock)
        {
            return _storage.TryGetValue(key, out value);
        }
    }

    public bool Delete(string key)
    {
        lock (_lock)
        {
            return _storage.Remove(key);
        }
    }

}
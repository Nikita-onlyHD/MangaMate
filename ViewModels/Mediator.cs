using System.Collections.Concurrent;

namespace MangaMate.ViewModels
{
    public sealed class Mediator
    {
        private static readonly Lazy<Mediator> _instance =
            new(() => new Mediator());

        public static Mediator Instance => _instance.Value;

        private readonly ConcurrentDictionary<string,
            List<WeakReference<Action<object?>>>> _routes = new();

        private Mediator() { }

        public void Register(string key, Action<object?> action)
        {
            var weak = new WeakReference<Action<object?>>(action);

            _routes.AddOrUpdate(
                key,
                _ => new List<WeakReference<Action<object?>>> { weak },
                (_, list) =>
                {
                    list.Add(weak);
                    return list;
                });
        }

        public void Notify(string key, object? parameter = null)
        {
            if (!_routes.TryGetValue(key, out var list)) return;

            var dead = new List<WeakReference<Action<object?>>>();

            foreach (var weak in list)
            {
                if (weak.TryGetTarget(out var action))
                    action(parameter);
                else
                    dead.Add(weak);
            }

            if (dead.Count > 0)
                list.RemoveAll(dead.Contains);
        }
    }
}

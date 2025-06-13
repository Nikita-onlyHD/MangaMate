using System.Collections.Concurrent;

namespace MangaMate.ViewModels
{
    public sealed class Mediator
    {
        private static readonly Lazy<Mediator> _instance =
            new(() => new Mediator());

        public static Mediator Instance => _instance.Value;

        private readonly ConcurrentDictionary<string,
            List<Action<object?>>> _routes = new();

        private Mediator() { }

        public void Register(string key, Action<object?> action)
        {
            var a = new Action<object?>(action);

            _routes.AddOrUpdate(
                key,
                _ => new List<Action<object?>> { a },
                (_, list) =>
                {
                    list.Add(a);
                    return list;
                });
        }

        public void Notify(string key, object? parameter = null)
        {
            if (!_routes.TryGetValue(key, out var list)) return;

            foreach (var action in list.ToList())
            {
                action(parameter);
            }
        }
    }
}

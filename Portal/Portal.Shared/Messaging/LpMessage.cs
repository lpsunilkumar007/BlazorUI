using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portal.Shared.Extensions;

namespace Portal.Shared.Messaging;

public class LpMessage<TArgs>
{
    protected event Func<TArgs, Task>? MessageEvent;

    private readonly Dictionary<object, Func<TArgs, Task>?> _callbacks = new();

    public void Subscribe(object subscriber, Func<TArgs, Task>? callback)
    {
        if (_callbacks.ContainsKey(subscriber))
        {
            return;
        }

        _callbacks.Add(subscriber, callback);
        MessageEvent += callback;
    }

    public void Unsubscribe(object subscriber)
    {
        if (!_callbacks.TryGetValue(subscriber, out var callback))
        {
            return;
        }
            
        _callbacks.Remove(subscriber);
        MessageEvent -= callback;
    }

    private TArgs? _prevSendArgs = default(TArgs);
    public async Task SendAsync(TArgs args, bool force = true)
    {
        if (MessageEvent != null)
        {
            if (force || (_prevSendArgs == null || !_prevSendArgs.Equals(args)))
            {
                _prevSendArgs = args;
                await MessageEvent.InvokeAsync(args);
            }
        }
    }
}
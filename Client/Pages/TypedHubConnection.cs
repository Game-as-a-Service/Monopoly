using Microsoft.AspNetCore.SignalR.Client;
using System.Linq.Expressions;

namespace Client.Pages;

internal class TypedHubConnection<TResponse> where TResponse : notnull
{
    public HubConnection HubConnection { get; }
    public TypedHubConnection(HubConnection hubConnection)
    {
        HubConnection = hubConnection;
    }
    public void On(Expression<Func<TResponse, Func<Task>>> @event, Action handler)
    {
        HandleOnEvent(@event,
            Type.EmptyTypes,
            args => handler());
    }

    public void On<T1>(Expression<Func<TResponse, Func<T1, Task>>> @event, Action<T1> handler)
    {
        HandleOnEvent(@event,
            new[] { typeof(T1) },
            args => handler((T1)args[0]!));
    }

    public void On<T1, T2>(Expression<Func<TResponse, Func<T1, T2, Task>>> @event, Action<T1, T2> handler)
    {
        HandleOnEvent(@event, 
            new[] { typeof(T1), typeof(T2) },
            args => handler((T1)args[0]!, (T2)args[1]!));
    }

    public void On<T1, T2, T3>(Expression<Func<TResponse, Func<T1, T2, T3, Task>>> @event, Action<T1, T2, T3> handler)
    {
        HandleOnEvent(@event,
            new[] { typeof(T1), typeof(T2), typeof(T3) },
            args => handler((T1)args[0]!, (T2)args[1]!, (T3)args[2]!));
    }

    public void On<T1, T2, T3, T4>(Expression<Func<TResponse, Func<T1, T2, T3, T4, Task>>> @event, Action<T1, T2, T3, T4> handler)
    {
        HandleOnEvent(@event,
            new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) },
            args => handler((T1)args[0]!, (T2)args[1]!, (T3)args[2]!, (T4)args[3]!));
    }

    public void On<T1, T2, T3, T4, T5>(Expression<Func<TResponse, Func<T1, T2, T3, T4, T5, Task>>> @event, Action<T1, T2, T3, T4, T5> handler)
    {
        HandleOnEvent(@event,
            new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) },
            args => handler((T1)args[0]!, (T2)args[1]!, (T3)args[2]!, (T4)args[3]!, (T5)args[4]!));
    }

    private void HandleOnEvent<TDelegate>(Expression<Func<TResponse, TDelegate>> @event, Type[] parameterTypes, Action<object?[]> handler)
         where TDelegate : Delegate
    {
        var func = @event.Compile().Invoke(default!);
        var expression = func.Method.GetBaseDefinition();
        if (expression.Name is null)
            throw new ArgumentException("事件名稱無法解析");
        HubConnection.On(expression.Name, parameterTypes, static (parameters, state) =>
        {
            var currentHandler = (Action<object?[]>)state;
            currentHandler(parameters);
            return Task.CompletedTask;
        }, handler);
    }
}

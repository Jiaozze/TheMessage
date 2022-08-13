using System;
using System.Collections.Generic;

public static class EventManager
{
    //所有委托都继承于System.Delegate
    static Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventType">事件类型名</param>
    /// <param name="action">事件委托</param>
    public static void RegisterEvent(string eventType, Action action)
    {
        OnEventAdding(eventType, action);
        eventDict[eventType] = (Action)Delegate.Combine((Action)eventDict[eventType], action);
    }
    public static void RegisterEvent<T>(string eventType, Action<T> action)
    {
        OnEventAdding(eventType, action);
        eventDict[eventType] = (Action<T>)Delegate.Combine((Action<T>)eventDict[eventType], action);
    }
    public static void RegisterEvent<T, U>(string eventType, Action<T, U> action)
    {
        OnEventAdding(eventType, action);
        eventDict[eventType] = (Action<T, U>)Delegate.Combine((Action<T, U>)eventDict[eventType], action);
    }
    public static void RegisterEvent<T, U, V>(string eventType, Action<T, U, V> action)
    {
        OnEventAdding(eventType, action);
        eventDict[eventType] = (Action<T, U, V>)Delegate.Combine((Action<T, U, V>)eventDict[eventType], action);
    }
    public static void RegisterEvent<T, U, V, W>(string eventType, Action<T, U, V, W> action)
    {
        OnEventAdding(eventType, action);
        eventDict[eventType] = (Action<T, U, V, W>)Delegate.Combine((Action<T, U, V, W>)eventDict[eventType], action);
    }
    /// <summary>
    /// 事件添加检查
    /// </summary>
    /// <param name="eventType">事件类型名</param>
    /// <param name="EventBeingAdded">原始委托类型</param>
    static void OnEventAdding(string eventType, Delegate EventBeingAdded)
    {
        if (!eventDict.ContainsKey(eventType))
        {
            eventDict.Add(eventType, null);
        }
        Delegate delegate2 = eventDict[eventType];
        if ((delegate2 != null) && (delegate2.GetType() != EventBeingAdded.GetType()))
        {
            throw new Exception(string.Format("Try to add not correct event {0}. Current type is {1}, adding type is {2}.", eventType, delegate2.GetType().Name, EventBeingAdded.GetType().Name));
        }
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventType">事件类型名</param>
    /// <param name="handler">事件委托</param>
    public static void RemoveEvent(string eventType, Action handler)
    {
        if (OnEventRemoving(eventType, handler))
        {
            eventDict[eventType] = (Action)Delegate.Remove((Action)eventDict[eventType], handler);
            OnEventRemoved(eventType);
        }
    }
    public static void RemoveEvent<T>(string eventType, Action<T> handler)
    {
        if (OnEventRemoving(eventType, handler))
        {
            eventDict[eventType] = (Action<T>)Delegate.Remove((Action<T>)eventDict[eventType], handler);
            OnEventRemoved(eventType);
        }
    }

    public static void RemoveEvent<T, U>(string eventType, Action<T, U> handler)
    {
        if (OnEventRemoving(eventType, handler))
        {
            eventDict[eventType] = (Action<T, U>)Delegate.Remove((Action<T, U>)eventDict[eventType], handler);
            OnEventRemoved(eventType);
        }
    }

    public static void RemoveEvent<T, U, V>(string eventType, Action<T, U, V> handler)
    {
        if (OnEventRemoving(eventType, handler))
        {
            eventDict[eventType] = (Action<T, U, V>)Delegate.Remove((Action<T, U, V>)eventDict[eventType], handler);
            OnEventRemoved(eventType);
        }
    }

    public static void RemoveEvent<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
    {
        if (OnEventRemoving(eventType, handler))
        {
            eventDict[eventType] = (Action<T, U, V, W>)Delegate.Remove((Action<T, U, V, W>)eventDict[eventType], handler);
            OnEventRemoved(eventType);
        }
    }
    static void OnEventRemoved(string eventType)
    {
        if (eventDict.ContainsKey(eventType) && (eventDict[eventType] == null))
        {
            eventDict.Remove(eventType);
        }
    }

    static bool OnEventRemoving(string eventType, Delegate EventBeingRemoved)
    {
        if (!eventDict.ContainsKey(eventType))
        {
            return false;
        }
        Delegate delegate2 = eventDict[eventType];
        if ((delegate2 != null) && (delegate2.GetType() != EventBeingRemoved.GetType()))
        {
            throw new Exception(string.Format("Remove Event {0}\" failed, Current type is {1}, adding type is {2}.", eventType, delegate2.GetType(), EventBeingRemoved.GetType()));
        }
        return true;
    }
    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="eventType">事件类型名</param>
    public static void PostEvent(string eventType)
    {
        Delegate delegate2;
        if (eventDict.TryGetValue(eventType, out delegate2))
        {
            Delegate[] invocationList = delegate2.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                Action action = invocationList[i] as Action;
                if (action == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    string.Format(exception.Message);
                }
            }
        }
    }
    public static void PostEvent<T>(string eventType, T t)
    {
        Delegate delegate2;
        if (eventDict.TryGetValue(eventType, out delegate2))
        {
            Delegate[] invocationList = delegate2.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                Action<T> action = invocationList[i] as Action<T>;
                if (action == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    action(t);
                }
                catch (Exception exception)
                {
                    string.Format(exception.Message);
                }
            }
        }
    }
    public static void PostEvent<T, U>(string eventType, T t, U u)
    {
        Delegate delegate2;
        if (eventDict.TryGetValue(eventType, out delegate2))
        {
            Delegate[] invocationList = delegate2.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                Action<T, U> action = invocationList[i] as Action<T, U>;
                if (action == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    action(t, u);
                }
                catch (Exception exception)
                {
                    string.Format(exception.Message);
                }
            }
        }
    }
    public static void PostEvent<T, U, V>(string eventType, T t, U u, V v)
    {
        Delegate delegate2;
        if (eventDict.TryGetValue(eventType, out delegate2))
        {
            Delegate[] invocationList = delegate2.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                Action<T, U, V> action = invocationList[i] as Action<T, U, V>;
                if (action == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    action(t, u, v);
                }
                catch (Exception exception)
                {
                    string.Format(exception.Message);
                }
            }
        }
    }
    public static void PostEvent<T, U, V, W>(string eventType, T t, U u, V v, W w)
    {
        Delegate delegate2;
        if (eventDict.TryGetValue(eventType, out delegate2))
        {
            Delegate[] invocationList = delegate2.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                Action<T, U, V, W> action = invocationList[i] as Action<T, U, V, W>;
                if (action == null)
                {
                    throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    action(t, u, v, w);
                }
                catch (Exception exception)
                {
                    string.Format(exception.Message);
                }
            }
        }
    }
}

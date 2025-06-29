using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class TimerManagerCommon : Manager
{
    private Dictionary<Guid, Timer> timers = new Dictionary<Guid, Timer>();
    private ConcurrentQueue<Action> queuedActions = new ConcurrentQueue<Action>();

    public Guid AddTimer(int delay, Action callback)
    {
        var timer = new Timer(_ => {
            TimerCallback(callback);
        }, null, delay, 0);
        Guid timerId = Guid.NewGuid();
        timers.Add(timerId, timer);
        return timerId;
    }

    public Guid AddRepeatedTimer(int delay, int interval, Action callback)
    {
        var timer = new Timer(_ => {
            TimerCallback(callback);
        }, null, delay, interval);
        Guid timerId = Guid.NewGuid();
        timers.Add(timerId, timer);
        return timerId;
    }

    public void RemoveTimer(Guid timerId)
    {
        if (timers.Remove(timerId, out var timer))
        {
            timer.Dispose();
        }
    }

    private void TimerCallback(Action callback)
    {
        queuedActions.Enqueue(callback);
    }

    public override void Update()
    {
        base.Update();
        HandleTimerCallbacks();
    }

    private void HandleTimerCallbacks()
    {
        int cnt = 0;
        while (cnt < Const.HandleTimerCntPerUpdate && queuedActions.TryDequeue(out Action callback))
        {
            callback();
            cnt += 1;
        }
    }
}

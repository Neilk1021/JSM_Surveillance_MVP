using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Simulator : MonoBehaviour
{
    [SerializeField] public float simulationDurationHRs = 8;
    [SerializeField] private float secondsPerTick = 0.2f;
    [SerializeField] private float simSpeed = 1;
    [SerializeField] private int ticksPerHR = 5;
    
    private float _timeSinceLastTick = 0;
    private bool _run = false;
    
    public int TotalTicks => (int)Mathf.Ceil(simulationDurationHRs * ticksPerHR);
    private int _totalTicks;
    private int _currentTicks;
    
    public float SimSpeed => simSpeed;
    public float SecondsPerTick => secondsPerTick;

    public event Action<int> OnTick;
    public readonly UnityEvent<float> OnTickProgress = new UnityEvent<float>();
    public int TicksPerHR => ticksPerHR;

    public void Run()
    {
        _currentTicks = 0;
        _totalTicks = TotalTicks;
        _run = true;
    }

    private void Stop()
    {
        _run = false;
    }
        
    private void FixedUpdate()
    {
        if(!_run) return;
        _timeSinceLastTick += Time.fixedDeltaTime;
        if (_timeSinceLastTick > SecondsPerTick)
        {
            int ticks = (int)(_timeSinceLastTick / SecondsPerTick);
            OnTick?.Invoke(ticks);
            OnTickProgress?.Invoke((float)_currentTicks/_totalTicks);
            _currentTicks += ticks;
            _timeSinceLastTick = Mathf.Max(0, _timeSinceLastTick - ticks * SecondsPerTick);
        }
        
        if(_currentTicks >= _totalTicks) Stop();
    }
}

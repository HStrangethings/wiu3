using System;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public BossState currentState;

    //keep storage for ALL the states in each boss. matches the state type to each state.
    //uses "Type" because each state will only ever have one instance of itself
    private Dictionary<Type, BossState> _states = new Dictionary<Type, BossState>();

    public void AddState(BossState state)
    {
        var type = state.GetType();
        if (!_states.ContainsKey(type))
        {
            _states.Add(type, state);
        }
    }

    public void Initialize<T>() where T : BossState
    {
        ChangeState<T>();
    }

    public void ChangeState<T>() where T : BossState
    {
        var type = typeof(T);
        if (_states.TryGetValue(type, out var newState))
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            Debug.LogError($"State {type} not found in the state machine.");
        }
    }

    private void Update()
    {
        if (Time.frameCount % 30 == 0 && currentState != null)
            Debug.Log($"[SM] currentState = {currentState.GetType().Name}");

        currentState?.Execute();
        
    }

    public void ComboFin()
    {
        currentState.ComboFin();
    }
}

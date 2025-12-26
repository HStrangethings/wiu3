using System.Collections.Generic;
using UnityEngine;

public class BossMoveMachine : MonoBehaviour
{
    //stores all the attacks to the move machine for each boss
    //works similarly to the state machine.
    //uses string because each move can have different values, parameters
    private Dictionary<string, BossMove> _moves = new Dictionary<string, BossMove>();
    public BossMove currentMove;

    public void AddMove(string id, BossMove move)
    {
        if (!_moves.ContainsKey(id))
            _moves.Add(id, move);
    }

    public void Update()
    {
        if (currentMove != null)
        {
            currentMove.Execute();

            // Let the move decide when it's done
            if (currentMove.isFinished)
            {
                currentMove.End();
                currentMove = null;
            }
        }
    }

    public void PlayMove(string id)
    {
        if (!_moves.ContainsKey(id))
        {
            Debug.LogWarning($"Move {id} not found!");
            return;
        }

        var newMove = _moves[id].Clone();

        // Do not start a new move if one is already active
        if (currentMove != null)
        {
            Debug.Log("There is a move playing right now!");
            return;
        }

        currentMove = newMove;
        currentMove.Start();
    }

    public void ForceStopMove()
    {
        currentMove.isFinished = true;
    }

    public void AnimEvent(string evt)
    {
        currentMove.AnimEvent(evt);
    }
}

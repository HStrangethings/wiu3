using System;
using System.Collections.Generic;
using UnityEngine;

public class BossMoveMachine : MonoBehaviour
{
    //stores all the attacks to the move machine for each boss
    //works similarly to the state machine.
    //uses string because each move can have different values, parameters
    private Dictionary<string, BossMove> moves = new Dictionary<string, BossMove>();
    public BossMove currentMove;
    public Queue<string> queuedMoves = new Queue<string>();
    private bool startNextFrame;

    private int comboCounter = 0;

    public float lastHitTime = -999f;
    public Type lastHitMove;

    public event Action comboFin;

    public void AddMove(string id, BossMove move)
    {
        if (!moves.ContainsKey(id))
            moves.Add(id, move);
    }

    void Update()
    {
        // Start queued move only on the next frame
        if (currentMove == null)
        {
            if (startNextFrame)
            {
                startNextFrame = false;

                if (queuedMoves.Count > 0)
                    PlayMoveImmediate(queuedMoves.Dequeue());
            }

            return;
        }

        currentMove.Execute();

        if (currentMove.isFinished)
        {
            currentMove.End();
            currentMove = null;

            // queue the start for next frame
            startNextFrame = true;
        }

    }

    public void PlayMove(string id)
    {
        if (comboCounter > 3)
        {
            Debug.Log("Move Ignored! Too many moves in one combo!");
            comboCounter = 0;
            comboFin?.Invoke();
            return;
        }
        if (id == "null")
        {
            Debug.Log("Combo Ended early!");
            comboCounter = 0;
            comboFin?.Invoke();
            return;
        }

        comboCounter++;
        // if something is playing, queue it
        if (currentMove != null)
        {
            queuedMoves.Enqueue(id);
            return;
        }

        PlayMoveImmediate(id);
    }

    private void PlayMoveImmediate(string id)
    {
        if (!moves.ContainsKey(id))
        {
            Debug.LogWarning($"Move {id} not found!");
            return;
        }

        currentMove = moves[id].Clone();
        currentMove.Start();
    }

    public void ClearQueue() => queuedMoves.Clear();

    public void ForceStopMove()
    {
        if (currentMove == null) return;
        currentMove.isFinished = true;
        // optional: also clear queued combo if you want hard cancel
        // ClearQueue();
    }

    //TODO: ADD clear queue, finish combo and test it
    public void AnimEvent(string evt)
    {
        if (currentMove == null) return;

        currentMove.AnimEvent(evt);
        
    }

    public void ReportHit(Type moveId)
    {
        lastHitTime = Time.time;
        lastHitMove = moveId;
    }

    public bool HitConfirmed(Type moveType, float window = 0.25f)
    => lastHitMove == moveType && (Time.time - lastHitTime) <= window;

    public string Choose(params string[] moves)
    {
        return moves[UnityEngine.Random.Range(0, moves.Length)];
    }
}

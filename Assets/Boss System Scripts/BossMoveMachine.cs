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

    private Queue<string> queuedMoves = new Queue<string>();

    public float lastHitTime = -999f;
    public Type lastHitMove;

    public void AddMove(string id, BossMove move)
    {
        if (!moves.ContainsKey(id))
            moves.Add(id, move);
    }

    void Update()
    {
        // If nothing is playing, try start next queued move
        if (currentMove == null)
        {
            if (queuedMoves.Count > 0)
                PlayMoveImmediate(queuedMoves.Dequeue());

            return;
        }

        currentMove.Execute();

        if (currentMove.isFinished)
        {
            currentMove.End();
            currentMove = null;

            // start next move immediately (same frame)
            if (queuedMoves.Count > 0)
                PlayMoveImmediate(queuedMoves.Dequeue());
        }
    }

    public void PlayMove(string id)
    {
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
        if (evt == "comboCheck")
        {
            // 1) identify current move
            var moveType = currentMove.GetType();

            // 2) did this move land a hit recently?
            bool hit = HitConfirmed(moveType, 0.25f);

            // 3) choose follow-up (example table)
            string nextMoveId = null;

            if (moveType == typeof(PosMeleeAttack))
            {
                nextMoveId = hit ? "posMelee2" : "posWavePush";
            }
            else if (moveType == typeof(WaterBlast))
            {
                nextMoveId = hit ? "posShotgun" : "posSmallShot";
            }

            // 4) if we picked something, queue it
            if (!string.IsNullOrEmpty(nextMoveId))
            {
                // optional: ensure we end the current move right now
                // (only do this if comboCheck is meant to be the end of the animation)
                currentMove.isFinished = true;

                // if you have a queue-enabled PlayMove, this is enough:
                PlayMove(nextMoveId);
            }

            return;
        }
        currentMove.AnimEvent(evt);
        
    }

    public void ReportHit(Type moveId)
    {
        lastHitTime = Time.time;
        lastHitMove = moveId;
    }

    public bool HitConfirmed(Type moveType, float window = 0.25f)
    => lastHitMove == moveType && (Time.time - lastHitTime) <= window;
}

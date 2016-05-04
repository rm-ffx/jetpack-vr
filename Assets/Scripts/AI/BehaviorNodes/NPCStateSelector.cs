using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class NPCStateSelector :  Composite
{
    public SharedObject npcInfo;
    private NPCInfo info;

    public int idleIndex;
    public int patrolIndex;
    public int searchIndex;
    public int attackIndex;

    // The index of the child that is currently running or is about to run.
    private int currentChildIndex = 0;

    public override void OnAwake()
    {
        // Cache Variables
        info = npcInfo.GetValue() as NPCInfo;
    }

    public override void OnStart()
    {
        // Check for the current NPC-State and set Index
        if (info.currentState == NPCInfo.npcState.Idle) currentChildIndex = idleIndex;
        else if (info.currentState == NPCInfo.npcState.Patrol) currentChildIndex = patrolIndex;
        else if (info.currentState == NPCInfo.npcState.Search) currentChildIndex = searchIndex;
        else if (info.currentState == NPCInfo.npcState.Attack) currentChildIndex = attackIndex;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
        // Check for the current NPC-State and set Index
        if (info.currentState == NPCInfo.npcState.Idle) currentChildIndex = idleIndex;
        else if (info.currentState == NPCInfo.npcState.Patrol) currentChildIndex = patrolIndex;
        else if (info.currentState == NPCInfo.npcState.Search) currentChildIndex = searchIndex;
        else if (info.currentState == NPCInfo.npcState.Attack) currentChildIndex = attackIndex;
    }

    public override int CurrentChildIndex()
    {
        return currentChildIndex;
    }

    public override bool CanExecute()
    {
        return true;
    }
}

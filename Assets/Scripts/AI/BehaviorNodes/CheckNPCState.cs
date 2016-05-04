using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets and Stores a waypoint
/// </summary>
public class CheckNPCState : Action
{
    public SharedObject npcInfo; // Contains all Info about this NPC
    private NPCInfo info;

    public bool isIdling;
    public bool isPatrolling;
    public bool isSearching;
    public bool isAttacking;

    public override void OnAwake()
    {
        info = npcInfo.GetValue() as NPCInfo;
    }

    public override TaskStatus OnUpdate()
    {
        if ((isIdling && info.currentState == NPCInfo.npcState.Idle) ||
            (isPatrolling && info.currentState == NPCInfo.npcState.Patrol) ||
            (isSearching && info.currentState == NPCInfo.npcState.Search) ||
            (isAttacking && info.currentState == NPCInfo.npcState.Attack))
            return TaskStatus.Success;
        else return TaskStatus.Failure;
    }
}

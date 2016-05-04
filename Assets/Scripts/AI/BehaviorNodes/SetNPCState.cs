using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets and Stores a waypoint
/// </summary>
public class SetNPCState : Action
{
    public SharedObject npcInfo; // Contains all Info about this NPC
    private NPCInfo info;

    public NPCInfo.npcState state;

    public override void OnAwake()
    {
        info = npcInfo.GetValue() as NPCInfo;
    }

    public override TaskStatus OnUpdate()
    {
        info.currentState = state;
        return TaskStatus.Success;
    }
}

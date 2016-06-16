using System.Linq;
using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Checks if the NPC can see the Player
/// </summary>
public class CheckSight : NPCActionNode
{
    // The tag of the targets
    public bool successIfInvisible;
    public enum checkSightType
    {
        FOV = 0,
        Spherical = 1
    }
    public checkSightType type;

    public override TaskStatus OnUpdate()
    {
        // Check if the NPC sees an Object with the given Tag
        if (CheckObjectInSight())
        {
            if (!successIfInvisible) return TaskStatus.Success;
            else return TaskStatus.Failure;
        }
        else
        {
            if (!successIfInvisible) return TaskStatus.Failure;
            else return TaskStatus.Success;
        }
    }

    /// <summary>
    /// Checks if the NPC sees an Object of Interest
    /// </summary>
    public bool CheckObjectInSight()
    {
        // Return success if a target is within sight and range
        for (int i = 0; i < GameInfo.playersInGame.Length; ++i)
        {
            if (type == checkSightType.FOV)
            {
                // Search for the Object using the NPC's FOV
                if (withinSightFov(GameInfo.playersInGame[i].transform)) return true;
            }
            else if (type == checkSightType.Spherical)
            {
                // Search for the Object using a spherical radius around the NPC
                if (withinSightSpherical(GameInfo.playersInGame[i].transform)) return true;
            }
        }
        return false;
    }
}

using System.Linq;
using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Checks if the NPC can see the Player
/// </summary>
public class CheckSight : Action
{
    public SharedObject npcInfo;
    private NPCInfo info;
    public SharedTransform target; // Target to look for, if its in sight

    // The tag of the targets
    public bool successIfInvisible;
    public enum checkSightType
    {
        FOV = 0,
        Spherical = 1
    }
    public checkSightType type;
    public float searchRadius;

    public override void OnAwake()
    {
        // Cache Variables
        info = npcInfo.GetValue() as NPCInfo;
    }

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
                if (withinSightFov(GameInfo.playersInGame[i].transform))
                {
                    return true;
                }
            }
            else if (type == checkSightType.Spherical)
            {
                // Search for the Object using a spherical radius around the NPC
                if (withinSightSpherical(GameInfo.playersInGame[i].transform))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public bool withinSightFov(Transform targetTransform)
    {
        Transform usedTransform;
        if (info.turretHead != null) usedTransform = info.turretHead;
        else usedTransform = transform;

        Vector3 direction = targetTransform.position - usedTransform.position;

        // An object is within sight if the angle is less than field of view
        if (Vector3.Angle(direction, usedTransform.forward) < info.fov)
        {
            var layerMask = 1 << 2 | 1 << 9 | 1 << 14; // Ignore NPCs, Ignore Raycast and Shield
            layerMask = ~layerMask;

            // Check if the object is obscured by something or visible
            RaycastHit hit;
            if (Physics.Raycast(info.eyePos.position, (targetTransform.position - info.eyePos.position).normalized, out hit, info.viewDistance, layerMask) && GameInfo.playersInGame.Contains(hit.transform.gameObject)) return true;
            else return false;
        }
        else return false;
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public bool withinSightSpherical(Transform targetTransform)
    {
        var layerMask = 1 << 2 | 1 << 9 | 1 << 14; // Ignore NPCs, Ignore Raycast and Shield
        layerMask = ~layerMask;
        // Check if the object is obscured by something or visible
        RaycastHit hit;
        if (Physics.Raycast(info.eyePos.position, (targetTransform.position - info.eyePos.position).normalized, out hit, searchRadius, layerMask) && GameInfo.playersInGame.Contains(hit.transform.gameObject)) return true;
        else return false;
    }
}

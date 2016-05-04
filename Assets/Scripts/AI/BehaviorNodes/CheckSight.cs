using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets and Stores a waypoint
/// </summary>
public class CheckSight : Action
{
    public SharedObject npcInfo;
    private NPCInfo info;
    public SharedTransform target; // Target to look for, if its in sight

    // The tag of the targets
    public string targetTag;
    public bool successIfInvisible;
    public enum checkSightType
    {
        FOV = 0,
        Spherical = 1
    }
    public checkSightType type;
    public float searchRadius;

    // A cache of all of the possible targets
    private Transform[] possibleTargets;

    public override void OnAwake()
    {
        // Cache Variables
        info = npcInfo.GetValue() as NPCInfo;

        // Cache all of the transforms that have a tag of targetTag
        var targets = GameObject.FindGameObjectsWithTag(targetTag);
        possibleTargets = new Transform[targets.Length];
        for (int i = 0; i < targets.Length; ++i)
        {
            possibleTargets[i] = targets[i].transform;
        }
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
        for (int i = 0; i < possibleTargets.Length; ++i)
        {
            if (type == checkSightType.FOV)
            {
                // Search for the Object using the NPC's FOV
                if (withinSightFov(possibleTargets[i]))
                {
                    return true;
                }
            }
            else if (type == checkSightType.Spherical)
            {
                // Search for the Object using a spherical radius around the NPC
                if (withinSightSpherical(possibleTargets[i]))
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
        Vector3 direction = targetTransform.position - transform.position;

        // An object is within sight if the angle is less than field of view
        if (Vector3.Angle(direction, transform.forward) < info.fov)
        {
            // Check if the object is obscured by something or visible
            RaycastHit hit;
            if (Physics.Raycast(info.eyePos.position, (targetTransform.position - info.eyePos.position).normalized, out hit, info.viewDistance) && hit.transform.gameObject.tag == targetTag) return true;
            else return false;
        }
        else return false;
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public bool withinSightSpherical(Transform targetTransform)
    {
        // Check if the object is obscured by something or visible
        RaycastHit hit;
        if (Physics.Raycast(info.eyePos.position, (targetTransform.position - info.eyePos.position).normalized, out hit, searchRadius) && hit.transform.gameObject.tag == targetTag) return true;
        else return false;
    }
}

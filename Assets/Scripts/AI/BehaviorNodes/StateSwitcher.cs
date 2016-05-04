using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets the State of the NPC
/// </summary>
public class StateSwitcher : Conditional
{
    public SharedObject npcInfo;
    private NPCInfo info;
    public SharedTransform target; // Set the target variable when a target has been found so the subsequent tasks know which object is the target

    // The tag of the targets
    public string targetTag;

    private NPCInfo.npcState lastState; // Last stored State

    // A cache of all of the possible targets
    private Transform[] possibleTargets;

    public override void OnAwake()
    {
        // Cache Variables
        info = npcInfo.GetValue() as NPCInfo;
        lastState = info.currentState;

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
        if (CheckSight() && lastState != info.currentState) { lastState = info.currentState; return TaskStatus.Success; }

        lastState = info.currentState;
        // Keep Checking for Changes
        return TaskStatus.Running;
    }

    /// <summary>
    /// Checks if the NPC sees an Object of Interest
    /// </summary>
    public bool CheckSight()
    {
        // Return success if a target is within sight and range
        for (int i = 0; i < possibleTargets.Length; ++i)
        {
            if (withinSight(possibleTargets[i], info.fov))
            {
                // Set the target so other tasks will know which transform is within sight
                target.Value = possibleTargets[i];

                // Set State
                info.currentState = NPCInfo.npcState.Attack;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public bool withinSight(Transform targetTransform, float fieldOfViewAngle)
    {
        Vector3 direction = targetTransform.position - transform.position;

        // An object is within sight if the angle is less than field of view
        if (Vector3.Angle(direction, transform.forward) < fieldOfViewAngle)
        {
            // Check if the object is obscured by something or visible
            RaycastHit hit;
            if (Physics.Raycast(info.eyePos.position, (targetTransform.position - info.eyePos.position).normalized, out hit, info.viewDistance) && hit.transform.gameObject.tag == targetTag) return true;
            else return false;
        }
        else return false;
    }
}
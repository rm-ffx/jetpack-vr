using System.Linq;
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

    private NPCInfo.npcState lastState; // Last stored State

    public override void OnAwake()
    {
        // Cache Variables
        info = npcInfo.GetValue() as NPCInfo;
        lastState = info.currentState;
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
        for (int i = 0; i < GameInfo.playersInGame.Length; ++i)
        {
            if (withinSight(GameInfo.playersInGame[i].transform, info.fov))
            {
                // Set the target so other tasks will know which transform is within sight
                target.Value = GameInfo.playersInGame[i].transform;

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
        Transform usedTransform;

        if (info.turretHead != null) usedTransform = info.turretHead;
        else usedTransform = transform;

        Vector3 direction = targetTransform.position - usedTransform.position;

        // An object is within sight if the angle is less than field of view
        if (Vector3.Angle(direction, usedTransform.forward) < fieldOfViewAngle)
        {
            var layerMask = 1 << 2 | 1 << 9; // Ignore NPCs and Ignore Raycast
            layerMask = ~layerMask;

            // Check if the object is obscured by something or visible
            RaycastHit hit;
            if (Physics.Raycast(info.eyePos.position, (targetTransform.position - info.eyePos.position).normalized, out hit, info.viewDistance, layerMask) && GameInfo.playersInGame.Contains(hit.transform.gameObject)) return true; // hit.transform.gameObject.tag == targetTag) return true;
            else return false;
        }
        else return false;
    }
}
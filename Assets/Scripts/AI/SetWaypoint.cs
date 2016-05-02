using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets and Stores a waypoint
/// </summary>
public class SetWaypoint : Action
{
    public SharedTransform foundWaypoint; // Next waypoint, the robot will walk to

    public Transform[] waypoints; // Array of all possible waypoints
    
    public override TaskStatus OnUpdate()
    {
        // Set Random Waypoint
        int currentWaypoint = Random.Range(0, waypoints.Length);
        foundWaypoint.SetValue(waypoints[currentWaypoint]);
        return TaskStatus.Success;
    }
}

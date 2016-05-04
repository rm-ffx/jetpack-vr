using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets and Stores a waypoint
/// </summary>
public class SetWaypoint : Action
{
    public SharedTransform generatedWaypoint; // Next waypoint, the robot will walk to
    public SharedTransform target; // CurrentTarget

    public enum waypointSelection
    {
        Random = 0,
        Target = 1
    }
    public waypointSelection waypointType = waypointSelection.Random;
    public Transform[] randomWaypoints; // Array of all possible waypoints
    
    public override TaskStatus OnUpdate()
    {
        // Generate Waypoint based on selected type
        switch (waypointType)
        {
            case waypointSelection.Random:
                // Set Random Waypoint
                int currentWaypoint = Random.Range(0, randomWaypoints.Length);
                generatedWaypoint.SetValue(randomWaypoints[currentWaypoint]);
                break;

            case waypointSelection.Target:
                break;
        }

        return TaskStatus.Success;
    }
}

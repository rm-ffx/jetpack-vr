using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Moves the NPC via Pathfinding in Wandering Motion
/// </summary>
public class PathWander : Action
{
    //public SharedObject npcInfo; // Contains all Info about this NPC
    //private NPCInfo info;

    public SharedObject animator;
    private Animator anim;
    public bool triggerAnimation;
    private List<string> triggers;
    public string animTrigger;

    public SharedObject seeker; // Contains the Seeker Object
    private Seeker pathSeeker;

    public bool useRVO;
    public SharedObject rvoController; // Contains the RVO Controller
    private RVOController controller;

    public enum wanderType
    {
        Random = 0,
        Area = 1
    }
    public wanderType type;
    public SharedTransformList wanderWaypoints; // List of all possible waypoints
    private List<Transform> wanderWaypointsList; // Array of all possible waypoints
    public SharedVector3 areaWanderPosition; // Around which point will the NPC generate own points
    public float areaRadius; // In Which Radius will the NPC generate an own point in Area Mode

    // Movement Variables
    public float moveSpeed;
    public float rotSpeed;
    public float characterHeight; // Height of the Character
    private float errorMargin; // How close does the robot need to get before the next waypoint is triggered.

    // Path Generation Variables
    private Path path; // Currently active Path
    private int currentPathWaypoint;
    private int currentWanderWaypoint;
    private int lastWanderWaypoint; // Currently active Waypoint
    private bool m_generatePath = true;

    public override void OnAwake()
    {
        // Cache Variables
        //info = npcInfo.GetValue() as NPCInfo;
        pathSeeker = seeker.GetValue() as Seeker;
        wanderWaypointsList = wanderWaypoints.GetValue() as List<Transform>;

        if (triggerAnimation)
        {
            anim = animator.GetValue() as Animator;
            AnimatorControllerParameter[] parameters = anim.parameters;
            triggers = new List<string>();
            for (int i = 0; i < parameters.Length; i++) triggers.Add(parameters[i].name);
        }
        if (useRVO) controller = rvoController.GetValue() as RVOController;
    }

    public override void OnStart()
    {
        errorMargin = moveSpeed * 0.05f;

        // Generate Path
        path = null;
        currentPathWaypoint = 1;

        if (triggerAnimation)
        {
            for (int i = 0; i < triggers.Count; i++) anim.ResetTrigger(triggers[i]);
            anim.SetTrigger(animTrigger);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (m_generatePath && path == null) CreatePath();

        // Moves the Character
        return MoveCharacter();
    }

    /// <summary>
    /// Generates the Path to the next wanderWaypoint
    /// </summary>
    public void CreatePath()
    {
        m_generatePath = false;

        if (type == wanderType.Random)
        {
            while (currentWanderWaypoint == lastWanderWaypoint)
            {
                currentWanderWaypoint = Random.Range(0, wanderWaypointsList.Count);
            }
            lastWanderWaypoint = currentWanderWaypoint;

            // Generate Path
            pathSeeker.StartPath(transform.position, wanderWaypointsList[currentWanderWaypoint].transform.position, OnPathComplete);
        }
        else if (type == wanderType.Area)
        {
            // Generate new point until a reachable point was found
            //bool canReach = false;
            Vector3 newPoint = Vector3.zero;
            Vector3 areaPos = (Vector3)areaWanderPosition.GetValue();
            //while (!canReach)
            //{
            //    // Generate a Waypoint in an area around the given position
            newPoint = new Vector3(areaPos.x + Random.Range(areaRadius * -1, areaRadius), areaPos.y, areaPos.z + Random.Range(areaRadius * -1, areaRadius));

            //    //set your constraints on a new instance for distance, graphmask ...etc
            //    NNConstraint pathConstraint = NNConstraint.Default;
            //    GraphNode node1 = AstarPath.active.GetNearest(transform.position, pathConstraint).node;
            //    GraphNode node2 = AstarPath.active.GetNearest(newPoint, pathConstraint).node;
            //    canReach = (node1 != null && node2 != null && PathUtilities.IsPathPossible(node1, node2));
            //}
            Debug.DrawRay(newPoint, Vector3.up * 3, Color.green, 10f);
            // Generate Path
            pathSeeker.StartPath(transform.position, newPoint, OnPathComplete);
        }
    }

    /// <summary>
    /// Moves and Rotates the Character
    /// </summary>
    public TaskStatus MoveCharacter()
    {
        // Move Character towards next waypoint
        if (path != null)
        {
            // Path only has one Waypoint - NPC is at target position already
            if (path.vectorPath.Count == 1)
            {
                path = null;
                currentPathWaypoint = 1;
                if (useRVO) controller.Move(Vector3.zero);
                return TaskStatus.Success;
            }

            ///////////////////////
            // SET POSITION
            // Get new Position
            Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
            Vector2 waypointPos = new Vector2(path.vectorPath[currentPathWaypoint].x, path.vectorPath[currentPathWaypoint].z);
            Vector2 dir = (waypointPos - currentPos).normalized;
            Vector2 newPos = currentPos + dir * moveSpeed * Time.deltaTime;

            // Get position Height
            RaycastHit hit;
            float height = 0;
            if (Physics.Raycast(new Vector3(newPos.x, transform.position.y + characterHeight, newPos.y), -Vector3.up * characterHeight * 2, out hit)) height = hit.point.y;

            /////////////////////
            // SET ROTATION
            Quaternion currentRotation = transform.rotation;
            transform.LookAt(new Vector3(newPos.x, transform.position.y, newPos.y));
            Quaternion newRotation = transform.rotation;
            transform.rotation = Quaternion.Slerp(currentRotation, newRotation, rotSpeed * Time.deltaTime);

            // Move Character
            if (!useRVO) transform.position = new Vector3(newPos.x, height, newPos.y);
            else controller.Move((new Vector3(newPos.x, transform.position.y, newPos.y) - transform.position).normalized * moveSpeed);

            // Check if the Character's distance to the target location is close enough to move on to the next waypoint
            if (Vector2.Distance(newPos, waypointPos) < errorMargin) currentPathWaypoint++;

            // Target Reached - Abort Movement
            if (currentPathWaypoint == path.vectorPath.Count)
            {
                path = null;
                currentPathWaypoint = 1;
                if (useRVO) controller.Move(Vector3.zero);
                return TaskStatus.Success;
            }
            else return TaskStatus.Running;
        }
        else return TaskStatus.Running;
    }

    /// <summary>
    /// Callback when a path was generated by the seeker
    /// </summary>
    public void OnPathComplete(Path p)
    {
        //We got our path back
        if (p.error)
        {
            Debug.Log(p.errorLog);
        }
        else
        {
            // Completed the Path Generation
            m_generatePath = true;
            path = p;
            currentPathWaypoint = 1;
        }
    }

    public override void OnReset()
    {
        currentPathWaypoint = 1;
        path = null;
    }
}

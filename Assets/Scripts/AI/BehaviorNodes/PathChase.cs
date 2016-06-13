using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Moves the NPC via Pathfinding - NPC chases Target transform as long as it is visible, then triggers VisibilityBuffer.
/// Returns success If the NPC is invisible but in "closeEnough" range -> Move on to "Searching" immediatly.
/// </summary>

public class PathChase : Action
{
    public SharedObject npcInfo; // Contains all Info about this NPC
    private NPCInfo info;

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

    public SharedTransform target; // Target that is going to be chased
    private Transform chaseTarget;

    public SharedVector3 lastSeenTargetPos; // When the Target disappears, the last seen position gets stored here

    public int repathFrequency; // Ticks until a re-calculation of the path towards the target happens
    private int m_currentRepathCounter = 0;

    [UnityEngine.Tooltip("How long is this NPC still chasing the target, even if the Target is no longer visible")]
    public int visibilityBuffer;
    private int m_currentVisibilityBuffer;

    public int minChaseTime; // How long will the NPC Chase the Target min, before Success is triggered?
    public int maxChaseTime; // How long will the NPC Chase the Target max, before Success is triggered?
    private int m_currentChaseTime;
    private int m_currentMaxChaseTime;

    // Movement Variables
    public float moveSpeed;
    public float rotSpeed;
    public float closeEnough;

    public float characterHeight; // Height of the Character
    private float errorMargin; // How close does the robot need to get before the next waypoint is triggered.

    // Path Generation Variables
    private Path path; // Currently active Path
    private int currentPathWaypoint;
    private bool m_generatePath = true;

    public override void OnAwake()
    {
        // Cache Variables
        info = npcInfo.GetValue() as NPCInfo;
        pathSeeker = seeker.GetValue() as Seeker;

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
        chaseTarget = target.GetValue() as Transform;
        m_currentVisibilityBuffer = 0;
        m_currentChaseTime = 0;
        m_currentMaxChaseTime = Random.Range(minChaseTime, maxChaseTime + 1);

        errorMargin = moveSpeed * 0.05f;

        // Generate Path
        path = null;
        currentPathWaypoint = 1;

        // Only Trigger the Animation if movement is actually going to be performed - Check closeEnough distance as well!
        if (triggerAnimation && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(chaseTarget.position.x, chaseTarget.position.z)) > closeEnough)
        {
            for (int i = 0; i < triggers.Count; i++) anim.ResetTrigger(triggers[i]);
            anim.SetTrigger(animTrigger);
        }
    }

    public override TaskStatus OnUpdate()
    {
        // Store the last known position of the Target in this task for further use by other tasks
        lastSeenTargetPos.SetValue(chaseTarget.transform.position);

        // Check if the Target is Visible to the NPC
        if (withinSightSpherical(chaseTarget)) m_currentVisibilityBuffer = 0;
        else { m_currentVisibilityBuffer++; m_currentChaseTime = 0; }

        if (m_currentVisibilityBuffer >= visibilityBuffer || m_currentChaseTime >= m_currentMaxChaseTime)
        {
            if (useRVO) controller.Move(Vector3.zero);
            return TaskStatus.Success;
        }

        // Target is Visible, or buffer didnt expire by now - continue pathing
        if ((m_generatePath && path == null) || m_currentRepathCounter >= repathFrequency) CreatePath();
        m_currentRepathCounter++;
        m_currentChaseTime++;

        // Moves the Character
        return MoveCharacter();
    }

    /// <summary>
    /// Generates a new Path to the target
    /// </summary>
    public void CreatePath()
    {
        m_generatePath = false;
        pathSeeker.StartPath(transform.position, (target.GetValue() as Transform).position, OnPathComplete);
        m_currentRepathCounter = 0;
    }

    /// <summary>
    /// Moves and Rotates the Character
    /// </summary>
    public TaskStatus MoveCharacter()
    {
        // Move Character towards next waypoint
        if (path != null)
        {
            // Path only has one Waypoint or is close enough - NPC is at target position already
            if (path.vectorPath.Count == 1 ||
                Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(chaseTarget.position.x, chaseTarget.position.z)) <= closeEnough)
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
        if (p.error) Debug.Log("Error: " + p.errorLog);
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

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public bool withinSightSpherical(Transform targetTransform)
    {
        // Check if the object is obscured by something or visible
        var layerMask = 1 << 2 | 1 << 9 | 1 << 10 | 1 << 14; // Ignore NPCs, Ignore Raycast, Controller and Shield

        //var layerMask = 1 << 9;
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(info.eyePos.position, (targetTransform.position - info.eyePos.position).normalized, out hit, info.viewDistance, layerMask) && hit.transform == targetTransform) return true;
        else return false;
    }
}

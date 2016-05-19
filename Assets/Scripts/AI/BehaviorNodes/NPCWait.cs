using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets and Stores a waypoint
/// </summary>
public class NPCWait : Action
{
    public SharedObject animator;
    private Animator anim;
    public bool triggerAnimation;
    private List<string> triggers;
    public string animTrigger;

    public SharedFloat waitTime = 1;
    public SharedBool randomWait = false;
    public SharedFloat randomWaitMin = 1;
    public SharedFloat randomWaitMax = 1;

    // The time to wait
    private float waitDuration;
    // The time that the task started to wait.
    private float startTime;
    // Remember the time that the task is paused so the time paused doesn't contribute to the wait time.
    private float pauseTime;

    public override void OnAwake()
    {
        if (triggerAnimation)
        {
            anim = animator.GetValue() as Animator;
            if (anim)
            {
                AnimatorControllerParameter[] parameters = anim.parameters;
                triggers = new List<string>();
                for (int i = 0; i < parameters.Length; i++) triggers.Add(parameters[i].name);
            }
        }
    }

    public override void OnStart()
    {
        // Remember the start time.
        startTime = Time.time;
        if (randomWait.Value)
        {
            waitDuration = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
        }
        else
        {
            waitDuration = waitTime.Value;
        }

        if (triggerAnimation)
        {
            for (int i = 0; i < triggers.Count; i++) anim.ResetTrigger(triggers[i]);
            anim.SetTrigger(animTrigger);
        }
    }

    public override TaskStatus OnUpdate()
    {
        // The task is done waiting if the time waitDuration has elapsed since the task was started.
        if (startTime + waitDuration < Time.time)
        {
            return TaskStatus.Success;
        }
        // Otherwise we are still waiting.
        return TaskStatus.Running;
    }

    public override void OnPause(bool paused)
    {
        if (paused)
        {
            // Remember the time that the behavior was paused.
            pauseTime = Time.time;
        }
        else
        {
            // Add the difference between Time.time and pauseTime to figure out a new start time.
            startTime += (Time.time - pauseTime);
        }
    }

    public override void OnReset()
    {
        // Reset the public properties back to their original values
        waitTime = 1;
        randomWait = false;
        randomWaitMin = 1;
        randomWaitMax = 1;
    }
}

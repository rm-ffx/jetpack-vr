using System.Linq;
using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
///Triggers an Animation on the NPC
/// </summary>
public class NPCTriggerAnimation : Action
{
    public SharedObject animator;
    private Animator anim;

    public string trigger;

    public override void OnAwake()
    {
        // Cache Variables
        anim = animator.GetValue() as Animator;
    }

    public override TaskStatus OnUpdate()
    {
        anim.SetTrigger(trigger);
        return TaskStatus.Success;
    }
}

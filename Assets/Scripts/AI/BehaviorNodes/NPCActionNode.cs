using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public abstract class NPCActionNode : Action
{
    // Base NPC Info
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Stored NPC-Info of this NPC")]
    public SharedObject npcInfo;
    public NPCInfo m_info { private set; get; }

    // Animation Variables
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Stored Animator of this NPC")]
    public SharedObject animator;
    public Animator anim { private set; get; }
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Should this action trigger an Animation?")]
    public bool triggerAnimation;
    public List<string> triggers { private set; get; }
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Animation to trigger.")]
    public string animTrigger;

    // Does this NPC use an RVO-Controller?
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Stored RVO Controller.")]
    public SharedObject rvoController;
    public RVOController m_rvoController { private set; get; }

    // CurrentTarget of this NPC Action
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Target of this Action.")]
    public SharedTransform target;
    public Transform m_currentTarget { private set; get; }

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Rotation Speed of this NPC.")]
    public float rotationSpeed;

    public abstract override TaskStatus OnUpdate();

    public override void OnAwake()
    {
        m_info = npcInfo.GetValue() as NPCInfo;

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
        if (rvoController.GetValue() != null) m_rvoController = rvoController.GetValue() as RVOController;
    }

    public override void OnStart()
    {
        // Get the current target everytime the node is executed
        m_currentTarget = target.GetValue() as Transform;

        // Trigger Animation
        if (triggerAnimation)
        {
            for (int i = 0; i < triggers.Count; i++) anim.ResetTrigger(triggers[i]);
            anim.SetTrigger(animTrigger);
        }
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public virtual bool withinSightSpherical(Transform targetTransform)
    {
        // Check if the object is obscured by something or visible
        var layerMask = 1 << 2 | 1 << 9 | 1 << 10 | 1 << 14; // Ignore: NPCs, Ignore Raycast, Controller and Shield
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(m_info.eyePos.position, (targetTransform.position - m_info.eyePos.position).normalized, out hit, m_info.viewDistance, layerMask) && hit.transform == targetTransform) return true;
        else return false;
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public virtual bool withinSightFov(Transform targetTransform)
    {
        Transform usedTransform;
        if (m_info.turretHead != null) usedTransform = m_info.turretHead;
        else usedTransform = m_info.eyePos.transform;

        Vector3 direction = targetTransform.position - usedTransform.position;

        // An object is within sight if the angle is less than field of view
        if (Vector3.Angle(direction, usedTransform.forward) < m_info.fov)
        {
            var layerMask = 1 << 2 | 1 << 9 | 1 << 10 | 1 << 14; // Ignore: NPCs, Ignore Raycast, Controller and Shield
            layerMask = ~layerMask;

            // Check if the object is obscured by something or visible
            RaycastHit hit;
            if (Physics.Raycast(m_info.eyePos.position, (targetTransform.position - m_info.eyePos.position).normalized, out hit, m_info.viewDistance, layerMask) && GameInfo.playersInGame.Contains(hit.transform.gameObject)) return true;
            else return false;
        }
        else return false;
    }
}

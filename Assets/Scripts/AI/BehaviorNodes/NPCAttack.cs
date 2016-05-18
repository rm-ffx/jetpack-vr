using UnityEngine;
using System.Collections.Generic;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Makes the NPC Attack the Target
/// </summary>
public class NPCAttack : Action
{
    public SharedObject npcInfo;
    private NPCInfo m_info;

    public SharedObject animator;
    private Animator anim;
    public bool triggerAnimation;
    private List<string> triggers;
    public string animTrigger;

    public bool useRVO;
    public SharedObject rvoController;
    private RVOController controller;

    public SharedTransform target; // CurrentTarget
    public float rotationSpeed;

    private Transform m_currentTarget;
    public int minAttacks; // Minimum Number of Attacks
    public int maxAttacks; // Maximum Number of Attacks

    private int m_currentAttacks;
    private int m_currentMaxAttacks;

    public int aimTime; // Time it takes between each shot
    private int m_currentAimTime;

    private bool m_charVisibleAtStart = true; // Was the character visible at the start of this node's execution?

    public float accuracy; // How accurate does the NPC shoot? 

    public bool isTurret; // Is this NPC a turret?

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
        if (useRVO) controller = rvoController.GetValue() as RVOController;
    }

    public override void OnStart()
    {
        // Stop Movement
        if (useRVO) controller.Move(Vector3.zero);
        
        // Set Variables
        m_currentAttacks = 0;
        m_currentMaxAttacks = Random.Range(minAttacks, maxAttacks + 1);
        m_currentAimTime = 0;
        m_currentTarget = target.GetValue() as Transform;

        // Check Visibility - is it necessary to perform and attack? Is character visible?
        m_charVisibleAtStart = withinSightSpherical(m_currentTarget);

        // Trigger Animation
        if (triggerAnimation)
        {
            for (int i = 0; i < triggers.Count; i++) anim.ResetTrigger(triggers[i]);
            anim.SetTrigger(animTrigger);
        }
    }

    public override TaskStatus OnUpdate()
    {
        // If Character was not visible at Start - Nothing to shoot at! Return Success!
        if (!m_charVisibleAtStart) return TaskStatus.Success;

        // SET ROTATION
        if (!isTurret)
        {
            Quaternion currentRotation = transform.rotation;
            transform.LookAt(new Vector3(m_currentTarget.position.x, transform.position.y, m_currentTarget.position.z));
            Quaternion newRotation = transform.rotation;
            transform.rotation = Quaternion.Slerp(currentRotation, newRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion currentRotation = m_info.turretHead.transform.rotation;
            //m_info.turretHead.transform.LookAt(new Vector3(m_currentTarget.position.x, transform.position.y, m_currentTarget.position.z));
            m_info.turretHead.transform.LookAt(new Vector3(m_currentTarget.position.x, m_currentTarget.position.y, m_currentTarget.position.z));

            Quaternion newRotation = m_info.turretHead.transform.rotation;
            m_info.turretHead.transform.rotation = Quaternion.Slerp(currentRotation, newRotation, rotationSpeed * Time.deltaTime);
        }

        // Wait for AimTime to pass
        if (m_currentAimTime < aimTime) { m_currentAimTime++; return TaskStatus.Running; }
        else
        {
            // Check Vis before attack! If (after aiming) vis is blocked --> dont shoot, but return success.
            if (!withinSightSpherical(m_currentTarget)) return TaskStatus.Success;

            // Perform Attack
            //Debug.DrawLine(transform.position, m_currentTarget.position, Color.red, 0.25f);
            for (int i = 0; i < m_info.gunPos.Length; i++)
            {
                GameObject.Instantiate(Resources.Load(m_info.projectile.name), m_info.gunPos[i].position, Quaternion.LookRotation(m_currentTarget.position - m_info.gunPos[i].position, Vector3.up));
            }
            m_currentAimTime = 0;
            m_currentAttacks++;

            // Check if Another Attack should be performed, or if success should be returned (max attack reached)
            if (m_currentAttacks >= m_currentMaxAttacks) return TaskStatus.Success;
            else return TaskStatus.Running;
        }
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public bool withinSightSpherical(Transform targetTransform)
    {
        // Check if the object is obscured by something or visible
        RaycastHit hit;
        if (Physics.Raycast(m_info.eyePos.position, (targetTransform.position - m_info.eyePos.position)*1.25f, out hit) && hit.transform == targetTransform) return true;
        else return false;
    }
}

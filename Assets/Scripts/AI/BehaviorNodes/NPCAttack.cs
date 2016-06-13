using UnityEngine;
using System.Collections.Generic;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Makes the NPC Attack the Target
/// </summary>
public class NPCAttack : NPCActionNode
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Minimum Number of fired shots")]
    public int minAttacks;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Maximum Number of fired shots")]
    public int maxAttacks;

    private int m_currentAttacks;
    private int m_currentMaxAttacks;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The Time it takes between each fired shot")]
    public int aimTime;
    private int m_currentAimTime;

    private bool m_charVisibleAtStart = true; // Was the character visible at the start of this node's execution?

    [BehaviorDesigner.Runtime.Tasks.Tooltip("How accurate does the NPC shoot?")]
    public float accuracy;

    // Is this NPC a turret? Relevant for Rotation!
    public bool isTurret;

    public override void OnStart()
    {
        base.OnStart();

        // Stop Movement
        if (m_rvoController != null) m_rvoController.Move(Vector3.zero);
        
        // Set Attack Variables
        m_currentAttacks = 0;
        m_currentMaxAttacks = UnityEngine.Random.Range(minAttacks, maxAttacks + 1);
        m_currentAimTime = 0;

        // Check Visibility - is it necessary to perform an attack? Is the target visible?
        m_charVisibleAtStart = withinSightSpherical(m_currentTarget);
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
            for (int i = 0; i < m_info.gunPos.Length; i++)
            {
                if (!m_info.turretHead) GameObject.Instantiate(Resources.Load(m_info.projectile.name), m_info.gunPos[i].position, Quaternion.LookRotation(m_currentTarget.position - m_info.gunPos[i].position, Vector3.up));
                else GameObject.Instantiate(Resources.Load(m_info.projectile.name), m_info.gunPos[i].position, Quaternion.LookRotation(m_currentTarget.position - m_info.turretHead.position, Vector3.up));
            }
            m_currentAimTime = 0;
            m_currentAttacks++;

            // Check if Another Attack should be performed, or if success should be returned (max attack reached)
            if (m_currentAttacks >= m_currentMaxAttacks) return TaskStatus.Success;
            else return TaskStatus.Running;
        }
    }
}

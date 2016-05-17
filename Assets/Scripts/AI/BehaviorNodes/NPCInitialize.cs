using System.Collections.Generic;
using UnityEngine;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// NPC Initialization
/// </summary>
public class NPCInitialize : Action
{
    public SharedObject npcInfo; // NPC Info that stores all information of the NPC
    public SharedObject seeker; // Seeker for Path Generation
    public SharedTransformList waypointTargets; // Targets for Path-Generation
    public SharedObject rvoController;
    public SharedObject animator; // Animator of the NPC

    private bool m_isInitialized = false;

    public override void OnAwake()
    {
        if (!m_isInitialized)
        {
            m_isInitialized = true;

            // Initialize Variables of this NPC
            npcInfo.SetValue(GetComponent<NPCInfo>());
            seeker.SetValue(GetComponent<Seeker>());
            rvoController.SetValue(GetComponent<RVOController>());
            animator.SetValue(gameObject.GetComponentInChildren<Animator>());
        }
        // Get all Target Transforms in the Scene
        List<Transform> targetsTransform = GameInfo.npcPatrolWaypoints;
        waypointTargets.SetValue(targetsTransform);
    }
}

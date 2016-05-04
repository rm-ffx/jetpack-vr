using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Sets and Stores a waypoint
/// </summary>
public class NPCInitialize : Action
{
    public SharedObject npcInfo; // NPC Info that stores all information of the NPC
    public SharedObject seeker; // Seeker for Path Generation

    private bool m_isInitialized = false;

    public override void OnAwake()
    {
        if (!m_isInitialized)
        {
            m_isInitialized = true;

            // Initialize Variables of this NPC
            npcInfo.SetValue(GetComponent<NPCInfo>());
            seeker.SetValue(GetComponent<Seeker>());
        }
    }
}

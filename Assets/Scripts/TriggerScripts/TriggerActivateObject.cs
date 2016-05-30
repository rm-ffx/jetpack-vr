using UnityEngine;
using System.Collections;

public class TriggerActivateObject : TriggerScript
{
    [Tooltip("The GameObject that will be activated/deactivated.")]
    public ActivateObjectScript activateObjectScript;

	public override void Activate()
    {
        if(!m_isActive)
        {
            m_isActive = true;
            if (activateObjectScript != null)
                activateObjectScript.Activate();
        }
    }

    public override void Deactivate()
    {
        if(m_isActive)
        {
            m_isActive = false;
            if (activateObjectScript != null)
                activateObjectScript.Deactivate();
        }
    }
}

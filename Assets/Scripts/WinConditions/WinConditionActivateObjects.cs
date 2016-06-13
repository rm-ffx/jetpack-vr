using UnityEngine;
using System.Collections;

public class WinConditionActivateObjects : WinCondition
{
    [Tooltip("How many objects need to be activated before the WinCondition is fullfilled.")]
    public int requiredObjects = 1;
    private int m_activeObjects = 0;

    public void ActivateObject()
    {
        m_activeObjects++;
        if (m_activeObjects >= requiredObjects)
            triggerWinCondition.Activate();
    }
    
    public void DeactivateObject()
    {
        m_activeObjects--;
        if (m_activeObjects < requiredObjects)
            triggerWinCondition.Deactivate();
    }
}

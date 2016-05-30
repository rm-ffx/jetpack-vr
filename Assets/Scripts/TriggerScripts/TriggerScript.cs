using UnityEngine;
using System.Collections;

public abstract class TriggerScript : MonoBehaviour
{
    protected bool m_isActive = false;
    public abstract void Activate();
    public abstract void Deactivate();
}

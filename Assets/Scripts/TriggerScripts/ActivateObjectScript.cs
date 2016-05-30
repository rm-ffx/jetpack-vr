using UnityEngine;
using System.Collections;

public abstract class ActivateObjectScript : MonoBehaviour
{
    [HideInInspector]
    public bool isActive = false;

    [Tooltip("Whether or not activating this objects counts towards a WinCondition.")]
    public bool countsForWinCondition = false;
    [Tooltip("The WinCondition. Will only be used if CountsForWinCondition is true.")]
    public WinConditionActivateObjects activateObjectsWinCondition;

	public abstract void Activate();
	public abstract void Deactivate();
}

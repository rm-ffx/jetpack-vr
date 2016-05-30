using UnityEngine;
using System.Collections;

public class ActivateObjectSwapMaterial : ActivateObjectScript
{
    [Tooltip("The material used when the object is active.")]
    public Material activeMaterial;
    [Tooltip("The material used when the object is deactivated.")]
    public Material deactivatedMaterial;

    public override void Activate()
    {
        isActive = true;
        GetComponent<MeshRenderer>().material = activeMaterial;
        if (countsForWinCondition)
            if (activateObjectsWinCondition != null)
                activateObjectsWinCondition.ActivateObject();

    }

    public override void Deactivate()
    {
        isActive = false;
        GetComponent<MeshRenderer>().material = deactivatedMaterial;
        if (countsForWinCondition)
            if (activateObjectsWinCondition != null)
                activateObjectsWinCondition.DeactivateObject();
    }
}

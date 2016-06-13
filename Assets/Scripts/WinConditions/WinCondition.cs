using UnityEngine;
using System.Collections;

public abstract class WinCondition : MonoBehaviour
{
    [Tooltip("The script that will be triggered once the WinCondition is fullfilled.")]
    public TriggerScript triggerWinCondition;
}

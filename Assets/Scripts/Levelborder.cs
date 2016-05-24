using UnityEngine;
using System.Collections;

public class Levelborder : MonoBehaviour
{
    private float m_TimerCounter;
    private bool m_isOutsideBorder = false;
    private PickupSystem m_pickUpSystemLeft;
    private PickupSystem m_pickUpSystemRight;
    private JetpackMovement m_jetpackMovementLeft;
    private JetpackMovement m_jetpackMovementRight;
    private float m_upwardMultiplier;
    
    public GameObject LeftController;
    public GameObject RightController;
    public int WarningTime;

    void Start()
    {
        m_pickUpSystemLeft = LeftController.GetComponent<PickupSystem>();
        m_pickUpSystemRight = RightController.GetComponent<PickupSystem>();
        m_jetpackMovementLeft = LeftController.GetComponent<JetpackMovement>();
        m_jetpackMovementRight = RightController.GetComponent<JetpackMovement>();
        m_upwardMultiplier = m_jetpackMovementLeft.upwardMultiplier;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Warning you are leaving the flyable area!");
            m_isOutsideBorder = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            Debug.Log("In Level");
            m_isOutsideBorder = false;
            m_TimerCounter = WarningTime;
            m_pickUpSystemLeft.enabled = true;
            m_pickUpSystemRight.enabled = true;
            m_jetpackMovementLeft.upwardMultiplier = m_upwardMultiplier;
            m_jetpackMovementRight.upwardMultiplier = m_upwardMultiplier;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (m_isOutsideBorder)
        {
            CountDown();
        }
    }

    void CountDown()
    {
        if (m_TimerCounter > 0)
            m_TimerCounter -= Time.deltaTime;
        Debug.Log("time left: " + (int)m_TimerCounter);
        if (m_TimerCounter <= 0)
        {
            Debug.Log("Jet Pack Error!");
            m_pickUpSystemLeft.enabled = false;
            m_pickUpSystemRight.enabled = false;
            m_jetpackMovementLeft.upwardMultiplier = 0.0f;
            m_jetpackMovementRight.upwardMultiplier = 0.0f;
        }
    }
}

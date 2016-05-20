﻿using UnityEngine;
using System.Collections;
using Valve.VR;

/// <summary>
/// Handles the raycast gun gadget
/// </summary>
public class RaycastGun : MonoBehaviour
{
    [Tooltip("How many seconds the gun needs to be ready again")]
    public float Cooldown;
    [Tooltip("The damage this weapon deals")]
    public float Damage;
    [Tooltip("The model that will be used for the gadget selector")]
    public GameObject GadgetPreviewPrefab;
    [Tooltip("The model that will be used as pointer. Note that this is only used for visual feedback")]
    public GameObject PointerModel;

    private SteamVR_Controller.Device m_device = null;
    private PickupSystem m_pickupSystem;
    private float m_triggerX;
    private float m_remainingCooldown;
    private Vector3 m_pointerModelLocalScale;

    // Use this for initialization
    void Start ()
    {
        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);
        m_pickupSystem = GetComponent<PickupSystem>();
        m_triggerX = 0.0f;
        PointerModel.SetActive(false);
        m_pointerModelLocalScale = PointerModel.transform.localScale;
    }

    void OnDisable()
    {
        PointerModel.SetActive(false);
    }

    void OnEnable()
    {
        PointerModel.SetActive(true);
    }

    // Update is called once per frame
    void Update ()
    {
        if (!m_pickupSystem.m_isHandBusy)
        {
            m_triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;
            if (!PointerModel.activeInHierarchy)
                PointerModel.SetActive(true);
        }
        else
            PointerModel.SetActive(false);

        if (m_triggerX >= 0.1f && m_remainingCooldown <= 0.0f)
        {
            Shoot();
            PointerModel.transform.localScale = new Vector3(m_pointerModelLocalScale.x * 10.0f, m_pointerModelLocalScale.y, m_pointerModelLocalScale.z * 10.0f);
        }
        else
        {
            m_remainingCooldown -= Time.deltaTime;
            if (m_remainingCooldown < Cooldown / 2f)
                PointerModel.transform.localScale = m_pointerModelLocalScale;
        }
    }

    private void Shoot()
    {
        m_remainingCooldown = Cooldown;
        RaycastHit hit;

        Vector3 newForward = (transform.forward + transform.up * -1) / 2;
        if (Physics.Raycast(transform.position, newForward, out hit, maxDistance: 100.0f))
        {
            if(hit.transform.gameObject.layer == 9)
            {
                NPCInfo npcInfo = hit.transform.gameObject.GetComponent<NPCInfo>();
                npcInfo.health -= Damage;
                if (npcInfo.health <= 0.0f)
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }
}

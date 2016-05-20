﻿using UnityEngine;
using System.Collections;
using Valve.VR;

/// <summary>
/// Handles the projectile gun gadget
/// </summary>
public class ProjectileGun : MonoBehaviour
{
    [Tooltip("How many seconds the gun needs to be ready again")]
    public float cooldown;
    [Tooltip("The model that will be used as projectile")]
    public GameObject projectilePrefab;
    [Tooltip("The model that will be used for the gadget selector")]
    public GameObject gadgetPreviewPrefab;
    [Tooltip("The angle the gun will shoot at. 45° around X should be just fine")]
    public Vector3 shootingAngle;
    [Tooltip("The model that will be used as pointer. Note that this is only used for visual feedback")]
    public GameObject pointerModel;

    private SteamVR_Controller.Device m_device = null;
    private PickupSystem m_pickupSystem;
    private float m_triggerX;
    private float m_remainingCooldown;

    // Use this for initialization
    void Start ()
    {
        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);
        m_pickupSystem = GetComponent<PickupSystem>();
        m_triggerX = 0.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!m_pickupSystem.m_isHandBusy)
            m_triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;

        if (m_triggerX >= 0.1f && m_remainingCooldown <= 0.0f)
            Shoot();
        else
            m_remainingCooldown -= Time.deltaTime;
    }

    private void Shoot()
    {
        m_device.TriggerHapticPulse(1500);
        m_remainingCooldown = cooldown;
        Vector3 eulerShootingAngle = transform.rotation.eulerAngles + shootingAngle;

        GameObject newProjectile = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.Euler(eulerShootingAngle));
        newProjectile.layer = 11;
    }
}

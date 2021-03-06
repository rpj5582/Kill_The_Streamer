﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    /// <summary>
    /// Amount of ammo remaining in the weapon.
    /// </summary>
    public int m_ammo;

    /// <summary>
    /// Timer keeping track of when the player can shoot again.
    /// Player can shoot when timer is less than 0
    /// </summary>
    public float m_timer;

    /// <summary>
    /// Timer keeping track of when to delete a non-held weapon.
    /// </summary>
    public float m_arenaTimer;

    /// <summary>
    /// Bool tracking whether a weapon is held by the player or not.
    /// </summary>
    public bool m_held;

    public SpriteRenderer m_spriteRenderer;

    /// <summary>
    /// Amount of time m_timer resets to.  A smaller number means
    /// a faster firerate.
    /// </summary>
    public abstract float FIRE_RATE
    {
        get;
    }

    /// <summary>
    /// Maximum ammo remaining 
    /// </summary>
    public abstract int MAX_AMMO
    {
        get;
    }

    public virtual int START_AMMO
    {
        get { return MAX_AMMO; }
    }

    public abstract Sprite WEAPON_SPRITE
    {
        get;
    }

    /// <summary>
    /// Maximum time a weapon can survive 
    /// </summary>
    public const float ARENA_LIFETIME = 10.0f;

    /// <summary>
    /// Name of the weapon type (e.g. Pistol, Sniper, Etc)
    /// </summary>
    public abstract string NAME
    {
        get;
    }

    /// <summary>
    /// Spawnrate of the weapon (from 1-1000 usually, 1 meaning almost never, 1000 meaning common).
    /// </summary>
    public abstract int SPAWNRATE
    {
        get;
    }

    /// <summary>
    /// Fires the weapon in the direction given.
    /// </summary>
    /// <param name="position">The character's position.</param>
    /// <param name="direction">The direction the character is aiming in.</param>
    public abstract void Fire(Vector3 position, Vector3 direction);

    public virtual void Start()
    {
        if (!m_held)
        {
            m_held = false;
        }
        m_ammo = START_AMMO;
        m_timer = 0.0f;
        m_arenaTimer = ARENA_LIFETIME;
        m_spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public virtual void Update()
    {
        if (!m_held)
        {
            m_arenaTimer -= Time.deltaTime;
            if(m_arenaTimer > 0.0f && m_arenaTimer <= 3.0f)
            {
                if(((int)(m_arenaTimer * 8)) % 2 == 0)
                {
                    this.m_spriteRenderer.enabled = false;
                }
                else
                {
                    this.m_spriteRenderer.enabled = true;
                }
            }
            else if(m_arenaTimer <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public virtual void LateUpdate()
    {
        if (!m_held)
        {
            if (WeaponInRange(Player.s_Player.transform.position))
            {
                Player.s_Player.m_weaponPickupText.enabled = true;
            }
        }
    }

    public bool WeaponInRange(Vector3 input)
    {
        return (input - transform.position).sqrMagnitude < 3.0f;
    }
}

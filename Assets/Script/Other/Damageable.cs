using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<float, Vector2> damageableHit;
    public UnityEvent<float, float> healthChanged;

    [SerializeField] protected Animator animator;

    [SerializeField] protected float _maxHealth = 100;
    public float MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField] protected float _health = 100;
    public float Health
    {
        get { return _health; }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);

            if (_health > 0) return;
            IsAlive = false;

        }
    }

    [SerializeField] protected bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            animator.SetBool(AnimationString.isAlive, value);
            Debug.Log("death");
        }
    }

    [SerializeField] protected bool isInvincible = false;
    [SerializeField] protected float timeSinceHit = 0f;
    [SerializeField] protected float invincibilityTime = 0.25f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        SetIsInvincible();
    }

    protected void SetIsInvincible()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(float damage, Vector2 knockback)
    {
        if (!IsAlive) return false;
        if (isInvincible) return false;

        Health -= damage;
        isInvincible = true;

        animator.SetTrigger(AnimationString.hit);

        damageableHit?.Invoke(damage, knockback);
        //CharacterEvents.characterDamaged.Invoke(gameObject, damage);

        return true;
    }

    public void Heal(float heathRestore)
    {
        if (!IsAlive) return;

        float actualHealthRestore = Mathf.Min(MaxHealth - Health, heathRestore);
        Health += actualHealthRestore;
        //CharacterEvents.characterHealed.Invoke(gameObject, actualHealthRestore);
    }
}

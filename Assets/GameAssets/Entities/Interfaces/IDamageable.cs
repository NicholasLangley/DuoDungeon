using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int maxHealth {get; set;}
    int currentHealth { get; set; }

    public void Damage(int dmg);

    public void Heal(int heal);
}

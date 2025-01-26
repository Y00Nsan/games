using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour

{
    
    public GameObject bloodEffect;
    // private Animator anim;
    public int maxHealth = 10;
    public int currenthealth;
    public HealthBar healthbar;
    

    void Start()
    {
        // anim = GetComponent<Animator>();
        currenthealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currenthealth -= amount;
        healthbar.SetHealth(currenthealth);
        GameObject effect = Instantiate(bloodEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
        
        // if(health <= 0)
        // {
            // anim.SetTrigger("Dead");
        // }
    }
}

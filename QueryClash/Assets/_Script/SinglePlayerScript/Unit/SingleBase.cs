using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SingleBase : SingleSoldier
{
    //public Image healthBar;

    
    // Start is called before the first frame update
    void Start()
    {
        OnPlaced();
        CurrentHp = MaxHp;
        isBase = true;
    }

    public override void ReduceHp(float damage)
    {
        CurrentHp -= damage;
        healthBar.fillAmount = CurrentHp/MaxHp;
        if (CurrentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void HealingHp(float heal)
    {
        
    }

    public override void FindHealthBar()
    {
        // Do nothing, since healthBar is manually assigned
    }

}

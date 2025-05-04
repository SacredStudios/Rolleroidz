using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName = "PoisonWeapon", menuName = "Scripts/PoisonWeapon")]
public class PoisonWeapon : RangedWeapon
{
    public float _poisonRadius = 5f;
    protected override float Radius => _poisonRadius;
    [SerializeField] float poison_amount;
    
    protected override void ApplyDamage(Player_Health ph, GameObject parent, RaycastHit hit)
    {
        // (1) do all the normal damage/death logic
        base.ApplyDamage(ph, parent, hit);

        

        if (ph != null)
        {
            ph.StartPoison(ph, parent, hit, poison_amount, this);
        }
    }
    
}



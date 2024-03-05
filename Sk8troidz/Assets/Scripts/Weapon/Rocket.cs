using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
[CreateAssetMenu(fileName = "new_weapon", menuName = "Scripts/RocketWeapon", order = 2)]
public class Rocket : Weapon
{
    GameObject target;
    [SerializeField] float radius;
    [SerializeField] float power;
    [SerializeField] float speed;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject smoke;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject rocket;

    
    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
        GameObject clone = PhotonNetwork.Instantiate(rocket.name, particle_pos.transform.position, particle_pos.transform.rotation);
        Explosion es = clone.GetComponent<Explosion>();
        es.explosion = this.explosion;
        es.smoke = this.smoke;
        es.damage = this.damage;
        es.power = this.power;
        es.radius = this.radius;
        es.speed = this.speed;
        es.pv = this.pv;
    }
}

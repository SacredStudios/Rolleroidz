using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
[CreateAssetMenu(fileName = "new_weapon", menuName = "Scripts/BouncyWeapon", order = 1)]
public class BouncyWeapon : Weapon
{

    public GameObject ball;
    public GameObject particle_explosion;
    [SerializeField] float radius;
    [SerializeField] float power;
    [SerializeField] float speed;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject smoke;




    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
     
        GameObject clone = PhotonNetwork.Instantiate(ball.name, particle_pos.transform.position, particle_pos.transform.rotation);
        Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);
        Ball bs = clone.GetComponent<Ball>();
        bs.explosion = this.explosion;
        bs.smoke = this.smoke;
        bs.damage = this.damage;
        bs.power = this.power;
        bs.radius = this.radius;
        bs.speed = this.speed;
        bs.pv = this.pv;
        bs.range = this.range;
    }
}









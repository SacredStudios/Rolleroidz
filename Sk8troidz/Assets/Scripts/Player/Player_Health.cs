using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class Player_Health : MonoBehaviour

{
    [SerializeField] float max_health;
    public float current_health;
    [SerializeField] GameObject death_effect;
    [SerializeField] GameObject parent;
    [SerializeField] Slider health_bar;
    [SerializeField] Slider health_bar_other;
    [SerializeField] PhotonView pv;
    private Coroutine _poisonRoutine;
    public int id;

    [Header("Damage-flash settings")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] float flashTime = 0.5f;
    private Renderer[] rends;
    private Color[][] originalColors;
    private Coroutine flashRoutine;
    void Start()
    {
        current_health = max_health;
        rends = GetComponentsInChildren<Renderer>(includeInactive: false);

        // capture true originals once
        originalColors = new Color[rends.Length][];
        for (int r = 0; r < rends.Length; ++r)
        {
            Material[] mats = rends[r].materials;   // instanced copies
            originalColors[r] = new Color[mats.Length];

            for (int m = 0; m < mats.Length; ++m)
                if (mats[m].HasProperty("_Color"))
                    originalColors[r][m] = mats[m].color;
        }
    }

    private IEnumerator FlashRed()
    {
        TintRed();                               // 1) turn red
        yield return new WaitForSeconds(flashTime);
        RestoreColors();                        // 2) back to original
        flashRoutine = null;                     // ready for next hit
    }

    /* ─── Helpers ─── */
    void TintRed()
    {
        foreach (Renderer r in rends)
            foreach (Material m in r.materials)
                if (m.HasProperty("_Color"))
                    m.color = flashColor;
    }

    void RestoreColors()
    {
        for (int r = 0; r < rends.Length; ++r)
        {
            Material[] mats = rends[r].materials;
            for (int m = 0; m < mats.Length; ++m)
                if (mats[m].HasProperty("_Color"))
                    mats[m].color = originalColors[r][m];
        }
    }

    public void Remove_Health(float amount)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRed());
        pv.RPC("ChangeHealth", RpcTarget.All, -1 * amount);
    }
    public void PlayerLastHit(int newId)
    {
        pv.RPC("SyncLastHit", RpcTarget.All, newId);
    }
    [PunRPC]
    void SyncLastHit(int newId)
    {
        //still need to do this for other weapon types
        id = newId;
        Debug.Log(id);
    }
    public void Add_Explosion(float power, float radius, float x, float y, float z)
    {
        pv.RPC("Explode", RpcTarget.All, power, radius, x ,y ,z);
    }
    [PunRPC]
    void Explode(float power, float radius, float x, float y, float z)
    {

        if (this.gameObject.tag == "Player")
        {
            GetComponent<Rigidbody>().AddExplosionForce(power, new Vector3(x, y, z), radius, 1.12f);
            GetComponent<CameraShake>().Shake(power / 2000f, 0.5f);
        }
        else
        {
            GetComponent<Rigidbody>().AddExplosionForce(power/10, new Vector3(x, y, z), radius, 1.12f);
        }
    }
    [PunRPC] void ChangeHealth(float amount)
    {
        current_health += amount;
        health_bar.value = current_health;
        health_bar_other.value = current_health;
        if (current_health <= 0 && pv.IsMine)
        {
            Death();
        }
        else if (current_health > max_health)
        {
            current_health = max_health;
        }

    }


    public void Add_Health(float amount)
    {
        pv.RPC("ChangeHealth", RpcTarget.All, amount);
        

    }
    public void StartPoison(Player_Health ph, GameObject parent, RaycastHit hit, float poison_amount, Weapon weapon)
    {
        if (_poisonRoutine != null)
            StopCoroutine(_poisonRoutine);

        // then start a new one and remember it
        _poisonRoutine = StartCoroutine(ApplyPoison(ph, parent, hit, poison_amount, weapon));
    }
    private IEnumerator ApplyPoison(
        Player_Health ph,
        GameObject parent,
        RaycastHit hit,
        float poison_amount,
        Weapon weapon
    )
    {
        for (int i = 0; i < 10; i++)
        {
            if(ph.current_health <= 0)
            {
                break;
            }
            if (ph.current_health - poison_amount <= 0)
            {
                // now pulling it from the Weapon you passed in
                PhotonNetwork.Instantiate(
                  weapon.death_effect.name,
                  hit.point,
                  Quaternion.identity
                );
                hit.collider.transform.position = new Vector3(9999, 9999, 9999);
                weapon.SpawnCoin(hit.transform.gameObject, hit.point);
                parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
                ph.Death();
                break;
            }

            ph.Remove_Health(poison_amount);
            yield return new WaitForSeconds(1f);
        }

        // clear your handle
        _poisonRoutine = null;
    }


void Death()
    {
        Respawn rs = GetComponentInParent<Respawn>();
        rs.Death(id);

     }

    private void OnEnable()
    {
        RestoreColors();
        current_health = 100;
        health_bar.value = current_health;
        health_bar_other.value = current_health;
    }
}

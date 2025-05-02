using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class Black_Hole : MonoBehaviourPun
{
    [Header("Black Hole Settings")]
    [Tooltip("Who fired this hole?  Assign on the firing client immediately after Instantiate.")]
    public GameObject player;

    public Weapon weapon;

    [SerializeField] private float basePullForce = 1000f;
    [SerializeField] private float pullRadius = 50f;
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseAmplitude = 0.5f;
    [SerializeField] private float growthDuration = 1f;
    [SerializeField] private float shrinkDuration = 1f;
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioSource sound;

    private Weapon_Handler weaponHandler;
    private Weapon tempWeapon;
    private Vector3 originalScale;
    private List<GameObject> targets = new List<GameObject>();
    private float nextUpdateTime;
    private bool isGrowing = true;
    private bool isShrinking = false;
    private float elapsedTime = 0f;

    void Start()
    {
        // 1) fallback: if nobody assigned us a player, try to find them by matching ActorNumber
        if (player == null)
            player = FindOwnerPlayer(photonView.OwnerActorNr);

        if (player == null)
            Debug.LogWarning($"Black_Hole: no player assigned or found for Actor#{photonView.OwnerActorNr}");

        // 2) record our “full” scale, then start invisible
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        // 3) position at the player (if we have one)
        if (player != null)
            transform.position = player.transform.position + offset;

        // 4) spawn SFX
        sound?.Play();

        // 5) if this is our own hole, disable our shooting while it lives
        if (photonView.IsMine && player != null)
        {
            weaponHandler = player.GetComponent<Weapon_Handler>();
            tempWeapon = weaponHandler.weapon;
            weaponHandler.weapon = null;
        }

        // 6) get our first target list and schedule the shrink
        CheckForNewObjects();
        if (photonView.IsMine)
            Invoke(nameof(BeginShrink), 10f);
    }

    void Update()
    {
        // follow the player
        if (player != null)
            transform.position = player.transform.position + offset;

        // refresh targets every so often
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            CheckForNewObjects();
        }

        // growth animation
        if (isGrowing)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / growthDuration);
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            if (t >= 1f) { isGrowing = false; elapsedTime = 0f; }
        }
        // shrink animation
        else if (isShrinking)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / shrinkDuration);
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            if (t >= 1f) Explode();
        }
        // idle pulsate
        else
        {
            float m = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            transform.localScale = originalScale * Mathf.Lerp(1f, 1.2f, m);
        }
    }

    void FixedUpdate()
    {
        if (isGrowing || isShrinking) return;

        // pull physics bodies in
        foreach (var obj in targets)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            float dist = Vector3.Distance(transform.position, rb.position);
            if (dist > pullRadius) continue;

            var dir = (transform.position - rb.position).normalized;
            float strength = obj.CompareTag("AI_Player")
                ? basePullForce / (dist * dist)
                : 20f * basePullForce / (dist * dist);

            rb.AddForce(dir * strength, ForceMode.Acceleration);
        }

        // handle kill & explosion logic
        ApplyBlackHoleEffects();
    }

    private void ApplyBlackHoleEffects()
    {
        var hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var col in hits)
        {
            var go = col.gameObject;
            if (!targets.Contains(go)) continue;

            var ph = go.GetComponent<Player_Health>();
            if (ph == null || ph.current_health <= 0) continue;

            if (ph.current_health <= 100)
            {
                PhotonNetwork.Instantiate(explosion.name, go.transform.position, Quaternion.identity);
                go.transform.position = new Vector3(9999f, 9999f, 9999f);
                tempWeapon?.SpawnCoin(go, go.transform.position);
            }
            ph.Remove_Health(100);
        }
    }

    private void CheckForNewObjects()
    {
        targets.Clear();
        targets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("AI_Player"));

        // drop same-team
        targets.RemoveAll(go =>
        {
            var tv = go.GetComponent<Team_Handler>();
            var myT = player?.GetComponent<Team_Handler>()?.GetTeam();
            return tv == null
                || (player != null && tv.GetTeam() == myT);
        });
    }

    private void BeginShrink()
    {
        isShrinking = true;
        elapsedTime = 0f;
    }

    private void Explode()
    {
        if (weaponHandler != null)
            weaponHandler.weapon = tempWeapon;
        PhotonNetwork.Destroy(gameObject);
    }

    private GameObject FindOwnerPlayer(int actorNumber)
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
        {
            var view = go.GetComponent<PhotonView>();
            if (view != null && view.OwnerActorNr == actorNumber)
                return go;
        }
        return null;
    }
}

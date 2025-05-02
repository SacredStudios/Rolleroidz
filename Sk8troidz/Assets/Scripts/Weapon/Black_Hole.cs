using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class Black_Hole : MonoBehaviourPun
{
    [Header("Black Hole Settings")]
    public Weapon weapon;
    [SerializeField] private float basePullForce = 1000f;
    [SerializeField] private float pullRadius = 50f;
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseAmplitude = 0.5f;
    [SerializeField] private float growthDuration = 1f;
    [SerializeField] private float shrinkDuration = 1f;
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioSource sound;

    // animation state
    private Vector3 originalScale;
    private float elapsedTime;
    private bool isGrowing = true;
    private bool isShrinking;
    private float nextUpdateTime;

    // pulling state
    private int spawnerTeam;
    private bool affectsLocal;
    private Rigidbody localBody;
    private List<GameObject> targets = new List<GameObject>();

    // weapon‐disable state (spawner only)
    private Weapon_Handler weaponHandler;
    private Weapon savedWeapon;

    /// <summary>
    /// Call this instead of PhotonNetwork.Instantiate directly.
    /// It spawns the hole and broadcasts the spawner’s team once (buffered).
    /// </summary>
    public static void Spawn(GameObject spawner, Vector3 position, Quaternion rotation)
    {
        int team = spawner.GetComponent<Team_Handler>().GetTeam();
        GameObject hole = PhotonNetwork.Instantiate(
            "Black_Hole", position, rotation
        );
        hole.GetComponent<PhotonView>()
            .RPC(nameof(Init), RpcTarget.AllBuffered, team);
    }

    [PunRPC]
    private void Init(int team)
    {
        spawnerTeam = team;

        // find this client's own player avatar
        foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
        {
            var pv = go.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                localBody = go.GetComponent<Rigidbody>();

                // decide if this client should be pulled
                int myTeam = go.GetComponent<Team_Handler>().GetTeam();
                affectsLocal = (myTeam != spawnerTeam);

                // only the spawning client disables its weapon & schedules shrink
                if (photonView.IsMine)
                {
                    weaponHandler = go.GetComponent<Weapon_Handler>();
                    savedWeapon = weaponHandler.weapon;
                    weaponHandler.weapon = null;

                    Invoke(nameof(BeginShrink), 10f);
                }
                break;
            }
        }
    }

    private void Start()
    {
        sound?.Play();
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        nextUpdateTime = Time.time + updateInterval;
        elapsedTime = 0f;
    }

    private void Update()
    {
        // --- Growth Phase ---
        if (isGrowing)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / growthDuration);
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            if (t >= 1f)
            {
                isGrowing = false;
                elapsedTime = 0f;
            }
        }
        // --- Shrink Phase ---
        else if (isShrinking)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / shrinkDuration);
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            if (t >= 1f)
                Explode();
        }
        // --- Pulsate Phase ---
        else
        {
            float modifier = 1f +
                pulseAmplitude * ((Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            transform.localScale = originalScale * modifier;
        }

        // periodically refresh the list of valid targets
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            CheckForNewObjects();
        }
    }

    private void FixedUpdate()
    {
        // don’t pull until growth completes, avoid during shrink, and skip if same‐team
        if (isGrowing || isShrinking || !affectsLocal) return;

        foreach (var obj in targets)
        {
            var pv = obj.GetComponent<PhotonView>();
            // only move players you own
            if (pv != null && !pv.IsMine) continue;
            // only master moves AI
            if (pv == null &&
                obj.CompareTag("AI_Player") &&
                !PhotonNetwork.IsMasterClient) continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            float dist = Vector3.Distance(transform.position, rb.position);
            if (dist > pullRadius || dist < 0.01f) continue;

            Vector3 dir = (transform.position - rb.position).normalized;
            bool isAI = obj.CompareTag("AI_Player");
            float force = isAI
                ? basePullForce / (dist * dist)
                : 20f * basePullForce / (dist * dist);

            rb.AddForce(dir * force, ForceMode.Acceleration);
        }

        // only the spawner handles health/destroy effects
        if (photonView.IsMine)
            ApplyBlackHoleEffects();
    }

    private void CheckForNewObjects()
    {
        targets.Clear();
        targets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("AI_Player"));

        // remove same‐team players
        targets.RemoveAll(go =>
        {
            var th = go.GetComponent<Team_Handler>();
            return (th != null) && (th.GetTeam() == spawnerTeam);
        });
    }

    private void BeginShrink() => isShrinking = true;

    private void Explode()
    {
        // restore weapon on the spawning client
        if (photonView.IsMine && weaponHandler != null)
            weaponHandler.weapon = savedWeapon;

        PhotonNetwork.Destroy(gameObject);

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
    }

    private void ApplyBlackHoleEffects()
    {
        // small overlap for “death” zone
        var hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var hit in hits)
        {
            if (!targets.Contains(hit.gameObject)) continue;

            var ph = hit.GetComponent<Player_Health>();
            if (ph == null || ph.current_health <= 0) continue;

            ph.Remove_Health(100);
            if (ph.current_health <= 0)
            {
                // spawn explosion and coins
                PhotonNetwork.Instantiate(
                    explosionPrefab.name,
                    hit.transform.position,
                    Quaternion.identity
                );
                hit.transform.position = new Vector3(9999f, 9999f, 9999f);
                savedWeapon.SpawnCoin(
                    hit.gameObject,
                    hit.transform.position
                );
            }
        }
    }
}

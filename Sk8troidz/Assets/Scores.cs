using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

public class Scores : MonoBehaviourPunCallbacks
{
    int team1count;
    int team2count;
    GameObject[] ai_players;

    [SerializeField] Text red_team;
    [SerializeField] Text pink_team;

    // Keep track of the last score to determine the change
    private int lastTeam1Score = 0;
    private int lastTeam2Score = 0;

    // Store reference to currently running coroutines to properly manage them
    private Coroutine redTeamCoroutine;
    private Coroutine pinkTeamCoroutine;
    float initialSize;

    private void Start()
    {
        ai_players = GameObject.FindGameObjectsWithTag("AI_Player");
        SyncScore();
        initialSize = red_team.fontSize;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        SyncScore();
    }

    void SyncScore()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            team1count = 0;
            team2count = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.GetPhotonTeam() != null)
                {
                    if (player.GetPhotonTeam().Code == 1)
                    {
                        team1count += player.GetScore();
                    }
                    else if (player.GetPhotonTeam().Code == 2)
                    {
                        team2count += player.GetScore();
                    }
                }
            }
            if (ai_players != null)
            {
                foreach (GameObject player in ai_players)
                {
                    if (player.GetComponent<Team_Handler>().GetTeam() == 1)
                    {
                        team1count += player.GetComponent<AI_Handler>().score;
                    }
                    if (player.GetComponent<Team_Handler>().GetTeam() == 2)
                    {
                        team2count += player.GetComponent<AI_Handler>().score;
                    }
                }
            }

            UpdateScoreUI(red_team, ref lastTeam1Score, team1count, ref redTeamCoroutine);
            UpdateScoreUI(pink_team, ref lastTeam2Score, team2count, ref pinkTeamCoroutine);
        }
    }

    void UpdateScoreUI(Text teamText, ref int lastScore, int newScore, ref Coroutine coroutine)
    {
        int scoreChange = newScore - lastScore;
        lastScore = newScore;
        teamText.text = "" + newScore;
        if (scoreChange != 0)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                // Reset font size to default after stopping coroutine to ensure consistency
                teamText.fontSize = (int)initialSize; // Adjust this to your actual default font size
            }
            coroutine = StartCoroutine(AnimateTextChange(teamText, scoreChange));
        }
    }

    IEnumerator AnimateTextChange(Text text, int scoreChange)
    {
        text.fontSize = (int)initialSize;
        
        float maxSize = initialSize + 200; // Limit growth
        float duration = 0.25f; // Animation duration

        // Scale up animation
        float timer = 0;
        while (timer < duration)
        {
            text.fontSize = (int)Mathf.Lerp(initialSize, maxSize, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        text.fontSize = (int)maxSize;

        // Scale down animation
        timer = 0;
        while (timer < duration)
        {
            text.fontSize = (int)Mathf.Lerp(maxSize, initialSize, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        text.fontSize = (int)initialSize;
    }
}

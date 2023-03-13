// <copyright file="ScoreHelper.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;


namespace Photon.Pun.Demo.Cockpit
{

    public class ScoreHelper : MonoBehaviour
    {
        public int Score;
        [SerializeField] Text score;
        int _currentScore;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {


            if (PhotonNetwork.LocalPlayer != null && Score != _currentScore)
            {
                _currentScore = Score;
                PhotonNetwork.LocalPlayer.SetScore(Score);
                score.text = "" + PhotonNetwork.LocalPlayer.GetScore();
            }

        }
    }

}
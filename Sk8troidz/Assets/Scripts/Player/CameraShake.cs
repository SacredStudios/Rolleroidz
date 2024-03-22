using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cam;
    float timer; //duration of shake
    float base_magnitude;
    float max_time;
    public void Shake(float magnitude, float time)
    {
        CinemachineBasicMultiChannelPerlin c =
            cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        c.m_AmplitudeGain = magnitude;

        base_magnitude = magnitude;
        max_time = time;
        timer = time;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin c =
                    cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                c.m_AmplitudeGain = 
                Mathf.Lerp(base_magnitude, 0, 1 - (timer / max_time));
            }
        }
    }


}

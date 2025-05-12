using UnityEngine;
using UnityEngine.UI;

public class Change_Volume : MonoBehaviour
{
    [SerializeField] private Slider slider;          // assign in the Inspector
    private const string VolumeKey = "masterVolume"; // PlayerPrefs key

    private void Awake()
    {
        // 1. Load saved volume (default to 1 f if the key doesn't exist)
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        // 2. Apply it to the audio system and the UI
        AudioListener.volume = savedVolume;
        slider.value = savedVolume;

        // 3. Listen for future slider changes
        slider.onValueChanged.AddListener(UpdateVolume);
    }

    // Called automatically whenever the slider moves
    public void UpdateVolume(float value)
    {
        AudioListener.volume = value;            // change master volume
        PlayerPrefs.SetFloat(VolumeKey, value);  // persist the setting
        PlayerPrefs.Save();                      // write to disk immediately
    }
}

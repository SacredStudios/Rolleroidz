using UnityEngine;

public class ExternalLink : MonoBehaviour
{
    [SerializeField] string external_link;

    public void GoToLink()
    {
        if (!string.IsNullOrWhiteSpace(external_link))
        {
            Application.OpenURL(external_link);
        }
        else
        {
            Debug.LogWarning("ExternalLink: URL string is empty.");
        }
    }


}

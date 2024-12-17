using UnityEngine;

public class DomainChecker : MonoBehaviour
{
    void Start()
    {
        // Check if the game is running in a WebGL build
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            string allowedDomain = "itch.io";
            string currentURL = Application.absoluteURL;

            if (!currentURL.Contains(allowedDomain))
            {
                Debug.Log("Unauthorized domain. The game will now quit.");
                Application.Quit(); // Exit the game
                // You could also redirect to a specific URL or display an error message here
            }
            else
            {
                Debug.Log("Domain check passed. Game starting.");
                // Initialize the game or continue with normal operations
            }
        }
        else
        {
            Debug.Log("Non-WebGL platform detected, skipping domain check.");
            // Perform any platform-specific initialization here
        }
    }
}

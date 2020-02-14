using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : HUDElement
{
    /// <summary>
    /// Only displays if the scene is set to 0
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.buildIndex == 0;
        }
    }

    private void Update()
    {
        
    }
}
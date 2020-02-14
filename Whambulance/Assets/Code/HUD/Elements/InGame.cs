using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame : HUDElement
{
    /// <summary>
    /// Only displays if the scene is set to 0
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.buildIndex == 1;
        }
    }

    private void Update()
    {
        
    }
}
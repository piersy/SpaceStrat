using UnityEngine;

public class Game : MonoBehaviour
{
    public delegate void PauseAction(bool paused);
    public static event PauseAction OnPaused;
    public static bool paused = true;
    // Use this for initialization
    void Start()
    {
//        Time.timeScale = 0f;
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
//            if (!paused)
//            {
//                Time.timeScale = 1f;
//            }
//            else
//            {
//                Time.timeScale = 0f;
//            }
            OnPaused(paused);
        }
    }
}

using UnityEngine;

public class Game : MonoBehaviour
{
    public delegate void PauseAction(bool paused);
    public static event PauseAction OnPaused;
    public static bool paused = true;
    public int turnLength = 50;
    int currentTurnUpdates = 0;
    // Use this for initialization
    void Start()
    {
//        Time.timeScale = 0f;
    }
	
    void FixedUpdate()
    {
        if (!paused)
        {
            if (currentTurnUpdates < turnLength)
            {
                currentTurnUpdates++;
            }
            else
            {
                pause();
                currentTurnUpdates = 0;
            }

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentTurnUpdates == 0)
        {
            pause();
        }
    }

    void pause()
    {
        paused = !paused;
        OnPaused(paused);
    }
}

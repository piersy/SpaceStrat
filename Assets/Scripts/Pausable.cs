using UnityEngine;
using System.Collections;
using System;

public class Pausable
{
    bool paused;
    
    Action a;
    public Pausable(bool paused, Action a)
    {
        this.a = a;
        this.paused = paused;
    }

    public void Call()
    {
        if((Game.paused && paused)||(!Game.paused && !paused)){
            a();
        }
    }
    

}

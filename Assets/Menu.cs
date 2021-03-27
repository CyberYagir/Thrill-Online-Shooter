using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public List<GameObject> windows;



    public void OpenWindow(int id)
    {
        for (int i = 0; i < windows.Count; i++)
        {
            if (i == id)
            {
                if (windows[i].active == true)
                {
                    windows[i].active = false;
                }
                else
                {
                    windows[i].active = true;
                }
            }
            else
            {
                windows[i].active = false;
            }
        }
    }
}

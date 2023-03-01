using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGround : MonoBehaviour
{
    public bool isOnGround;
    int touchCount;
    private void OnTriggerEnter2D(Collider2D col)
    {
        touchCount++;
        isOnGround = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        touchCount--;
        if (touchCount < 1)
        {
            isOnGround = false;
        }
    }
}

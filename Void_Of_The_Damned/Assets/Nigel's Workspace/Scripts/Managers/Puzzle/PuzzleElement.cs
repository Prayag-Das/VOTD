using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PuzzleElement : MonoBehaviour
{
    public bool IsCompleted { get; private set; }

    public void MarkCompleted()
    {
        IsCompleted = true;
        // You can also trigger visuals, sounds, etc. here
    }

    public void ResetElement()
    {
        IsCompleted = false;
    }
}

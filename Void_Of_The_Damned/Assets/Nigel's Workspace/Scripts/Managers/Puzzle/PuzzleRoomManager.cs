using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class PuzzleRoomManager : MonoBehaviour
{
    [SerializeField] private PuzzleElement[] puzzleElements;
    [SerializeField] private TaskManager taskManager; // <- Reference to your TaskManager

    private bool roomCompleted = false;

    public void CheckPuzzleProgress()
    {
        if (roomCompleted) return; // Prevent double-completing

        foreach (var element in puzzleElements)
        {
            if (!element.IsCompleted)
            {
                Debug.Log("Puzzle not done yet.");
                return;
            }
        }

        Debug.Log("Puzzle completed!");
        roomCompleted = true;

        if (taskManager != null)
        {
            taskManager.NotifyTaskCompleted(this);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<PuzzleRoomManager> tasks = new List<PuzzleRoomManager>();

    public void NotifyTaskCompleted(PuzzleRoomManager completedTask)
    {
        tasks.Remove(completedTask);
        Debug.Log("Task removed. Remaining: " + tasks.Count);

        if (tasks.Count == 0)
        {
            Debug.Log("All tasks complete! Telling GameManager...");
            GameManager.Instance.StartEndSequence();
        }
    }
}

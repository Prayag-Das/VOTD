using UnityEngine;
using System.Collections.Generic;

public enum TaskType
{
    Task1,
    Task2,
    Task3,
    Task4
}

public class PuzzleRoomManager : MonoBehaviour
{
    [SerializeField] private PuzzleElement[] puzzleElements;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] public TaskType taskID;

    private bool roomCompleted = false;

    private void Start()
    {
        if (taskManager == null)
            taskManager = FindObjectOfType<TaskManager>();

        if (taskManager != null)
            taskManager.RegisterTask(this);
    }

    public void CheckPuzzleProgress()
    {
        if (roomCompleted) return;

        foreach (var element in puzzleElements)
        {
            if (!element.IsCompleted)
                return;
        }

        roomCompleted = true;
        taskManager?.NotifyTaskCompleted(this);
    }
}

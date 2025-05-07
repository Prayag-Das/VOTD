using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<PuzzleRoomManager> tasks = new List<PuzzleRoomManager>();

    public bool Task1Complete { get; private set; } = false;
    public bool Task2Complete { get; private set; } = false;
    public bool Task3Complete { get; private set; } = false;
    public bool Task4Complete { get; private set; } = false;

    public void RegisterTask(PuzzleRoomManager newTask)
    {
        if (!tasks.Contains(newTask))
        {
            // If the corresponding task is already marked as complete, immediately notify
            switch (newTask.taskID)
            {
                case TaskType.Task1:
                    if (Task1Complete)
                    {
                        NotifyTaskCompleted(newTask);
                        return;
                    }
                    break;
                case TaskType.Task2:
                    if (Task2Complete)
                    {
                        NotifyTaskCompleted(newTask);
                        return;
                    }
                    break;
                case TaskType.Task3:
                    if (Task3Complete)
                    {
                        NotifyTaskCompleted(newTask);
                        return;
                    }
                    break;
                case TaskType.Task4:
                    if (Task4Complete)
                    {
                        NotifyTaskCompleted(newTask);
                        return;
                    }
                    break;
            }

            // Otherwise, add the task normally
            tasks.Add(newTask);
            Debug.Log($"Registered {newTask.taskID}. Total tasks: {tasks.Count}");
        }
    }


    public void NotifyTaskCompleted(PuzzleRoomManager completedTask)
    {
        tasks.Remove(completedTask);
        Debug.Log($"Task removed: {completedTask.taskID}. Remaining: {tasks.Count}");

        switch (completedTask.taskID)
        {
            case TaskType.Task1:
                Task1Complete = true;
                break;
            case TaskType.Task2:
                Task2Complete = true;
                break;
            case TaskType.Task3:
                Task3Complete = true;
                break;
            case TaskType.Task4:
                Task4Complete = true;
                break;
            default:
                Debug.LogWarning($"Unrecognized TaskType: {completedTask.taskID}");
                break;
        }

        if (Task1Complete && Task2Complete && Task3Complete && Task4Complete)
        {
            Debug.Log("All tasks complete! Telling GameManager...");
            GameManager.Instance.StartEndSequence();
        }
    }
}

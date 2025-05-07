using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TaskDisplayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI task1Text;
    [SerializeField] private TextMeshProUGUI task2Text;
    [SerializeField] private TextMeshProUGUI task3Text;
    [SerializeField] private TextMeshProUGUI task4Text;

    public void UpdateTaskStatus(int taskNumber, bool isComplete)
    {
        string statusText = isComplete ? "<color=green>Complete</color>" : "<color=red>Incomplete</color>";
        switch (taskNumber)
        {
            case 1:
                task1Text.text = UpdateStatus(task1Text.text, statusText);
                break;
            case 2:
                task2Text.text = UpdateStatus(task2Text.text, statusText);
                break;
            case 3:
                task3Text.text = UpdateStatus(task3Text.text, statusText);
                break;
            case 4:
                task4Text.text = UpdateStatus(task4Text.text, statusText);
                break;
        }
    }

    private string UpdateStatus(string originalText, string newStatus)
    {
        int statusIndex = originalText.IndexOf("Status:");
        if (statusIndex >= 0)
        {
            int endOfLine = originalText.IndexOf('\n', statusIndex);
            if (endOfLine == -1) endOfLine = originalText.Length;
            return originalText.Substring(0, statusIndex) + "Status: " + newStatus + originalText.Substring(endOfLine);
        }
        return originalText;
    }
}

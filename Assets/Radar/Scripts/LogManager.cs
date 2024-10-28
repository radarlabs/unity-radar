using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance { get; private set; }

    public Text logTextBox;
    public int maxLines = 20;

    private string logContent = ""; // To keep track of all messages


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void Log(string message, LogType logType = LogType.Log)
    {
        Debug.Log(message);
        string formattedMessage = FormatMessage(message, logType);
        logContent += formattedMessage + "\n"; // Append the new message

        string[] lines = logContent.Split('\n');
        if (lines.Length > maxLines)
        {
            logContent = string.Join("\n", lines, lines.Length - maxLines, maxLines);
        }

        logTextBox.text = logContent;
    }


    private string FormatMessage(string message, LogType logType)
    {
        switch (logType)
        {
            case LogType.Error:
                return $"<color=red>Error: {message}</color>";
            case LogType.Warning:
                return $"<color=yellow>Warning: {message}</color>";
            case LogType.Attention:
                return $"<color=orange>Attention: {message}</color>";
            case LogType.Log:
            default:
                return $"<color=white>{message}</color>";
        }
    }
}


public enum LogType
{
    Log,
    Warning,
    Error,
    Attention
}

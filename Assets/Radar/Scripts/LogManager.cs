using System;
using UnityEngine;
using UnityEngine.UI;

namespace RadarSDK
{
    /// <summary>
    /// Manages logging for the Radar SDK, displaying messages with color-coded log types in a Text component.
    /// </summary>
    public class LogManager : MonoBehaviour
    {
        public static LogManager Instance { get; private set; }
        public Text logTextBox;
        public int maxLines = 20;
        private string logContent = ""; // To keep track of all messages
        private bool logConsole;


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


        public void SetLogConsole(bool isLogEnabled)
        {
            logConsole = isLogEnabled;
        }


        [ContextMenu("Clear Console")]
        public void ClearConsole()
        {
            logTextBox.text = String.Empty;
        }


        public void Log(string message, LogType logType = LogType.Log)
        {
            string formattedMessage = FormatLogMessage(message, logType, logConsole);
            logContent += formattedMessage + "\n"; // Append the new message

            string[] lines = logContent.Split('\n');
            if (lines.Length > maxLines)
            {
                logContent = string.Join("\n", lines, lines.Length - maxLines, maxLines);
            }

            logTextBox.text = logContent;
        }


        private string FormatLogMessage(string message, LogType logType, bool logConsole)
        {
            switch (logType)
            {
                case LogType.Error:
                    if (logConsole) Debug.LogError(message);
                    return $"<color=red>Error: {message}</color>";
                case LogType.Warning:
                    if (logConsole) Debug.LogWarning(message);
                    return $"<color=yellow>Warning: {message}</color>";
                case LogType.Attention:
                    if (logConsole) Debug.LogWarning(message);
                    return $"<color=orange>Attention: {message}</color>";
                case LogType.Log:
                default:
                    if (logConsole) Debug.Log(message);
                    return $"<color=white>{message}</color>";
            }
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
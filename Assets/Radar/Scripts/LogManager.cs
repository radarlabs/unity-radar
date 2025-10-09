using System;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

namespace RadarSDK
{
    /// <summary>
    /// Manages logging for the Radar SDK, displaying messages with color-coded log types in a Text component.
    /// Thread-safe logging to handle calls from any thread.
    /// </summary>
    public class LogManager : MonoBehaviour
    {
        public static LogManager Instance { get; private set; }
        public Text logTextBox;
        public int maxLines = 20;
        private string logContent = ""; // To keep track of all messages
        private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();



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


        [ContextMenu("Clear Console")]
        public void ClearConsole()
        {
            logContent = String.Empty;
            logTextBox.text = String.Empty;
        }


        public void Log(string message, LogType logType = LogType.Log)
        {
            string formattedMessage = FormatLogMessage(message, logType);
            logQueue.Enqueue(formattedMessage);
        }


        private void Update()
        {
            while (logQueue.TryDequeue(out string message))
            {
                AppendLog(message);
            }
        }


        private void AppendLog(string formattedMessage)
        {
            logContent += formattedMessage + "\n"; // Append the new message

            string[] lines = logContent.Split('\n');
            if (lines.Length > maxLines)
            {
                logContent = string.Join("\n", lines, lines.Length - maxLines, maxLines);
            }

            logTextBox.text = logContent;
        }


        private string FormatLogMessage(string message, LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    return $"<color=red>Error: {message}</color>";
                case LogType.Warning:
                    Debug.LogWarning(message);
                    return $"<color=yellow>Warning: {message}</color>";
                case LogType.Attention:
                    Debug.LogWarning(message);
                    return $"<color=orange>Attention: {message}</color>";
                case LogType.Log:
                default:
                    Debug.Log(message);
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

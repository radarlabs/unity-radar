using System.Text;
using UnityEngine;

public static class JsonFormatter
{
    public static string FormatJson(string json)
    {
        return FormatJson(json, new[] { Color.white });
    }

    public static string FormatJson(string json, Color[] indentationColors)
    {
        var colorHashes = new string[indentationColors.Length];
        for (int i = 0; i < colorHashes.Length; i++)
        {
            colorHashes[i] = $"#{ColorUtility.ToHtmlStringRGBA(indentationColors[i])}";
        }

        string getIndentationString(int level)
        {
            string color = colorHashes[level % colorHashes.Length];
            return $"<color={color}>{new string(' ', level * 4)}";
        }

        var stringBuilder = new StringBuilder();
        bool escaping = false;
        bool inQuotes = false;
        int indentation = 0;
        stringBuilder.Append(getIndentationString(indentation));
        foreach (char character in json)
        {
            switch (character)
            {
                case '{':
                case '[':
                    stringBuilder.Append(character);
                    if (!inQuotes)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append(getIndentationString(++indentation));
                    }

                    break;
                case '}':
                case ']':
                    if (!inQuotes)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append(getIndentationString(--indentation));
                    }

                    stringBuilder.Append(character);
                    break;
                case '"':
                    stringBuilder.Append(character);
                    bool escaped = escaping;
                    escaping = !escaping && character == '\\';
                    if (!escaped)
                    {
                        inQuotes = !inQuotes;
                    }

                    break;
                case ',':
                    stringBuilder.Append(character);
                    if (!inQuotes)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append(getIndentationString(indentation));
                    }

                    break;
                case ':':
                    stringBuilder.Append(character);
                    if (!inQuotes)
                    {
                        stringBuilder.Append(" ");
                    }

                    break;
                default:
                    stringBuilder.Append(character);
                    break;
            }
        }

        return stringBuilder.ToString();
    }
}
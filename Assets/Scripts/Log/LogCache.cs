using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LogMessage
{
    public string logName;
    public string logText;

    public LogMessage(string inName, string inText)
    {
        this.logName = inName;
        this.logText = inText;
    }

    public override string ToString()
    {
        return $"({logName}, {logText})";
    }
};

public class LogCache
{
    static List<LogMessage> logQueue = new List<LogMessage>();

    public static void AddLog(string inName, string inText)
    {
        if (inName.Length <= 0 || inText.Length <= 0)
        {
            return;
        }

        logQueue.Add(new LogMessage(inName, inText));
    }

    public static void PopAllLog(List<LogMessage> outQueue)
    {
        if (0 < logQueue.Count)
        {
            outQueue.AddRange(logQueue);

            logQueue.Clear();
        }
    }

    public static void ClearAllLog() { logQueue.Clear(); }  
}



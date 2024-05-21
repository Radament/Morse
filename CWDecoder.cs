using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

public class CWDecoder
{
    private Dictionary<string, string> _alphabet = new Dictionary<string, string> { };
    private StringBuilder _currentMessage;
    private DateTime _lastSignalTime;
    private TimeSpan _messageTimeout;
    private string _logFilePath = "CWLog.txt";

    public CWDecoder()
    {
        _currentMessage = new StringBuilder();
        _messageTimeout = TimeSpan.FromSeconds(1);
    }

    public unsafe void Process(float* buffer, int length)
    {
        string decodedMessage = DecodeCW(buffer, length);
        if (!string.IsNullOrEmpty(decodedMessage))
        {
            _currentMessage.Append(decodedMessage);
            _lastSignalTime = DateTime.Now;
        }

        if (DateTime.Now - _lastSignalTime > _messageTimeout)
        {
            LogMessage(_currentMessage.ToString());
            _currentMessage.Clear();
        }
    }

    private unsafe string DecodeCW(float* buffer, int length)
    {
        string morseCode = ConvertBufferToMorse(buffer, length);
        return _alphabet.ContainsKey(morseCode) ? _alphabet[morseCode].ToString() : string.Empty;
    }

    private unsafe string ConvertBufferToMorse(float* buffer, int length)
    {
        StringBuilder morseCode = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            char morseChar = buffer[i] > 0 ? '.' : '-'; ; // TODO check real buffer format and implement converter
            morseCode.Append(morseChar);
        }
        return morseCode.ToString();
    }

    private void LogMessage(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy.MM.dd HH:mm} {message}";
        File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
    }

    public void LoadAlphabetFromXml(string filePath, string alphabetName)
    {
        var doc = XDocument.Load(filePath);
        var alphabetNode = doc.Descendants("Alphabet")
                              .FirstOrDefault(x => x.Attribute("language").Value == alphabetName);

        if (alphabetNode != null)
        {
            _alphabet.Clear(); 
            foreach (var node in alphabetNode.Elements("letter"))
            {
                string letter = node.Attribute("symbol").Value;
                string code = node.Value;
                _alphabet[letter] = code;
            }
        }
    }

    public void SetMessageTimeout(TimeSpan timeout)
    {
        _messageTimeout = timeout;
    }
}
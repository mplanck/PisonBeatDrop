using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class PisonFrame
{
    public List<int> channels;
    public Quaternion quaternion;
    public string activation;
    public int batteryLife;
    public int timeStamp;
    public Dictionary<string, FilteredFrame> filteredFrames;

    public static PisonFrame CreateFromJSON(string jsonString)
    {
        if (jsonString == null)
            return null;
        var jsonFrame = JSON.Parse(jsonString);

        var numChannels = jsonFrame["numChannels"];
        var timeStamp = jsonFrame["timeStamp"];
        var channels = new List<int>();
        for(int i = 0; i < numChannels; i++)
        {
            channels.Add(jsonFrame["channels"][i]);
        }

        Dictionary<string, FilteredFrame> filteredFrames = new Dictionary<string, FilteredFrame>();
        var fFrames = jsonFrame["filteredFrames"];
        foreach (string key in fFrames.Keys)
        {
            var curFFrameJson = fFrames[key];
            var curFilteredChannels = new List<int>();
            var curNumChannels = curFFrameJson["numChannels"];
            for (int i = 0; i < curNumChannels; i++)
            {
                curFilteredChannels.Add(curFFrameJson["channels"][i]);
            }
            var curFilteredFrame = new FilteredFrame(key, curFilteredChannels);
            filteredFrames.Add(key, curFilteredFrame);
        }

        float qx = jsonFrame["imuQuat"]["qx"];
        float qy = jsonFrame["imuQuat"]["qy"];
        float qz = jsonFrame["imuQuat"]["qz"];
        float qw = jsonFrame["imuQuat"]["qw"];
        Quaternion quaternion = new Quaternion(-qx, qz, qy, qw);
        string activation = jsonFrame["activation"];
        int batteryLife = jsonFrame["batteryPercentage"];
        
        return new PisonFrame(timeStamp, channels, filteredFrames, quaternion, activation, batteryLife);
    }

    public PisonFrame(int timeStamp, List<int> channels, Dictionary<string, FilteredFrame> filteredFrames, Quaternion quaternion, string activation, int batteryLife)
    {
        this.timeStamp = timeStamp;
        this.channels = channels;
        this.quaternion = quaternion;
        this.activation = activation;
        this.batteryLife = batteryLife;
        this.filteredFrames = filteredFrames;
    }

    public override string ToString()
    {
        string result = "PisonFrame: ";
        result += "TIMESTAMP:";
        result += timeStamp.ToString() + ", ";
        result += "CHANNELS: [";
        foreach (int channel in channels)
        {
            result += channel.ToString() + ", ";
        }
        result = result.Substring(0, result.Length - 2) + "], ";
        result += "QUAT: ";
        result += quaternion.ToString() + ", ";
        result += "FILTERED: {";
        foreach (string key in filteredFrames.Keys)
        {
            result += key + ": [";
            foreach (int channel in filteredFrames[key].channels)
            {
                result += channel.ToString() + ", ";
            }
            result = result.Substring(0, result.Length - 2) + "], ";
        }

        result += "}, ";
        result += "ACTIVATION: " + activation + ", ";
        result += "BATTERY: " + batteryLife.ToString();
        return result;
    }
}
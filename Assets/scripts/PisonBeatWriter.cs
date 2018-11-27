using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pison
{
  public class PisonBeatWriter
  {
    private float           timeInSeconds        = 0.0f;
    private float           secondsBetweenBeats_ = 0.0f;
    private float           lastBeatInSeconds_   = 0.0f;
    private List<BeatFrame> beatFrames_;
    private int             numberBeatBuckets_ = 5;
    private bool            recording_         = false;

    public PisonBeatWriter(int inNumberBeatBuckets, int inBPM)
    {
      secondsBetweenBeats_ = 60.0f / inBPM;
      beatFrames_          = new List<BeatFrame>();
      numberBeatBuckets_   = inNumberBeatBuckets;
    }

    public void StartRecording()
    {
      recording_         = true;
      timeInSeconds      = 0.0f;
      lastBeatInSeconds_ = 0.0f;
      beatFrames_.Clear();
    }

    public void StopRecording()
    {
      recording_ = false;
    }

    public bool IsRecording()
    {
      return recording_;
    }

    public bool Update(float inPlayTime, int[] activeBeatBuckets)
    {
      if (!recording_)
      {
        return false;
      }

      timeInSeconds = inPlayTime;
      if ((timeInSeconds - lastBeatInSeconds_) > secondsBetweenBeats_)
      {
        lastBeatInSeconds_ = timeInSeconds;
      }

      bool bitToRecord = false;
      for (int i = 0; i < numberBeatBuckets_; i++)
      {
        if (activeBeatBuckets[i] > 0)
        {
          bitToRecord = true;
          break;
        }
      }

      if (bitToRecord)
      {
        var frame = new BeatFrame();
        frame.timeInSeconds = timeInSeconds;
        frame.bucketBits    = new int[numberBeatBuckets_];
        activeBeatBuckets.CopyTo(frame.bucketBits, 0);
        beatFrames_.Add(frame);
        return true;
      }

      return false;
    }

    public void WriteBeatsTo(string path)
    {
      string[] output     = new string[beatFrames_.Count];
      int      lineNumber = 0;
      foreach (var frame in beatFrames_)
      {
        string line = $"{frame.timeInSeconds:###.###} ";
        for (int i = 0; i < numberBeatBuckets_; i++)
        {
          var token = frame.bucketBits[i] > 0 ? '@' : '-';
          line += $"{token} ";
        }

        output[lineNumber++] = line;
      }

      Debug.Log(String.Join("\n", output));
      System.IO.File.WriteAllLines(path, output);
    }
  }
}
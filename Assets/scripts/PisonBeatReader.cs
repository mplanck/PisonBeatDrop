using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pison
{
  public class BeatFrame
  {
    public float timeInSeconds;
    public int[] bucketBits;
  }

  public class PisonBeatReader
  {
    static public string sawBeatScript = @"
1 @ - - - -
2 - @ - - -
3 - - @ - -
4 - - - @ -
5 - - - - @
6 - - - @ -
7 - - @ - -
8 - @ - - -
";

    private int             numberBeatBuckets_ = 5;
    private List<BeatFrame> beatFrames_;
    private int             currentFrame_;
    private float           timeInSeconds        = 0.0f;
    private float           lastBeatInSeconds_   = 0.0f;
    private float           secondsBetweenBeats_ = 0.0f;

    public PisonBeatReader(string inBeatScript, int inNumberOfBuckets, int inBPM)
    {
      beatFrames_ = new List<BeatFrame>();

      timeInSeconds        = 0.0f;
      currentFrame_        = 0;
      secondsBetweenBeats_ = 60.0f / inBPM;
      lastBeatInSeconds_   = -float.MaxValue;

      string[] newlineSeperators = new string[] {"\n", "\r"};
      string[] tokenSeperators   = new string[] {" "};
      string[] lines             = inBeatScript.Split(newlineSeperators, StringSplitOptions.RemoveEmptyEntries);
      foreach (var line in lines)
      {
        var tokens = line.Split(tokenSeperators, StringSplitOptions.None);
        if (tokens.Length < numberBeatBuckets_ + 1)
        {
          Debug.Log($"Not a valid beat script line to parse: ${line} | ${tokens.Length}");
          continue;
        }

        BeatFrame frame = new BeatFrame();
        frame.timeInSeconds = float.Parse(tokens[0],
                                          System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        frame.bucketBits = new int[numberBeatBuckets_];
        for (int i = 1; i <= numberBeatBuckets_; i++)
        {
          if (tokens[i][0] == '@')
          {
            frame.bucketBits[i - 1] = 1;
          }
          else
          {
            frame.bucketBits[i - 1] = 0;
          }
        }

        beatFrames_.Add(frame);
      }
    }

    public bool Update(float inPlayTime, out BeatFrame outFrame)
    {
      timeInSeconds = inPlayTime;

      if (currentFrame_ >= beatFrames_.Count)
      {
        outFrame = null;
        return false;
      }

      outFrame = beatFrames_[currentFrame_];

      if ((timeInSeconds - lastBeatInSeconds_) > secondsBetweenBeats_)
      {
        lastBeatInSeconds_ = timeInSeconds;
      }

      if (timeInSeconds >= (outFrame?.timeInSeconds ?? float.PositiveInfinity))
      {
        currentFrame_++;
        return true;
      }

      return false;
    }
  }
}
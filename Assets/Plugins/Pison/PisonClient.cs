using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;

public class PisonClient
{
    TcpClient client;
    StreamReader reader;
    PisonFrameReceiver receiver;
    Thread curThread;
    public bool running = true;

    public PisonClient(int port, PisonFrameReceiver receiver)
    {
        Debug.Log("Waiting for connection...");
        client = new TcpClient("localhost", port);
        reader = new StreamReader(client.GetStream());
        Debug.Log("Connected!");
        this.receiver = receiver; 
        curThread = new Thread(receiveFrame);
        curThread.Start();
    }

    private void receiveFrame()
    {
        while(running)
        {
            var curLine = reader.ReadLine();
            var curFrame = PisonFrame.CreateFromJSON(curLine);
            receiver.receiveFrame(curFrame);
            Thread.Sleep(1);
        }
    }

    private PisonFrame[] LinesToFrames(string[] lines)
    {
        var result = new PisonFrame[lines.Length];
        for(int i = 0; i < lines.Length; i++)
        {
            var curLine = lines[i];
            var curFrame = PisonFrame.CreateFromJSON(curLine);
            result[i] = curFrame;
        }
        return result;
    }

    public void dispose()
    {
        running = false;
        client.Close();
    }

    public interface PisonFrameReceiver
    {
        void receiveFrame(PisonFrame frame);
    }
}

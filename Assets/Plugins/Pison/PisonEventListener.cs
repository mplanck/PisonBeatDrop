using UnityEngine;

public class PisonEventListener: AndroidJavaProxy
{
    private AndroidJavaObject serviceInstance;
    private AndroidJavaClass wrapperClass;
    private PisonClient.PisonFrameReceiver receiver;

    public bool connected;

    public PisonEventListener() : base("com.pison.sdk.PisonService$EventListener")
    {
        wrapperClass = new AndroidJavaClass("com.pison.sdk.UnityWrapper");
        serviceInstance = wrapperClass.CallStatic<AndroidJavaObject>("getServiceInstance");
    }
    
    private void bind()
    {
        var context = getContext();
        serviceInstance.Call<bool>("bindService", context, this);
    }

    public void unBind()
    {
        var context = getContext();
        serviceInstance.Call<bool>("unbindService", context);
    }

    public void connect(PisonClient.PisonFrameReceiver receiver)
    {
        this.receiver = receiver;
        this.bind();
    }

    private AndroidJavaObject getContext()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        return jc.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
    }
    
    public void onDeviceConnected()
    {
        connected = true;
        Debug.Log("Device Connected!");
    }

    public void onDeviceDisconnected()
    {
        connected = false;
        Debug.Log("Device disconnected.");
    }

    public void onDeviceError(string error)
    {
        Debug.Log(error);
    }

    public void onJsonFrame(string json)
    {
        var curFrame = PisonFrame.CreateFromJSON(json);
        receiver.receiveFrame(curFrame);
    }

    public void onDeviceFrame(AndroidJavaObject frame)
    {
    }
}
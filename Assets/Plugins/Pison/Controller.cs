/******************************************************************************
 * Pison Controller
 * Script for creating a Pison client and receiving device frames. The script will automatically create a Windows client 
 * to Exec or to the Android Hub App, depending on the Runtime Platform. The object this script is attached to will inherit
 * the devices rotation.
 * 
 * Activation Frames: "START", "END", "HOLD", "NONE"
 */

using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, PisonClient.PisonFrameReceiver
{
    private GameObject controlledObj; //Controlled object in scene
    
    public Quaternion? startRot = null; //Quaternion for the first frame 
    public Quaternion rot; //Quaternion for corrected controlled object rotation 
    public Quaternion objStartRot; //starting rotation of the gameobject in scene
    public Quaternion objectRotation; //used for raw, uncalibrated rotation 
    
    private PisonClient client; //Windows Client to Exec
    private PisonEventListener listener; //Android client to Hub App
    
    public string activation; //Activation frames
    public string startEnd; //Start and end of activation
    public int battery; //Device battery life
    public int timeStamp; //Amount of milliseconds since device was turned on
    public float x; // X euler angle of device
    public float y; // Y euler angle of device 
    public float z; // Z euler angle of device
    
    //Raw activation value, used for finger lift
    private Dictionary<string, FilteredFrame> filteredFrames;
    public int liftValue; //

    public void receiveFrame(PisonFrame frame)
    {
        // Debug.Log(frame);

        // Set rot to the delta between the starting orientation of the hand and the current orientation
        if (startRot == null)
            startRot = frame.quaternion;

        //Frame data
        objectRotation = frame.quaternion;
        activation = frame.activation;
        battery = frame.batteryLife;
        timeStamp = frame.timeStamp;
        liftValue = Mathf.Abs(frame.filteredFrames["MotionSilencer"].channels[0]);


        // Set rot to the delta between the starting orientation of the hand and the current orientation to correct gameobject rotation
        rot = Quaternion.Inverse((Quaternion)startRot) * frame.quaternion; 
        
        //Separate the Start and End of each activation
        if (frame.activation == "START")
        {
            startEnd = "START";
        }
        else if (frame.activation == "END")
        {
            startEnd = "END";

        }
    }

    void Start()
    {
        //If Unity is running in the editor or a Windows .exe, connect to Exec. If on Android, connect to the Hub App
        if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            client = new PisonClient(13375, this);
        else if (Application.platform == RuntimePlatform.Android)
        {
            listener = new PisonEventListener();
            listener.connect(this);
        }
        
        //Controlled object is set to whatever the script is attached to
        controlledObj = gameObject; 
        
        //Set starting rotation of gameobject
        objStartRot = controlledObj.transform.rotation; 
        
    }

    void Update()
    {
        //Apply device rotation to object
        controlledObj.transform.rotation = objStartRot * rot;
    }

    //Easy set of functions to call in order to get device euler angles
    public float GetXEuler()
    {
        return objectRotation.eulerAngles.x;
    }
    public float GetYEuler()
    {
        return objectRotation.eulerAngles.y;
    }

    public float GetZEuler()
    {
        return controlledObj.transform.rotation.eulerAngles.z;
    }

    void OnDisable()
    {
        client.dispose(); //close client and set thread running to false
    }

 #if UNITY_ANDROID
    private void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
            listener.unBind();
        else if(!pauseStatus && !listener.connected )
        {
            listener.connect(this);
        }
    }
 #endif
}

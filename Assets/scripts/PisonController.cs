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
using UnityEngine.Serialization;
using TMPro;

namespace Pison
{
    public class PisonController : MonoBehaviour, PisonClient.PisonFrameReceiver
    {
        private GameObject controlledObj; //Controlled object in scene
        public Quaternion? startRot = null;
        public Quaternion? lastObjectRotation = null;

        public Quaternion objStartRot;
        public Quaternion objectRotation;

        private PisonClient client;
        private PisonEventListener listener;

        public TextMeshProUGUI signalText;
        public PisonCursor cursor;
        public string activation; //Activation frames
        public string startEnd; //Start and end of activation
        public int battery; //Device battery life
        public int timeStamp; //Amount of milliseconds since device was turned on
        public float x; // X euler angle of device
        public float y; // Y euler angle of device 
        public float z; // Z euler angle of device
        public int liftValue;

        public float minX = -2.0f;
        public float maxX = 2.0f;

        public float qfactor = 4.0f;
        public float keyfactor = .1f;

        public CircularBuffer<float> liftBuffer = new CircularBuffer<float>(1000);

        public void receiveFrame(PisonFrame frame)
        {
            //Frame data
            objectRotation = new Quaternion(-frame.quaternion.x,
                -frame.quaternion.y,
                frame.quaternion.z,
                frame.quaternion.w);

            activation = frame.activation;
            battery = frame.batteryLife;
            timeStamp = frame.timeStamp;
            liftValue = Mathf.Abs(frame.filteredFrames["MotionSilencer"].channels[0]);
            liftBuffer.PushFront(liftValue);

            // Set rot to the delta between the starting orientation of the hand and the current orientation
            if (startRot == null)
            {
                startRot = objectRotation;
            }


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
            //Controlled object is set to whatever the script is attached to
            controlledObj = gameObject;

            //Set starting rotation of gameobject
            objStartRot = controlledObj.transform.rotation;

            //If Unity is running in the editor or a Windows .exe, connect to Exec. If on Android, connect to the Hub App
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer)
            {
                client = new PisonClient(13375, this);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                listener = new PisonEventListener();
                listener.connect(this);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                controlledObj.transform.position = new Vector3(0.0f,
                    controlledObj.transform.position.y,
                    controlledObj.transform.position.z);
            }

            float delta = 0.0f;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                delta += -keyfactor;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                delta += keyfactor;
            }

            var deltaRotation = Quaternion.Inverse(lastObjectRotation ?? objectRotation) * objectRotation;
            delta += qfactor * deltaRotation.z;

            float x = Mathf.Clamp(controlledObj.transform.position.x + delta, minX, maxX);
            controlledObj.transform.position = new Vector3(x,
                controlledObj.transform.position.y,
                controlledObj.transform.position.z);

            lastObjectRotation = objectRotation;

            if (activation == "START" || activation == "HOLD")
            {
                cursor.ActivateCubeBuster();
            }
            else
            {
                cursor.ResetCubeBuster();
            }

            if (signalText != null)
            {
                signalText.text = $"{liftValue} :Signal";
            }
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
            client?.dispose(); //close client and set thread running to false
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
}
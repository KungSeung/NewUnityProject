using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nuitrack.Frame;

public class GetFloor : MonoBehaviour
{
    TPoseCalibration calibration;

    static Quaternion sensorOrientation = Quaternion.identity;
    static Quaternion sensorOrientationMarker = Quaternion.identity;
    public static Quaternion SensorOrientation { get { return sensorOrientation; } }

    [SerializeField] bool useCalibrationSensorOrientation = false;

    [SerializeField] bool trackFloorHeight = false;
    nuitrack.UserFrame userFrame = null;

    static float floorHeight = 1f;
    public static float FloorHeight { get { return floorHeight; } }


    public static void SetSensorHeightManually(float newHeight) //may be used when floor is not tracked / UserTracker not enabled
    {
        floorHeight = newHeight;
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        if (useCalibrationSensorOrientation)
        {
            calibration = FindObjectOfType<TPoseCalibration>();
            if (calibration != null) calibration.onSuccess += Calibration_onSuccess;
            NuitrackManager.onUserTrackerUpdate += OnUserTrackerUpdate; //needed for floor info
        }

    }
    void OnUserTrackerUpdate(nuitrack.UserFrame frame)
    {
        userFrame = frame;
    }

    //can be used for sensor (angles, floor distance, maybe?) / user calibration (height, lengths)
    void Calibration_onSuccess(Quaternion rotation)
    {
        //sensor orientation:
        Vector3 torso = CurrentUserTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Torso).ToVector3();
        Vector3 neck = CurrentUserTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Neck).ToVector3();
        Vector3 diff = neck - torso;
        sensorOrientation = Quaternion.Euler(-Mathf.Atan2(diff.z, diff.y) * Mathf.Rad2Deg, 0f, 0f);

        //floor height:
        if (trackFloorHeight && (userFrame != null))
        {

            Vector3 floor = 0.001f * userFrame.Floor.ToVector3();
            Vector3 normal = userFrame.FloorNormal.ToVector3();
            Debug.Log("Floor: " + floor.ToString("0.00") + "; normal: " + normal.ToString("0.00"));
            if (normal.sqrMagnitude > 0.01f) //
            {
                Plane floorPlane = new Plane(normal, floor);

                floorHeight = floorPlane.GetDistanceToPoint(Vector3.zero);
            }
        }
    }

    void Update()
    {
        
    }
}

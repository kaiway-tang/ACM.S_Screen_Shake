using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_Finished : MonoBehaviour
{
    [SerializeField] Transform targetTransform, cameraTransform;
    [SerializeField] float trackingRate; //0 = no tracking; 1.0 = no lag

    static int trauma;
    [SerializeField] float screenShakeStrength, translationalStrength, rotationalStrength;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SetTrauma(10); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SetTrauma(20); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SetTrauma(30); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SetTrauma(40); }
    }

    //50x per second
    void FixedUpdate()
    {
        TrackTarget();
        ProcessTrauma();
        RecalibrateRotation();
    }

    //smoothly follow `targetTransform` (<- the player)
    void TrackTarget()
    {
        // 1) determine the positional offset
        Vector3 offset = targetTransform.position - cameraTransform.position;

        // 2) calculate a percentage of that vector
        offset = offset * trackingRate;

        // 2.5) set offset's Z-coordinate to 0
        offset.z = 0;

        // 3) add that to the camera's current position
        cameraTransform.position += offset;
    }

    public static void SetTrauma(int amount)
    {
        if (trauma < amount)
        {
            trauma = amount;
        }
    }

    //decrement trauma like a timer and shake the screen
    void ProcessTrauma()
    {
        if (trauma > 0)
        {
            //generate a value dictating how strong the screen is shaking in this instance
            float processedTrauma = trauma * trauma * screenShakeStrength;

            //generate a Vector3 for translational shake
            Vector3 translationalShake = new Vector2(Random.Range(-processedTrauma, processedTrauma), Random.Range(-processedTrauma, processedTrauma)) * translationalStrength;

            //generate a float for rotational shake
            float rotationalShake = Random.Range(-processedTrauma, processedTrauma) * rotationalStrength;

            //apply the shake to the camera
            cameraTransform.localPosition += translationalShake;
            cameraTransform.Rotate(Vector3.forward * rotationalShake);

            //decrement trauma
            trauma--;
        }
    }

    void RecalibrateRotation()
    {
        // 1) determine the rotational offset
        float offset = 0;
        if (cameraTransform.localEulerAngles.z < 180)
        {
            offset = targetTransform.localEulerAngles.z - cameraTransform.localEulerAngles.z;
        }
        else
        {
            offset = 360 - cameraTransform.localEulerAngles.z;
        }

        // 2) calculate a percentage of that float
        offset = offset * trackingRate;

        // 3) add that to the camera's current rotation
        cameraTransform.localEulerAngles += new Vector3(0, 0, offset);
    }
}

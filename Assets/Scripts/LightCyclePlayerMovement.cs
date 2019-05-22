using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCyclePlayerMovement : MonoBehaviour
{

    public Transform LightCycle;
    public Transform LookTarget;
    public Transform ForwardDirection;
    public Camera PlayerCamera;

    public Vector3 CameraDistanceTarget;
    public float SlerpTime = 1.0f;
    public float LightCycleSpeed = 1.0f;
    public float SpeedIncrement = 0.1f;
    public float LightCycleSpeedMax = 1.0f;
    public float LightCycleSpeedMin = 0.1f;
    public float LookAtSpeed = 0.1f;

    public class RotationArgs : EventArgs {
        public Quaternion FromRotation { get; set; }
        public Quaternion ToRotation { get; set; }
        public Vector3 WorldPosition { get; set; }
        public RotationArgs(Quaternion from, Quaternion to, Vector3 position) : base()
        {
            this.FromRotation = from;
            this.ToRotation = to;
            this.WorldPosition = position;
        }
    }
    public delegate void PlayerRotationEventHandler(object sender, RotationArgs e);
    public event PlayerRotationEventHandler PlayerRotationEvent;

    // Use this for initialization
    void Start () {
		
	}

    void UpdateLookatPosition() {
        if (Input.GetKey(KeyCode.W)) LookTarget.Translate(new Vector3(0f, LookAtSpeed, 0f));
        if (Input.GetKey(KeyCode.S)) LookTarget.Translate(new Vector3(0f, -1f * LookAtSpeed, 0f));
        if (Input.GetKey(KeyCode.A)) LookTarget.Translate(new Vector3(-1f * LookAtSpeed, 0f, 0f));
        if (Input.GetKey(KeyCode.D)) LookTarget.Translate(new Vector3(LookAtSpeed, 0f, 0f));
        if (Input.GetKey(KeyCode.C)) LookTarget.Translate(new Vector3(0f, 0f, -1f * LookAtSpeed));
        if (Input.GetKey(KeyCode.V)) LookTarget.Translate(new Vector3(0f, 0f, LookAtSpeed));
        if (Input.GetKey(KeyCode.Q)) CameraDistanceTarget.y -= LookAtSpeed;
        if (Input.GetKey(KeyCode.E)) CameraDistanceTarget.y += LookAtSpeed;
        if (Input.GetKey(KeyCode.Z)) CameraDistanceTarget.z -= LookAtSpeed;
        if (Input.GetKey(KeyCode.X)) CameraDistanceTarget.z += LookAtSpeed;
        if (Input.GetKey(KeyCode.R)) CameraDistanceTarget.x -= LookAtSpeed;
        if (Input.GetKey(KeyCode.F)) CameraDistanceTarget.x += LookAtSpeed;
    }

    void SetRotation(float yrotation) {
        Quaternion fromRot = LightCycle.transform.rotation;
        LightCycle.Rotate(new Vector3(0f, yrotation, 0f));
        Quaternion toRot = LightCycle.transform.rotation;

        if (this.PlayerRotationEvent != null) {
            this.PlayerRotationEvent(this, new RotationArgs(fromRot, toRot, LightCycle.transform.position));
        }
    }

    void UpdateLightCyclePosition() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            SetRotation(-90f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            SetRotation(90f); 
        }
        else {
            if (Input.GetKey(KeyCode.UpArrow)) LightCycleSpeed += SpeedIncrement; else LightCycleSpeed -= SpeedIncrement;
            if (Input.GetKey(KeyCode.DownArrow)) LightCycleSpeed -= SpeedIncrement;
            LightCycleSpeed = Mathf.Clamp(LightCycleSpeed, LightCycleSpeedMin, LightCycleSpeedMax);

            Vector3 direction = ForwardDirection.position - LightCycle.position;
            direction = direction.normalized;
            direction *= LightCycleSpeed;
            LightCycle.position += direction;
        }
    }

    void UpdateCameraPosition() {
        Vector3 cameraDirection = LightCycle.position - ForwardDirection.position;
        cameraDirection = cameraDirection.normalized;

        Vector3 r = LightCycle.right * CameraDistanceTarget.x;
        Vector3 u = LightCycle.up * CameraDistanceTarget.y;
        Vector3 f = LightCycle.forward * CameraDistanceTarget.z;
        Vector3 newCameraPosition = LightCycle.position + r + u + f;

        //float t = 1.0f;
        float t = Time.deltaTime * SlerpTime;
        t = Mathf.Clamp(t, 0f, 1f);

        Vector3 newPos = Vector3.Lerp(PlayerCamera.transform.position, newCameraPosition, t);
        PlayerCamera.transform.position = newPos;

        Quaternion newRotation = Quaternion.LookRotation(LookTarget.position - PlayerCamera.transform.position);
        Quaternion newSlerpRotation = Quaternion.Slerp(PlayerCamera.transform.rotation, newRotation, t);
        PlayerCamera.transform.rotation = newSlerpRotation;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateLightCyclePosition();
        UpdateLookatPosition();
        UpdateCameraPosition();
	}
}
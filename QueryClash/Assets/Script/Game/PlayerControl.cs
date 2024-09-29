using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    [Header("Base Setup")]
    public float WalkingSpeed = 7.5f;
    public float RunningSpeed = 11.5f;
    public float JumpSpeed = 8.0f;
    public float Gravity = 0f;//20.0f;
    public float LookSpeed = 4.0f;
    public float LookSpeedLimit = 45.0f;
    bool IsFlying = true;//false;

    CharacterController CharacterController;
    Vector3 MoveDirection = Vector3.zero;
    float RotationX = 0;

    [HideInInspector]
    public bool CanMove = true;

    [SerializeField]
    private float CameraYOffset = 0.4f;
    private Camera PlayerCamera;

    [Header("Animator Setup")]
    public Animator animator;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            PlayerCamera = Camera.main;
            PlayerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + CameraYOffset, transform.position.z);
            PlayerCamera.transform.SetParent(transform);
        }
        else { gameObject.GetComponent<PlayerControl>().enabled = false; }
    }

    // Start is called before the first frame update
    void Start()
    {
        CharacterController =  GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.None; // Free the cursor
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        //bool IsRunning = false;
        //IsRunning = Input.GetKey(KeyCode.LeftShift); // Press left shift to run
        //if (Input.GetKeyDown(KeyCode.Space)) { IsFlying = !IsFlying; } // Press right shift to change fly state

        // At grounded level, recalculated move direction based on axis
        Vector3 Forward = transform.TransformDirection(Vector3.forward);
        Vector3 Right = transform.TransformDirection(Vector3.right);

        float CurSpeedX = CanMove ? (WalkingSpeed) * Input.GetAxis("Vertical") : 0;
        float CurSpeedY = CanMove ? (WalkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float MovementDirectionY = MoveDirection.y;
        MoveDirection = (Forward * CurSpeedX) + (Right * CurSpeedY);
        if (IsFlying) {
            if (Input.mouseScrollDelta.y > 0) { MoveDirection.y = JumpSpeed * LookSpeedLimit; }  //&& CharacterController.isGrounded GetKey(KeyCode.UpArrow)
            else if (Input.mouseScrollDelta.y < 0) { MoveDirection.y = -JumpSpeed * LookSpeedLimit; } // !CharacterController.isGrounded &&   (IsFlying) ? 0 :   GetKey(KeyCode.DownArrow)  Gravity * Time.deltaTime
        }
        else { MoveDirection.y = MovementDirectionY; }
        
        

        // Move the controller
        CharacterController.Move(MoveDirection * Time.deltaTime);

        // Player and camera rotation
        if (CanMove && PlayerCamera != null) {
            RotationX += -Input.GetAxis("Mouse Y") * LookSpeed;
            RotationX = Mathf.Clamp(RotationX, -LookSpeedLimit, LookSpeedLimit);
            PlayerCamera.transform.localRotation = Quaternion.Euler(RotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * LookSpeed, 0);
        }
    }
}

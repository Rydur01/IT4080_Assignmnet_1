using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>();

    public float movementSpeed = 50f;
    public float rotationSpeed = 130f;
    private Camera playerCamera;
    private GameObject playerBody;

    private Vector3 initialPosition;

    private void NetworkInit()
    {
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

        playerBody = transform.Find("PlayerBody").gameObject;

        initialPosition = transform.position;

        //ApplyPlayerColor();
        StartCoroutine(DelayedApplyColor()); // color cannot be called in start because it loads to fast and the color is always red on client
    }

    private void Awake()
    {
        NetworkHelper.Log(this, "Awake");
    }

    private void Start()
    {
        NetworkHelper.Log(this, "Start");
    }

    public override void OnNetworkSpawn()
    {
        NetworkHelper.Log(this, "OnNetworkSpawn");
        NetworkInit();
        base.OnNetworkSpawn();
    }

    private IEnumerator DelayedApplyColor()
    {
        yield return new WaitForSeconds(1.0f); // Wait for 1 second
        ApplyPlayerColor(); // Call the ApplyColor method after the delay
        playerColor.OnValueChanged += OnPlayerColorChanged;
    }

    private void ApplyPlayerColor()
    {
        NetworkHelper.Log(this, $"Applying color {playerColor.Value}");
        playerBody.GetComponent<MeshRenderer>().material.color = playerColor.Value;
    }

    public void OnPlayerColorChanged(Color previous, Color Current)
    {
        ApplyPlayerColor();
    }

    private void Update()
    {
        if (IsOwner)
        {
            OwnerHandleInput();
        }
    }

    private void OwnerHandleInput()
    {
        Vector3 movement = CalcMovement();
        Vector3 rotation = CalcRotation();

        if (movement != Vector3.zero || rotation != Vector3.zero)
        {
            //MoveServerRpc(CalcMovement(), CalcRotation());
            Vector3 newPosition = transform.position + movement;

            // Check if the new position is within bounds
            if (!IsServer)
            {
                if (IsPositionWithinBounds(newPosition))
                {
                    MoveServerRpc(movement, rotation);
                }
            }
            else
            {
                MoveServerRpc(movement, rotation);
            }
            
        }
    }

    private bool IsPositionWithinBounds(Vector3 position)
    {
        Vector3 minBounds = initialPosition + new Vector3(-25f, 0f, -25f);
        Vector3 maxBounds = initialPosition + new Vector3(25f, 0f, 25f);

        return position.x >= minBounds.x && position.x <= maxBounds.x &&
               position.z >= minBounds.z && position.z <= maxBounds.z;
    }

    [ServerRpc(RequireOwnership = true)]
    private void MoveServerRpc(Vector3 movement, Vector3 rotation)
    {
        transform.Translate(movement);
        transform.Rotate(rotation);
    }

    // Rotate around the y axis when shift is not pressed
    private Vector3 CalcRotation()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown)
        {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
        return rotVect;
    }


    // Move up and back, and strafe when shift is pressed
    private Vector3 CalcMovement()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");

        if (isShiftKeyDown)
        {
            x_move = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed * Time.deltaTime;

        return moveVect;
    }
}

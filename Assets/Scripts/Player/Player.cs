using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    public Inventory inventory { get; set; } // reference to the game inventory
    public Functions functions { get; set; }

    [SerializeField] private float playerMoveSpeed;
    private bool isSprinting = false;
    [SerializeField] private float playerLookSpeed;
    [SerializeField] private float smoothInputSpeed = 0.2f;
    private Vector2 smoothInputVelocity;


    private PlayerControls playerControls;
    private CharacterController controller;
    private Vector3 playerMovement;
    private Rigidbody rb;


    private Camera playerCamera;
    [SerializeField] Transform cameraMount;
    private Vector2 playerLook;
    private float pitch = 0f;
    [SerializeField] private float maxPitch = 89f;
    [SerializeField] private bool isInverted = false;

    private ArtGallery ag;

    float interactionDistance = 30f; // maximum distance to check for raycast collision


    public void Start()
    {
        ag = ArtGallery.GetArtGallery();

        playerCamera = cameraMount.gameObject.AddComponent<Camera>();
        playerCamera.transform.parent = cameraMount;
        playerLook = Vector2.zero;

        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();

        ag.player = this;
        playerMovement = Vector3.zero;

        isInverted = ag.invertY;

        playerControls = new PlayerControls();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnMove(InputValue value)
    {
        playerMovement = new Vector3(value.Get<Vector2>().x, 0f, value.Get<Vector2>().y);
    }

    private void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    private void PlayerMove()
    {
        float mod = playerMoveSpeed * Time.deltaTime;
        float sprint = isSprinting ? 2 : 1;

        Vector3 move = transform.right * playerMovement.x + 0f * transform.up + transform.forward * playerMovement.z * sprint;
        move *= mod;
        controller.Move(move);
        //Mathf.Clamp(move.magnitude, 0, playerSpeed);
    }

    private void OnLook(InputValue value)
    {
        playerLook = value.Get<Vector2>();
    }

    private void PlayerLook()
    {
        pitch += playerLook.y * playerLookSpeed * (isInverted ? 1 : -1);
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        cameraMount.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        transform.Rotate(0, playerLook.x * playerLookSpeed, 0);
    }

    /// <summary>
    /// Handle collisions
    /// </summary>
    /// <param name="collider">Object player collided with</param>
    void OnTriggerEnter(Collider collider)
    {
        /* TAG: portal */
        if(collider.gameObject.tag == "portal")
        {
            //Debug.Log("player activating portal " + collider.gameObject.GetComponent<Portal>().PortalID);
            /* Tell portal controller to handle collision between specified portal and this player */
            FindObjectOfType<Room>().DoTeleport(this, collider.gameObject.GetComponent<Portal>().PortalID);
        }

        /* TAG: sculpturePlatform */
        if (collider.gameObject.tag == "sculpturePlatform")
        {
            //Debug.Log("player activating sculpture teleport ");
            /* Tell portal controller to handle collision between specified portal and this player */
            FindObjectOfType<Room>().DoTeleport(this, collider.gameObject.GetComponent<SculpturePlatform>().PortalID + 4);

        }

        /* TAG: Function Pickup */
        if (collider.tag == "FunctionPickup")
        {
            //Debug.Log("player sollided with function pickup " + collider.gameObject.GetComponent<FunctionPickup>().ToString());
            FunctionPickup fp = collider.GetComponent<FunctionPickup>();
            if (!functions.HasFunction(fp.Function))
            {
                functions.AddFunction(fp.Function);
                //ag.ActivateFunction(fp.Function.fTYPE);
                Destroy(fp.gameObject);
            }
            else
            {
                //Destroy(fp.gameObject);
                Debug.LogError("Has Fuction!");
            }
        }
    }

    public void Update()
    {
        PlayerLook();
        PlayerMove();
    }
}

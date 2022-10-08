using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(PlayerStamina))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, Min(1), Tooltip("Determines the walking speed of the player.")]
        private float walkingSpeed = 15;

        [SerializeField, Min(1), Tooltip("Determines the walking acceleration of the player.")]
        private float walkingAcceleration = 25;

        [SerializeField, Tooltip("Key used to run.")]
        private KeyCode runKey;

        [SerializeField, Min(1), Tooltip("Determines the running speed of the player.")]
        private float runningSpeed = 30;

        [SerializeField, Min(1), Tooltip("Determines the running acceleration of the player.")]
        private float runningAcceleration = 50;

        [SerializeField, Min(0), Tooltip("Determines the rotation speed of the player. If 0, rotation becomes instantaneous.")]
        private float rotationSpeed = 1;

        [SerializeField, Range(0, 180), Tooltip("Determines the angle of rotation that the head can perfom vertically.")]
        private float maximumVerticalAngle = 45;

        [SerializeField, Min(0), Tooltip("Smooth interpolation applied to rotation. If 0, smooth rotation is disabled.")]
        private float smoothRotation = 3;

        [SerializeField, Range(0, 1), Tooltip("Rotation speed multiplier when player is dead.")]
        private float rotationSpeedWhenDead = .5f;

        [Header("Setup")]
        [SerializeField, Tooltip("Transform used to rotate head (and camera).")]
        private Transform head;

        [SerializeField, Tooltip("Transform used to detect floor.")]
        private Transform feet;

        [SerializeField, Min(.01f), Tooltip("Radius from feet used to check floor.")]
        private float feetCheckRadius = .1f;

        [SerializeField, Tooltip("Determines layers that are walkable.")]
        private LayerMask walkableLayers;

        [SerializeField, Tooltip("The body animator that controls walking and run animation.")]
        private Animator bodyAnimator;

        private new Rigidbody rigidbody;
        private PlayerStamina stamina;

        private static readonly new Collider[] collider = new Collider[1];

        public static bool IsMoving { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            stamina = GetComponent<PlayerStamina>();

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            // Negation of Y-axis fixes camera rotating to opposite side on start
            targetRotation = currentRotation = new Vector2(head.transform.localEulerAngles.x, -rigidbody.rotation.eulerAngles.y);
            rigidbody.rotation = Quaternion.AngleAxis(currentRotation.y, Vector3.up);
            head.transform.localRotation = Quaternion.AngleAxis(currentRotation.x, Vector3.left);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void FixedUpdate()
        {
            if (PlayerBody.IsAlive)
            {
                bodyAnimator.SetFloat("PlayerSpeed", rigidbody.velocity.magnitude);

                if (Physics.OverlapSphereNonAlloc(feet.position, feetCheckRadius, collider, walkableLayers) > 0)
                {
                    Vector3 axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    if (axis.x == 0 && axis.z == 0)
                    {
                        IsMoving = false;
                        stamina.Rest();
                    }
                    else
                    {
                        IsMoving = true;
                        if (Input.GetKey(runKey) && stamina.TryRun())
                            Move(runningSpeed, runningAcceleration, axis);
                        else
                        {
                            stamina.Walk();
                            Move(walkingSpeed, walkingAcceleration, axis);
                        }
                    }
                }
                else
                {
                    IsMoving = false;
                    stamina.Rest();
                }
            }

            Rotate();
        }

        private void Move(float speed, float acceleration, Vector3 axis)
        {
            Vector3 targetSpeed = axis * speed;
            targetSpeed = transform.TransformDirection(targetSpeed);

            rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, targetSpeed, acceleration * Time.fixedDeltaTime);
        }

        private Vector2 currentRotation;
        private Vector2 targetRotation;
        private Vector2 lastFrameRotation;
        private void Rotate()
        {
            const float wrapAt = .05f;

            Vector3 rawMousePosition = GetRawMousePosition();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Confined;
                lastFrameRotation = GetMousePosition(rawMousePosition);
            }

            Vector2 mousePosition = GetMousePosition(rawMousePosition);
            Vector2 difference = mousePosition - lastFrameRotation;
            lastFrameRotation = mousePosition;
            targetRotation += difference;
            targetRotation = new Vector2(Mathf.Clamp(targetRotation.x, -maximumVerticalAngle, maximumVerticalAngle), targetRotation.y);

            if (rawMousePosition.x < wrapAt || rawMousePosition.x > 1 - wrapAt || rawMousePosition.y < wrapAt || rawMousePosition.y > 1 - wrapAt)
                Cursor.lockState = CursorLockMode.Locked;

            float multiplier = PlayerBody.IsAlive ? 1 : rotationSpeedWhenDead;

            if (rotationSpeed > 0)
                currentRotation = Vector3.MoveTowards(currentRotation, targetRotation, rotationSpeed * multiplier);
            else
                currentRotation = targetRotation;

            Quaternion xQ = Quaternion.AngleAxis(currentRotation.x, Vector3.left);
            Quaternion yQ = Quaternion.AngleAxis(currentRotation.y, Vector3.up);

            if (smoothRotation > 0)
            {
                // Horizontal rotation is applied on the player body.
                float delta = Time.fixedDeltaTime * smoothRotation * multiplier;
                rigidbody.rotation = Quaternion.Lerp(rigidbody.rotation, yQ, delta);
                // Vertical rotation is only applied on the player head.
                head.transform.localRotation = Quaternion.Lerp(head.transform.localRotation, xQ, delta);
            }
            else
            {
                // Horizontal rotation is applied on the player body.
                rigidbody.rotation = yQ;
                // Vertical rotation is only applied on the player head.
                head.transform.localRotation = xQ;
            }

            Vector3 GetRawMousePosition()
                => Camera.main.ScreenToViewportPoint(Input.mousePosition);

            Vector2 GetMousePosition(Vector3 rawMousePosition)
                => new Vector2((rawMousePosition.y - .5f) * maximumVerticalAngle, rawMousePosition.x * 360);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(feet.transform.position, feetCheckRadius);
        }
    }
}

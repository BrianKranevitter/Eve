using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Level.Doors
{
   // [RequireComponent(typeof(Animator))]
    public sealed class Door : MonoBehaviour, IInteractable
    {
        private static readonly WaitForFixedUpdate wait = new WaitForFixedUpdate();

        private float colorCheckRefreshTime = 2f;
        private float currentColorCheckRefreshTime = 0f;
        [Header("Configuration")]
        [SerializeField, Range(0, 1), Tooltip("Determines the percent of door that is open or closed per second.")]
        private float doorSpeed;

        [SerializeField, Tooltip("Initial state of the door. If true it's opened.")]
        private bool initialStateIsOpened;

        [SerializeField, Tooltip("Key required to open this door. If null or empty, no key is required.")]
        private string key;

        [SerializeField, Tooltip("Determines if the door if locked from one direction. The lock expires when the door is interacted successfully once.")]
        private Locking locking;

        [Header("Setup")]
        [SerializeField, Tooltip("Doors to interact.")]
        private SubDoor[] doors;
        
        public Animator colorAnimator;

        [Header("Sounds")]
        [SerializeField, Tooltip("Sound played when door is opening.")]
        private AudioUnit openSound;

        [SerializeField, Tooltip("Sound played when door is closing.")]
        private AudioUnit closeSound;

        [SerializeField, Tooltip("Sound played when door is unlocked.")]
        private AudioUnit unlockSound;

        [SerializeField, Tooltip("Sound played when player try to interact with a locked door and doesn't have the required key.")]
        private AudioUnit lockedSound;

        [SerializeField, Tooltip("Sound played when door interacted from the blocked side of the door.")]
        private AudioUnit blockeSideSound;

        [Header("Animation Triggers")]
        [SerializeField, Tooltip("Name of the animation trigger when door is unlocked with a key.")]
        private string unlockAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when door is door is locked but palyer try to interact.")]
        private string lockedAnimationTrigger;

        [Header("Override")]
        public bool overrideInteraction;
        public UnityEvent OnInteractionOverride;


        private Animator animator;
        private State state;
        private bool isInAnimation;

        public enum State
        {
            Opened,
            Closed,
            Moving,
        }

        public enum Locking
        {
            None = default,
            Back,
            Front,
            FullLock
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            if (doors.Length == 0)
                return;

            for (int i = 0; i < doors.Length; i++)
                doors[i].Initialize(initialStateIsOpened);

            state = initialStateIsOpened ? State.Opened : State.Closed;

            animator = GetComponent<Animator>();
        }
        private void Start()
        {
            UpdateLockColor();
        }

        private void Update()
        {
            currentColorCheckRefreshTime -= Time.deltaTime;

            if (currentColorCheckRefreshTime < 0)
            {
                currentColorCheckRefreshTime = colorCheckRefreshTime;
                UpdateLockColor();
            }
        }

        void UpdateLockColor()
        {
            if (colorAnimator == null) return;
            
            switch (locking)
            {
                case Locking.Front:
                case Locking.Back:
                    Vector3 toPlayer = (PlayerBody.Player.transform.position - transform.position).normalized;
                    float dot = Vector3.Dot(toPlayer, transform.forward);
                    if (dot > 0)
                    {
                        if (locking == Locking.Front)
                        {
                            colorAnimator.SetBool("Locked", true);
                        }
                        else
                        {
                            colorAnimator.SetBool("Locked", false);
                        }
                    }
                    else
                    {
                        if (dot == 0 || locking == Locking.Back)
                        {
                            colorAnimator.SetBool("Locked", true);
                        }
                        else
                        {
                            colorAnimator.SetBool("Locked", false);
                        }
                    }
                    break;
                
                case Locking.FullLock:
                    colorAnimator.SetBool("Locked", true);
                    break;
                
                case Locking.None:
                    colorAnimator.SetBool("Locked", false);
                    break;
            }
        }
        public void Interact()
        {
            if (isInAnimation)
                return;

            if (locking == Locking.FullLock)
                return;

            if (locking != Locking.None)
            {
                Vector3 toPlayer = (PlayerBody.Player.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(toPlayer, transform.forward);
                if (dot > 0)
                {
                    if (locking == Locking.Front)
                        goto error;
                    goto next;
                }
                else
                {
                    if (dot == 0)
                        Debug.LogWarning("Door checking produced 0, which is neither front nor back, but it was fallbacked as if it were back.");

                    if (locking == Locking.Back)
                        goto error;
                    goto next;
                }

                error:
                Try.PlayOneShoot(transform, blockeSideSound, "blocked side");
                return;

                next:;
            }

            if (!string.IsNullOrEmpty(key))
            {
                if (DoorKeysManager.HasKey(key))
                {
                    key = "";
                    if (!overrideInteraction)
                    {
                        Try.PlayOneShoot(transform, unlockSound, "unlock");
                        isInAnimation = true;
                        if (!Try.SetAnimationTrigger(animator, unlockAnimationTrigger, "blocked"))
                            FromUnlock();
                    }
                }
                else
                {
                    Try.PlayOneShoot(transform, lockedSound, "locked");
                    Try.SetAnimationTrigger(animator, lockedAnimationTrigger, "locked");
                }
                return;
            }

            if (overrideInteraction)
            {
                OnInteractionOverride.Invoke();
            }
            else
            {
                Switch();
            }
            
        }

        bool open = false;
        private void Switch()
        {
            locking = Locking.None;

            if (state == State.Moving)
            {
                return;
            }

            if (open)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Open()
        {
            if (open) return;
            
            state = State.Moving;
            open = true;
            Try.PlayOneShoot(transform, openSound, "open");
            
            StopAllCoroutines();
            StartCoroutine(Work());
        }
        public void Close()
        {
            if (!open) return;
            
            state = State.Moving;
            open = false;
            Try.PlayOneShoot(transform, closeSound, "close");
        
            StopAllCoroutines();
            StartCoroutine(Work());
        }

        public void FullLock()
        {
            locking = Locking.FullLock;
            UpdateLockColor();
        }
        public void LockFront()
        {
            locking = Locking.Front;
            UpdateLockColor();
        }
        
        public void LockBack()
        {
            locking = Locking.Back;
            UpdateLockColor();
        }
        
        public void Unlock()
        {
            locking = Locking.None;
            UpdateLockColor();
        }

        IEnumerator Work()
        {
            for (float progress = 0; progress < 1; progress += doorSpeed * Time.fixedDeltaTime)
            {
                for (int i = 0; i < doors.Length; i++)
                {
                    doors[i].Move(open, progress);
                }
                yield return wait;
            }
            state = open ? State.Opened : State.Closed;
        }
        
        public void FromUnlock()
        {
            isInAnimation = false;
            Switch();
        }

        public void Highlight() { }

        public void Unhighlight() { }

        public void InSight() { }

        public void OutOfSight() { }

        [Serializable]
        public struct SubDoor
        {
            [SerializeField, Tooltip("Door to move.")]
            private Transform door;

            [SerializeField, Tooltip("Open state of the door.")]
            private Transform opened;
            private Vector3 opened_;

            [SerializeField, Tooltip("Closed state of the door.")]
            private Transform closed;
            private Vector3 closed_;

            private NavMeshObstacle obstacle;

            public void Initialize(bool isOpened)
            {
                opened_ = opened.position;
                closed_ = closed.position;
                Destroy(opened.gameObject);
                Destroy(closed.gameObject);
                opened = null;
                closed = null;

                obstacle = door.GetComponent<NavMeshObstacle>();

                door.gameObject.SetActive(true);

                if (isOpened)
                {
                    door.position = opened_;
                    if (obstacle != null)
                        obstacle.enabled = false;
                }
                else
                {
                    door.position = closed_;
                    if (obstacle != null)
                        obstacle.enabled = true;
                }
            }

            public void Move(bool open, float progress)
            {
                if (open)
                {
                    door.position = Vector3.Lerp(closed_, opened_, progress);
                    if (progress == 1 && obstacle != null)
                        obstacle.enabled = false;
                }
                else
                {
                    door.position = Vector3.Lerp(opened_, closed_, progress);
                    obstacle.enabled = true;
                }
            }
        }
    }
}
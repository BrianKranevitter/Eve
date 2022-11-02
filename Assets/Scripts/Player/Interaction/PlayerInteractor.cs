using Game.Level;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Player
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, Tooltip("Key used to interact with an object.")]
        private KeyCode interactKey;

        [SerializeField, Min(0), Tooltip("Maximum distance of an object to be interactable.")]
        private float maximumInteractionDistance = 2;

        [SerializeField, Tooltip("Whenever it should auto pickup items when collide with them.")]
        private bool autoPickupItems;

        [SerializeField, Min(0), Tooltip("Maximum distance of an object to be in sight.")]
        private float maximumInSightDistance = 10;

        [Header("Setup")]
        [SerializeField, Tooltip("Camera where interaction ray is produced.")]
        private Camera interationSource;

        [SerializeField, Tooltip("Layers that collide with interaction ray.")]
        private LayerMask collisionMask;

        [SerializeField, Tooltip("Fallback used when raycast can't find object to interact.")]
        private InteractionVolume volume;

        private IInteractable lastInteraction;

        private Collider[] colliders = new Collider[1];
        private HashSet<IInteractable> interactables = new HashSet<IInteractable>();

#if UNITY_EDITOR
        private List<Vector3> gizmos = new List<Vector3>();
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            if (PauseMenu.Paused) return;
            
            Ray ray = interationSource.ViewportPointToRay(new Vector3(.5f, .5f));
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo, maximumInteractionDistance, collisionMask, QueryTriggerInteraction.Collide))
            {
                if (hitInfo.transform.TryGetComponent(out IInteractable interactable) || (volume != null && volume.ClosestTo(ray, out interactable)))
                {
                    if (lastInteraction != interactable)
                    {
                        if (lastInteraction != null)
                        {
                            lastInteraction.Unhighlight();
                            lastInteraction = interactable;
                        }
                        interactable.Highlight();
                        lastInteraction = interactable;
                    }

                    if (Input.GetKeyDown(interactKey))
                        interactable.Interact();
                }
            }
            else if (lastInteraction != null)
            {
                lastInteraction.Unhighlight();
                lastInteraction = null;
            }

            foreach (IInteractable interactable in interactables)
                interactable.OutOfSight();
            interactables.Clear();

            int count = Physics.OverlapSphereNonAlloc(transform.position, maximumInSightDistance, colliders, collisionMask);
            if (count == colliders.Length)
            {
                colliders = Physics.OverlapSphere(transform.position, maximumInSightDistance, collisionMask);
                count = colliders.Length;
            }

#if UNITY_EDITOR
            gizmos.Clear();
#endif

            for (int i = 0; i < count; i++)
            {
                Collider collider = colliders[i];
                if (collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    if (interactables.Contains(interactable))
                        continue;

                    if (CheckRaycast(interactable, collider.transform.position))
                        continue;

                    if (CheckCollider(interactable, collider))
                        continue;

                    interactable.InSight();
                    interactables.Add(interactable);
                }
            }
        }

        private bool CheckCollider(IInteractable interactable, Collider collider)
        {
            switch (collider)
            {
                case BoxCollider boxCollider:
                    return CheckCube(boxCollider.size, boxCollider);
                case SphereCollider sphereCollider:
                {
                    Vector3 radius = sphereCollider.radius * sphereCollider.transform.lossyScale;
                    Vector3 center = sphereCollider.bounds.center;

                    int n = (int)Mathf.Max(8 * radius.magnitude, 8);
                    float offset = 2f / n;
                    float increment = Mathf.PI * (3 - Mathf.Sqrt(5));
                    for (int j = 0; j < n; j++)
                    {
                        float y = (j * offset) - 1 + (offset / 2);
                        float r = Mathf.Sqrt(1 - (y * y));
                        float phi = j * increment;
                        float x = Mathf.Cos(phi) * r;
                        float z = Mathf.Sin(phi) * r;

                        if (CheckRaycast(interactable, new Vector3(x * radius.x, y * radius.y, z * radius.z) + center))
                            return true;
                    }
                    break;
                }
                case CapsuleCollider capsuleCollider:
                {
                    Vector3 size = Vector3.one * capsuleCollider.radius;

                    switch (capsuleCollider.direction)
                    {
                        case 0:
                            size.x = capsuleCollider.height;
                            break;
                        case 1:
                            size.y = capsuleCollider.height;
                            break;
                        case 2:
                            size.z = capsuleCollider.height;
                            break;
                    }

                    // TODO: This doesn't not check propery the curved part of capsules.

                    return CheckCube(size, capsuleCollider);
                }
            }

            return false;

            bool CheckCube(Vector3 size, Collider collider)
            {
                Vector3 halfSize = size * .5f;
                Transform boxTransform = collider.transform;
                Vector3 lossyScale = boxTransform.lossyScale;
                Matrix4x4 matrix = Matrix4x4.TRS(collider.bounds.center, boxTransform.localRotation, lossyScale);

                float xSteps = Mathf.Max(2 * size.x * lossyScale.x, 2);
                float ySteps = Mathf.Max(2 * size.y * lossyScale.y, 2);
                float zSteps = Mathf.Max(2 * size.z * lossyScale.z, 2);

                if (xSteps == 2 && ySteps == 2 && zSteps == 2)
                {
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, -halfSize.y, -halfSize.z))))
                        return true;
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, -halfSize.y, +halfSize.z))))
                        return true;
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, +halfSize.y, +halfSize.z))))
                        return true;
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(+halfSize.x, +halfSize.y, +halfSize.z))))
                        return true;
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(+halfSize.x, +halfSize.y, -halfSize.z))))
                        return true;
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(+halfSize.x, -halfSize.y, -halfSize.z))))
                        return true;
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(+halfSize.x, -halfSize.y, +halfSize.z))))
                        return true;
                    if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, +halfSize.y, -halfSize.z))))
                        return true;
                }
                else
                {
                    for (float x = 0; x <= xSteps; x++)
                    {
                        for (float y = 0; y <= ySteps; y++)
                        {
                            for (float z = 0; z <= zSteps; z++)
                            {
                                if (CheckRaycast(interactable, matrix.MultiplyPoint3x4(new Vector3(
                                    halfSize.x * Mathf.Lerp(-1, 1, x / xSteps),
                                    halfSize.y * Mathf.Lerp(-1, 1, y / ySteps),
                                    halfSize.z * Mathf.Lerp(-1, 1, z / zSteps)
                                ))))
                                    return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        private bool CheckRaycast(IInteractable interactable, Vector3 end)
        {
#if UNITY_EDITOR
            gizmos.Add(end);
#endif

            return Physics.Linecast(interationSource.transform.position, end, out RaycastHit hitInfo, collisionMask)
                    && hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable2)
                    && interactable != interactable2;
        }

#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            Gizmos.color = lastInteraction == null ? Color.yellow : Color.green;
            Ray ray = interationSource.ViewportPointToRay(new Vector3(.5f, .5f));
            Gizmos.DrawLine(ray.origin, ray.GetPoint(maximumInteractionDistance));

            Gizmos.color = Color.magenta;
            foreach (Vector3 point in gizmos)
                Gizmos.DrawSphere(point, .02f);
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnCollisionEnter(Collision collision)
        {
            if (autoPickupItems && collision.transform.TryGetComponent(out IPickup pickupt))
                pickupt.Pickup();
        }
    }
}
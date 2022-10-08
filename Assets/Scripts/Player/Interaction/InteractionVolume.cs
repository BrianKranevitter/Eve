using Game.Level;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Player
{
    public sealed class InteractionVolume : MonoBehaviour
    {
        private List<(IInteractable, Collider)> collection = new List<(IInteractable, Collider)>();

        public bool ClosestTo(Ray ray, out IInteractable interactable)
        {
            interactable = null;
            float closestDistance = float.PositiveInfinity;

            for (int j = collection.Count - 1; j >= 0; j--)
            {
                (IInteractable i, Collider c) = collection[j];
                if (i == null || c == null)
                    collection.RemoveAt(j);
                else
                {
                    float distance = float.PositiveInfinity;
                    if (c.Raycast(ray, out RaycastHit info, float.PositiveInfinity))
                        distance = info.distance;

                    Transform transform = c != null ? c.transform : ((MonoBehaviour)i).transform;
                    distance = Mathf.Min(distance, Vector3.Cross(ray.direction, transform.position - ray.origin).sqrMagnitude);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        interactable = i;
                    }
                }
            }
            return interactable != null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable) && !collection.Contains((interactable, other)))
                collection.Add((interactable, other));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
                collection.Remove((interactable, other));
        }
    }
}
using Game.Player;

using UnityEngine;

namespace Game.Utility
{
    [RequireComponent(typeof(Renderer))]
    public sealed class EmissionByDistance : MonoBehaviour
    {
        private Material material;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake() => material = GetComponent<Renderer>().material;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update() => material.SetVector("_PlayerPosition", PlayerBody.Player.transform.position);
    }
}
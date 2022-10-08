using UnityEngine;

namespace Game.Utility
{
    public sealed class RenderOnTop : MonoBehaviour
    {
        private void Start() => GetComponent<Renderer>().material.SetInt("_Zwrite", 1);
    }
}

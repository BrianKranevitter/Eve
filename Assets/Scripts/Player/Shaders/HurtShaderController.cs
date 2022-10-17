using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Game.Player
{
    public sealed class HurtShaderController : MonoBehaviour
    {
        [SerializeField, Tooltip("The speed that blood will dissapear with")]
        private float bloodDissapearSpeed;

        [SerializeField]
        private PostProcessVolume ppVolume;

        private HurtShaderPPSSettings hurtShader;

        private int lastIndex;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake() => ppVolume.profile.TryGetSettings(out hurtShader);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            if (PauseMenu.Paused) return;
            
            if (hurtShader == null)
                Debug.LogWarning("Missing hurt post process.");
            else
                ResetBlood();
        }

        public void SetBlood(float damageValue)
        {
            if (hurtShader == null)
            {
                Debug.LogWarning("Missing hurt post process.");
                return;
            }

            int index = Random.Range(0, 3);

            if (lastIndex == index)
            {
                if (index == 2)
                    index = 0;
                else
                    index++;
            }

            switch (index)
            {
                case 0:
                    {
                        if (damageValue + hurtShader._Splats1.value > 1)
                            hurtShader._Splats1.value = 1;
                        else
                            hurtShader._Splats1.value += damageValue;
                        break;
                    }
                case 1:
                    {
                        if (damageValue + hurtShader._Splats2.value > 1)
                            hurtShader._Splats2.value = 1;
                        else
                            hurtShader._Splats2.value += damageValue;
                        break;
                    }
                case 2:
                    {
                        if (damageValue + hurtShader._Splats3.value > 1)
                            hurtShader._Splats3.value = 1;
                        else
                            hurtShader._Splats3.value += damageValue;
                        break;
                    }
            }

            lastIndex = index;
        }

        public void ResetBlood()
        {
            if (hurtShader._Splats1.value > 0)
                hurtShader._Splats1.value -= Time.deltaTime * bloodDissapearSpeed;
            else if (hurtShader._Splats1.value < 0)
                hurtShader._Splats1.value = 0;
            else if (hurtShader._Splats1.value > 1)
                hurtShader._Splats1.value = 1;

            //Mathf.Clamp(hurtShader._Splats1.value, 0, 1);

            if (hurtShader._Splats2.value > 0)
                hurtShader._Splats2.value -= Time.deltaTime * bloodDissapearSpeed;
            else if (hurtShader._Splats2.value < 0)
                hurtShader._Splats2.value = 0;
            else if (hurtShader._Splats2.value > 1)
                hurtShader._Splats2.value = 1;
            // Mathf.Clamp(hurtShader._Splats2.value, 0, 1);

            if (hurtShader._Splats3.value > 0)
                hurtShader._Splats3.value -= Time.deltaTime * bloodDissapearSpeed;
            else if (hurtShader._Splats3.value < 0)
                hurtShader._Splats3.value = 0;
            else if (hurtShader._Splats2.value > 1)
                hurtShader._Splats2.value = 1;
            //Mathf.Clamp(hurtShader._Splats3.value, 0, 1);
        }
    }
}
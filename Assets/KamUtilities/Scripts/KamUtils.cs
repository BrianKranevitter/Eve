using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kam.CustomInput
{
    public class Utilities_Input_HoldKey
    {
        float curHold;

        bool everyFrame;

        public Utilities_Input_HoldKey(bool everyFrame)
        {
            this.everyFrame = everyFrame;
        }

        /// <summary>
        /// Returns true after the key has been held down for the specified amount of seconds.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool HoldKey(KeyCode key, float seconds)
        {
            if (UnityEngine.Input.GetKey(key))
            {
                curHold += Time.deltaTime;

                if (curHold > seconds)
                {
                    if (!everyFrame)
                    {
                        curHold = 0;
                    }
                    return true;
                }
            }

            if (UnityEngine.Input.GetKeyUp(key))
            {
                curHold = 0;
            }

            return false;
        }
    }
}

namespace Kam.Utils
{
    public class KamUtilities
    {
        public static float Map(float original, float originalMin, float originalMax, float newMin, float newMax)
        {
            return newMin + (original - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }

        public static List<T> ForAllNearby<T>(GameObject self, List<T> listOfObjects, float maxDistance,
            Action<T> action) where T : MonoBehaviour
        {
            List<T> nearby = new List<T>();

            foreach (var obj in listOfObjects)
            {
                if (obj.gameObject != self &&
                    Vector3.Distance(obj.transform.position, self.transform.position) < maxDistance)
                {
                    action(obj);
                    nearby.Add(obj);
                }
            }

            return nearby;
        }

        public static IEnumerator Delay(float time, Action action)
        {
            yield return new WaitForSeconds(time);

            action.Invoke();
        }

        public static TextMeshPro CreateWorldText(string text, Transform parent = null,
            Vector3 localPostiion = default(Vector3), float fontSize = 40, Color color = default(Color),
            TextAnchor textAnchor = default(TextAnchor), TextAlignmentOptions textAlignment = default(TextAlignmentOptions),
            int sortingOrder = 0)
        {
            GameObject obj = new GameObject("Text", typeof(TextMeshPro));
            Transform transform = obj.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPostiion;
            TextMeshPro txtMesh = obj.GetComponent<TextMeshPro>();
            txtMesh.alignment = textAlignment;
            txtMesh.text = text;
            txtMesh.fontSize = fontSize;
            txtMesh.color = color;
            txtMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return txtMesh;
        }
        
        public static Vector3 GetMouseWorldPos_WithZ(LayerMask mask)
        {
            return GetMouseWorldPos_WithZ(Camera.main, mask);
        }

        public static Vector3 GetMouseWorldPos_WithZ(Camera worldCamera, LayerMask mask)
        {
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 9999f, mask))
            {
                return raycastHit.point;
            }
            else
            {
                return Vector3.zero;
            }
            
        }
    }

    public class KamColor
    {
        public static Color purple = new Color(0.73f, 0.02f, 1f);
    }
}
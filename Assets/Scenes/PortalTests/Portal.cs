using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Portal : MonoBehaviour
{
    Camera mainCamera;
    public RenderTexture texture;
    public MeshRenderer renderer;
    public Portal pairPortal;
    private Camera pairPortalCamera;
    private GameObject mainCameraCopyAsChild;

    public Color mainCameraGizmo;
    public Color pairedCameraGizmo;
    public bool ableToDrawGizmos = true;
    private bool drawGizmos = false;
    
    private static readonly int RenderTexture1 = Shader.PropertyToID("_RenderTexture");

    private Action OnUpdate = () => {};
    private void Awake()
    {
        if (pairPortal == null) return;
        
        mainCamera = Camera.main;
        drawGizmos = true;

        RenderTexture newTexture = new RenderTexture(texture)
            {
                height = Screen.height, width = Screen.width, depth = 24,name = gameObject.name + " (Instance)"
            };

        GameObject pairObj = new GameObject($"{gameObject.name}'s Camera", typeof(Camera));
        pairObj.transform.parent = pairPortal.transform;
        
        Camera pairCamera = pairObj.GetComponent<Camera>();
        pairCamera.CopyFrom(mainCamera);
        pairCamera.targetTexture = newTexture;
        
        
        pairPortalCamera = pairCamera;

        mainCameraCopyAsChild = new GameObject("Main Camera Copy");
        mainCameraCopyAsChild.transform.parent = transform;
    }

    private void Start()
    {
        if (pairPortal == null) return;
        
        Material newMaterial = new Material(renderer.material){name = gameObject.name + " (Instance)"};
        newMaterial.SetTexture(RenderTexture1, pairPortalCamera.targetTexture);

        renderer.material = newMaterial;

        OnUpdate += OnUpdateFunction;
    }

    private void Update()
    {
        OnUpdate.Invoke();
    }

    private void OnUpdateFunction()
    {
        UpdateCamera(pairPortalCamera);
        
        mainCamera = Camera.main;
        mainCameraCopyAsChild.transform.rotation = mainCamera.transform.rotation;
        mainCameraCopyAsChild.transform.position = mainCamera.transform.position;
    }

    private void UpdateCamera(Camera camera)
    {
        camera.transform.localPosition = -mainCameraCopyAsChild.transform.localPosition;
        camera.transform.localEulerAngles = mainCameraCopyAsChild.transform.localEulerAngles + new Vector3(0, 180, 0);

        camera.projectionMatrix = mainCamera.projectionMatrix;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        if (!ableToDrawGizmos) return;
        
        Gizmos.color = mainCameraGizmo;
        
        Gizmos.DrawSphere(mainCamera.transform.position, 0.25f);
        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + mainCamera.transform.forward);
        
        Gizmos.color = pairedCameraGizmo;
        
        Gizmos.DrawSphere(pairPortal.pairPortalCamera.transform.position, 0.5f);
        Gizmos.DrawLine(pairPortal.pairPortalCamera.transform.position, pairPortal.pairPortalCamera.transform.position + pairPortal.pairPortalCamera.transform.forward);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Portal))]
public class PortalCustomEditor : KamCustomEditor
{
    private Portal targetScript;
    public override void GameDesignerInspector()
    {
        targetScript = (Portal)target;
        targetScript.pairPortal = EditorGUILayout.ObjectField(new GUIContent("Paired Portal", "This is the portal this portal is linked to. This means it'll show the perspective of its pair through it."),targetScript.pairPortal,typeof(Portal),true) as Portal;
    }
}
#endif

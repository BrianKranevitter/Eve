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
    public Transform testTransform;
    public RenderTexture texture;
    public MeshRenderer renderer;
    public Portal pairPortal;
    private Camera pairPortalCamera;
    private GameObject mainCameraCopyAsChild;
    public float  nearClipOffset;
    public float  nearClipLimit;

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

        pairPortalCamera.enabled = false;
    }

    private void Start()
    {
        if (pairPortal == null) return;
        
        Material newMaterial = new Material(renderer.material){name = gameObject.name + " (Instance)"};
        newMaterial.SetTexture(RenderTexture1, pairPortalCamera.targetTexture);

        renderer.material = newMaterial;

        OnUpdate += OnUpdateFunction;
    }

    private bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    private void Update()
    {
        OnUpdate.Invoke();
    }
    
    private void OnUpdateFunction()
    {
        mainCamera = Camera.main;
        
        if (!VisibleFromCamera(renderer, mainCamera))
        {
            return;
        }
        
        renderer.material.SetTexture(RenderTexture1, pairPortalCamera.targetTexture);
        UpdateCamera(pairPortalCamera);
        
        mainCameraCopyAsChild.transform.rotation = mainCamera.transform.rotation;
        mainCameraCopyAsChild.transform.position = mainCamera.transform.position;
    }

    private void UpdateCamera(Camera camera)
    {
        renderer.enabled = false;
        
        //rotation and position
        /*
        var localToWorldMatrix = mainCamera.transform.localToWorldMatrix;
        localToWorldMatrix = transform.localToWorldMatrix * pairPortal.transform.worldToLocalMatrix * localToWorldMatrix;

        pairPortalCamera.transform.SetPositionAndRotation(localToWorldMatrix.GetColumn(3),localToWorldMatrix.rotation);*/
        camera.transform.localPosition = new Vector3(-mainCameraCopyAsChild.transform.localPosition.x,mainCameraCopyAsChild.transform.localPosition.y,-mainCameraCopyAsChild.transform.localPosition.z);
        camera.transform.localEulerAngles = mainCameraCopyAsChild.transform.localEulerAngles + new Vector3(0, 180, 0);

        camera.projectionMatrix = mainCamera.projectionMatrix;
        //SetNearClipPlane(camera);
        
        camera.Render();

        renderer.enabled = true;
    }

    void SetNearClipPlane(Camera camera)
    {
        Transform clipPlane = testTransform;
        float dot = Mathf.Sign(Vector3.Dot(clipPlane.forward, transform.position - camera.transform.position));

        Vector3 camSpacePos = camera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = camera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camspacedist = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;
        
        if (Mathf.Abs (camspacedist) > nearClipLimit) {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camspacedist);

            camera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        } else {
            camera.projectionMatrix = mainCamera.projectionMatrix;
        }
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

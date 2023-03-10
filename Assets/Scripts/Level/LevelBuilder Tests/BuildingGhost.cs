using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    private Transform visual;
    private PlaceableObjectSO placedObjectTypeSO;

    private void Start() {
        RefreshVisual();

        CustomGridBuilderManager.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(PlaceableObjectSO selected) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Vector3 targetPosition = CustomGridBuilderManager.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = CustomGridBuilderManager.Instance.GetSelectedGrid().GetOrigin().y + 0.1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, CustomGridBuilderManager.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        placedObjectTypeSO = CustomGridBuilderManager.Instance.GetSelectedObject();

        if (placedObjectTypeSO != null) {
            visual = Instantiate(placedObjectTypeSO.prefab, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer) {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

}


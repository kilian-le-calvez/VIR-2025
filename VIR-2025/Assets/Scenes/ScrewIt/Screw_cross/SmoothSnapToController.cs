using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SmoothSnapToController : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Transform controllerTransform;

    [SerializeField] private float snapDuration = 0.5f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 180, 0);

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!(args.interactorObject is XRBaseInteractor interactor)) return;

        controllerTransform = interactor.transform;
        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;

        StartCoroutine(SmoothSnapCoroutine());
    }

    private IEnumerator SmoothSnapCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < snapDuration)
        {
            if (controllerTransform == null) yield break;

            float t = elapsedTime / snapDuration;
            transform.position = Vector3.Lerp(startPosition, controllerTransform.position, t);
            transform.rotation = Quaternion.Slerp(startRotation, controllerTransform.rotation * Quaternion.Euler(rotationOffset), t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final snap to the controller position and rotation
        if (controllerTransform != null)
        {
            transform.position = controllerTransform.position;
            transform.rotation = controllerTransform.rotation * Quaternion.Euler(rotationOffset);
        }

        // Re-enable position and rotation tracking
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;

        // Manually update the position and rotation to match the snap
        grabInteractable.attachTransform.position = transform.position;
        grabInteractable.attachTransform.rotation = transform.rotation;

        // Optionally, you can simulate the "SelectExit" and "SelectEnter" process to ensure the system resets correctly:
        var interactionManager = grabInteractable.interactionManager;

        // Trigger SelectExit to release the grab
        if (interactionManager != null)
        {
            interactionManager.SelectExit(grabInteractable.interactorsSelecting as IXRSelectInteractor, grabInteractable);
        }

        // Trigger SelectEnter to re-grab the object and update the position
        if (interactionManager != null && controllerTransform != null)
        {
            interactionManager.SelectEnter(grabInteractable as IXRSelectInteractor, controllerTransform as IXRSelectInteractable);
        }
    }
}

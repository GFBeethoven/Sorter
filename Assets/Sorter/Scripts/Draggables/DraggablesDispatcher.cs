using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

public class DraggablesDispatcher
{
    private readonly static int DraggablesLayerMask = LayerMask.GetMask(Draggable.LayerMask);
    private readonly static int DraggableDropZonesLayerMask = LayerMask.GetMask(DraggableDropZone.LayerMask);

    [Inject] private Camera _camera;

    private Dictionary<int, IDraggable> _draggedObjects = new();
    private HashSet<IDraggable> _activeDraggables = new();

    private bool _isLaunched = false;

    public void Launch()
    {
        if (_isLaunched) return;

        TouchSimulation.Enable();

        _isLaunched = true;
    }

    public void Stop()
    {
        if (!_isLaunched) return;

        TouchSimulation.Disable();

        _isLaunched = false;
    }

    public void Update()
    {
        if (Touchscreen.current == null)
            return;

        var touchArray = Touchscreen.current.touches;

        for (int i = 0; i < touchArray.Count; i++)
        {
            var touch = touchArray[i];

            if (!touch.press.isPressed)
                continue;

            int touchId = touch.touchId.ReadValue();
            Vector2 touchPos = touch.position.ReadValue();

            var phase = touch.phase.ReadValue();

            switch (phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    TryBeginDrag(touchId, touchPos);
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    ContinueDrag(touchId, touchPos);
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    EndDrag(touchId, touchPos);
                    break;
            }
        }
    }

    private void TryBeginDrag(int pointerId, Vector2 screenPos)
    {
        if (_draggedObjects.ContainsKey(pointerId))
            return;

        Ray ray = _camera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, DraggablesLayerMask, QueryTriggerInteraction.Collide))
        {
            var draggable = hit.collider.GetComponent<IDraggable>();

            if (_activeDraggables.Contains(draggable) == false)
            {
                draggable.DragStart(hit.point);

                _draggedObjects[pointerId] = draggable;

                _activeDraggables.Add(draggable);
            }
        }
    }

    private void ContinueDrag(int pointerId, Vector2 screenPos)
    {
        if (_draggedObjects.TryGetValue(pointerId, out var draggable))
        {
            Vector3 worldPos = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0.0f));

            draggable.Drag(worldPos);
        }
    }

    private void EndDrag(int pointerId, Vector2 screenPos)
    {
        if (_draggedObjects.TryGetValue(pointerId, out var draggable))
        {
            Vector3 worldPos = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0.0f));

            draggable.DragEnd(worldPos);

            Ray ray = _camera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, DraggableDropZonesLayerMask, QueryTriggerInteraction.Collide))
            {
                draggable.SetOwner(hit.collider.gameObject.GetComponent<IDraggableDropZone>());
            }
        }

        _draggedObjects.Remove(pointerId);

        _activeDraggables.Remove(draggable);
    }
}

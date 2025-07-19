using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Zenject;

public class DraggablesDispatcher
{
    private readonly static int DraggablesLayerMask = LayerMask.GetMask(Draggable.LayerMask);
    private readonly static int DraggableDropZonesLayerMask = LayerMask.GetMask(DraggableDropZone<Draggable>.LayerMask);

    private Camera _camera;

    private SignalBus _signalBus;

    private Dictionary<int, IDraggable> _draggedObjects = new();
    private Dictionary<IDraggable, int> _activeDraggables = new();
    private Dictionary<int, Vector2> _pointerPositions = new();

    private bool _isLaunched = false;

    public DraggablesDispatcher(Camera camera, SignalBus signalBus)
    {
        _camera = camera;

        _signalBus = signalBus;
    }

    public void Launch()
    {
        if (_isLaunched) return;

        TouchSimulation.Enable();

        _signalBus.Subscribe<OnCannotDragDraggable>(OnCannotDrag);

        _isLaunched = true;
    }

    public void Stop()
    {
        if (!_isLaunched) return;

        TouchSimulation.Disable();

        _signalBus.TryUnsubscribe<OnCannotDragDraggable>(OnCannotDrag);

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

    private void OnCannotDrag(OnCannotDragDraggable signal)
    {
        if (signal.Draggable == null) return;

        IDraggable draggable = signal.Draggable;

        if (_activeDraggables.TryGetValue(draggable, out int pointerId) && 
            _pointerPositions.TryGetValue(pointerId, out Vector2 pointerPos))
        {
            EndDrag(pointerId, pointerPos);
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

            if (_activeDraggables.ContainsKey(draggable) == false)
            {
                draggable.DragStart(hit.point);

                _draggedObjects[pointerId] = draggable;

                _activeDraggables.Add(draggable, pointerId);

                _pointerPositions[pointerId] = screenPos;
            }
        }
    }

    private void ContinueDrag(int pointerId, Vector2 screenPos)
    {
        if (_draggedObjects.TryGetValue(pointerId, out var draggable))
        {
            Vector3 worldPos = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0.0f));

            draggable.Drag(worldPos);

            _pointerPositions[pointerId] = screenPos;
        }
    }

    private void EndDrag(int pointerId, Vector2 screenPos)
    {
        if (_draggedObjects.TryGetValue(pointerId, out var draggable))
        {
            _pointerPositions[pointerId] = screenPos;

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

        _pointerPositions.Remove(pointerId);
    }
}

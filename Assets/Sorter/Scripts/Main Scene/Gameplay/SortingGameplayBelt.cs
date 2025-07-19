using UnityEngine;

[RequireComponent(typeof(WorldRectTransform))]
public class SortingGameplayBelt : DraggableDropZone<Draggable>
{
    [SerializeField] private Belt _belt;


}

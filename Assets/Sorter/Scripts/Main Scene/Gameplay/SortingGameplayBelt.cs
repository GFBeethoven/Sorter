using UnityEngine;

[RequireComponent(typeof(WorldRectTransform))]
public class SortingGameplayBelt : DraggableDropZone<SortingGameplayFigure>
{
    [SerializeField] private Belt _belt;

    private void Update()
    {
        Rect beltLine = _belt.MiddleWorldRect;

        int count = DraggablesCount;

        for (int i = 0; i < count; i++)
        {
            SortingGameplayFigure figure = GetItem(i);

            
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "GameplayLayoutConfig", menuName = "Sorter/Gameplay Layout Config")]
public class GameplayLayoutConfig : ScriptableObject
{
    [field: SerializeField] public Rect LinesRect { get; private set; }

    [field: SerializeField] public Rect SlotsRect { get; private set; }

    [field: SerializeField] public Rect SpawnRect { get; private set; }

    [field: SerializeField] public Rect DestroyRect { get; private set; }

    public void OnValidate()
    {
        LinesRect = ValidateRect(LinesRect);
        SlotsRect = ValidateRect(SlotsRect);
        SpawnRect = ValidateRect(SpawnRect);
        DestroyRect = ValidateRect(DestroyRect);
    }

    private Rect ValidateRect(Rect rect)
    {
        rect.xMin = Mathf.Clamp01(rect.xMin);
        rect.xMax = Mathf.Clamp01(rect.xMax);
        rect.yMin = Mathf.Clamp01(rect.yMin);
        rect.yMax = Mathf.Clamp01(rect.yMax);

        return rect;
    }
}

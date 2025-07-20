using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SortingGameplayView : FSMStateMono<GameplayState.EnterData>
{
    [SerializeField] private LineWorldLayoutGroup _beltsGroup;

    [SerializeField] private LineWorldLayoutGroup _holesGroup;

    [Inject] private GameViewport _viewport;

    public override void Setup(FSMState<GameplayState.EnterData> fsmState)
    {
        base.Setup(fsmState);

        _beltsGroup.Initialize(_viewport, Vector2.zero, Vector2.one);
        _holesGroup.Initialize(_viewport, Vector2.zero, Vector2.one);

        _beltsGroup.Direction = LineWorldLayoutGroup.Axis.Vertical;
        _holesGroup.Direction = LineWorldLayoutGroup.Axis.Horizontal;
    }

    public void UpdateView(GameplayLayoutConfig layout, IEnumerable<SortingGameplayBelt> belts, 
        IEnumerable<SortingGameplayFigureHole> holes)
    {
        _beltsGroup.MinAnchor.Value = layout.LinesRect.min;
        _beltsGroup.MaxAnchor.Value = layout.LinesRect.max;

        _holesGroup.MinAnchor.Value = layout.SlotsRect.min;
        _holesGroup.MaxAnchor.Value = layout.SlotsRect.max;

        _beltsGroup.Children.Clear();
        _holesGroup.Children.Clear();

        foreach (var belt in belts)
        {
            _beltsGroup.Children.Add(belt.WorldRectTransform);
        }

        foreach (var hole in holes)
        {
            _holesGroup.Children.Add(hole.WorldRectTransform);
        }
    }
}

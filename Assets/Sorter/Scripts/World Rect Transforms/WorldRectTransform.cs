using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRectTransform : MonoBehaviour
{
    public ReactiveProperty<Vector2> Position { get; private set; } = new(Vector2.zero);

    public ReactiveProperty<Vector2> Size { get; private set; } = new(Vector2.one);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRectTransform : MonoBehaviour
{
    public ReactiveProperty<Vector2> Size { get; private set; } = new(Vector2.one);

    public ReactiveProperty<Vector3> Scale { get; private set; } = new(Vector3.one);
}

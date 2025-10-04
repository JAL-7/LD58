using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    static readonly List<Building> ActiveBuildings = new List<Building>();

    public static IReadOnlyList<Building> Active => ActiveBuildings;

    public Rect GetWorldRect()
    {
        if (TryGetComponent<Renderer>(out var renderer))
        {
            return BoundsToRect(renderer.bounds);
        }

        Vector3 lossyScale = transform.lossyScale;
        Vector2 size = new Vector2(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y));
        if (size.sqrMagnitude <= Mathf.Epsilon)
        {
            size = Vector2.one;
        }

        Vector2 center = new Vector2(transform.position.x, transform.position.y);
        Vector2 min = center - (size * 0.5f);
        return new Rect(min, size);
    }

    public Rect GetExpandedWorldRect(float margin)
    {
        Rect worldRect = GetWorldRect();
        float clampedMargin = Mathf.Max(0f, margin);
        Vector2 expansion = new Vector2(clampedMargin * 2f, clampedMargin * 2f);
        Vector2 min = new Vector2(worldRect.xMin - clampedMargin, worldRect.yMin - clampedMargin);
        return new Rect(min, worldRect.size + expansion);
    }

    void OnEnable()
    {
        if (!ActiveBuildings.Contains(this))
        {
            ActiveBuildings.Add(this);
        }
    }

    void OnDisable()
    {
        ActiveBuildings.Remove(this);
    }

    static Rect BoundsToRect(Bounds bounds)
    {
        Vector3 size3 = bounds.size;
        Vector3 min3 = bounds.min;
        return new Rect(new Vector2(min3.x, min3.y), new Vector2(size3.x, size3.y));
    }
}

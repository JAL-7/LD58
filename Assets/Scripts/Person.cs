using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public string personName;
    public int age;
    public List<Trait> traits;
    public bool isWandering;
    public float speed = 1f;

    [SerializeField]
    float arrivalThreshold = 0.05f;

    [SerializeField, Min(0f)]
    float minimumSeparation = 0.25f;

    [Header("Viewport Padding (%)")]
    [SerializeField]
    float viewportPaddingLeftPercent = 5f;

    [SerializeField]
    float viewportPaddingRightPercent = 5f;

    [SerializeField]
    float viewportPaddingTopPercent = 5f;

    [SerializeField]
    float viewportPaddingBottomPercent = 5f;

    static readonly List<Person> ActivePeople = new List<Person>();
    const float PathIntersectionEpsilon = 0.0001f;

    Vector3 wanderTarget;
    bool hasWanderTarget;
    Camera cachedCamera;

    void Awake()
    {
        cachedCamera = Camera.main;
    }

    void OnEnable()
    {
        if (!ActivePeople.Contains(this))
        {
            ActivePeople.Add(this);
        }
    }

    void OnDisable()
    {
        ActivePeople.Remove(this);
    }

    void Update()
    {
        if (!isWandering)
        {
            hasWanderTarget = false;
            return;
        }

        bool needsTarget = !hasWanderTarget || ReachedTarget() || !IsPositionRespectingSeparation(transform.position);

        if (needsTarget)
        {
            TryPickNewTarget();
        }

        if (hasWanderTarget)
        {
            MoveTowardsTarget();
        }
    }

    bool TryPickNewTarget()
    {
        const int maxAttempts = 12;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (TryGetRandomPointInView(out Vector3 candidate) && IsPositionRespectingSeparation(candidate) && IsPathClearOfBuildings(transform.position, candidate))
            {
                wanderTarget = candidate;
                hasWanderTarget = true;
                return true;
            }
        }

        hasWanderTarget = false;
        return false;
    }

    bool TryGetRandomPointInView(out Vector3 target)
    {
        target = transform.position;

        Vector3 toPerson = transform.position - cachedCamera.transform.position;
        float distance = Vector3.Dot(toPerson, cachedCamera.transform.forward);

        if (Mathf.Approximately(distance, 0f))
        {
            if (cachedCamera.orthographic)
            {
                distance = Mathf.Abs(transform.position.z - cachedCamera.transform.position.z);
            }
            else
            {
                distance = toPerson.magnitude;
            }
        }

        if (Mathf.Approximately(distance, 0f))
        {
            distance = cachedCamera.nearClipPlane;
        }

        Vector3 bottomLeft = cachedCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distance));
        Vector3 topRight = cachedCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distance));

        float minX = Mathf.Min(bottomLeft.x, topRight.x);
        float maxX = Mathf.Max(bottomLeft.x, topRight.x);
        float minY = Mathf.Min(bottomLeft.y, topRight.y);
        float maxY = Mathf.Max(bottomLeft.y, topRight.y);

        float width = maxX - minX;
        float height = maxY - minY;

        if (width <= 0f || height <= 0f)
        {
            return false;
        }

        minX += width * viewportPaddingLeftPercent * 0.01f;
        maxX -= width * viewportPaddingRightPercent * 0.01f;
        maxY -= height * viewportPaddingTopPercent * 0.01f;
        minY += height * viewportPaddingBottomPercent * 0.01f;
        minX += minimumSeparation;
        maxX -= minimumSeparation;
        minY += minimumSeparation;
        maxY -= minimumSeparation;

        if (maxX <= minX || maxY <= minY)
        {
            return false;
        }

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        target = new Vector3(x, y, transform.position.z);
        return true;
    }

    public bool TryPlaceAtRandomPosition(int maxAttempts = 20)
    {
        if (maxAttempts < 1)
        {
            maxAttempts = 1;
        }

        Vector3 startingPosition = transform.position;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (!TryGetRandomPointInView(out Vector3 candidate))
            {
                break;
            }

            if (!IsPositionRespectingSeparation(candidate))
            {
                continue;
            }

            transform.position = candidate;
            hasWanderTarget = false;
            return true;
        }

        transform.position = startingPosition;
        return false;
    }

    void MoveTowardsTarget()
    {
        if (!hasWanderTarget || speed <= 0f)
        {
            return;
        }

        Vector3 current = transform.position;
        Vector3 target = new Vector3(wanderTarget.x, wanderTarget.y, current.z);
        float step = speed * Time.deltaTime;

        Vector3 nextPosition = Vector3.MoveTowards(current, target, step);

        if (!IsPathClearOfBuildings(current, nextPosition) || !IsPositionRespectingSeparation(nextPosition))
        {
            hasWanderTarget = false;
            return;
        }

        transform.position = nextPosition;
    }

    bool ReachedTarget()
    {
        if (!hasWanderTarget)
        {
            return false;
        }

        Vector2 current2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 target2D = new Vector2(wanderTarget.x, wanderTarget.y);
        float threshold = Mathf.Max(0.001f, arrivalThreshold);
        return Vector2.Distance(current2D, target2D) <= threshold;
    }

    bool IsPositionRespectingSeparation(Vector3 position)
    {
        if (!IsPositionClearOfBuildings(position))
        {
            return false;
        }

        if (minimumSeparation <= 0f)
        {
            return true;
        }

        Vector2 position2D = new Vector2(position.x, position.y);

        for (int i = ActivePeople.Count - 1; i >= 0; i--)
        {
            Person other = ActivePeople[i];
            if (other == null)
            {
                ActivePeople.RemoveAt(i);
                continue;
            }

            if (other == this)
            {
                continue;
            }

            Vector3 otherPosition = other.transform.position;
            Vector2 other2D = new Vector2(otherPosition.x, otherPosition.y);
            if (Vector2.Distance(position2D, other2D) < minimumSeparation)
            {
                return false;
            }
        }

        return true;
    }

    bool IsPositionClearOfBuildings(Vector3 position)
    {
        IReadOnlyList<Building> buildings = Building.Active;
        if (buildings == null || buildings.Count == 0)
        {
            return true;
        }

        Vector2 position2D = new Vector2(position.x, position.y);
        float clearance = Mathf.Max(0f, minimumSeparation);

        for (int i = 0; i < buildings.Count; i++)
        {
            Building building = buildings[i];
            if (building == null)
            {
                continue;
            }

            Rect expanded = building.GetExpandedWorldRect(clearance);
            if (expanded.Contains(position2D))
            {
                return false;
            }
        }

        return true;
    }

    bool IsPathClearOfBuildings(Vector3 start, Vector3 end)
    {
        IReadOnlyList<Building> buildings = Building.Active;
        if (buildings == null || buildings.Count == 0)
        {
            return true;
        }

        Vector2 start2D = new Vector2(start.x, start.y);
        Vector2 end2D = new Vector2(end.x, end.y);
        float clearance = Mathf.Max(0f, minimumSeparation);

        for (int i = 0; i < buildings.Count; i++)
        {
            Building building = buildings[i];
            if (building == null)
            {
                continue;
            }

            Rect expanded = building.GetExpandedWorldRect(clearance);
            if (SegmentIntersectsRect(start2D, end2D, expanded))
            {
                return false;
            }
        }

        return true;
    }

    static bool SegmentIntersectsRect(Vector2 start, Vector2 end, Rect rect)
    {
        if (rect.Contains(start) || rect.Contains(end))
        {
            return true;
        }

        Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
        Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);
        Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
        Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);

        if (SegmentsIntersect(start, end, bottomLeft, bottomRight))
        {
            return true;
        }

        if (SegmentsIntersect(start, end, bottomRight, topRight))
        {
            return true;
        }

        if (SegmentsIntersect(start, end, topRight, topLeft))
        {
            return true;
        }

        if (SegmentsIntersect(start, end, topLeft, bottomLeft))
        {
            return true;
        }

        return false;
    }

    static bool SegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float o1 = Orientation(p1, p2, q1);
        float o2 = Orientation(p1, p2, q2);
        float o3 = Orientation(q1, q2, p1);
        float o4 = Orientation(q1, q2, p2);

        if ((o1 > 0f && o2 < 0f || o1 < 0f && o2 > 0f) && (o3 > 0f && o4 < 0f || o3 < 0f && o4 > 0f))
        {
            return true;
        }

        if (Mathf.Abs(o1) <= PathIntersectionEpsilon && IsPointOnSegment(p1, p2, q1))
        {
            return true;
        }

        if (Mathf.Abs(o2) <= PathIntersectionEpsilon && IsPointOnSegment(p1, p2, q2))
        {
            return true;
        }

        if (Mathf.Abs(o3) <= PathIntersectionEpsilon && IsPointOnSegment(q1, q2, p1))
        {
            return true;
        }

        if (Mathf.Abs(o4) <= PathIntersectionEpsilon && IsPointOnSegment(q1, q2, p2))
        {
            return true;
        }

        return false;
    }

    static float Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    static bool IsPointOnSegment(Vector2 a, Vector2 b, Vector2 point)
    {
        return point.x >= Mathf.Min(a.x, b.x) - PathIntersectionEpsilon &&
               point.x <= Mathf.Max(a.x, b.x) + PathIntersectionEpsilon &&
               point.y >= Mathf.Min(a.y, b.y) - PathIntersectionEpsilon &&
               point.y <= Mathf.Max(a.y, b.y) + PathIntersectionEpsilon;
    }

}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingController : MonoBehaviour
{
    public Color drawColor = new Color(0.961f, 0.651f, 0.137f, 1f);
    public Sprite inkSprite;
    public float minPointDistance = 0.15f;

    private Camera cam;
    private LineRenderer previewLine;
    private List<Vector2> points = new List<Vector2>();
    private bool isDrawing;

    private GameManager gameManager;

    private void Awake()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        BuildPreviewLine();
    }

    private void BuildPreviewLine()
    {
        GameObject go = new GameObject("PreviewLine");
        go.transform.SetParent(transform);
        previewLine = go.AddComponent<LineRenderer>();
        previewLine.material = new Material(Shader.Find("Sprites/Default"));
        previewLine.startColor = drawColor;
        previewLine.endColor = drawColor;
        previewLine.startWidth = DrawnObject.LineThickness;
        previewLine.endWidth = DrawnObject.LineThickness;
        previewLine.numCapVertices = 6;
        previewLine.numCornerVertices = 6;
        previewLine.sortingOrder = 10;
        previewLine.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrawing();
        else if (Input.GetMouseButton(0) && isDrawing)
            ContinueDrawing();
        else if (Input.GetMouseButtonUp(0) && isDrawing)
            FinishDrawing();
    }

    private void TryStartDrawing()
    {
        if (IsPointerOverUI()) return;
        if (gameManager != null && !gameManager.CanInteract) return;
        if (InkManager.Instance != null && !InkManager.Instance.HasInkLeft(minPointDistance)) return;

        isDrawing = true;
        points.Clear();

        Vector2 worldPos = GetWorldPoint();
        points.Add(worldPos);
        UpdatePreview();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("draw");
    }

    private void ContinueDrawing()
    {
        Vector2 worldPos = GetWorldPoint();

        if (points.Count == 0)
        {
            points.Add(worldPos);
            return;
        }

        float dist = Vector2.Distance(points[points.Count - 1], worldPos);
        if (dist < minPointDistance) return;

        float currentLength = GetPathLength();
        if (InkManager.Instance != null && !InkManager.Instance.CanDraw(currentLength + dist))
        {
            FinishDrawing();
            return;
        }

        points.Add(worldPos);
        UpdatePreview();
    }

    private void FinishDrawing()
    {
        isDrawing = false;
        previewLine.positionCount = 0;

        if (points.Count < 2)
        {
            points.Clear();
            return;
        }

        float length = GetPathLength();

        bool heavy = PowerUpManager.HeavyNextDraw;
        bool bomb = PowerUpManager.BombNextDraw;

        GameObject go = new GameObject("DrawnObject");
        DrawnObject drawn = go.AddComponent<DrawnObject>();
        drawn.segmentSprite = inkSprite;
        drawn.Build(new List<Vector2>(points), drawColor, heavy, bomb);

        PowerUpManager.ClearPendingDrawFlags();

        if (InkManager.Instance != null)
            InkManager.Instance.SpendInk(length);

        if (gameManager != null)
            gameManager.RegisterDrawnObject(go);

        points.Clear();
    }

    private float GetPathLength()
    {
        float length = 0f;
        for (int i = 0; i < points.Count - 1; i++)
            length += Vector2.Distance(points[i], points[i + 1]);
        return length;
    }

    private void UpdatePreview()
    {
        previewLine.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
            previewLine.SetPosition(i, new Vector3(points[i].x, points[i].y, 0f));
    }

    private Vector2 GetWorldPoint()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = -cam.transform.position.z;
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // TextMeshPro kütüphanesi

 // Text yerine TextMeshProUGUI

public class LineGame : MonoBehaviour
{
    private TextMeshProUGUI lineCountText;
    private LineRenderer currentLineRenderer;
    private List<LineData> lines = new List<LineData>();
    private bool isDrawing = false;
    private bool isGameOver = false;
    private List<Vector3> currentPoints = new List<Vector3>();
    public GameObject gameOverPanel; // Game Over ekraný
  
    private int lineCount = 0; // Çizgi sayýsý

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateLineCount();
        CreateNewLineRenderer();
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            currentPoints.Clear();

            Vector3 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPoint.z = 0;
            currentPoints.Add(startPoint);

            currentLineRenderer.positionCount = 1;
            currentLineRenderer.SetPosition(0, startPoint);
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            Vector3 currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPoint.z = 0;

            if (currentPoints.Count == 0 || Vector3.Distance(currentPoints[currentPoints.Count - 1], currentPoint) > 0.1f)
            {
                currentPoints.Add(currentPoint);
                currentLineRenderer.positionCount = currentPoints.Count;
                currentLineRenderer.SetPositions(currentPoints.ToArray());
            }
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;

            if (currentPoints.Count < 2)
            {
                Debug.Log("Çizgi çok kýsa!");
                return;
            }

            Vector3 startPoint = currentPoints[0];
            Vector3 endPoint = currentPoints[currentPoints.Count - 1];

            if (IsGameOver(startPoint, endPoint))
            {
                Debug.Log("Game Over");
                ShowGameOverScreen();
                isGameOver = true;
            }
            else
            {
                lines.Add(new LineData(startPoint, endPoint));
                lineCount++;
                UpdateLineCount();
                CreateNewLineRenderer();
            }
        }
    }

    private void CreateNewLineRenderer()
    {
        GameObject lineObject = new GameObject("Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.numCapVertices = 10;
        currentLineRenderer = lineRenderer;
    }

    private bool IsGameOver(Vector3 start, Vector3 end)
    {
        float newLineLength = Vector3.Distance(start, end);

        foreach (var line in lines)
        {
            if (DoLinesIntersect(line.Start, line.End, start, end))
            {
                return true;
            }
        }

        if (lines.Count > 0)
        {
            float lastLineLength = Vector3.Distance(lines[lines.Count - 1].Start, lines[lines.Count - 1].End);
            if (newLineLength <= lastLineLength)
            {
                return true;
            }
        }

        return false;
    }

    private bool DoLinesIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        float d = (a1.x - a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x - b2.x);
        if (d == 0) return false;

        float u = ((b1.x - a1.x) * (b1.y - b2.y) - (b1.y - a1.y) * (b1.x - b2.x)) / d;
        float v = ((b1.x - a1.x) * (a1.y - a2.y) - (b1.y - a1.y) * (a1.x - a2.x)) / d;

        return (u >= 0 && u <= 1 && v >= 0 && v <= 1);
    }

    private void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Game Over paneli bulunamadý. Lütfen UI elemanlarýný sahneye doðru þekilde eklediðinizden emin olun.");
        }
    }

    private void UpdateLineCount()
    {
        if (lineCountText != null)
        {
            lineCountText.text = $"Lines: {lineCount}";
        }
        else
        {
            Debug.LogWarning("LineCountText baðlý deðil!");
        }
    }

    public void RestartGame()
    {
        foreach (var line in GameObject.FindGameObjectsWithTag("Line"))
        {
            Destroy(line);
        }

        lines.Clear();
        lineCount = 0;
        UpdateLineCount();
        CreateNewLineRenderer();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        isGameOver = false;
    }

    private class LineData
    {
        public Vector3 Start;
        public Vector3 End;

        public LineData(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }
    }
}
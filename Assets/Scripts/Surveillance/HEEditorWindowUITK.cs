using System;
using System.Collections.Generic;
using JSM.Surveillance.Surveillance;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;


public class HEEditorWindowUITK : EditorWindow
{
    [MenuItem("Tools/Half-Edge/UITK Graph")]
    public static void Open() => GetWindow<HEEditorWindowUITK>("HE Graph (UITK)");

    VisualElement canvas, content;
    GraphDrawElement draw;
    HEGraphData data = new();
    HEGraphInteraction interaction;
    HEToolbar toolbarLogic;
    HEToolbar.Mode mode;

    Vector2 lastCanvasSize;

    void CreateGUI()
    {
        var root = rootVisualElement;
        root.style.flexGrow = 1;

        // Toolbar
        toolbarLogic = new HEToolbar();
        var toolbar = toolbarLogic.Build();
        toolbarLogic.OnModeChanged += m => mode = m;
        toolbarLogic.OnClear += () => { data.Clear(); draw.MarkDirtyRepaint(); };
        root.Add(toolbar);

        // Canvas + content
        canvas = new VisualElement { style = { flexGrow = 1, backgroundColor = new Color(0.13f,0.13f,0.13f) } };
        root.Add(canvas);
        content = new VisualElement { style = { position = Position.Absolute, left = 0, right = 0, top = 0, bottom = 0 } };
        canvas.Add(content);

        draw = new GraphDrawElement();
        content.Add(draw);

        interaction = new HEGraphInteraction(data, draw, canvas);
        toolbarLogic.OnSubdivide += SubdivideSelectedQuad;

        // Event hooks
        canvas.RegisterCallback<WheelEvent>(OnWheel);
        canvas.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        canvas.RegisterCallback<PointerMoveEvent>(interaction.HandlePointerMove);
        canvas.RegisterCallback<PointerUpEvent>(_ => panning = false);
        canvas.RegisterCallback<GeometryChangedEvent>(OnCanvasResized);
        canvas.RegisterCallback<PointerDownEvent>(OnPointerDown);
        canvas.RegisterCallback<PointerUpEvent>(OnPointerUp);
        canvas.RegisterCallback<PointerUpEvent>(interaction.HandlePointerUp);
        canvas.RegisterCallback<PointerDownEvent>(e =>
        {
            interaction.HandlePointerDown(e, mode, shift: e.shiftKey);
            
            UpdateTransform();
            if (interaction.selectedFace != -1)
            {
                // Convert UITK panel position to screen pixels
                Rect anchorRect = new Rect(e.position, Vector2.zero);
                UnityEditor.PopupWindow.Show(anchorRect, new HEFaceEditor());

            }
        });
        interaction.ReloadGraph.AddListener(UpdateTransform);
        
        data.EnsureCornerVertices(draw.GridCols, draw.GridRows, pin: true);
        UpdateTransform();
    }

    bool panning;
    Vector2 last;
    
    
    void SubdivideSelectedQuad()
    {
        var sel = interaction.selectedVertices;
        if (sel.Count != 4) { Debug.LogWarning("Select exactly 4 vertices to subdivide."); return; }

        // Replace perimeter + insert lattice
        int[] nine = data.SubdivideQuadReplacePerimeter(
            data, sel[0], sel[1], sel[2], sel[3], addInteriorEdges: true, pinNew: false);

        // Pick the center afterwards
        interaction.selectedVertices.Clear();
        interaction.selectedVertices.Add(nine[4]);
        interaction.selectedV = nine[4];
        data.RebuildFaces();
        
        UpdateTransform();
    }
    
    void OnPointerDown(PointerDownEvent e)
    {
        if (e.button == 2 || e.altKey) { panning = true; last = e.localPosition; e.StopPropagation(); return; }
        UpdateTransform();
    }
    
    void OnPointerMove(PointerMoveEvent e)
    {
        if (!panning) return;
        var d = (Vector2)e.localPosition - last;
        interaction.pan += d;
        last = e.localPosition;
        UpdateTransform();
    }
    
    void OnPointerUp(PointerUpEvent e) { panning = false; Vector2 mouseCanvas = draw.ChangeCoordinatesTo(canvas, e.localPosition); }
    
    void OnWheel(WheelEvent e)
    {
        Vector2 mouseCanvas = canvas.WorldToLocal(e.mousePosition);
        Vector2 center = new(canvas.resolvedStyle.width * 0.5f, canvas.resolvedStyle.height * 0.5f);
        Vector2 world = (mouseCanvas - interaction.pan - center) / interaction.zoom;
        float step = Mathf.Pow(1.1f, -e.delta.y);
        float newZoom = Mathf.Clamp(interaction.zoom * step, 0.25f, 4f);
        interaction.pan = mouseCanvas - center - world * newZoom;
        interaction.zoom = newZoom;
        UpdateTransform();
    }

    void OnCanvasResized(GeometryChangedEvent e)
    {
        var newSize = new Vector2(canvas.resolvedStyle.width, canvas.resolvedStyle.height);
        if (lastCanvasSize != Vector2.zero)
        {
            Vector2 oldCenter = lastCanvasSize * 0.5f;
            Vector2 newCenter = newSize * 0.5f;
            interaction.pan += (newCenter - oldCenter);
        }
        lastCanvasSize = newSize;
        UpdateTransform();
    }

    void UpdateTransform()
    {
        draw.Zoom = interaction.zoom;
        draw.Pan = interaction.pan;
        draw.CanvasSize = new Vector2(canvas.resolvedStyle.width, canvas.resolvedStyle.height);
        draw.VertPositions = data.verts.ConvertAll(v => draw.GridToWorld(v.ij));
        draw.EdgePairs = data.edges.ConvertAll(e => (e.a, e.b));
        draw.SelectedVertex = interaction.selectedV;
        draw.SelectedEdge = interaction.selectedE;
        draw.SelectedVerticesSet = new HashSet<int>(interaction.selectedVertices);
        draw.SelectedFace = interaction.selectedFace;
        draw.Faces = data.faces;
        draw.MarkDirtyRepaint();
    }
}


public class GraphDrawElement : VisualElement
{
    public float Zoom { get; set; } = 1f;         
    public Vector2 Pan { get; set; } = Vector2.zero; 
    public Vector2 CanvasSize { get; set; } = Vector2.zero;

    public int GridCols = 50;        
    public int GridRows = 50;      
    public float GridSpacing = 15f;   
    public bool CenterGrid = true;   
   
    public List<Vector2> VertPositions; // read-only reference
    public List<(int a,int b)> EdgePairs;
    public List<HEFace> Faces;     // set from window
    public HashSet<int> SelectedVerticesSet;
    public int SelectedVertex = -1;
    public int SelectedEdge   = -1;
    public int SelectedFace = -1;
    
    public GraphDrawElement()
    {
        generateVisualContent += OnGenerate;
        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        style.left = 0; style.top = 0; style.right = 0; style.bottom = 0;
    }

    Vector2 Center => new Vector2(CanvasSize.x * 0.5f, CanvasSize.y * 0.5f);
    Vector2 ScreenFromWorld(Vector2 w) => Pan + Center + Zoom * w;
    Vector2 WorldFromScreen(Vector2 s) => (s - Pan - Center) / Zoom;
    public float XStart => CenterGrid ? -(GridCols/2) * GridSpacing : 0f;
    public float YStart => CenterGrid ? -(GridRows/2) * GridSpacing : 0f;

    public Vector2 GridToWorld(Vector2 ij)
        => new Vector2(XStart + ij.x * GridSpacing, YStart + ij.y * GridSpacing);

    // World -> nearest grid index
    public Vector2 WorldToGrid(Vector2 w)
        => new Vector2(
            ((w.x - XStart) / GridSpacing),
            ((w.y - YStart) / GridSpacing)
        );

    public bool GridInBounds(Vector2 ij)
        => ij.x >= 0 && ij.x < GridCols && ij.y >= 0 && ij.y < GridRows;
    
    
    void DrawFaces(Painter2D p)
    {
        if (Faces == null) return;

        foreach (var f in Faces)
        {
            if (f.isExterior) continue;
            var col = new Color(f.color.r, f.color.g, f.color.b, 0.75f); 
            p.fillColor = SelectedFace >= 0 && Faces[SelectedFace] == f ? col:  f.color;
            p.BeginPath();
            var v0 = ScreenFromWorld(VertPositions[f.loop[0]]);
            p.MoveTo(v0);
            
            for (int k = 1; k < f.loop.Count; k++)
            {
                p.LineTo(ScreenFromWorld(VertPositions[f.loop[k]]));
            }

            p.ClosePath();
            p.Fill();
        }
    }

    void DrawVertices(Painter2D p)
    {
        if (VertPositions == null) return;
        float r = 6f / Mathf.Max(Zoom, 1e-6f);
        for (int i = 0; i < VertPositions.Count; i++)
        {
            var v = VertPositions[i];
            bool isMultiSel = SelectedVerticesSet != null && SelectedVerticesSet.Contains(i);
            p.fillColor = isMultiSel
                ? new Color(1f, 0.6f, 0.2f, 1f)   // highlight selection
                : new Color(0.95f, 0.95f, 0.95f, 1f);
            p.BeginPath(); p.Arc(v, r, 0, 360); p.ClosePath(); p.Fill();
        }
    }
    
    void OnGenerate(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        var r = contentRect; // draw-local rect; we treat draw-local == world coords BEFORE transform

        Vector2 w00 = WorldFromScreen(Vector2.zero);
        Vector2 w11 = WorldFromScreen(CanvasSize);
        float wxMin = Mathf.Min(w00.x, w11.x);
        float wxMax = Mathf.Max(w00.x, w11.x);
        float wyMin = Mathf.Min(w00.y, w11.y);
        float wyMax = Mathf.Max(w00.y, w11.y);

        float xStart, yStart;
        if (CenterGrid)
        {
            int halfC = GridCols / 2;
            int halfR = GridRows / 2;
            xStart = -halfC * GridSpacing;
            yStart = -halfR * GridSpacing;
        }
        else
        {
            xStart = 0f;
            yStart = 0f;
        }

        p.lineWidth = 1f;

        for (int i = 0; i < GridCols; i++)
        {
            float xw = xStart + i * GridSpacing;
            if (xw < wxMin || xw > wxMax) continue; 
            
            float xy = yStart + GridCols * GridSpacing;
            
            if (xw < wxMin || xw > wxMax) continue;
            Vector2 a = ScreenFromWorld(new Vector2(xw, yStart));
            Vector2 b = ScreenFromWorld(new Vector2(xw, xy));

            p.strokeColor = (i % 5 == 0) ? new Color(1,1,1,0.18f) : new Color(1,1,1,0.08f);
            p.BeginPath(); p.MoveTo(a); p.LineTo(b); p.Stroke();
        }

        for (int j = 0; j < GridRows; j++)
        {
            float yw = yStart + j * GridSpacing;
            if (yw < wyMin || yw > wyMax) continue;
            
            float xw = xStart + GridRows * GridSpacing;

            Vector2 a = ScreenFromWorld(new Vector2(xStart, yw));
            Vector2 b = ScreenFromWorld(new Vector2(xw, yw));

            p.strokeColor = (j % 5 == 0) ? new Color(1,1,1,0.18f) : new Color(1,1,1,0.08f);
            p.BeginPath(); p.MoveTo(a); p.LineTo(b); p.Stroke();
        }
        
        DrawFaces(p);
        
        if (EdgePairs != null || VertPositions != null)
        {
            for (int i=0;i<EdgePairs.Count;i++)
            {
                var (a,b) = (EdgePairs[i].a , EdgePairs[i].b);
                if (a<0 || b<0 || a>=VertPositions.Count || b>=VertPositions.Count) continue;

                ctx.painter2D.strokeColor = (i == SelectedEdge) ? new Color(1f,0.85f,0.3f,1f) : new Color(0.8f,0.9f,1f,0.9f);
                ctx.painter2D.lineWidth = 2f * Mathf.Max(Zoom, 0.0001f); // constant pixel width
                ctx.painter2D.BeginPath();
                ctx.painter2D.MoveTo(ScreenFromWorld(VertPositions[a]));
                ctx.painter2D.LineTo(ScreenFromWorld(VertPositions[b]));
                ctx.painter2D.Stroke();
            }

          
            float rNew = 6f * Mathf.Max(Zoom, 0.0001f); // constant pixel radius
            for (int i=0;i<VertPositions.Count;i++)
            {
                var v =  ScreenFromWorld((VertPositions[i]));
                
                bool isMultiSel = SelectedVerticesSet != null && SelectedVerticesSet.Contains(i);
                ctx.painter2D.fillColor = isMultiSel
                    ? new Color(1f, 0.6f, 0.2f, 1f)   // highlight selection
                    : new Color(0.95f, 0.95f, 0.95f, 1f);
                
                ctx.painter2D.BeginPath();
                ctx.painter2D.Arc(v, rNew, 0, 360);
                ctx.painter2D.ClosePath();
                ctx.painter2D.Fill();
            }
        }
        
    }
}

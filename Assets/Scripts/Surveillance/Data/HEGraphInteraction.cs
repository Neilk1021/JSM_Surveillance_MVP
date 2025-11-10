using UnityEngine.Events;

namespace JSM.Surveillance.Surveillance
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class HEGraphInteraction
    {
        HEGraphData data;
        readonly GraphDrawElement draw;
        readonly VisualElement canvas;

        public float zoom = 1f;
        public Vector2 pan = Vector2.zero;
        public const float PickRadiusPx = 10f;

        int edgeStartV = -1;
        public int selectedV = -1, selectedE = -1;
        public int selectedFace = -1;
        private bool _pointerDown;
        private HEToolbar.Mode _mode;

        public void SetData(HEGraphData newData)
        {
            data = newData;
        }
        
        public HEGraphInteraction(HEGraphData data, GraphDrawElement draw, VisualElement canvas)
        {
            this.data = data;
            this.draw = draw;
            this.canvas = canvas;
        }

        Vector2 CenterPx => new(canvas.resolvedStyle.width * 0.5f, canvas.resolvedStyle.height * 0.5f);
        Vector2 ScreenToWorld(Vector2 s) => (s - pan - CenterPx) / zoom;
        Vector2 WorldToScreen(Vector2 w) => pan + CenterPx + zoom * w;

        public UnityEvent ReloadGraph = new UnityEvent();
        
        public readonly System.Collections.Generic.List<int> selectedVertices = new(); 
        
        public void HandlePointerDown(PointerDownEvent e, HEToolbar.Mode mode, bool shift)
        {
            _mode = mode;
            if (e.button == 2 || e.altKey) return;

            if (e.button == 0)
            {
                _pointerDown = true;
            }
            Vector2 mouseCanvas = canvas.WorldToLocal(e.position);
            Vector2 world = ScreenToWorld(mouseCanvas);
            Vector2 ij = draw.WorldToGrid(world);

            switch (mode)
            {
                case HEToolbar.Mode.AddVertex:
                    if (!draw.GridInBounds(ij)) return;
                    data.verts.Add(new HEGraphData.Vertex(ij.x, ij.y));
                    selectedV = data.verts.Count - 1;
                    data.RebuildFaces();
                    break;

                case HEToolbar.Mode.AddEdge:
                    int va = PickVertex(mouseCanvas);
                    if (va >= 0)
                    {
                        selectedV = va;
                        if (edgeStartV < 0) edgeStartV = va;
                        else if (edgeStartV != va)
                        {
                            data.edges.Add(new HEGraphData.Edge(edgeStartV, va));
                            edgeStartV = -1;
                        }
                        data.RebuildFaces();
                    }
                    break;

                case HEToolbar.Mode.Select:
                    selectedV = PickVertex(mouseCanvas);
                    if (selectedV >= 0)
                    {
                        selectedE = -1;
                        selectedFace = -1;
                        if (!shift) selectedVertices.Clear();
                        if (!selectedVertices.Contains(selectedV)) selectedVertices.Add(selectedV);
                        selectedV = PickVertex(mouseCanvas);
                        if (selectedV < 0) selectedE = PickEdge(mouseCanvas);
                    }
                    else if (!shift)
                    {
                        selectedVertices.Clear();
                        selectedV = -1; selectedE = -1;
                    }

                    if (selectedV < 0)
                    {
                        selectedE = PickEdge(mouseCanvas);
                        if (selectedE != -1)
                        {
                            selectedFace = -1;
                        }
                        else
                        {
                            selectedFace = PickFace(mouseCanvas);
                        }
                    }
                    break;

                case HEToolbar.Mode.Delete:
                    int v = PickVertex(mouseCanvas);
                    data.RebuildFaces();
                    if (v >= 0) data.RemoveVertex(v);
                    else
                    {
                        int eIdx = PickEdge(mouseCanvas);
                        if (eIdx >= 0) data.edges.RemoveAt(eIdx);
                    }
                    break;
            }
        }

        private int PickFace(Vector2 mouseCanvas)
        {
            Vector2 graphPos = ScreenToWorld(mouseCanvas);
            graphPos = draw.WorldToGrid(graphPos);
            return HEGraphDataUtil.PickFace(data, graphPos);
        }

        public void HandlePointerMove(PointerMoveEvent e)
        {
            if (_mode == HEToolbar.Mode.Select && _pointerDown && selectedV != -1)
            {
                float zoom = draw.Zoom;
                data.MoveVertexBy(selectedV,   (e.deltaPosition / draw.GridSpacing)/zoom);
                ReloadGraph?.Invoke();
            }
        }
        
        public void HandlePointerUp(PointerUpEvent e)
        {
            if (e.button == 2 || e.altKey) return;
            
            _pointerDown = false;
        }

        int PickVertex(Vector2 mouseCanvasPx)
        {
            int hit = -1; float best = PickRadiusPx;
            for (int i = 0; i < data.verts.Count; i++)
            {
                Vector2 w = draw.GridToWorld(data.verts[i].ij);
                Vector2 s = WorldToScreen(w);
                float d = Vector2.Distance(mouseCanvasPx, s);
                if (d <= best) { best = d; hit = i; }
            }
            return hit;
        }

        int PickEdge(Vector2 mouseCanvasPx)
        {
            int hit = -1; float best = PickRadiusPx;
            for (int i = 0; i < data.edges.Count; i++)
            {
                var e = data.edges[i];
                Vector2 a = WorldToScreen(draw.GridToWorld(data.verts[e.a].ij));
                Vector2 b = WorldToScreen(draw.GridToWorld(data.verts[e.b].ij));
                float d = PointSegDist(mouseCanvasPx, a, b);
                if (d <= best) { best = d; hit = i; }
            }
            return hit;
        }

        static float PointSegDist(Vector2 p, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / (ab.sqrMagnitude + 1e-6f));
            return Vector2.Distance(p, a + t * ab);
        }
    }

}
namespace JSM.Surveillance.Surveillance
{
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using System;
    using System.Collections.Generic;

    public class HEToolbar
    {
        public enum Mode { Select, AddVertex, AddEdge, Delete }
        public Mode CurrentMode { get; private set; } = Mode.Select;

        public event Action<Mode> OnModeChanged;
        public event Action OnClear;

        // keep references so we can sync states without recursion
        private readonly Dictionary<Mode, ToolbarToggle> _toggles = new();

        public event Action OnSubdivide;
        public event Action OnSave;
        public event Action OnLoad;

        public Toolbar Build()
        {
            var toolbar = new Toolbar();
            AddToggle(toolbar, "Select",     Mode.Select,  start: true);
            AddToggle(toolbar, "Add Vertex", Mode.AddVertex);
            AddToggle(toolbar, "Add Edge",   Mode.AddEdge);
            AddToggle(toolbar, "Delete",     Mode.Delete);

            toolbar.Add(new ToolbarButton(() => OnSubdivide?.Invoke()) { text = "Subdivide 3×3" });
            toolbar.Add(new ToolbarSpacer());
            toolbar.Add(new ToolbarButton(() => OnClear?.Invoke()) { text = "Clear" });
            
            toolbar.Add(new ToolbarButton(() => OnLoad?.Invoke()){text = "Load"});
            toolbar.Add(new ToolbarButton((() => OnSave?.Invoke())) {text = "Save"});

            ApplyModeToToggles(CurrentMode);
            return toolbar;
        }


        void AddToggle(Toolbar toolbar, string label, Mode mode, bool start = false)
        {
            var t = new ToolbarToggle { text = label };
            t.value = start;
            _toggles[mode] = t;
            toolbar.Add(t);

            t.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    SetMode(mode);
                }
                else
                {
                    if (mode == CurrentMode)
                        t.SetValueWithoutNotify(true);
                }
            });
        }

        public void SetMode(Mode m)
        {
            if (m == CurrentMode) return;
            CurrentMode = m;
            ApplyModeToToggles(m);
            OnModeChanged?.Invoke(m);
        }

        void ApplyModeToToggles(Mode active)
        {
            foreach (var kv in _toggles)
                kv.Value.SetValueWithoutNotify(kv.Key == active);
        }
    }
}

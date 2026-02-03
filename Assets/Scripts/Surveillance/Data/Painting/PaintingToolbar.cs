
#if UNITY_EDITOR

using System;
using JSM.Surveillance.Data;
using JSM.Surveillance.Surveillance;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class PaintingToolbar
{
    private static HEGraphInteraction _interaction;
    
    public static VisualElement MakePaintingToolBar(HEGraphInteraction interaction)
    {
        _interaction = interaction;
        var toolbar = new VisualElement();
        SetToolBarStyles(toolbar);
        
        var handle = new VisualElement();
        SetHandleStyles(handle);
        toolbar.Add(handle);
        
        MakeDraggable(handle, toolbar);   // drag toolbar via handle
   
        AddToolButton(toolbar, "AvatarSelector@2x", "Population", () =>
        {
            _interaction.SwitchPaintMode(PopulationPainter.PaintType.Population);
        });
        
        AddToolButton(toolbar, "d_scenevis_visible_hover@2x",     "Risk", () =>
        {
            _interaction.SwitchPaintMode(PopulationPainter.PaintType.Risk);
        });
        
        
        AddToolButton(toolbar, "d_scenevis_visible_hover@2x",     "Consumer", () =>
        {
            _interaction.SwitchPaintMode(PopulationPainter.PaintType.Consumer);
        });
        
        
        AddToolButton(toolbar, "d_scenevis_visible_hover@2x",     "GovtInfo", () =>
        {
            _interaction.SwitchPaintMode(PopulationPainter.PaintType.GovtInfo);
        });
        
        AddToolButton(toolbar, "d_scenevis_visible_hover@2x",     "CorpInfo", () =>
        {
            _interaction.SwitchPaintMode(PopulationPainter.PaintType.CorpInfo);
        });


        return toolbar;
        
    }

    private static void SetHandleStyles(VisualElement handle)
    {
        handle.style.height = 6;
        handle.style.width = 28;
        handle.style.marginTop = 4;
        handle.style.marginBottom = 4;
        handle.style.backgroundColor = new Color(1f, 1f, 1f, 0.12f);
    }

    private static void SetToolBarStyles(VisualElement toolbar)
    {
        toolbar.name = "floating-toolbar";
        toolbar.style.position = Position.Absolute;
        toolbar.style.left     = 10;
        toolbar.style.top      = 40;
        toolbar.style.width    = 40;
        SetPadding(toolbar, 4);
        toolbar.style.borderTopLeftRadius     = 4;
        toolbar.style.borderTopRightRadius    = 4;
        toolbar.style.borderBottomLeftRadius  = 4;
        toolbar.style.borderBottomRightRadius = 4;

        toolbar.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f);
        toolbar.style.flexDirection = FlexDirection.Column;
        toolbar.style.alignItems = Align.Center;
    }

    private static void SetPadding(VisualElement element, float padding)
    {
        element.style.paddingBottom = padding;
        element.style.paddingBottom = padding;
        element.style.paddingBottom = padding;
        element.style.paddingBottom = padding;
    }
    
    private static void AddToolButton(VisualElement parent, string iconName, string tooltip, Action action)
    {
        var tex = EditorGUIUtility.IconContent(iconName).image as Texture2D;

        var button = new Button();
        button.tooltip = tooltip;
        button.style.width = 30;
        button.style.height = 30;
        button.style.marginTop = 2;
        button.style.marginBottom = 2;
        var icon = new Image {
            image = tex
        };
        button.Add(icon);
        button.style.alignContent = Align.Center;
        button.style.alignItems = Align.Center;
        button.style.justifyContent = Justify.Center;
  
        button.style.unityBackgroundImageTintColor = Color.white;
        button.clicked += action;
        parent.Add(button);
    }
    
    private static void MakeDraggable(VisualElement dragHandle, VisualElement moved)
    {
        Vector3 offset = default;

        dragHandle.RegisterCallback<PointerDownEvent>(evt =>
        {
            var mousePanelPos = evt.position;
            var toolbarPanelPos = moved.transform.position;

            offset = mousePanelPos - toolbarPanelPos;

            dragHandle.CaptureMouse();
            evt.StopPropagation();
        }, TrickleDown.TrickleDown);

        dragHandle.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (!dragHandle.HasMouseCapture())
                return;

            var mousePanelPos = evt.position;
            var newPanelPos  = mousePanelPos - offset;

            moved.transform.position = newPanelPos;

            evt.StopPropagation();
        }, TrickleDown.TrickleDown);

        dragHandle.RegisterCallback<PointerUpEvent>(evt =>
        {
            if (dragHandle.HasMouseCapture())
                dragHandle.ReleaseMouse();
            evt.StopPropagation();
        }, TrickleDown.TrickleDown);
    }

    
    
}
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphicPanel {
    public string panelName;
    public GameObject rootPanel;
    //This will not be used due to deadline.
    private List<GraphicLayer> layers = new List<GraphicLayer>();

    public GraphicLayer GetLayer(int layerDepth, bool forceCreation = false)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            GraphicLayer layer = layers[i];
            if (layer.layerDepth == layerDepth)
                return layer;
        }

        if (forceCreation)
            return CreateLayer(layerDepth);

        return null;
    }

    public GraphicLayer CreateLayer(int layerDepth)
    {
        GraphicLayer layer = new GraphicLayer();
        GameObject panelObj = new GameObject(string.Format(GraphicLayer.LAYER_NAME_FORMAT, layerDepth));
        RectTransform rect = panelObj.AddComponent<RectTransform>();

        //For transparency and stuff.
        panelObj.AddComponent<CanvasGroup>();
        panelObj.transform.SetParent(rootPanel.transform, false);

        //Adjust it to the parent panel.
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        layer.panel = panelObj.transform;
        layer.layerDepth = layerDepth;

        int index = layers.FindIndex(l => l.layerDepth > layerDepth);

        //Insertar por orden de creación.
        if (index == -1)
            layers.Add(layer);
        else
            layers.Insert(index, layer);

        for (int i = 0; i < layers.Count; ++i)
        {
            layers[i].panel.SetSiblingIndex(layers[i].layerDepth);
        }

        return layer;
    }
}

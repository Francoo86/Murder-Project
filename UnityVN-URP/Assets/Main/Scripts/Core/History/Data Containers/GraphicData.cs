using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace History
{

    public class GraphicData
    {
        public string panelName;
        public List <LayerData> layers;

        [System.Serializable]
        public class LayerData
        { 
            public int depth = 0;
            public string graphicName;
            public string graphicPath;
            public bool isVideo;
            public bool useAudio;

            public LayerData(GraphicLayer layer) 
            { 
                depth = layer.layerDepth;
                if (layer.CurrentGraphic == null)
                    return;

                var graphic = layer.CurrentGraphic;
                graphicName = graphic.GraphicName;
                graphicPath = graphic.graphicPath;
                isVideo = graphic.IsVideo;
                useAudio = graphic.useAudio;
            }

        }

        public static List<GraphicData> Capture()
        {
            List<GraphicData> graphicPanels = new List<GraphicData>();
            foreach (var panel in GraphicPanelController.Instance.allPanels)
            {
                if (panel.isClear)
                    continue;

                GraphicData data = new GraphicData();
                data.panelName = panel.panelName;
                data.layers = new List<LayerData>();

                foreach (var layer in panel.layers)
                {
                    LayerData entry = new LayerData(layer);
                    data.layers.Add(entry);
                }

                graphicPanels.Add(data);
            }
            return graphicPanels;
        }

    }
}

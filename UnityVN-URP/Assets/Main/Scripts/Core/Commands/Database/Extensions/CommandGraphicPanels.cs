﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class CommandGraphicPanels : CommandDBExtension
{
    private static string[] PARAM_PANEL = new string[] { "-p", "-panel" };
    private static string[] PARAM_LAYER = new string[] { "-l", "-layer" };
    private static string[] PARAM_MEDIA = new string[] { "-g", "-media" };
    private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
    private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
    private static string[] PARAM_BLENDTEX = new string[] { "-b", "-blend" };
    private static string[] PARAM_USEVIDEOAUDIO = new string[] { "-aud", "-audio" };
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    new public static void Extend(CommandDB commandDB)
    {
        commandDB.AddCommand("setlayermedia", new Func<string[], IEnumerator>(SetLayerMedia));
        commandDB.AddCommand("clearlayermedia", new Func<string[], IEnumerator>(ClearLayerMedia));
    }
    private static IEnumerator SetLayerMedia(string[] data)
    {
        string panelName = "";
        string mediaName = "";
        float transitionSpeed = 1;
        bool immediate = false;
        //bool useAudio = false;
        string pathToGraphic = "";

        UnityEngine.Object graphic = null;
        var parameters = ConvertToParams(data);
        parameters.TryGetValue(PARAM_PANEL, out panelName);
        GraphicPanel panel = GraphicPanelController.Instance.GetPanel(panelName);
        if (panel == null)
        {
            Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel. please check the panel name and adjust the command.");
            yield break;
        }

        parameters.TryGetValue(PARAM_MEDIA, out mediaName);
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

        Debug.LogWarning($"Current graphic path thing.");
        pathToGraphic = GetPathToGraphic(FilePaths.ResourcesBGImages, mediaName);
        graphic = Resources.Load<Texture>(pathToGraphic);
        if (graphic == null)
        {
            pathToGraphic = GetPathToGraphic(FilePaths.ResourcesBGVideos, mediaName);
            graphic = Resources.Load<VideoClip>(pathToGraphic);
        }
        if (graphic == null)
        {
            Debug.LogError($"Could not find a graphic called '{mediaName}' in the Resources directories");
            yield break;
        }

        GraphicLayer graphicLayer = panel.GetLayer(0, forceCreation: true);

        if (graphic is Texture)
        {
            if(!immediate)
                CommandController.Instance.AddTerminationActionToActualProcess(() => { graphicLayer?.SetTexture (graphic as Texture, path: pathToGraphic, immediate: true); });
            yield return graphicLayer.SetTexture(graphic as Texture, transitionSpeed, pathToGraphic, immediate);
        }
        else
        {
            if(!immediate)
                CommandController.Instance.AddTerminationActionToActualProcess(() => { graphicLayer?.SetVideo(graphic as VideoClip, path: pathToGraphic, immediate: true); });
            yield return graphicLayer.SetVideo(graphic as VideoClip, transitionSpeed, false, pathToGraphic, immediate);
        }
    }

    private static IEnumerator ClearLayerMedia(string[] data)
    {
        string panelName = "";
        int layer = 0;
        float transitionSpeed = 0;
        bool immediate = false;

        var parameters = ConvertToParams(data);

        parameters.TryGetValue(PARAM_PANEL, out panelName);
        GraphicPanel panel = GraphicPanelController.Instance.GetPanel(panelName);
        if (panel == null)
        {
            Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel. please check the pnael name and adjust the command.");
            yield break;
        }

        parameters.TryGetValue(PARAM_LAYER, out layer, defaultVal: -1);

        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

        if (!immediate)
            parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultVal: 1);

        if (layer == -1)
            panel.Clear(transitionSpeed, immediate);
        else
        {
            GraphicLayer graphicLayer = panel.GetLayer(layer);
            if (graphicLayer == null)
            {
                Debug.LogError($"could not clear layer [{layer}] on panel '{panel.panelName}'");
                yield break;
            }

            graphicLayer.Clear(transitionSpeed, immediate);
        }
    }

    private static string GetPathToGraphic(string defaultPath, string graphicName)
    {
        if (graphicName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return graphicName.Substring(HOME_DIRECTORY_SYMBOL.Length);
        return defaultPath + graphicName;
    }
}
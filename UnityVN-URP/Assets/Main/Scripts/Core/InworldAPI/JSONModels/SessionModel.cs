using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Create models to hold infos.
//Similar to POCOs.

//Serialize sessions.

class CharacterAssetInfo { 
    public string Avatar {  get; set; }
    public string AvatarOG {  get; set; }
}
class CharacterSessionInfo { 
    public string Name {  get; set; }
    public string Character {  get; set; }
    public string DisplayName { get; set; }
    //public CharacterAssetInfo CharAssets {  get; set; }
}
class SessionResponseModel {
    public string Name { get; set; }
    public List<CharacterSessionInfo> SessionCharacters { get; set; }
    public string LoadedScene { get; set;}
}

class InteractionInfo { 
    public string Name { get; set; }
    public List<string> TextList { get; set; }
    public CustomEventInfo CustomEvent { get; set; }
    public List<string> Params {  get; set; }
    public EmotionInfo Emotion { get; set; }
    public string Session { get; set; }
    public RelationshipUpdate Relationship { get; set; }
    public List<string> ActiveTriggers { get; set; }
}

class CustomEventInfo { 
    public string Event {  get; set; }
    public List<string> Params { get; set; }
}

class EmotionInfo
{
    public string Behavior { get; set; }
    public string Strength { get; set; }

}

class RelationshipUpdate
{
    public int Trust { get; set; }
    public int Respect { get; set; }
    public int Familiar { get; set; }
    public int Flirtatious { get; set; }
    public int Attraction { get; set; }
}
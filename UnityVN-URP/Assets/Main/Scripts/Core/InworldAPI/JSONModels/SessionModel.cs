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
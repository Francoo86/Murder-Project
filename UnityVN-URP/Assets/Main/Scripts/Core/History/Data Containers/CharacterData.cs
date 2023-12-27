using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace History
{
    [System.Serializable]
    public class CharacterData
    {
        public string characterName;
        public string displayName;
        public bool enabled;
        public Color color;
        public int priority;
        public bool isHighlighted;
        public bool isFacingLeft;
        public Vector2 position;
        public CharacterConfigCache characterConfig;

        public string dataJSON;

        [System.Serializable]
        public class CharacterConfigCache
        {
            public string name;
            public string alias;

            public Character.CharacterType characterType;

            public Color nameCol;
            public Color diagCol;

            public float nameFont = 1f;
            public float diagFont = 1f;

            public CharacterConfigCache(CharacterConfigData reference)
            {
                name = reference.name;
                alias = reference.alias;
                characterType = reference.charType;

                nameCol = reference.nameCol;
                diagCol = reference.diagCol;

                // No los pillo en CharacterConfig Data
                // No estan por que dentro de los requerimientos no necesitamos tanta info.
                /*
                nameFontScale = FilePaths.ResourcesFonts + reference.nameFont.name;
                dialogueFontScale = FilePaths.ResourcesFonts + reference.diagFont.name;

                nameFont = reference.nameFont;
                diagFont = reference.diagFont;*/
            }
        }

        public static List<CharacterData> Capture()
        {
            List<CharacterData> characters = new List<CharacterData>();
            foreach (var character in CharacterController.Instance.allCharacters)
            {
                if (!character.IsVisible)
                    continue;

                CharacterData entry = new CharacterData();
                entry.characterName = character.name;
                entry.displayName = character.displayName;
                entry.enabled = character.IsVisible;
                entry.color = character.color;
                entry.priority = character.priority;
                entry.isFacingLeft = character.isFacingLeft;
                entry.isHighlighted = character.highlighted;
                entry.position = character.targetPosition;
                entry.characterConfig = new CharacterConfigCache(character.config);

                switch (character.config.charType)
                {
                    case Character.CharacterType.Sprite:
                        //case Character.CharacterType.SpriteSheet:
                        SpriteData sData = new SpriteData();
                        sData.layers = new List<SpriteData.LayerData>();

                        SpriteCharacter sc = character as SpriteCharacter;
                        foreach (var layer in sc.layers)
                        {
                            var layerData = new SpriteData.LayerData();
                            layerData.color = layer.renderer.color;
                            layerData.spriteName = layer.renderer.sprite.name;
                            sData.layers.Add(layerData);
                        }
                        entry.dataJSON = JsonUtility.ToJson(sData);
                        break;
                }
                characters.Add(entry);
            }
            return characters;
        }

        public static void Apply(List<CharacterData> data)
        {
            List<string> cache = new List<string>();

            foreach (CharacterData characterData in data)
            {
                //Whats this?
                //if (characterData.characterName == "Innkepper")
                    //characterData.characterName = "Innkeepeer as Generic";

                Character character = CharacterController.Instance.GetCharacter(characterData.characterName, create: true);
                character.displayName = characterData.displayName;
                character.SetColor(characterData.color);

                if (characterData.isHighlighted)
                    character.Highlight(inmediate: true);
                else
                    character.UnHighlight(inmediate: true);

                //Vere esté video después si puedo.
                //character.SetPriority(characterData.priority);

                if (characterData.isFacingLeft)
                    character.FaceLeft(immediate: true);
                else
                    character.FaceRight(immediate: true);

                Debug.Log($"Position of the character to set: {characterData.position}");

                //HACK: This fixes the characters going to the begining of the screen.
                if (characterData.position != Vector2.zero)
                    character.SetPos(characterData.position);

                character.IsVisible = characterData.enabled;

                if(character.config.charType == Character.CharacterType.Sprite)
                {
                    SpriteData sData = JsonUtility.FromJson<SpriteData>(characterData.dataJSON);
                    SpriteCharacter sc = character as SpriteCharacter;

                    //for (int i = 0; i < sData.layers.Count; i++)
                    //{
                    //Solo ocupamos una capa.
                    int i = 0;
                    var layer = sData.layers[i];
                    if (sc.layers[i].renderer.sprite != null && sc.layers[i].renderer.sprite.name != layer.spriteName)
                    {
                        Sprite sprite = sc.GetSprite(layer.spriteName);
                        if (sprite != null)
                            sc.SetSprite(sprite, i);
                        else
                            Debug.LogWarning($"History State could not load sprite '{layer.spriteName}'");
                    }
                    //}

                    //Debug.Log($"LOL COUNT: {sData.layers.Count}");
                }

                cache.Add(character.name);
            }

            foreach (Character character in CharacterController.Instance.allCharacters)
            {
                if (!cache.Contains(character.name))
                    character.IsVisible = false;
            }
        }

        [System.Serializable]
        public class SpriteData 
        {
            public List<LayerData> layers;
            [System.Serializable]
            public class LayerData
            {
                public string spriteName;
                public Color color;
            }
        }
    }
}

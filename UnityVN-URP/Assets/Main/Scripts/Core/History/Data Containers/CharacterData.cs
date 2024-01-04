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
                //entry.priority = character.priority;
                entry.isFacingLeft = character.isFacingLeft;
                entry.isHighlighted = character.highlighted;
                entry.position = character.targetPosition;
                entry.characterConfig = new CharacterConfigCache(character.config);

                Character.CharacterType actualType = character.config.charType;

                if(actualType == Character.CharacterType.Sprite)
                {
                    SpriteData sData = new SpriteData();
                    sData.actualLayer = new SpriteData.LayerData();

                    SpriteCharacter sc = character as SpriteCharacter;
                    var layer = sc.currentLayer;
                    var layerData = new SpriteData.LayerData();
                    layerData.color = layer.renderer.color;
                    layerData.spriteName = layer.renderer.sprite.name;
                    sData.actualLayer = layerData;
                    entry.dataJSON = JsonUtility.ToJson(sData);
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

                    if (characterData.enabled)
                        sc.Show();

                    var layer = sData.actualLayer;
                    if (sc.currentLayer.renderer.sprite != null && sc.currentLayer.renderer.sprite.name != layer.spriteName)
                    {
                        Sprite sprite = sc.GetSprite(layer.spriteName);
                        if (sprite != null)
                            sc.SetSprite(sprite);
                        else
                            Debug.LogWarning($"History State could not load sprite '{layer.spriteName}'");
                    }
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
            public LayerData actualLayer;
            //public List<LayerData> layers;
            [System.Serializable]
            public class LayerData
            {
                public string spriteName;
                public Color color;
            }
        }
    }
}

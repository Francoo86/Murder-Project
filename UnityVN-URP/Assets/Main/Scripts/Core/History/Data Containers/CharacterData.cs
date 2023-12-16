using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace History
{

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
                characterType = reference.characterType;

                nameCol = reference.nameCol;
                diagCol = reference.diagCol;

                // No los pillo en CharacterConfig Data
                nameFontScale = FilePaths.ResourcesFonts + reference.nameFont.name;
                dialogueFontScale = FilePaths.ResourcesFonts + reference.diagFont.name;

                nameFont = reference.nameFont;
                diagFont = reference.diagFont;
            }
        }

        public static List<CharacterData> Capture()
        {
            List<CharacterData> characters = new List<CharacterData>();
            foreach (var character in CharacterController.Instance.storedChars)
            {
                if (!character.isVisible)
                    continue;

                CharacterData entry = new CharacterData();
                entry.characterName = character.name;
                entry.displayName = character.displayName;
                entry.enabled = character.isVisible;
                entry.color = character.color;
                entry.priority = character.priority;
                entry.isHighlighted = character.highlighted;
                entry.position = character.targetPosition;
                entry.characterConfig = new CharacterConfigCache(character.config);

                switch (character.config.characterType)
                {
                    case Character.CharacterType.Sprite:
                    case Character.CharacterType.SpriteSheet:
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

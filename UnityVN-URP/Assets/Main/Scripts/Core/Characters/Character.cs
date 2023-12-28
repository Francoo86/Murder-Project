using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS 
{
    /// <summary>
    /// Base class of Character.
    /// </summary>
    public abstract class Character
    {
        public const bool ENABLE_ON_START = false;
        private const float UNHIGHLIGHTED_DARKEN_STRENGTH = 0.65f;
        public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = true;

        public string name;
        public string displayName;
        //Hacer un cuadro para cada imagen de personaje.
        public RectTransform root;
        public CharacterConfigData config;
        public Animator animator;

        public Color color { get; protected set; } = Color.white;
        protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;

        protected CharacterController controller => CharacterController.Instance;

        protected Color highlightedColor => color;
        protected Color unhighlightedColor => new Color(color.r * UNHIGHLIGHTED_DARKEN_STRENGTH, color.g * UNHIGHLIGHTED_DARKEN_STRENGTH, color.b * UNHIGHLIGHTED_DARKEN_STRENGTH, color.a);
        public bool highlighted { get; private set; } = true;
        protected bool facingLeft = DEFAULT_ORIENTATION_IS_FACING_LEFT;
        public int priority { get; protected set; }
        public Vector2 targetPosition { get; protected set; }

        //Corutinas de mostrado.                              
        protected Coroutine CO_Hiding, CO_Showing, CO_Moving, co_highlighting, co_changingColor, co_flipping;
        //Logica de mostrar.
        public bool IsShowing => CO_Showing != null;
        public bool IsHiding => CO_Hiding != null;
        private bool IsMoving => CO_Moving != null;

        private bool isChangingColor => co_changingColor != null;
        private bool isHighlighting => (highlighted && co_highlighting != null);
        private bool isUnHighlighting => (!highlighted && co_highlighting != null);

        public virtual bool IsVisible { get; set; } = false;
        public bool isFacingLeft => facingLeft;
        public bool isFacingRight => !facingLeft;
        public bool isFlipping => co_flipping != null;
        public bool IsInInworld { get; set; } = false;

        /// <summary>
        /// Setups the character to be associated with its config and its prefab (image collection to be rendered).
        /// </summary>
        /// <param name="name">Character name</param>
        /// <param name="config">Configuration associated with it</param>
        /// <param name="prefab">Images associated with the character.</param>
        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            this.config = config;
            displayName = name;

            if (prefab != null)
            {
                GameObject obj = UnityEngine.Object.Instantiate(prefab, controller.CharacterPanel);
                obj.SetActive(true);
                root = obj.GetComponent<RectTransform>();
                animator = obj.GetComponentInChildren<Animator>();
            }
            Debug.Log($"Creating character in base: {name}");
        }

        public DialogController DController => DialogController.Instance;

        /// <summary>
        /// Changes the dialog color of the character in the dialog box.
        /// </summary>
        /// <param name="col">New color.</param>
        public void SetDialogColor(Color col) => config.diagCol = col;
        /// <summary>
        /// Changes the name color of the character in the name box.
        /// </summary>
        /// <param name="col">New color.</param>
        public void SetNameColor(Color col) => config.nameCol = col;

        /// <summary>
        /// Changes the name font of the character in the name box.
        /// </summary>
        /// <param name="font">New TMPro font (this can be assigned manually in the config).</param>
        public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
        /// <summary>
        /// Changes the dialog font of the character in the dialog box.
        /// </summary>
        /// <param name="font">New TMPro font (this can be assigned manually in the config).</param>
        public void SetDialogFont(TMP_FontAsset font) => config.diagFont = font;

        /// <summary>
        /// Updates the configuration by the settings applied before (font, color, name).
        /// </summary>
        public void UpdateConfigOnScreen() => DController.ApplySpeakerDataToBox(config);
        /// <summary>
        /// Resets the character configuration to the defaults.
        /// </summary>
        public void ResetCharacterConfig() => config = CharacterController.Instance.GetCharacterConfig(name);

        //Hacer que el personaje hable.
        /// <summary>
        /// Makes the character say something raw.
        /// </summary>
        /// <param name="dialog">The line the character should say.</param>
        /// <returns>The talking process to be yielded.</returns>
        public Coroutine Say(string dialog) => Say(new List<string> { dialog });
        /// <summary>
        /// Makes the character say something line by line.
        /// </summary>
        /// <param name="dialogLines">The dialog lines.</param>
        /// <returns>The talking process to be yielded.</returns>
        public Coroutine Say(List<string> dialogLines)
        {
            DController.ShowSpeakerName(displayName);
            UpdateConfigOnScreen();
            return DController.Say(dialogLines);
        }

        /// <summary>
        /// Shows the character on the screen.
        /// </summary>
        /// <returns>The coroutine process of showing.</returns>
        public virtual Coroutine Show()
        {
            if (IsShowing)
                controller.StopCoroutine(CO_Showing);

            if (IsHiding)
                controller.StopCoroutine(CO_Hiding);

            CO_Showing = controller.StartCoroutine(HandleShowing(true));

            return CO_Showing;
        }

        /// <summary>
        /// Hides the character on the screen.
        /// </summary>
        /// <returns>The coroutine of hiding.</returns>
        public virtual Coroutine Hide()
        {
            if (IsHiding)
                controller.StopCoroutine(CO_Hiding);

            if (IsShowing)
                controller.StopCoroutine(CO_Showing);

            CO_Hiding = controller.StartCoroutine(HandleShowing(false));

            return CO_Hiding;
        }

        //Estos elementos no corresponden a esta clase.
        /// <summary>
        /// Changes the position of a character based on their anchors, also changes it to normalized vector for screen operatins.
        /// </summary>
        /// <param name="pos">The target position.</param>
        public virtual void SetPos(Vector2 pos)
        {
            if (root == null) return;
            (Vector2 minAnchor, Vector2 maxAnchor) = ConvertPosToRelative(pos);
            root.anchorMin = minAnchor;
            root.anchorMax = maxAnchor;

            targetPosition = pos;
        }

        /// <summary>
        /// Gets the position of the character based on their anchors. Think this like a box.
        /// </summary>
        /// <returns>The anchor minimums, The anchor maximums.</returns>
        public virtual (Vector2, Vector2) GetPos()
        {
            return (root.anchorMin, root.anchorMax);
        }
        /// <summary>
        /// Moves the character across the screen, but not inmediatly.
        /// </summary>
        /// <param name="pos">Target pos</param>
        /// <param name="speed">How fast the character should move.</param>
        /// <param name="smooth">Makes the movement smooth.</param>
        /// <returns>The Coroutine process related to the moving process.</returns>

        public virtual Coroutine MoveToPosition(Vector2 pos, float speed = 2f, bool smooth = false)
        {
            if (root == null) return null;
            if (IsMoving) controller.StopCoroutine(CO_Moving);

            CO_Moving = controller.StartCoroutine(MoveToPositionCoroutine(pos, speed, smooth));

            targetPosition = pos;

            return CO_Moving;
        }

        /// <summary>
        /// The wrapped function of MoveToPosition that handles the logic for character movement in the screen.
        /// </summary>
        /// <param name="pos">Target pos</param>
        /// <param name="speed">How fast the character should move.</param>
        /// <param name="smooth">Makes the movement smooth.</param>
        /// <returns>The IEnumerator to be yielded.</returns>
        private IEnumerator MoveToPositionCoroutine(Vector2 pos, float speed = 2f, bool smooth = false)
        {
            (Vector2 minAnch, Vector2 maxAnch) = ConvertPosToRelative(pos);
            Vector2 padding = root.anchorMax - root.anchorMin;

            while (root.anchorMin != minAnch || root.anchorMax != maxAnch)
            {
                root.anchorMin = smooth ? Vector2.Lerp(root.anchorMin, minAnch, speed * Time.deltaTime)
                    : Vector2.MoveTowards(root.anchorMin, minAnch, speed * Time.deltaTime * 0.35f);

                root.anchorMax = root.anchorMin + padding;

                if (smooth && Vector2.Distance(root.anchorMin, root.anchorMax) <= 0.001f)
                {
                    root.anchorMin = minAnch;
                    root.anchorMax = maxAnch;
                    break;
                }

                yield return null;
            }

            Debug.Log($"Finalización de movimiento.");
            CO_Moving = null;
        }

        /// <summary>
        /// Converts the world position to a normalized position for screen coordinates.
        /// </summary>
        /// <param name="pos">Position to be converted.</param>
        /// <returns>New anchor bounds.</returns>
        protected (Vector2, Vector2) ConvertPosToRelative(Vector2 pos)
        {
            //Vector normalizado ya que es una posición relativa como en cualquier juego.
            //Podria dar ejemplos de esto, por ejemplo un cuadrado dentro de un panel.
            Vector2 padding = root.anchorMax - root.anchorMin;
            float maxX = 1 - padding.x;
            float maxY = 1 - padding.y;

            Vector2 minAnchorTgt = new Vector2(maxX * pos.x, maxY * pos.y);
            Vector2 maxAnchorTgt = minAnchorTgt + padding;

            return (minAnchorTgt, maxAnchorTgt);

        }

        /// <summary>
        /// Handles the showing logic of the characters.
        /// </summary>
        /// <param name="shouldShow">Indicates if it should be shown or hidden.</param>
        /// <returns></returns>
        public virtual IEnumerator HandleShowing(bool shouldShow)
        {
            //Debug.LogWarning("Can't be called on abstract character class");
            yield return null;
        }

        /// <summary>
        /// Changes the character color.
        /// </summary>
        /// <param name="color">New color of the character.</param>

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        /// <summary>
        /// Changes the color but with a transition to make it smoother.
        /// </summary>
        /// <param name="color">New color of the character.</param>
        /// <param name="speed">How fast should be the transition.</param>
        /// <returns>The coroutine process of changing color.</returns>
        public Coroutine TransitionColor(Color color, float speed = 1f)
        {
            this.color = color;

            if (isChangingColor)
                controller.StopCoroutine(co_changingColor);

            co_changingColor = controller.StartCoroutine(ChangingColor(displayColor, speed));
            return co_changingColor;
        }
        /// <summary>
        /// The logic of making the transition for changing the color.
        /// </summary>
        /// <param name="color">New color to be set.</param>
        /// <param name="speed">How fast the transition should be.</param>
        /// <returns>The IEnumerator process to be yielded.</returns>
        public virtual IEnumerator ChangingColor(Color color, float speed)
        {
            Debug.LogWarning("Color changing is not applicable on this character type!");
            yield return null;
        }


        /// <summary>
        /// Highlights the character, makes the character like the main one.
        /// </summary>
        /// <param name="speed">How fast the transition of highlighting should be.</param>
        /// <param name="inmediate">Make it inmediate.</param>
        /// <returns>The coroutine process associated with the highlighting.</returns>
        public Coroutine Highlight(float speed = 1f, bool inmediate = false)
        {
            if (isHighlighting || isUnHighlighting)
                controller.StopCoroutine(co_highlighting);

            highlighted = true;
            co_highlighting = controller.StartCoroutine(Highlighting(highlighted, speed, inmediate));

            return co_highlighting;
        }

        /// <summary>
        /// Unhighlights the character, makes the character like if it was hiding.
        /// </summary>
        /// <param name="speed">How fast the transition of highlighting should be.</param>
        /// <param name="inmediate">Make it inmediate.</param>
        /// <returns>The coroutine process associated with the highlighting.</returns>
        public Coroutine UnHighlight(float speed = 1f, bool inmediate = false)
        {
            if (isUnHighlighting || isHighlighting)
                controller.StopCoroutine(co_highlighting);

            highlighted = false;
            co_highlighting = controller.StartCoroutine(Highlighting(highlighted, speed, inmediate));

            return co_highlighting;
        }

        /// <summary>
        /// The highlighting logic for the character.
        /// </summary>
        /// <param name="highlight">Make it highlight.</param>
        /// <param name="speedMultiplier">How fast will the transition be.</param>
        /// <param name="inmediate"></param>
        /// <returns>The IEnumerator to be yielded.</returns>
        public virtual IEnumerator Highlighting(bool highlight, float speedMultiplier, bool inmediate = false)
        {
            Debug.Log("Highlighting is not available on this character type!");
            yield return null;
        }

        /// <summary>
        /// Flips character to another direction.
        /// </summary>
        /// <param name="speed">How fast should the character flip.</param>
        /// <param name="immediate">Make it inmediatly.</param>
        /// <returns>The Flipping coroutine process (maybe left or right).</returns>
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (isFacingLeft)
                return FaceRight(speed, immediate);
            else
                return FaceLeft(speed, immediate);
        }
        /// <summary>
        /// Faces the character left.
        /// </summary>
        /// <param name="speed">How fast it should be facing.</param>
        /// <param name="immediate">Make it instantly.</param>
        /// <returns>The flipping coroutine process.</returns>
        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                controller.StopCoroutine(co_flipping);
            facingLeft = true;
            co_flipping = controller.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
            return co_flipping;
        }
        /// <summary>
        /// Faces the character right.
        /// </summary>
        /// <param name="speed">How fast should be the facing.</param>
        /// <param name="immediate">Make it instantly.</param>
        /// <returns>The flipping coroutine process.</returns>
        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                controller.StopCoroutine(co_flipping);
            facingLeft = false;
            co_flipping = controller.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
            return co_flipping;
        }
        /// <summary>
        /// Changes the character facing direction (left or right).
        /// </summary>
        /// <param name="faceLeft">Make it face left.</param>
        /// <param name="speedMultiplier">How fast should be the facing transition.</param>
        /// <param name="immediate">Make it instantly.</param>
        /// <returns>The IEnumerator to be yielded.</returns>
        public virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            Debug.Log("Cannot flip a character of this type!");
            yield return null;
        }

        /// <summary>
        /// Indicates what should do when a character receives an expression.
        /// </summary>
        /// <param name="layer">(Most for spritesheet) the layer.</param>
        /// <param name="expression">The expression associated with the character.</param>
        public virtual void OnExpressionReceive(int layer, string expression)
        {
            return;
        }

        /// <summary>
        /// Character types used in VNs.
        /// </summary>
        public enum CharacterType
        {
            Text,
            Sprite,
        }
    }

}



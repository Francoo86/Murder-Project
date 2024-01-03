using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{
    /// <summary>
    /// Manages character sprite layers, currently we use the first layer (the 0th one). This is for doing expressions on the screen.
    /// </summary>
    public class CharacterImage
    {

        private CharacterController charController => CharacterController.Instance;

        private const float DEFAULT_TRANSITION_SPEED = 3f;
        private float transitionSpeedMultiplier = 1;

        //public int layer { get; private set; } = 0;

        public Image renderer { get; private set; } = null;

        public CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();

        private List<CanvasGroup> oldRenderers = new List<CanvasGroup> ();

        private Coroutine co_transitioningLayer = null;
        private Coroutine co_levelingAlpha = null;
        private Coroutine co_changingColor = null;
        private Coroutine co_flipping = null;
        private bool isFacingLeft = SpriteCharacter.DEFAULT_ORIENTATION_IS_FACING_LEFT;

        public bool isTransitioningLayer => co_transitioningLayer != null;
        public bool isLevelingAlpha => co_levelingAlpha != null;
        public bool isChangingColor => co_changingColor != null;
        public bool isFlipping => co_flipping != null;


        /// <summary>
        /// Creates the object and adds the sprite to an associated layer.
        /// </summary>
        /// <param name="defaultRenderer">The image that should be rendered with the layer.</param>
        /// <param name="layer">The layer num, always 0.</param>
        public CharacterImage(Image defaultRenderer)
        {
            renderer = defaultRenderer;
            //this.layer = layer;
        }

        /// <summary>
        /// Sets the current sprite of the character.
        /// </summary>
        /// <param name="sprite">The sprite resource.</param>
        public void SetSprite(Sprite sprite)
        {
            renderer.sprite = sprite;
        }

        /// <summary>
        /// Sets the sprite but with a transition to make it smooth.
        /// </summary>
        /// <param name="sprite">The sprite resource.</param>
        /// <param name="speed">How fast the transition should be.</param>
        /// <returns>The Coroutine associated to the transition process.</returns>
        public Coroutine TransitionSprite(Sprite sprite, float speed = 1)
        {
            if (sprite == renderer.sprite)
            {
                return null;
            }
            if (isTransitioningLayer)
                charController.StopCoroutine(co_transitioningLayer);

            co_transitioningLayer = charController.StartCoroutine(TransitioningSprite(sprite, speed));

            return co_transitioningLayer;
        }

        
        /// <summary>
        /// Internal method that handles the logic of the sprite transition.
        /// Creates a new renderer and sets the passed sprite to it.
        /// </summary>
        /// <param name="sprite">The sprite resource to be loaded.</param>
        /// <param name="speedMultiplier">How fast the transition should be.</param>
        /// <returns>The IEnumerator to be yielded.</returns>
        private IEnumerator TransitioningSprite(Sprite sprite, float speedMultiplier)
        {
            transitionSpeedMultiplier = speedMultiplier;

            Image newRender = CreateRenderer(renderer.transform.parent);
            newRender.sprite = sprite;

            yield return TryStartLevelingAlphas();

            co_transitioningLayer = null;

        }

        /// <summary>
        /// Creates a renderer for an associated parent, this is used for Unity for associating the sprites to the character GameObject.
        /// </summary>
        /// <param name="parent">The parent transform.</param>
        /// <returns>The image to be rendered in that GameObject.</returns>
        private Image CreateRenderer(Transform parent)
        {
            Image newRenderer = Object.Instantiate(renderer, parent);
            oldRenderers.Add(rendererCG);

            newRenderer.name = renderer.name;
            renderer = newRenderer;
            renderer.gameObject.SetActive(true);
            rendererCG.alpha = 0;

            return newRenderer;
        }

        
        /// <summary>
        /// Levels the alpha of the rendered CanvasGroup.
        /// </summary>
        /// <returns>The Coroutine associated to alpha leveling.</returns>
        private Coroutine TryStartLevelingAlphas()
        {
            if (isLevelingAlpha)
                //return co_levelingAlpha;
                CharacterController.Instance.StopCoroutine(co_levelingAlpha);

            co_levelingAlpha = charController.StartCoroutine(RunAlphaLeveling());

            return co_levelingAlpha;
        }

        /// <summary>
        /// Logic of the renderer CanvasGroup leveling.
        /// </summary>
        /// <returns>The IEnumerator to be yielded in the animation.</returns>
        private IEnumerator RunAlphaLeveling()
        {
            while (rendererCG.alpha < 1 || oldRenderers.Any(oldCG => oldCG.alpha > 0))
            {
                float speed = DEFAULT_TRANSITION_SPEED * transitionSpeedMultiplier * Time.deltaTime;

                rendererCG.alpha = Mathf.MoveTowards(rendererCG.alpha, 1, speed);

                for (int i = oldRenderers.Count - 1; i >= 0; i--)
                {
                    CanvasGroup oldCG = oldRenderers[i];
                    oldCG.alpha = Mathf.MoveTowards(oldCG.alpha, 0, speed);

                    if (oldCG.alpha <= 0)
                    {
                        oldRenderers.RemoveAt(i);
                        Object.Destroy(oldCG.gameObject);
                    }

                }

                yield return null;
            }

            co_levelingAlpha = null;
        }

        
        /// <summary>
        /// Sets the color to the renderer and images of the character.
        /// </summary>
        /// <param name="color">The new color.</param>
        public void SetColor(Color color)
        {
            renderer.color = color;
            foreach (CanvasGroup oldCG in oldRenderers)
            {
                oldCG.GetComponent<Image>().color = color;
            }
        }

        /// <summary>
        /// Makes a transition for changing the color of the renderer and the images associated with it.
        /// </summary>
        /// <param name="color">The color to be set.</param>
        /// <param name="speed">How fast should the transition should be.</param>
        /// <returns>The Coroutine associated to the changing color process.</returns>
        public Coroutine TransitionColor(Color color, float speed)
        {
            if (isChangingColor)
                charController.StopCoroutine(co_changingColor);

            co_changingColor = charController.StartCoroutine(ChangingColor(color, speed));

            return co_changingColor;
        }
        /// <summary>
        /// Stops the ChangingColor coroutine and keeps the color in the same state it was before.
        /// </summary>
        public void StopChangingColor()
        {
            if (!isChangingColor)
                return;
            
            charController.StopCoroutine(co_changingColor);
            co_changingColor = null;
        }

        /// <summary>
        /// The logic for changing the color with a transition.
        /// </summary>
        /// <param name="color">The color to be set.</param>
        /// <param name="speedMultiplier">How fast the transition should be.</param>
        /// <returns>The IEnumerator to be yielded for the coroutine.</returns>
        private IEnumerator ChangingColor(Color color, float speedMultiplier)
        {
            Color oldColor = renderer.color;
            List<Image> oldImages = new List<Image>();

            foreach (var oldCG in oldRenderers)
            {
                oldImages.Add(oldCG.GetComponent<Image>());
            }

            float colorPercent = 0;
            while (colorPercent < 1)
            {
                colorPercent += DEFAULT_TRANSITION_SPEED * speedMultiplier * Time.deltaTime;

                renderer.color = Color.Lerp(oldColor, color, colorPercent);

                for(int i = oldImages.Count - 1; i >= 0; i--)
                {
                    Image image = oldImages[i];
                    if (image != null)
                        image.color = renderer.color;
                    else
                        oldImages.RemoveAt(i);
                }

                yield return null;
            }

            co_changingColor = null;
        }

        /// <summary>
        /// Faces the character to the left direction.
        /// </summary>
        /// <param name="speed">How fast the transition should be.</param>
        /// <param name="immediate">Skips the speed and do its instantly.</param>
        /// <returns>The Coroutine process associated to flipping.</returns>
        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                charController.StopCoroutine(co_flipping);

            isFacingLeft = true;
            co_flipping = charController.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));

            return co_flipping;
        }

        /// <summary>
        /// Faces the character to the right direction.
        /// </summary>
        /// <param name="speed">How fast the transition should be.</param>
        /// <param name="immediate">Skips the speed and do its instantly.</param>
        /// <returns>The Coroutine process associated to flipping.</returns>
        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                charController.StopCoroutine(co_flipping);

            isFacingLeft = false;
            co_flipping = charController.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));

            return co_flipping;
        }

        /// <summary>
        /// Flips the character to left or right.
        /// </summary>
        /// <param name="speed">How fast should the flip should be.</param>
        /// <param name="immediate">Skips the speed and do it instantly.</param>
        /// <returns>The Coroutine associated to the Flip process.</returns>
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (isFacingLeft)
                return FaceRight(speed, immediate);
            else
                return FaceLeft(speed, immediate);
        }

        /// <summary>
        /// Internal method that handles the logic of flipping characters direction.
        /// </summary>
        /// <param name="faceLeft">Face the character to the left side of screen.</param>
        /// <param name="speedMultiplier">How fast the transition should be.</param>
        /// <param name="immediate">Skip the transition and do it instantly.</param>
        /// <returns>The IEnumerator to be yielded.</returns>
        private IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            float xScale = faceLeft ? 1 : -1;
            Vector3 newScale = new Vector3(xScale, 1, 1);

            if (immediate)
            {
                Image newRenderer = CreateRenderer(renderer.transform.parent);

                newRenderer.transform.localScale = newScale;

                transitionSpeedMultiplier = speedMultiplier;
                TryStartLevelingAlphas();

                while(isLevelingAlpha)
                    yield return null;

            }
            else 
            {
                renderer.transform.localScale = newScale;
            }

            co_flipping = null;
        }
    }
}
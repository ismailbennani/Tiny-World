using TMPro;
using UI.Theme;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Button button;
        public TextMeshProUGUI text;
        public Image image;
        
        public Image cursor;

        private bool _selected;
        private Sprite _image;

        void Start()
        {
            SetSelected(_selected);
            SetImage(_image);
        }

        public void SetSelected(bool selected)
        {
            if (cursor)
            {
                cursor.gameObject.SetActive(selected);
            }
            
            _selected = selected;
        }

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(sprite);

            _image = sprite;
        }

        public void SetTheme(UITheme theme)
        {
            if (button)
            {
                button.transition = Selectable.Transition.SpriteSwap;

                button.image.sprite = theme.button;
                button.spriteState = new SpriteState
                {
                    highlightedSprite = theme.buttonHighlighted, selectedSprite = theme.buttonHighlighted, disabledSprite = theme.buttonDisabled,
                    pressedSprite = theme.buttonPressed
                };
            }

            if (text)
            {
                text.font = theme.text.font;
                text.color = theme.text.color;
            }

            if (cursor)
            {
                cursor.sprite = theme.cursor;
            }
        }

        public void SaveTheme(UITheme theme)
        {
            if (button)
            {
                theme.button = button.image.sprite;
                theme.buttonHighlighted = button.spriteState.highlightedSprite;
                theme.buttonPressed = button.spriteState.pressedSprite;
                theme.buttonDisabled = button.spriteState.disabledSprite;
            }

            if (text)
            {
                theme.text.font = text.font;
                theme.text.color = text.color;
            }

            if (cursor)
            {
                theme.cursor = cursor.sprite;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetSelected(true);
            eventData.Use();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetSelected(false);
            eventData.Use();
        }
    }
}

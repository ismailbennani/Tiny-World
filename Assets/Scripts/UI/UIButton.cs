using TMPro;
using UI.Theme;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public abstract class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Button button;
        public TextMeshProUGUI text;
        public Image image;

        public Image cursor;

        private bool _selected;
        private Sprite _image;
        private string _text;

        void Start()
        {
            SetSelected(_selected);
            SetImage(_image);
            SetText(_text);
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

        public void SetText(string str)
        {
            text.text = str;

            _text = str;
        }

        public void SetTheme(UITheme theme)
        {
            UIButtonTheme buttonTheme = GetTheme(theme);

            if (button)
            {
                button.transition = Selectable.Transition.SpriteSwap;

                button.image.sprite = buttonTheme.sprite;
                button.spriteState = new SpriteState
                {
                    highlightedSprite = buttonTheme.highlightedSprite, selectedSprite = buttonTheme.highlightedSprite,
                    disabledSprite = buttonTheme.disabledSprite,
                    pressedSprite = buttonTheme.pressedSprite
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

            OnSetTheme(theme);
        }

        public void SaveTheme(UITheme theme)
        {
            UIButtonTheme buttonTheme = GetTheme(theme);

            if (button)
            {
                buttonTheme.sprite = button.image.sprite;
                buttonTheme.highlightedSprite = button.spriteState.highlightedSprite;
                buttonTheme.pressedSprite = button.spriteState.pressedSprite;
                buttonTheme.disabledSprite = button.spriteState.disabledSprite;
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

            OnSaveTheme(theme);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetSelected(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetSelected(false);
        }

        protected abstract UIButtonTheme GetTheme(UITheme theme);

        protected virtual void OnSetTheme(UITheme theme)
        {
        }

        protected virtual void OnSaveTheme(UITheme theme)
        {
        }
    }
}

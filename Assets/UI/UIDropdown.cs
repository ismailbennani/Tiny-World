using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIDropdown : MonoBehaviour
    {
        public UIDocument root;
        public VisualTreeAsset dropdownButtonTemplate;

        private List<VisualElement> _dropdownButtons = new();

        /// <remarks>
        /// Low-level API used by <see cref="UIMenusManager"/>. <br/>
        /// YOU SHOULD USE <see cref="UIMenusManager.OpenDropdown"/> INSTEAD 
        /// </remarks>
        public void Show(IReadOnlyList<UIDropdownChoice> choices, Vector2 position)
        {
            foreach (VisualElement button in _dropdownButtons)
            {
                button.RemoveFromHierarchy();
            }

            _dropdownButtons.Clear();

            VisualElement container = root.rootVisualElement.Q("ButtonsContainer");
            for (int i = 0; i < choices.Count; i++)
            {
                UIDropdownChoice choice = choices[i];

                TemplateContainer newButton = dropdownButtonTemplate.CloneTree();
                container.Add(newButton);
                _dropdownButtons.Add(newButton);

                Label label = newButton.Q<Label>("DropdownButtonLabel");
                label.text = choice.Label;

                Button button = newButton.Q<Button>("DropdownButton");
                button.clicked += choice.Callback;

                // Close dropdown on cancel
                button.RegisterCallback<NavigationCancelEvent>(
                    evt =>
                    {
                        if (!UIMenusManager.Instance.CloseDropdown())
                        {
                            evt.PreventDefault();
                            evt.StopImmediatePropagation();
                        }
                    }
                );

                int index = i;
                button.RegisterCallback<NavigationMoveEvent>(
                    evt =>
                    {
                        evt.PreventDefault();
                        evt.StopImmediatePropagation();

                        if (evt.direction is NavigationMoveEvent.Direction.Left or NavigationMoveEvent.Direction.Right)
                        {
                            UIMenusManager.Instance.CloseDropdown();
                        }
                        else if (evt.direction is NavigationMoveEvent.Direction.Up)
                        {
                            if (index > 0)
                            {
                                _dropdownButtons[index - 1].Q<Button>().Focus();
                            }
                        }
                        else if (evt.direction is NavigationMoveEvent.Direction.Down)
                        {
                            if (index < _dropdownButtons.Count - 1)
                            {
                                _dropdownButtons[index + 1].Q<Button>().Focus();
                            }
                        }
                    }
                );
            }

            root.rootVisualElement.visible = true;
            root.rootVisualElement.style.position = new StyleEnum<Position>(Position.Absolute);
            root.rootVisualElement.style.left = position.x;
            root.rootVisualElement.style.bottom = position.y - root.rootVisualElement.layout.height / 2;
        }

        public void Focus()
        {
            if (_dropdownButtons.Count > 0)
            {
                _dropdownButtons[0].Q<Button>("DropdownButton").Focus();
            }
        }

        /// <remarks>
        /// Low-level API used by <see cref="UIMenusManager"/>. <br/>
        /// YOU SHOULD USE <see cref="UIMenusManager.CloseDropdown"/> INSTEAD 
        /// </remarks>
        public void Hide()
        {
            root.rootVisualElement.visible = false;
        }
    }

    public class UIDropdownChoice
    {
        public readonly string Label;
        public readonly Action Callback;

        public UIDropdownChoice(string label, Action callback)
        {
            Label = label;
            Callback = callback;
        }
    }
}

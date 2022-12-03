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
            VisualElement container = root.rootVisualElement.Q("ButtonsContainer");
            for (int i = _dropdownButtons.Count; i < choices.Count; i++)
            {
                TemplateContainer newButton = dropdownButtonTemplate.CloneTree();
                container.Add(newButton);

                _dropdownButtons.Add(newButton);
            }

            for (int i = choices.Count; i < _dropdownButtons.Count; i++)
            {
                _dropdownButtons[i].visible = false;
            }

            for (int i = 0; i < choices.Count; i++)
            {
                UIDropdownChoice choice = choices[i];

                Label label = _dropdownButtons[i].Q<Label>("DropdownButtonLabel");
                label.text = choice.Label;

                Button button = _dropdownButtons[i].Q<Button>("DropdownButton");
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

                button.RegisterCallback<NavigationMoveEvent>(
                    evt =>
                    {
                        if (evt.direction is NavigationMoveEvent.Direction.Left or NavigationMoveEvent.Direction.Right)
                        {
                            if (!UIMenusManager.Instance.CloseDropdown())
                            {
                                evt.PreventDefault();
                                evt.StopImmediatePropagation();
                            }
                        }
                    }
                );

                // Prevent UP on first element and DOWN on last element
                if (i == 0)
                {
                    button.RegisterCallback<NavigationMoveEvent>(
                        evt =>
                        {
                            if (evt.direction == NavigationMoveEvent.Direction.Up)
                            {
                                evt.PreventDefault();
                                evt.StopImmediatePropagation();
                            }
                        }
                    );
                }
                else if (i == choices.Count - 1)
                {
                    button.RegisterCallback<NavigationMoveEvent>(
                        evt =>
                        {
                            if (evt.direction == NavigationMoveEvent.Direction.Down)
                            {
                                evt.PreventDefault();
                                evt.StopImmediatePropagation();
                            }
                        }
                    );
                }
            }

            root.rootVisualElement.visible = true;
            root.rootVisualElement.style.position = new StyleEnum<Position>(Position.Absolute);
            root.rootVisualElement.style.left = position.x;
            root.rootVisualElement.style.bottom = position.y;
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

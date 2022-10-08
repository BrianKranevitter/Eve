using Enderlook.Unity.Toolset.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Level.Triggers
{
    [CustomEditor(typeof(PlayerTrigger))]
    internal sealed class PlayerTriggerEditor : UnityEditor.Editor
    {
        private static readonly List<Type> types = typeof(PlayerTrigger).Assembly.GetTypes().Where(e => !e.IsAbstract && typeof(PlayerTriggerAction).IsAssignableFrom(e)).ToList();

        public override VisualElement CreateInspectorGUI()
        {
            FieldInfo fieldInfo = typeof(PlayerTrigger).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo.GetValue(target) is null)
                fieldInfo.SetValue(target, new PlayerTriggerAction[0]);

            SerializedProperty array = serializedObject.FindProperty("actions");

            VisualElement root = new VisualElement();
            {
                root.style.flexGrow = 1;

                ObjectField objectField = new ObjectField("Script");
                {
                    objectField.SetEnabled(false);
                    objectField.value = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
                }
                root.Add(objectField);

                PropertyField propertyField = new PropertyField(serializedObject.FindProperty("triggerOnce"));
                root.Add(propertyField);

                Label lengthLabel;
                Button addButton;
                Button closeButton;
                VisualElement div = new VisualElement();
                {
                    div.style.flexDirection = FlexDirection.Row;

                    Label label = new Label("Actions:");
                    {
                        label.tooltip = "Actions to execute when player get in or out of the trigger.";
                    }
                    div.Add(label);
                    lengthLabel = new Label($"Length {array?.arraySize ?? 0}");
                    div.Add(lengthLabel);

                    addButton = new Button();
                    {
                        addButton.text = "Add";
                        addButton.tooltip = "Add a new action.";
                    }
                    div.Add(addButton);

                    closeButton = new Button();
                    {
                        closeButton.text = "Stop";
                        closeButton.tooltip = "Stop adding a new action.";
                        closeButton.style.display = DisplayStyle.None;
                    }
                    div.Add(closeButton);
                }
                root.Add(div);

                Box display = null;
                Box actionsBox = null;
                Box addBox = new Box();
                {
                    addBox.style.minHeight = 100;
                    addBox.style.flexGrow = 1;
                    addBox.style.display = DisplayStyle.None;
                    ListView addList = new ListView(types, 20, () => new Label(), (e, i) => ((Label)e).text = types[i].ToString());
                    {
                        addList.selectionType = SelectionType.Single;
                        addList.style.flexGrow = 1;
                        addList.onItemsChosen += e =>
                        {
                            array.InsertArrayElementAtIndex(array.arraySize);
                            array.serializedObject.ApplyModifiedProperties();
                            array
                                .GetArrayElementAtIndex(array.arraySize - 1)
                                .GetTargetObjectAccessors()
                                .Set(Activator.CreateInstance((Type)e.First()));
                            array.serializedObject.ApplyModifiedProperties();
                            lengthLabel.text = $"Length {array.arraySize}";
                            Close();
                        };
                    }
                    addBox.Add(addList);
                }
                root.Add(addBox);

                VisualElement select = null;
                ListView actionsList = null;
                actionsBox = new Box();
                {
                    actionsBox.style.flexGrow = 1;
                    actionsBox.style.minHeight = 100;

                    actionsList = new ListView();
                    {
                        actionsList.itemHeight = 20;
                        actionsList.makeItem = () =>
                        {
                            VisualElement root_ = new VisualElement();
                            {
                                root_.name = "Slot";
                                root_.style.flexDirection = FlexDirection.Row;

                                Button removeButton = new Button();
                                {
                                    removeButton.name = "Remove";
                                    removeButton.text = "X";
                                    removeButton.tooltip = "Remove this action.";
                                }
                                root_.Add(removeButton);

                                Label label_ = new Label("_");
                                {
                                    label_.name = "Label";
                                }
                                root_.Add(label_);
                            }
                            return root_;
                        };
                        actionsList.bindItem = (e, i) =>
                        {
                            if (i >= array.arraySize)
                            {
                                e.Q<VisualElement>("Slot").style.display = DisplayStyle.None;
                                return;
                            }

                            e.Q<VisualElement>("Slot").style.display = DisplayStyle.Flex;

                            SerializedProperty serializedProperty = array.GetArrayElementAtIndex(i);

                            e.Q<Label>("Label").text = serializedProperty.managedReferenceFullTypename.Remove(0, "Assembly-CSharp Game.Level.Triggers.".Length);

                            Button button = e.Q<Button>("Remove");
                            if (button.userData is Action callback)
                                button.clicked -= callback;
                            callback = () =>
                            {
                                array.DeleteArrayElementAtIndex(i);
                                array.serializedObject.ApplyModifiedProperties();
                                lengthLabel.text = $"Length {array.arraySize}";
                                actionsList.Refresh();
                                select.style.display = DisplayStyle.None;
                            };
                            button.userData = callback;
                            button.clicked += callback;
                        };
                        actionsList.selectionType = SelectionType.Single;
                        actionsList.style.flexGrow = 1;
                        actionsList.reorderable = true;
                        actionsList.BindProperty(array);
                    }
                    actionsBox.Add(actionsList);

                    addButton.clicked += () =>
                    {
                        addBox.style.display = DisplayStyle.Flex;
                        actionsBox.style.display = DisplayStyle.None;
                        display.style.display = DisplayStyle.None;
                        addButton.style.display = DisplayStyle.None;
                        closeButton.style.display = DisplayStyle.Flex;
                    };
                    closeButton.clicked += Close;
                }
                root.Add(actionsBox);

                select = new VisualElement();
                {
                    select.style.display = DisplayStyle.None;
                    select.style.flexGrow = 1;

                    Label label_ = new Label("Selected");
                    select.Add(label_);

                    display = new Box();
                    {
                        display.style.flexGrow = 1;

                        PropertyField property = new PropertyField();
                        {
                            property.style.flexGrow = 1;
                            actionsList.onItemsChosen += _ =>
                            {
                                select.style.display = DisplayStyle.Flex;
                                property.Unbind();
                                SerializedProperty serializedProperty = array.GetArrayElementAtIndex(actionsList.selectedIndex);
                                property.BindProperty(serializedProperty);
                                property.label = serializedProperty.managedReferenceFieldTypename.Remove(0, "Assembly-CSharp ".Length);
                            };
                        }
                        display.Add(property);
                    }
                    select.Add(display);
                }
                root.Add(select);

                void Close()
                {
                    addBox.style.display = DisplayStyle.None;
                    actionsBox.style.display = DisplayStyle.Flex;
                    display.style.display = DisplayStyle.Flex;
                    addButton.style.display = DisplayStyle.Flex;
                    closeButton.style.display = DisplayStyle.None;
                }
            }
            return root;
        }
    }
}
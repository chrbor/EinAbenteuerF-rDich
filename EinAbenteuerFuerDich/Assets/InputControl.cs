// GENERATED AUTOMATICALLY FROM 'Assets/InputControl.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputControl : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControl()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControl"",
    ""maps"": [
        {
            ""name"": ""TouchMap"",
            ""id"": ""b9be49de-ee98-4fe5-8f69-9dbf11a4fa6e"",
            ""actions"": [
                {
                    ""name"": ""primary"",
                    ""type"": ""Button"",
                    ""id"": ""37f15b41-86d7-48a6-ab09-7c59622398a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""touch_0"",
                    ""type"": ""Button"",
                    ""id"": ""97f90514-b687-4924-a06c-e443864a74bb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0979a6dc-e0f0-4b98-ae41-b0a47d23884f"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""TouchScheme"",
                    ""action"": ""primary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""99503271-7a30-48c0-93aa-db408401d333"",
                    ""path"": ""<Touchscreen>/touch0/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""TouchScheme"",
                    ""action"": ""touch_0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""TouchScheme"",
            ""bindingGroup"": ""TouchScheme"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // TouchMap
        m_TouchMap = asset.FindActionMap("TouchMap", throwIfNotFound: true);
        m_TouchMap_primary = m_TouchMap.FindAction("primary", throwIfNotFound: true);
        m_TouchMap_touch_0 = m_TouchMap.FindAction("touch_0", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // TouchMap
    private readonly InputActionMap m_TouchMap;
    private ITouchMapActions m_TouchMapActionsCallbackInterface;
    private readonly InputAction m_TouchMap_primary;
    private readonly InputAction m_TouchMap_touch_0;
    public struct TouchMapActions
    {
        private @InputControl m_Wrapper;
        public TouchMapActions(@InputControl wrapper) { m_Wrapper = wrapper; }
        public InputAction @primary => m_Wrapper.m_TouchMap_primary;
        public InputAction @touch_0 => m_Wrapper.m_TouchMap_touch_0;
        public InputActionMap Get() { return m_Wrapper.m_TouchMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchMapActions set) { return set.Get(); }
        public void SetCallbacks(ITouchMapActions instance)
        {
            if (m_Wrapper.m_TouchMapActionsCallbackInterface != null)
            {
                @primary.started -= m_Wrapper.m_TouchMapActionsCallbackInterface.OnPrimary;
                @primary.performed -= m_Wrapper.m_TouchMapActionsCallbackInterface.OnPrimary;
                @primary.canceled -= m_Wrapper.m_TouchMapActionsCallbackInterface.OnPrimary;
                @touch_0.started -= m_Wrapper.m_TouchMapActionsCallbackInterface.OnTouch_0;
                @touch_0.performed -= m_Wrapper.m_TouchMapActionsCallbackInterface.OnTouch_0;
                @touch_0.canceled -= m_Wrapper.m_TouchMapActionsCallbackInterface.OnTouch_0;
            }
            m_Wrapper.m_TouchMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @primary.started += instance.OnPrimary;
                @primary.performed += instance.OnPrimary;
                @primary.canceled += instance.OnPrimary;
                @touch_0.started += instance.OnTouch_0;
                @touch_0.performed += instance.OnTouch_0;
                @touch_0.canceled += instance.OnTouch_0;
            }
        }
    }
    public TouchMapActions @TouchMap => new TouchMapActions(this);
    private int m_TouchSchemeSchemeIndex = -1;
    public InputControlScheme TouchSchemeScheme
    {
        get
        {
            if (m_TouchSchemeSchemeIndex == -1) m_TouchSchemeSchemeIndex = asset.FindControlSchemeIndex("TouchScheme");
            return asset.controlSchemes[m_TouchSchemeSchemeIndex];
        }
    }
    public interface ITouchMapActions
    {
        void OnPrimary(InputAction.CallbackContext context);
        void OnTouch_0(InputAction.CallbackContext context);
    }
}

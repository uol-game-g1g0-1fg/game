// GENERATED AUTOMATICALLY FROM 'Assets/InputActions/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Submarine"",
            ""id"": ""7bc67602-2b18-4936-bfae-f7301265a61a"",
            ""actions"": [
                {
                    ""name"": ""Boost"",
                    ""type"": ""Button"",
                    ""id"": ""f56f3b42-c4c2-403c-b512-88761eaeca3b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""1cb7c3c8-7f50-45d3-919e-eaa5275caa88"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ballast"",
                    ""type"": ""Button"",
                    ""id"": ""107c5e68-134a-4bb1-84fb-18d22af5db19"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Harpoon"",
                    ""type"": ""Button"",
                    ""id"": ""9c850ace-a8ac-428c-b38d-8c9e53b8332b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Arm"",
                    ""type"": ""Button"",
                    ""id"": ""7c41af38-95f3-4318-93cf-47212b6e5ce5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bda2a62a-df61-470f-bfc2-c5885723e1e9"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""6c18f57b-a5bd-40b7-9252-cdd22c78496c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9b0c45d9-e1b8-4aee-ab87-ef98842ecca8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c618ce97-c310-409d-b708-f9a499d63b4a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a01c5d6d-1eba-4816-b4ff-447d23e1dd94"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2d502fe4-9abc-475e-bab8-b52259f80d61"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ef1a7140-2242-4832-bf39-7c79f4f429c6"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ballast"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""fe90c225-1a3f-4ee6-9097-24a51a761ff4"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ballast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""3372ee76-d669-4200-8e86-a5a3e2c53521"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ballast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c2f6b704-d0d2-45a4-858f-104f002b521c"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Harpoon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7c44e82-9750-4bce-b4eb-111e74eb8ab9"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Arm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Submarine
        m_Submarine = asset.FindActionMap("Submarine", throwIfNotFound: true);
        m_Submarine_Boost = m_Submarine.FindAction("Boost", throwIfNotFound: true);
        m_Submarine_Movement = m_Submarine.FindAction("Movement", throwIfNotFound: true);
        m_Submarine_Ballast = m_Submarine.FindAction("Ballast", throwIfNotFound: true);
        m_Submarine_Harpoon = m_Submarine.FindAction("Harpoon", throwIfNotFound: true);
        m_Submarine_Arm = m_Submarine.FindAction("Arm", throwIfNotFound: true);
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

    // Submarine
    private readonly InputActionMap m_Submarine;
    private ISubmarineActions m_SubmarineActionsCallbackInterface;
    private readonly InputAction m_Submarine_Boost;
    private readonly InputAction m_Submarine_Movement;
    private readonly InputAction m_Submarine_Ballast;
    private readonly InputAction m_Submarine_Harpoon;
    private readonly InputAction m_Submarine_Arm;
    public struct SubmarineActions
    {
        private @PlayerControls m_Wrapper;
        public SubmarineActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Boost => m_Wrapper.m_Submarine_Boost;
        public InputAction @Movement => m_Wrapper.m_Submarine_Movement;
        public InputAction @Ballast => m_Wrapper.m_Submarine_Ballast;
        public InputAction @Harpoon => m_Wrapper.m_Submarine_Harpoon;
        public InputAction @Arm => m_Wrapper.m_Submarine_Arm;
        public InputActionMap Get() { return m_Wrapper.m_Submarine; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SubmarineActions set) { return set.Get(); }
        public void SetCallbacks(ISubmarineActions instance)
        {
            if (m_Wrapper.m_SubmarineActionsCallbackInterface != null)
            {
                @Boost.started -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnBoost;
                @Boost.performed -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnBoost;
                @Boost.canceled -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnBoost;
                @Movement.started -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnMovement;
                @Ballast.started -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnBallast;
                @Ballast.performed -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnBallast;
                @Ballast.canceled -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnBallast;
                @Harpoon.started -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnHarpoon;
                @Harpoon.performed -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnHarpoon;
                @Harpoon.canceled -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnHarpoon;
                @Arm.started -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnArm;
                @Arm.performed -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnArm;
                @Arm.canceled -= m_Wrapper.m_SubmarineActionsCallbackInterface.OnArm;
            }
            m_Wrapper.m_SubmarineActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Boost.started += instance.OnBoost;
                @Boost.performed += instance.OnBoost;
                @Boost.canceled += instance.OnBoost;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Ballast.started += instance.OnBallast;
                @Ballast.performed += instance.OnBallast;
                @Ballast.canceled += instance.OnBallast;
                @Harpoon.started += instance.OnHarpoon;
                @Harpoon.performed += instance.OnHarpoon;
                @Harpoon.canceled += instance.OnHarpoon;
                @Arm.started += instance.OnArm;
                @Arm.performed += instance.OnArm;
                @Arm.canceled += instance.OnArm;
            }
        }
    }
    public SubmarineActions @Submarine => new SubmarineActions(this);
    public interface ISubmarineActions
    {
        void OnBoost(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnBallast(InputAction.CallbackContext context);
        void OnHarpoon(InputAction.CallbackContext context);
        void OnArm(InputAction.CallbackContext context);
    }
}

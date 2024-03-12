//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/Runtime/Input/ResatInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Input
{
    public partial class @ResatInput: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @ResatInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""ResatInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a4072e11-acab-48d4-9ced-471f410a8912"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""2ec7ea1d-cf49-4b67-a1ab-ce9c9f9629ed"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""03b6470b-c717-4d16-b11f-a4bb13f7b073"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ec6be871-e6ce-44a8-a368-47d74facc5da"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e2d07e27-ff1f-4deb-8b46-a7b96a100ffa"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cf520cf5-b673-4c25-81e8-298d3def7b62"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""57beb27b-cb83-45b7-933e-8be43b75a431"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""3168970d-bf17-4057-ae70-fd414d533cf7"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""271ba873-3766-4332-b0ba-bb2a1e2180ae"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""199e4004-503b-4623-b962-1c8d199d8412"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5180db8a-6bdd-4e9c-ba83-f3e23c11c9ad"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Camera"",
            ""id"": ""cb8382ca-629e-4fa4-9957-962047a99d44"",
            ""actions"": [
                {
                    ""name"": ""TakePicture"",
                    ""type"": ""Button"",
                    ""id"": ""b753f2a5-7c59-4d8f-b33e-0af7452cd303"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToggleCamera"",
                    ""type"": ""Button"",
                    ""id"": ""09815662-18c4-4571-85d2-6137f7a1701a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""a4905599-a26c-4136-a563-9d030d048ff7"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ResetZoom"",
                    ""type"": ""Button"",
                    ""id"": ""f9a0f076-6b7a-4097-874c-b8c6705bca53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2caf402c-e164-4de2-87f8-22e921e9cdab"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakePicture"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ee22190-52a3-4a6e-936c-f8151129c37c"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30e5188e-3a43-418d-9585-36160576c299"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""08fe268e-043b-4667-8aa8-62ed3cb3f097"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Debugging"",
            ""id"": ""94237826-a629-49ef-ab10-3347cf5b10d7"",
            ""actions"": [
                {
                    ""name"": ""DefaultCamView"",
                    ""type"": ""Button"",
                    ""id"": ""1b571058-70dd-4a39-a48b-d4de984525fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OKHSLView"",
                    ""type"": ""Button"",
                    ""id"": ""465f84b2-0312-4a4f-aa1c-cde5a1b69d1c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DesaturatedView"",
                    ""type"": ""Button"",
                    ""id"": ""beeec1dc-08a0-4de5-a881-da3c9f0f564d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ResaturatedView"",
                    ""type"": ""Button"",
                    ""id"": ""53bfc7c6-8083-481b-aafe-e811d8894107"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ResetGlobalArray"",
                    ""type"": ""Button"",
                    ""id"": ""eb13b52f-dfe5-42a2-8c50-ab42c2fcd9c8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b7a88e8c-21f0-4569-99b2-6e4263c68f4a"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OKHSLView"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fba8e1ed-2108-4d48-8c9e-bdc5d7974e79"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DesaturatedView"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1159e3d-0bbc-48ac-8f64-5235c6f0bb83"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DefaultCamView"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b322bcb-61a1-4e9e-bf5d-80120f97ef94"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResaturatedView"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6e77b81e-60f6-4b37-9859-23800d126ed4"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetGlobalArray"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Dialogue"",
            ""id"": ""f36f3a6b-b574-4bdb-8e12-5b59e8dcb93c"",
            ""actions"": [
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""1b15559b-93f9-432c-b843-2be995e10af4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Continue"",
                    ""type"": ""Button"",
                    ""id"": ""3d099494-7e9f-481e-94d3-a71693fec67e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5198aaff-c6d8-4aca-81e7-0bbdafabaf35"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7eccf0d1-3476-4b0e-a29c-0aefae5da5b3"",
                    ""path"": ""<Mouse>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Continue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
            m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
            // Camera
            m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
            m_Camera_TakePicture = m_Camera.FindAction("TakePicture", throwIfNotFound: true);
            m_Camera_ToggleCamera = m_Camera.FindAction("ToggleCamera", throwIfNotFound: true);
            m_Camera_Zoom = m_Camera.FindAction("Zoom", throwIfNotFound: true);
            m_Camera_ResetZoom = m_Camera.FindAction("ResetZoom", throwIfNotFound: true);
            // Debugging
            m_Debugging = asset.FindActionMap("Debugging", throwIfNotFound: true);
            m_Debugging_DefaultCamView = m_Debugging.FindAction("DefaultCamView", throwIfNotFound: true);
            m_Debugging_OKHSLView = m_Debugging.FindAction("OKHSLView", throwIfNotFound: true);
            m_Debugging_DesaturatedView = m_Debugging.FindAction("DesaturatedView", throwIfNotFound: true);
            m_Debugging_ResaturatedView = m_Debugging.FindAction("ResaturatedView", throwIfNotFound: true);
            m_Debugging_ResetGlobalArray = m_Debugging.FindAction("ResetGlobalArray", throwIfNotFound: true);
            // Dialogue
            m_Dialogue = asset.FindActionMap("Dialogue", throwIfNotFound: true);
            m_Dialogue_Interact = m_Dialogue.FindAction("Interact", throwIfNotFound: true);
            m_Dialogue_Continue = m_Dialogue.FindAction("Continue", throwIfNotFound: true);
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

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Player
        private readonly InputActionMap m_Player;
        private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_Look;
        private readonly InputAction m_Player_Jump;
        public struct PlayerActions
        {
            private @ResatInput m_Wrapper;
            public PlayerActions(@ResatInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @Look => m_Wrapper.m_Player_Look;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void AddCallbacks(IPlayerActions instance)
            {
                if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }

            private void UnregisterCallbacks(IPlayerActions instance)
            {
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @Look.started -= instance.OnLook;
                @Look.performed -= instance.OnLook;
                @Look.canceled -= instance.OnLook;
                @Jump.started -= instance.OnJump;
                @Jump.performed -= instance.OnJump;
                @Jump.canceled -= instance.OnJump;
            }

            public void RemoveCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IPlayerActions instance)
            {
                foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // Camera
        private readonly InputActionMap m_Camera;
        private List<ICameraActions> m_CameraActionsCallbackInterfaces = new List<ICameraActions>();
        private readonly InputAction m_Camera_TakePicture;
        private readonly InputAction m_Camera_ToggleCamera;
        private readonly InputAction m_Camera_Zoom;
        private readonly InputAction m_Camera_ResetZoom;
        public struct CameraActions
        {
            private @ResatInput m_Wrapper;
            public CameraActions(@ResatInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @TakePicture => m_Wrapper.m_Camera_TakePicture;
            public InputAction @ToggleCamera => m_Wrapper.m_Camera_ToggleCamera;
            public InputAction @Zoom => m_Wrapper.m_Camera_Zoom;
            public InputAction @ResetZoom => m_Wrapper.m_Camera_ResetZoom;
            public InputActionMap Get() { return m_Wrapper.m_Camera; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
            public void AddCallbacks(ICameraActions instance)
            {
                if (instance == null || m_Wrapper.m_CameraActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_CameraActionsCallbackInterfaces.Add(instance);
                @TakePicture.started += instance.OnTakePicture;
                @TakePicture.performed += instance.OnTakePicture;
                @TakePicture.canceled += instance.OnTakePicture;
                @ToggleCamera.started += instance.OnToggleCamera;
                @ToggleCamera.performed += instance.OnToggleCamera;
                @ToggleCamera.canceled += instance.OnToggleCamera;
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
                @ResetZoom.started += instance.OnResetZoom;
                @ResetZoom.performed += instance.OnResetZoom;
                @ResetZoom.canceled += instance.OnResetZoom;
            }

            private void UnregisterCallbacks(ICameraActions instance)
            {
                @TakePicture.started -= instance.OnTakePicture;
                @TakePicture.performed -= instance.OnTakePicture;
                @TakePicture.canceled -= instance.OnTakePicture;
                @ToggleCamera.started -= instance.OnToggleCamera;
                @ToggleCamera.performed -= instance.OnToggleCamera;
                @ToggleCamera.canceled -= instance.OnToggleCamera;
                @Zoom.started -= instance.OnZoom;
                @Zoom.performed -= instance.OnZoom;
                @Zoom.canceled -= instance.OnZoom;
                @ResetZoom.started -= instance.OnResetZoom;
                @ResetZoom.performed -= instance.OnResetZoom;
                @ResetZoom.canceled -= instance.OnResetZoom;
            }

            public void RemoveCallbacks(ICameraActions instance)
            {
                if (m_Wrapper.m_CameraActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(ICameraActions instance)
            {
                foreach (var item in m_Wrapper.m_CameraActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_CameraActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public CameraActions @Camera => new CameraActions(this);

        // Debugging
        private readonly InputActionMap m_Debugging;
        private List<IDebuggingActions> m_DebuggingActionsCallbackInterfaces = new List<IDebuggingActions>();
        private readonly InputAction m_Debugging_DefaultCamView;
        private readonly InputAction m_Debugging_OKHSLView;
        private readonly InputAction m_Debugging_DesaturatedView;
        private readonly InputAction m_Debugging_ResaturatedView;
        private readonly InputAction m_Debugging_ResetGlobalArray;
        public struct DebuggingActions
        {
            private @ResatInput m_Wrapper;
            public DebuggingActions(@ResatInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @DefaultCamView => m_Wrapper.m_Debugging_DefaultCamView;
            public InputAction @OKHSLView => m_Wrapper.m_Debugging_OKHSLView;
            public InputAction @DesaturatedView => m_Wrapper.m_Debugging_DesaturatedView;
            public InputAction @ResaturatedView => m_Wrapper.m_Debugging_ResaturatedView;
            public InputAction @ResetGlobalArray => m_Wrapper.m_Debugging_ResetGlobalArray;
            public InputActionMap Get() { return m_Wrapper.m_Debugging; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DebuggingActions set) { return set.Get(); }
            public void AddCallbacks(IDebuggingActions instance)
            {
                if (instance == null || m_Wrapper.m_DebuggingActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_DebuggingActionsCallbackInterfaces.Add(instance);
                @DefaultCamView.started += instance.OnDefaultCamView;
                @DefaultCamView.performed += instance.OnDefaultCamView;
                @DefaultCamView.canceled += instance.OnDefaultCamView;
                @OKHSLView.started += instance.OnOKHSLView;
                @OKHSLView.performed += instance.OnOKHSLView;
                @OKHSLView.canceled += instance.OnOKHSLView;
                @DesaturatedView.started += instance.OnDesaturatedView;
                @DesaturatedView.performed += instance.OnDesaturatedView;
                @DesaturatedView.canceled += instance.OnDesaturatedView;
                @ResaturatedView.started += instance.OnResaturatedView;
                @ResaturatedView.performed += instance.OnResaturatedView;
                @ResaturatedView.canceled += instance.OnResaturatedView;
                @ResetGlobalArray.started += instance.OnResetGlobalArray;
                @ResetGlobalArray.performed += instance.OnResetGlobalArray;
                @ResetGlobalArray.canceled += instance.OnResetGlobalArray;
            }

            private void UnregisterCallbacks(IDebuggingActions instance)
            {
                @DefaultCamView.started -= instance.OnDefaultCamView;
                @DefaultCamView.performed -= instance.OnDefaultCamView;
                @DefaultCamView.canceled -= instance.OnDefaultCamView;
                @OKHSLView.started -= instance.OnOKHSLView;
                @OKHSLView.performed -= instance.OnOKHSLView;
                @OKHSLView.canceled -= instance.OnOKHSLView;
                @DesaturatedView.started -= instance.OnDesaturatedView;
                @DesaturatedView.performed -= instance.OnDesaturatedView;
                @DesaturatedView.canceled -= instance.OnDesaturatedView;
                @ResaturatedView.started -= instance.OnResaturatedView;
                @ResaturatedView.performed -= instance.OnResaturatedView;
                @ResaturatedView.canceled -= instance.OnResaturatedView;
                @ResetGlobalArray.started -= instance.OnResetGlobalArray;
                @ResetGlobalArray.performed -= instance.OnResetGlobalArray;
                @ResetGlobalArray.canceled -= instance.OnResetGlobalArray;
            }

            public void RemoveCallbacks(IDebuggingActions instance)
            {
                if (m_Wrapper.m_DebuggingActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IDebuggingActions instance)
            {
                foreach (var item in m_Wrapper.m_DebuggingActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_DebuggingActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public DebuggingActions @Debugging => new DebuggingActions(this);

        // Dialogue
        private readonly InputActionMap m_Dialogue;
        private List<IDialogueActions> m_DialogueActionsCallbackInterfaces = new List<IDialogueActions>();
        private readonly InputAction m_Dialogue_Interact;
        private readonly InputAction m_Dialogue_Continue;
        public struct DialogueActions
        {
            private @ResatInput m_Wrapper;
            public DialogueActions(@ResatInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Interact => m_Wrapper.m_Dialogue_Interact;
            public InputAction @Continue => m_Wrapper.m_Dialogue_Continue;
            public InputActionMap Get() { return m_Wrapper.m_Dialogue; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DialogueActions set) { return set.Get(); }
            public void AddCallbacks(IDialogueActions instance)
            {
                if (instance == null || m_Wrapper.m_DialogueActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_DialogueActionsCallbackInterfaces.Add(instance);
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Continue.started += instance.OnContinue;
                @Continue.performed += instance.OnContinue;
                @Continue.canceled += instance.OnContinue;
            }

            private void UnregisterCallbacks(IDialogueActions instance)
            {
                @Interact.started -= instance.OnInteract;
                @Interact.performed -= instance.OnInteract;
                @Interact.canceled -= instance.OnInteract;
                @Continue.started -= instance.OnContinue;
                @Continue.performed -= instance.OnContinue;
                @Continue.canceled -= instance.OnContinue;
            }

            public void RemoveCallbacks(IDialogueActions instance)
            {
                if (m_Wrapper.m_DialogueActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IDialogueActions instance)
            {
                foreach (var item in m_Wrapper.m_DialogueActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_DialogueActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public DialogueActions @Dialogue => new DialogueActions(this);
        public interface IPlayerActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
        }
        public interface ICameraActions
        {
            void OnTakePicture(InputAction.CallbackContext context);
            void OnToggleCamera(InputAction.CallbackContext context);
            void OnZoom(InputAction.CallbackContext context);
            void OnResetZoom(InputAction.CallbackContext context);
        }
        public interface IDebuggingActions
        {
            void OnDefaultCamView(InputAction.CallbackContext context);
            void OnOKHSLView(InputAction.CallbackContext context);
            void OnDesaturatedView(InputAction.CallbackContext context);
            void OnResaturatedView(InputAction.CallbackContext context);
            void OnResetGlobalArray(InputAction.CallbackContext context);
        }
        public interface IDialogueActions
        {
            void OnInteract(InputAction.CallbackContext context);
            void OnContinue(InputAction.CallbackContext context);
        }
    }
}

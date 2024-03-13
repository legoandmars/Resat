using Input;
using Resat.Cameras;
using Resat.Colors;
using Resat.Input;
using Resat.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Debugging
{
    public class DebuggingController : MonoBehaviour, ResatInput.IDebuggingActions
    {
        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;
        
        [SerializeField]
        private PhotoController _photoController = null!;
        
        [SerializeField]
        private OKHSLController _okhslController = null!;

        [SerializeField]
        private GameObject? _debugSpawnThing;

        [SerializeField]
        private PlayerController? _playerController;
        public void OnDefaultCamView(InputAction.CallbackContext context)
        {
            if (!context.performed || _desaturationCamera.Material == null)
                return;
            
            // _desaturationCamera.Material.EnableKeyword("DISABLE_SHOW_OKHSL_VIEW");
            // throw new System.NotImplementedException();
        }

        public void OnOKHSLView(InputAction.CallbackContext context)
        {
            if (!context.performed || _desaturationCamera.Material == null)
                return;
            
            // _desaturationCamera.Material.DisableKeyword("DISABLE_SHOW_OKHSL_VIEW");
            // throw new System.NotImplementedException();
        }

        public void OnDesaturatedView(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;

            return;
        }

        public void OnResaturatedView(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;

            // throw new System.NotImplementedException();
        }

        public void OnResetGlobalArray(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            // throw new System.NotImplementedException();
            // _okhslController.ClearGlobalArray();
        }

        public void OnDebugSpawn(InputAction.CallbackContext context)
        {
            if (!context.performed || _debugSpawnThing == null || _playerController == null)
                return;

            var newThing = Instantiate(_debugSpawnThing);
            newThing.SetActive(true);
            newThing.transform.position = _playerController.transform.GetChild(0).position + new Vector3(0, 2.5f, 0);
            // debug spawn
        }

        private void Start()
        {
// #if UNITY_EDITOR
            _inputController.EnableDebugInput();
// #endif
        }
        
        private void OnEnable()
        {
            _inputController.Input.Debugging.AddCallbacks(this);
        }

        private void OnDisable()
        {
            _inputController.Input.Debugging.RemoveCallbacks(this);
        }
    }
}
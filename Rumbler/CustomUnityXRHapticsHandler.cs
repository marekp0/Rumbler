using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

namespace Rumbler
{
    /// <summary>
    /// Copy of KnucklesUnityXRHapticsHandler but:
    /// - TriggerHapticPulse takes a frequency parameter
    /// - OpenXRInput.SendHapticImpulse() is called directly instead of InputDevice.SendHapticImpulse()
    /// </summary>
    internal class CustomUnityXRHapticsHandler : IDisposable
    {
        private const float kRate = 0.0125f;

        private readonly MonoBehaviour _coroutineRunner;

        private readonly Coroutine _hapticsCoroutine;

        private readonly XRNode _node;
        private UnityEngine.InputSystem.XR.XRController _device;
        private InputAction _hapticAction;

        private float _remainingTime = 0f;

        private float _amplitude = 0f;

        private float _frequency = 0f;

        

        public CustomUnityXRHapticsHandler(XRNode node, MonoBehaviour coroutineRunner)
        {
            _node = node;
            _coroutineRunner = coroutineRunner;
            _hapticsCoroutine = coroutineRunner.StartCoroutine(HapticsCoroutine());
        }

        private bool InitDevice()
        {
            // convert XRNode to CommonUsage
            var usage = _node switch
            {
                XRNode.LeftHand => UnityEngine.InputSystem.CommonUsages.LeftHand,
                XRNode.RightHand => UnityEngine.InputSystem.CommonUsages.RightHand,
                _ => throw new NotImplementedException()
            };

            // get controller device
            _device = InputSystem.GetDevice<UnityEngine.InputSystem.XR.XRController>(usage);
            if (_device == null)
            {
                Plugin.Log?.Warn($"Couldn't find device for {_node}");
                return false;
            }

            // get the path of a haptic control, assuming the path contains the string "haptic"
            var hapticControl = _device.allControls.FirstOrDefault(c => c.path.Contains("haptic"));

            if (hapticControl != null)
            {
                _hapticAction = new InputAction(name: "HapticFeedbackOverride", type: InputActionType.PassThrough);
                _hapticAction.AddBinding(hapticControl.path);
                _hapticAction.Enable();
                return true;
            }
            else
            {
                // print all paths from the device to assist with debugging
                Plugin.Log?.Debug($"Device {_device.name} has no recognized haptic control. Paths:");
                foreach (var control in _device.allControls)
                {
                    Plugin.Log?.Debug(control.path);
                }

                return false;
            }
        }

        private bool DeviceIsValid()
        {
            return _device != null;
        }

        public void TriggerHapticPulse(float strength, float duration, float frequency)
        {
            _remainingTime = duration * 2f;
            _amplitude = Mathf.Clamp01(strength);
            _frequency = frequency;
        }

        public void StopHaptics()
        {
            _remainingTime = Mathf.Min(_remainingTime, kRate);
        }

        private IEnumerator HapticsCoroutine()
        {
            WaitForSecondsRealtime waiter = new WaitForSecondsRealtime(kRate);
            while (true)
            {
                if ((DeviceIsValid() || InitDevice()) && _remainingTime > 0f)
                {
                    OpenXRInput.SendHapticImpulse(_hapticAction, _amplitude, _frequency, _remainingTime, _device);
                }

                _remainingTime -= kRate;
                yield return waiter;
            }
        }

        public void Dispose()
        {
            if ((bool)_coroutineRunner)
            {
                _coroutineRunner.StopCoroutine(_hapticsCoroutine);
            }
        }
    }
}

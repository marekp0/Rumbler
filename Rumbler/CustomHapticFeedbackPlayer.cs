using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Input;

namespace Rumbler
{
    public struct RumbleInfo
    {
        public float rumbleDuration;        // duration of the rumble in seconds
        public float pulseStrength;         // strength of the pulse, between 0 and 1
        public float pulseFrequency;        // frequency of an individual pulse in Hz
        public float pulseDuration;         // duration of an individual pulse in seconds
        public float pulseTrainPeriod;      // time between start of successive pulses in seconds
    }


    /// <summary>
    /// 
    /// </summary>
    internal class CustomHapticFeedbackPlayer : IDisposable
    {
        private MonoBehaviour coroutineRunner;
        private readonly Coroutine hapticsCoroutine;

        /// <summary>
        /// Parameters for a currently active rumble
        /// </summary>
        private class ActiveRumble
        {
            public int remainingPulses;
            public float nextPulse;

            public float pulseStrength;
            public float pulseFrequency;       // frequency of an individual pulse in Hz
            public float pulseDuration;        // duration of an individual pulse in seconds
            public float pulseTrainPeriod;     // time between start of successive pulses in seconds
        }

        /// <summary>
        /// Params for a single controller (left/right)
        /// </summary>
        private class ControllerInfo
        {
            public LinkedList<ActiveRumble> activeRumbles = new LinkedList<ActiveRumble>();
            public UnityEngine.InputSystem.XR.XRController device;
            public InputAction hapticAction;

            // TODO: check that the device didn't become invalid during runtime
            public bool DeviceValid => device != null;

            // try not to spam too many warnings if we can't find the device
            public bool suppressWarnings = false;
        }

        // map of XRNode (left hand or right hand) ControllerInfo
        private readonly Dictionary<XRNode, ControllerInfo> controllers = new Dictionary<XRNode, ControllerInfo>();

        public CustomHapticFeedbackPlayer(MonoBehaviour coroutineRunner)
        {
            controllers[XRNode.LeftHand] = new ControllerInfo();
            controllers[XRNode.RightHand] = new ControllerInfo();

            this.coroutineRunner = coroutineRunner;
            hapticsCoroutine = coroutineRunner.StartCoroutine(HapticsCoroutine());
        }

        public void PlayHapticFeedback(XRNode node, RumbleInfo rumbleInfo)
        {
            if (node != XRNode.LeftHand && node != XRNode.RightHand)
            {
                throw new ArgumentException($"Invalid XRNode {node}");
            }
            var controller = controllers[node];

            // immediately play the first pulse to avoid potential delays
            if (controller.DeviceValid || InitDevice(node))
            {
                OpenXRInput.SendHapticImpulse(controller.hapticAction, rumbleInfo.pulseStrength, rumbleInfo.pulseFrequency, rumbleInfo.pulseDuration, controller.device);
            }

            var totalPulses = (int)(rumbleInfo.rumbleDuration / rumbleInfo.pulseTrainPeriod);
            if (totalPulses <= 1)
            {
                // nothing left to do
                return;
            }

            // add a new rumble to the list
            controller.activeRumbles.AddLast(new ActiveRumble
            {
                remainingPulses = totalPulses - 1,
                nextPulse = Time.time + rumbleInfo.pulseTrainPeriod,
                pulseStrength = rumbleInfo.pulseStrength,
                pulseFrequency = rumbleInfo.pulseFrequency,
                pulseDuration = rumbleInfo.pulseDuration,
                pulseTrainPeriod = rumbleInfo.pulseTrainPeriod
            });
        }

        public void StopHaptics(XRNode node)
        {
            if (node != XRNode.LeftHand && node != XRNode.RightHand)
            {
                throw new ArgumentException($"Invalid XRNode {node}");
            }

            // remove all rumbles for the given node
            var controller = controllers[node];
            controller.activeRumbles.Clear();

            // stop any ongoing haptics
            OpenXRInput.StopHaptics(controller.hapticAction, controller.device);
        }

        private bool InitDevice(XRNode node)
        {
            // convert XRNode to CommonUsage
            var usage = node switch
            {
                XRNode.LeftHand => UnityEngine.InputSystem.CommonUsages.LeftHand,
                XRNode.RightHand => UnityEngine.InputSystem.CommonUsages.RightHand,
                _ => throw new NotImplementedException()
            };

            var controllerInfo = controllers[node];

            // get controller device
            controllerInfo.device = InputSystem.GetDevice<UnityEngine.InputSystem.XR.XRController>(usage);
            if (controllerInfo.device == null)
            {
                if (!controllerInfo.suppressWarnings)
                {
                    Plugin.Log?.Error($"Couldn't find device for {node}");
                    controllerInfo.suppressWarnings = true;
                }
                return false;
            }

            // get the path of a haptic control, assuming the path contains the string "haptic"
            // TODO: test this on something other than the Index controllers
            var hapticControl = controllerInfo.device.allControls.FirstOrDefault(c => c.path.Contains("haptic"));

            if (hapticControl != null)
            {
                controllerInfo.hapticAction = new InputAction(name: "HapticFeedbackOverride", type: InputActionType.PassThrough);
                controllerInfo.hapticAction.AddBinding(hapticControl.path);
                controllerInfo.hapticAction.Enable();
                controllerInfo.suppressWarnings = false;
                return true;
            }
            else
            {
                if (!controllerInfo.suppressWarnings)
                {
                    // print all paths from the device to assist with debugging
                    Plugin.Log?.Error($"Device {controllerInfo.device.name} has no recognized haptic control. Paths:");
                    foreach (var control in controllerInfo.device.allControls)
                    {
                        Plugin.Log?.Error(control.path);
                    }
                    controllerInfo.suppressWarnings = true;
                }

                return false;
            }
        }

        /// <summary>
        /// Background coroutine that handles sending of individual haptic pulses
        /// </summary>
        /// <returns></returns>
        private IEnumerator HapticsCoroutine()
        {
            var waiter = new WaitForSecondsRealtime(0f);
            while (true)
            {
                // wait at most half a frame for any potential new pulses
                float waitTime = Time.deltaTime/2f;

                foreach (var (node, controller) in controllers)
                {
                    // parameters for the next haptic pulse
                    bool doPulse = false;
                    float pulseStrength = 0f;
                    float pulseFrequency = 0f;
                    float pulseDuration = 0f;

                    var rumbleNode = controller.activeRumbles.First;
                    while (rumbleNode != null)
                    {
                        var next = rumbleNode.Next;
                        var rumble = rumbleNode.Value;

                        if (rumble.nextPulse <= Time.time)
                        {
                            // if multiple pulses, choose the strongest of each parameter
                            doPulse = true;
                            pulseStrength = Math.Max(pulseStrength, rumble.pulseStrength);
                            pulseFrequency = Math.Max(pulseFrequency, rumble.pulseFrequency);
                            pulseDuration = Math.Max(pulseDuration, rumble.pulseDuration);

                            if (--rumble.remainingPulses <= 0)
                            {
                                controller.activeRumbles.Remove(rumbleNode);
                            }
                            else
                            {
                                rumble.nextPulse += rumble.pulseTrainPeriod;
                                waitTime = Math.Min(waitTime, rumble.pulseTrainPeriod);
                            }
                        }
                        else
                        {
                            waitTime = Math.Min(waitTime, rumble.nextPulse - Time.time);
                        }

                        rumbleNode = next;
                    }

                    if (doPulse && (controller.DeviceValid || InitDevice(node)))
                    {
                        OpenXRInput.SendHapticImpulse(controller.hapticAction, pulseStrength, pulseFrequency, pulseDuration, controller.device);
                    }
                }

                waiter.waitTime = waitTime;
                yield return waiter;
            }
        }

        public void Dispose()
        {
            if ((bool)coroutineRunner)
            {
                coroutineRunner.StopCoroutine(hapticsCoroutine);
            }
        }
    }
}

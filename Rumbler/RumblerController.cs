using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

namespace Rumbler
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class RumblerController : MonoBehaviour
    {
        public static RumblerController Instance { get; private set; }

        internal CustomUnityXRHapticsHandler LeftHapticsHandler { get; private set; }
        internal CustomUnityXRHapticsHandler RightHapticsHandler { get; private set; }

        internal CustomUnityXRHapticsHandler GetHapticsHandler(XRNode node)
        {
            return node switch
            {
                XRNode.LeftHand => LeftHapticsHandler,
                XRNode.RightHand => RightHapticsHandler,
                _ => null
            };
        }

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;

            LeftHapticsHandler = new CustomUnityXRHapticsHandler(UnityEngine.XR.XRNode.LeftHand, this);
            RightHapticsHandler = new CustomUnityXRHapticsHandler(UnityEngine.XR.XRNode.RightHand, this);

            Plugin.Log?.Debug($"{name}: Awake()");
            Plugin.Log?.Debug($"LH={LeftHapticsHandler}, RH={RightHapticsHandler}");
        }
        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate()
        {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable()
        {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
    }
}

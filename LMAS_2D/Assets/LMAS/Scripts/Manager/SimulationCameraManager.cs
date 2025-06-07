using UnityEngine;
using System.Collections.Generic;

namespace LMAS.Scripts.Manager
{
    public class SimulationCameraManager : MonoSingleton<SimulationCameraManager>
    {
        private List<SimulationCamera> simulationCameras = new List<SimulationCamera>();
        private int currentCameraIndex = 0;
        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            // Find all SimulationCamera components in the scene
            simulationCameras.AddRange(FindObjectsByType<SimulationCamera>(FindObjectsSortMode.None));

            if (simulationCameras.Count == 0)
            {
                Debug.LogWarning("No SimulationCamera found in the scene.");
            }
            else
            {
                // Activate the first camera by default
                UpdateActiveCamera();
            }
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Switch to the previous camera
                currentCameraIndex = (currentCameraIndex - 1 + simulationCameras.Count) % simulationCameras.Count;
                UpdateActiveCamera();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Switch to the next camera
                currentCameraIndex = (currentCameraIndex + 1) % simulationCameras.Count;
                UpdateActiveCamera();
            }
        }

        private void UpdateActiveCamera()
        {
            int cameraCount = simulationCameras.Count;
            if (cameraCount == 0) return; // No cameras to switch
            for (int i = 0; i < cameraCount; i++)
            {
                if (i == currentCameraIndex)
                {
                    simulationCameras[i].Cam.enabled = true;
                    simulationCameras[i].gameObject.SetActive(true);

                }
                else
                {
                    simulationCameras[i].Cam.enabled = false;
                    simulationCameras[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
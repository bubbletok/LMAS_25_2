using System;

using UnityEngine;
using UnityEngine.Tilemaps;

using LMAS.Scripts.Manager;
using LMAS.Scripts.PathFinder;
using LMAS.Scripts.Types;
using LMAS.Scripts.TypedTiles;
using LMAS.Scripts.Utility;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace LMAS.Scripts.Agent
{
    public class LLMAgentController : MonoBehaviour
    {
        public LLMAgent Agent;

        LLMAgentMemory m_Memory;
        LLMAgentPlanner m_Planner;
        LLMAgentBehavior m_Behavior;
        LLMAgentVAD m_Vad;

        LLMAgentInteractor m_Interactor;

        #region Unity Methods
        void Awake()
        {
            m_Memory = GetComponent<LLMAgentMemory>();
            if (m_Memory == null)
            {
                Debug.LogError("No AgentMemory found on this GameObject.");
            }
            m_Planner = GetComponent<LLMAgentPlanner>();
            if (m_Planner == null)
            {
                Debug.LogError("No AgentPlanner found on this GameObject.");
            }
            m_Behavior = GetComponent<LLMAgentBehavior>();
            if (m_Behavior == null)
            {
                Debug.LogError("No AgentBehavior found on this GameObject.");
            }
            m_Vad = GetComponent<LLMAgentVAD>();
            if (m_Vad == null)
            {
                Debug.LogError("No AgentVAD found on this GameObject.");
            }
            m_Interactor = GetComponent<LLMAgentInteractor>();
            if (m_Interactor == null)
            {
                Debug.LogError("No AgentInteractor found on this GameObject.");
            }
        }

        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     // AnalyzeEmotion();
            //     string examplePrompt = "You are deep in the middle of the thick forest, " +
            //         "with old trees all around you. " +
            //         "All at once, a terrifying scream cuts through the quiet, " +
            //         "bouncing off the trees in the darkness.";
            //     GenerateEmotion(examplePrompt);
            // }
            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     Observe(GetObservation(), SimulationManager.Instance.SimulationTime);
            // }
            // if (Input.GetKeyDown(KeyCode.G))
            // {
            //     GetSummary();
            // }

            // if (Input.GetKeyDown(KeyCode.M))
            // {
            //     ObserveAndSummarize(GetObservation(), SimulationManager.Instance.SimulationTime);
            // }

            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     Plan(SimulationManager.Instance.SimulationTime);
            // }

            // if (Input.GetKeyDown(KeyCode.B))
            // {
            //     Act(SimulationManager.Instance.SimulationTime);
            // }
        }

        void FixedUpdate()
        {
            // dir = Vector3Int.zero;

            // if (Input.GetKeyDown(KeyCode.W)) dir = Vector3Int.up;
            // else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3Int.down;
            // else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3Int.left;
            // else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3Int.right;
            // if (dir != Vector3Int.zero)
            // {
            //     Vector3 newPos = TilemapManager.Instance.GetWorldPosOnTilemap(transform.position + dir);
            //     if (newPos != Vector3.negativeInfinity)
            //     {
            //         Debug.Log($"Attempting to move to new position: {newPos}");
            //         List<TypedTile> tiles = TilemapManager.Instance.GetTiles(TilemapManager.Instance.GetPosOnTilemap(newPos));
            //         bool canMove = true;
            //         foreach (var tile in tiles)
            //         {
            //             if (tile.Type == LMASType.Wall)
            //             {
            //                 canMove = false;
            //                 Debug.LogWarning($"Cannot move to {newPos}, obstacle detected: {tile.name}");
            //                 break;
            //             }
            //         }
            //         if (canMove)
            //         {
            //             transform.position = newPos;
            //         }
            //         else
            //         {
            //             Debug.LogWarning($"Cannot move to {newPos}, it is blocked by a wall or obstacle.");
            //         }
            //     }
            //     else
            //     {
            //         Debug.LogWarning("Cannot move to the target position, it is not a valid tile position.");
            //     }
            // }
        }

        public void OnGUI()
        {
            // GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            // // GUILayout.Label("Press R to Generate Emotion");
            // // GUILayout.Label("Press M to Observe Surroundings");
            // GUILayout.Label("Press O to Observe Surroundings");
            // GUILayout.Label("Press G to Get Summary");
            // GUILayout.Label("Press M to Observe and Summarize");
            // GUILayout.Label("Press P to Plan");
            // GUILayout.Label("Press B to Act");
            // GUILayout.EndArea();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }
        #endregion

        // Test Functions
        void AnalyzeEmotion(string prompt)
        {
            // Calculate the time it takes to generate the emotion
            Debug.Log("Generating Emotion... ");
            Debug.Log("Prompt: " + prompt);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            m_Vad.AnalyzeEmotion(Agent.AgentInfo, prompt, (result) =>
            {
                stopwatch.Stop();
                Debug.Log("Emotion Analysis Result: " + result);
                Debug.Log("Time taken for emotion analysis: " + stopwatch.ElapsedMilliseconds + " ms");
            });
        }

        void GenerateEmotion(string prompt)
        {
            // Calculate the time it takes to generate the emotion
            Debug.Log("Generating Emotion... ");
            Debug.Log("Prompt: " + prompt);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            m_Vad.GenerateEmotion(Agent.AgentInfo, prompt, (result) =>
            {
                stopwatch.Stop();
                Debug.Log("Emotion Analysis Result: " + result);
                Debug.Log("Time taken for emotion analysis: " + stopwatch.ElapsedMilliseconds + " ms");
            });
        }

        public string GetObservation()
        {
            Vector3 worldPos = Agent.AgentWorldPos;
            var colliders = TilemapManager.Instance.GetSurrondingColliders(worldPos);
            var tiles = TilemapManager.Instance.GetSurroundingTiles(worldPos);
            var prompt = "\n";
            foreach (var collider in colliders)
            {
                if (collider.GetComponent<TilemapCollider2D>()) continue;
                if (collider.gameObject == this.gameObject)
                {
                    Vector2 tilemapWorldPos = TilemapManager.Instance.GetWorldPosOnTilemap(collider.gameObject.transform.position);
                    prompt += "Me: " + this.name + " at " + tilemapWorldPos + "\n";
                    continue;
                }

                if (collider.TryGetComponent(out LMASObject lmasObject))
                {
                    Vector2 tilemapWorldPos = TilemapManager.Instance.GetWorldPosOnTilemap(collider.gameObject.transform.position);
                    LMASType type = lmasObject.Type;
                    switch (type)
                    {
                        case LMASType.Agent:
                            prompt += "Agent: " + lmasObject.name + " at " + tilemapWorldPos + "\n";
                            break;
                        case LMASType.Ground:
                            prompt += "Ground: ";
                            break;
                        default:
                            prompt += "Unknown: ";
                            break;
                    }
                }
            }
            string surroundingTiles = "Surrounding: ";
            foreach (var tile in tiles)
            {
                // Vector2 tileWorldPos = TilemapManager.Instance.GetTileWorldPos(tile);
                TypedTile typedTile = tile.Item1;
                Vector3 tilePos = tile.Item2;
                if (typedTile != null)
                {
                    surroundingTiles += typedTile.Type + " " + typedTile.TileName + " at " + tilePos + ", ";
                }
            }
            prompt += surroundingTiles;
            return prompt;
        }

        public void Observe(string prompt, float time, Action<string> callback = null)
        {
            Debug.Log("Observing... ");
            Debug.Log("Prompt: " + prompt);

            m_Memory.Observe(Agent.AgentInfo, prompt, time, (result) =>
            {
                Debug.Log($"{Agent.AgentName} Observation Result: {result}");
                callback?.Invoke(result);
            });
        }

        public void GetSummary()
        {
            Debug.Log("Getting Summary... ");

            m_Memory.GetSummary(Agent.AgentInfo, (result) =>
            {
                Debug.Log("Summary Result: " + result);
            });
        }

        public void ObserveAndSummarize(string prompt, float time)
        {
            Debug.Log("Observing... ");
            Debug.Log("Prompt: " + prompt);

            m_Memory.Observe(Agent.AgentInfo, prompt, time, (result) =>
            {
                Debug.Log("Observation Result: " + result);
                Debug.Log("Getting Summary... ");

                m_Memory.GetSummary(Agent.AgentInfo, (result) =>
                {
                    Debug.Log("Summary Result: " + result);
                });
            });
        }

        public void Plan(float time)
        {
            Debug.Log("Planning... ");

            m_Planner.Plan(Agent.AgentInfo, time, (result) =>
            {
                Debug.Log($"{Agent.AgentName} Plan Result: {result}");
            });
        }

        public void Act(float time, Action callback = null)
        {
            Debug.Log("Behaving... ");
            string action = string.Empty;
            m_Behavior.Act(Agent.AgentInfo, time, (result) =>
            {
                Debug.Log($"{Agent.AgentName} Behavior Result: {result}");
                action = result;

                (BehaviorType behaviorType, string value) = JsonParser.ParseBehavior(action);
                Debug.Log("Parsed Behavior Type: " + behaviorType);
                Debug.Log("Parsed Behavior Value: " + value);
                TilePathFollow tilePathFollow = GetComponent<TilePathFollow>();
                switch (behaviorType)
                {
                    case BehaviorType.None:
                        Debug.LogWarning("No valid behavior type found.");
                        callback?.Invoke();
                        return;
                    case BehaviorType.Move:

                        if (tilePathFollow != null)
                        {
                            tilePathFollow.ParseAndMove(value, callback);
                        }
                        else
                        {
                            Debug.LogWarning("TilePathFollow component not found. Cannot move.");
                            callback?.Invoke();
                        }
                        break;
                    case BehaviorType.Pickup:
                        System.Action pickupAction = () =>
                        {
                            Vector2 tilePos = JsonParser.ParsePosition(value);
                            if (tilePos == Vector2.negativeInfinity)
                            {
                                Debug.LogWarning("Invalid tile position for pickup: " + value);
                                callback?.Invoke();
                                return;
                            }
                            Vector3Int tilePosInt = TilemapManager.Instance.GetPosOnTilemap(tilePos);
                            PickableTile pickableTile = TilemapManager.Instance.GetTile(tilePosInt) as PickableTile;
                            m_Interactor.PickTile(Agent, pickableTile, callback);
                        };
                        // First, move to the target tile
                        // Second, pick up the tile

                        if (tilePathFollow != null)
                        {
                            tilePathFollow.ParseAndMove(value, pickupAction);
                        }
                        else
                        {
                            Debug.LogWarning("TilePathFollow component not found. Cannot move.");
                            callback?.Invoke();
                        }
                        break;
                    case BehaviorType.Drop:
                        Debug.Log("Dropping: " + value);
                        callback?.Invoke();
                        break;
                    case BehaviorType.Talk:
                        // Debug.Log("Talking: " + value);
                        (string agentName, string message) = JsonParser.ParseTalk(value);
                        if (string.IsNullOrEmpty(agentName) || string.IsNullOrEmpty(message))
                        {
                            Debug.LogWarning("Invalid talk command format: " + value);
                            callback?.Invoke();
                            return;
                        }
                        StartCoroutine(m_Behavior.GetReadyToTalk(agentName, message, tilePathFollow, (string value) =>
                        {
                            Debug.Log($"Talking to {agentName}: {message}");
                            callback?.Invoke();
                        }));
                        break;
                    case BehaviorType.Interact:
                        System.Action interacttionAction = () =>
                        {
                            Vector2 tilePos = JsonParser.ParsePosition(value);
                            if (tilePos == Vector2.negativeInfinity)
                            {
                                Debug.LogWarning("Invalid tile position for interaction: " + value);
                                callback?.Invoke();
                                return;
                            }
                            Vector3Int tilePosInt = TilemapManager.Instance.GetPosOnTilemap(tilePos);
                            InteractableTile interactableTile = TilemapManager.Instance.GetTile(tilePosInt) as InteractableTile;
                            m_Interactor.InteractTile(Agent, interactableTile, callback);
                        };
                        // First, move to the target tile
                        // Second, interact with the tile
                        if (tilePathFollow != null)
                        {
                            tilePathFollow.ParseAndMove(value, interacttionAction);
                        }
                        else
                        {
                            Debug.LogWarning("TilePathFollow component not found. Cannot move.");
                            callback?.Invoke();
                        }

                        break;
                    default:
                        callback?.Invoke();
                        break;
                }
            });
        }
    }
}
using System;
using LMAS.Scripts;
using LMAS.Scripts.Agent;
using LMAS.Scripts.Manager;
using LMAS.Scripts.PathFinder;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class LLMAgentController : MonoBehaviour
{
    public LLMAgent Agent;
    Vector3Int dir;

    LLMAgentMemory m_Memory;
    LLMAgentPlanner m_Planner;
    LLMAgentBehavior m_Behavior;
    LLMAgentVAD m_Vad;

    void Start()
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
        // if (Input.GetKeyDown(KeyCode.M))
        // {

        //     Debug.Log($"Observed: {prompt}");

        //     Observe(prompt, SimulationManager.Instance.SimulationTime);
        // }
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     GetSummary();
        // }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ObserveAndSummarize(GetObservation(), SimulationManager.Instance.SimulationTime);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Plan(SimulationManager.Instance.SimulationTime);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Act(SimulationManager.Instance.SimulationTime);
        }
    }

    void FixedUpdate()
    {
        dir = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3Int.right;
        if (dir != Vector3Int.zero)
        {
            transform.position += dir;
        }
    }

    public void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        // GUILayout.Label("Press R to Generate Emotion");
        // GUILayout.Label("Press M to Observe Surroundings");
        // GUILayout.Label("Press G to Get Summary");
        GUILayout.Label("Press G to Observe and Summarize");
        GUILayout.Label("Press P to Plan");
        GUILayout.Label("Press B to Act");
        GUILayout.EndArea();
    }

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

    string GetObservation()
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
        string surroundingTiles = "Surrounding Tiles: ";
        foreach (var tile in tiles)
        {
            // Vector2 tileWorldPos = TilemapManager.Instance.GetTileWorldPos(tile);
            TypedTile typedTile = tile.Item1;
            Vector3 tilePos = tile.Item2;
            if (typedTile != null)
            {
                surroundingTiles += typedTile.Type + " " + typedTile.name + " at " + tilePos + ", ";
            }
        }
        prompt += surroundingTiles;
        return prompt;
    }

    void Observe(string prompt, float time)
    {
        Debug.Log("Observing... ");
        Debug.Log("Prompt: " + prompt);

        m_Memory.Observe(Agent.AgentInfo, prompt, time, (result) =>
        {
            Debug.Log("Observation Result: " + result);
        });
    }

    void GetSummary()
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

    void Plan(float time)
    {
        Debug.Log("Planning... ");

        m_Planner.Plan(Agent.AgentInfo, time, (result) =>
        {
            Debug.Log("Plan Result: " + result);
        });
    }

    void Act(float time)
    {
        Debug.Log("Behaving... ");
        string action = string.Empty;
        m_Behavior.Act(Agent.AgentInfo, time, (result) =>
        {
            Debug.Log("Behavior Result: " + result);
            action = result;

            BehaviorType behaviorType = m_Behavior.ParseBehaviorType(action);
            Debug.Log("Parsed Behavior Type: " + behaviorType);

            if (behaviorType == BehaviorType.Move)
            {
                TilePathFollow tilePathFollow = GetComponent<TilePathFollow>();
                if (tilePathFollow != null)
                {
                    tilePathFollow.ParseAndMove(action);
                }
                else
                {
                    Debug.LogWarning("TilePathFollow component not found. Cannot move.");
                }
            }
        });


    }
}
using LMAS.Scripts;
using LMAS.Scripts.Agent;
using LMAS.Scripts.Manager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class LLMAgentController : MonoBehaviour
{
    public LLMAgent Agent;
    Vector3Int dir;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // AnalyzeEmotion();
            string examplePrompt = "You are deep in the middle of the thick forest, " +
                "with old trees all around you. " +
                "All at once, a terrifying scream cuts through the quiet, " +
                "bouncing off the trees in the darkness.";
            GenerateEmotion(examplePrompt);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector3 worldPos = Agent.AgentWorldPos;
            Vector3Int pos = new Vector3Int((int)worldPos.x, (int)worldPos.y, (int)worldPos.z);
            var colliders = TilemapManager.Instance.GetSurrondingColliders(pos);
            var tiles = TilemapManager.Instance.GetSurroundingTiles(pos);
            var prompt = "";
            foreach (var collider in colliders)
            {
                if (collider.GetComponent<TilemapCollider2D>()) continue;

                if (collider.TryGetComponent(out LMASObject lmasObject))
                {
                    LMASType type = lmasObject.Type;
                    switch (type)
                    {
                        case LMASType.Agent:
                            prompt += "Agent: " + lmasObject.name + "\n";
                            break;
                        case LMASType.Ground:
                            prompt += "Ground: ";
                            break;
                        case LMASType.Wall:
                            prompt += "Wall: ";
                            break;
                        case LMASType.Water:
                            prompt += "Water: ";
                            break;
                        case LMASType.Door:
                            prompt += "Door: ";
                            break;
                        case LMASType.Window:
                            prompt += "Window: ";
                            break;
                        case LMASType.Item:
                            prompt += "Item: ";
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
                surroundingTiles += tile.name + "\n";
            }
            Debug.Log($"Observed: {prompt}");

            Observe(prompt, SimulationManager.Instance.SimulationTime);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetSummary();
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

    // Test Functions
    void AnalyzeEmotion(string prompt)
    {
        LLMAgent agent = GetComponent<LLMAgent>();
        if (agent == null)
        {
            Debug.LogError("No Agent found on this GameObject.");
            return;
        }

        LLMAgentVAD vad = GetComponent<LLMAgentVAD>();
        if (vad == null)
        {
            Debug.LogError("No AgentVAD found on this GameObject.");
            return;
        }
        /*string prompt = "You are deep in the middle of the thick forest, " +
                         "with old trees all around you. " +
                         "All at once, a terrifying scream cuts through the quiet, " +
                         "bouncing off the trees in the darkness.";*/

        // Calculate the time it takes to generate the emotion
        Debug.Log("Generating Emotion... ");
        Debug.Log("Prompt: " + prompt);

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        vad.AnalyzeEmotion(agent.AgentInfo, prompt, (result) =>
        {
            stopwatch.Stop();
            Debug.Log("Emotion Analysis Result: " + result);
            Debug.Log("Time taken for emotion analysis: " + stopwatch.ElapsedMilliseconds + " ms");
        });
    }

    void GenerateEmotion(string prompt)
    {
        LLMAgent agent = GetComponent<LLMAgent>();
        if (agent == null)
        {
            Debug.LogError("No Agent found on this GameObject.");
            return;
        }

        LLMAgentVAD vad = GetComponent<LLMAgentVAD>();
        if (vad == null)
        {
            Debug.LogError("No AgentVAD found on this GameObject.");
            return;
        }
        /*string prompt = "You are deep in the middle of the thick forest, " +
                         "with old trees all around you. " +
                         "All at once, a terrifying scream cuts through the quiet, " +
                         "bouncing off the trees in the darkness.";*/

        // Calculate the time it takes to generate the emotion
        Debug.Log("Generating Emotion... ");
        Debug.Log("Prompt: " + prompt);

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        vad.GenerateEmotion(agent.AgentInfo, prompt, (result) =>
        {
            stopwatch.Stop();
            Debug.Log("Emotion Analysis Result: " + result);
            Debug.Log("Time taken for emotion analysis: " + stopwatch.ElapsedMilliseconds + " ms");
        });
    }

    void Observe(string prompt, float time)
    {
        LLMAgent agent = GetComponent<LLMAgent>();
        if (agent == null)
        {
            Debug.LogError("No Agent found on this GameObject.");
            return;
        }

        LLMAgentMemory memory = GetComponent<LLMAgentMemory>();
        if (memory == null)
        {
            Debug.LogError("No AgentMemory found on this GameObject.");
            return;
        }
        // string prompt = "You are deep in the middle of the thick forest, " +
        //                  "with old trees all around you. " +
        //                  "All at once, a terrifying scream cuts through the quiet, " +
        //                  "bouncing off the trees in the darkness.";

        Debug.Log("Observing... ");
        Debug.Log("Prompt: " + prompt);

        memory.Observe(agent.AgentInfo, prompt, time, (result) =>
        {
            Debug.Log("Observation Result: " + result);
        });
    }

    void GetSummary()
    {
        LLMAgent agent = GetComponent<LLMAgent>();
        if (agent == null)
        {
            Debug.LogError("No Agent found on this GameObject.");
            return;
        }

        LLMAgentMemory memory = GetComponent<LLMAgentMemory>();
        if (memory == null)
        {
            Debug.LogError("No AgentMemory found on this GameObject.");
            return;
        }

        Debug.Log("Getting Summary... ");

        memory.GetSummary(agent.AgentInfo, (result) =>
        {
            Debug.Log("Summary Result: " + result);
        });
    }
}

using LMAS.Scripts.Agent;
using UnityEngine;

public class LLMAgentController : MonoBehaviour
{
    Vector3Int dir;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // AnalyzeEmotion();
            GenerateEmotion();
        }
    }

    // Test Functions
    void AnalyzeEmotion()
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
        string prompt = "You are deep in the middle of the thick forest, " +
                         "with old trees all around you. " +
                         "All at once, a terrifying scream cuts through the quiet, " +
                         "bouncing off the trees in the darkness.";

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

    void GenerateEmotion()
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
        string prompt = "You are deep in the middle of the thick forest, " +
                         "with old trees all around you. " +
                         "All at once, a terrifying scream cuts through the quiet, " +
                         "bouncing off the trees in the darkness.";

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
}

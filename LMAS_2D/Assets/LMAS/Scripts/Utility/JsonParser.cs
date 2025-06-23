using System;
using UnityEngine;

using LMAS.Scripts.Agent;
using System.Text.RegularExpressions;
using System.Globalization;

namespace LMAS.Scripts.Utility
{
    public static class JsonParser
    {
        public static (BehaviorType, string) ParseBehavior(string json)
        {
            string valueResult = string.Empty;
            if (string.IsNullOrWhiteSpace(json))
                return (BehaviorType.None, valueResult);

            // 1. Trim surrounding whitespace
            string trimmed = json.Trim();

            // 2. If it starts and ends with a quote, remove those outer quotes
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                trimmed = trimmed.Substring(1, trimmed.Length - 2);

            // 3. Replace literal escape sequences (e.g., "\n", "\t") with actual characters
            //    or remove them, so that JsonUtility can parse it correctly.
            //    Regex.Unescape will convert sequences like "\\n" to "\n", "\\\"" to "\"", etc.
            string unescaped = Regex.Unescape(trimmed);

            // 4. Finally, trim again to remove any accidental leading/trailing whitespace/newlines
            string cleaned = unescaped.Trim();

            BehaviorData data;
            try
            {
                data = JsonUtility.FromJson<BehaviorData>(cleaned);
            }
            catch (Exception)
            {
                Debug.LogWarning($"Failed to parse JSON: {cleaned}");
                return (BehaviorType.None, valueResult);
            }

            // 5. Extract the "value" field (or default to empty string)
            valueResult = data.value ?? string.Empty;

            // 6. Attempt to parse the "type" field into our BehaviorType enum
            if (Enum.TryParse(data.type, true, out BehaviorType behaviorType))
            {
                return (behaviorType, valueResult);
            }
            else
            {
                Debug.LogWarning($"Unknown behavior type: {data.type}");
                return (BehaviorType.None, valueResult);
            }
        }

        public static Vector2 ParsePosition(string value)
        {
            // 정규식:
            //  1) 'x' 이후 ':' 또는 '=' 중 하나, 그 뒤 공백 0개 이상, 부호 선택(-?), 숫자+소수부 선택
            //  2) x 뒤와 y 앞 사이에 (콤마+공백) 또는 (공백 1개 이상) 허용
            //  3) 'y' 이후 ':' 또는 '=' 중 하나, 그 뒤 공백 0개 이상, 부호 선택, 숫자+소수부 선택
            Regex regex = new Regex(@"
            x\s*[:=]\s*             # 'x' 뒤에 ':' 또는 '=' , 그 뒤 공백 0개 이상
            (-?\d+(?:\.\d+)?)       # 그룹1: 정수 또는 소수 (예: -0.5, 10, 3.1415 등)
            \s*                     # 숫자 뒤 공백 0개 이상
            (?:,\s*|\s+)            # (콤마 뒤 공백0개 이상) 또는 (공백1개 이상)
            y\s*[:=]\s*             # 'y' 뒤에 ':' 또는 '=' , 그 뒤 공백 0개 이상
            (-?\d+(?:\.\d+)?)       # 그룹2: 정수 또는 소수
        ", RegexOptions.IgnorePatternWhitespace);

            Match match = regex.Match(value);
            if (!match.Success)
            {
                Debug.LogWarning("Invalid move command format: " + value);
                return Vector2.negativeInfinity;
            }

            // CultureInfo.InvariantCulture 사용: 소수점을 '.'로 파싱
            float deltaX = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            float deltaY = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

            return new Vector2(deltaX, deltaY);
        }

        // "value": "agent_name: Hello!"
        public static (string, string) ParseTalk(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (string.Empty, string.Empty);

            // 정규식: "agent_name: message"
            Regex regex = new Regex(@"^(?<name>[^:]+):\s*(?<message>.+)$");
            Match match = regex.Match(value);

            if (!match.Success)
            {
                Debug.LogWarning("Invalid talk command format: " + value);
                return (string.Empty, string.Empty);
            }

            string agentName = match.Groups["name"].Value.Trim();
            string message = match.Groups["message"].Value.Trim();

            return (agentName, message);
        }
    }
}
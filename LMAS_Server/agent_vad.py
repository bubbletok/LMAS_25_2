from langchain_core.documents import Document
# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field
from typing import Optional, List, Dict, Tuple
from agent_chat import AgentChat

class AgentVAD(BaseModel):
    """Agent pad for analyzing emotions."""
    valence: Dict[str, float] = Field(default_factory=dict, description="Valence level of the agent")
    arousal: Dict[str, float] = Field(default_factory=dict,description="Arousal level of the agent")
    dominance: Dict[str, float] = Field(default_factory=dict,description="Dominance level of the agent")
    emotion: Dict[str, str] = Field(default_factory=dict,description="Emotion of the agent")
    mood: Dict[str, str] = Field(default_factory=dict,description="Mood of the agent")
    chat: AgentChat = Field(description="Chat model for the agent")
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}
    
    def analyze_emotion(self, name: str, prompt: str) -> Tuple[float, float, float]:
        """Analyze the emotion of the prompt."""
        # Placeholder for emotion analysis logic
        prompt = "Calculate the valence, arousal, and dominance of the prompt. \
            The prompt is: " + prompt + "Response with as following format: \
            valence: <valence>, arousal: <arousal>, dominance: <dominance>"
        result = "" # self.llm.invoke(prompt)
        # Parse the result to extract valence, arousal, and dominance
        try:
            valence, arousal, dominance = map(float, result.split(','))
            self.valence[name] = valence
            self.arousal[name] = arousal
            self.dominance[name] = dominance
        except ValueError:
            raise ValueError("Invalid response format from LLM")
        return valence, arousal, dominance
       
    def _generate_emotion(self, name) -> str:
        """Generate emotion based on valence, arousal, and dominance."""
        valence, arousal, dominance = self.valence[name], self.arousal[name], self.dominance[name]
        (self.valence, self.arousal, self.dominance) = self.analyze_emotion(valence, arousal, dominance)
        # Generate emotion based on valence, arousal, and dominance
        prompt = f"Generate the emotion based on the following parameters: \
            valence: {self.valence}, arousal: {self.arousal}, dominance: {self.dominance}" \
                + "Response with as following format: emotion: <emotion>"
        # Parse the result to extract emotion
        try:
            result = "" #self.llm.invoke(prompt)
            emotion = result.split(':')[1].strip()
            self.emotion[name].append(emotion)
        except ValueError:
            raise ValueError("Invalid response format from LLM")
        return self.emotion[name]
    
    def _generate_mood(self, name: str) -> str:
        """Generate mood based on the recent emotions."""
        # Based on the recent emotions, generate the mood
        prompt = f"Generate the mood based on the following emotions:"
        for emotion in self.emotion[name]:
            prompt += f" {emotion},"
        prompt += "Response with as following format: mood: <mood>"
        # Parse the result to extract mood
        try:
            result = "" # self.llm.invoke(prompt)
            mood = result.split(':')[1].strip()
            self.mood = mood
        except ValueError:
            raise ValueError("Invalid response format from LLM")
        return self.mood    
    
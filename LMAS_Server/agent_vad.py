from langchain_core.documents import Document
# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field
from typing import Optional, List, Dict, Tuple
from agent_chat import AgentChat
import json

class AgentVAD(BaseModel):
    """Agent pad for analyzing emotions."""
    valence: Dict[str, float] = Field(default_factory=dict, description="Valence level of the agent")
    arousal: Dict[str, float] = Field(default_factory=dict,description="Arousal level of the agent")
    dominance: Dict[str, float] = Field(default_factory=dict,description="Dominance level of the agent")
    emotion: Dict[str, List] = Field(default_factory=dict,description="Emotion of the agent")
    mood: Dict[str, str] = Field(default_factory=dict,description="Mood of the agent")
    chat: AgentChat = Field(description="Chat model for the agent")
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}
    
    def _analyze_emotion(self, name: str, prompt: str) -> Tuple[float, float, float]:
        """Analyze the emotion of the prompt."""
        text = prompt  # your original text to analyze
        prompt = f'''
        You are an emotion‐analysis model based on the Valence‐Arousal‐Dominance framework.
        Internally think step by step, but DO NOT output any of that reasoning.
        
        When done, output **exactly one line**:
        That is, your entire response must be like:
        {{"valence":-0.5,"arousal":0.8,"dominance":-0.6}}
        
        Each value has a range from -1.0 to 1.0.
        Each value has a meaning as follows:
        Valence: The pleasantness of a stimulus
        Arousal: The intensity of emotion provoked by a stimulus
        Dominance: the degree of control exerted by a stimulus

        **No** backticks, **no** quotes around the JSON, **no** extra spaces or newlines, **no** markdown.
        
        Text to analyze:
        \"\"\"{text}\"\"\"
        '''
        # The definition of valence, arousal, dominance is from
        # https://pubmed.ncbi.nlm.nih.gov/23404613/#:~:text=Three%20components%20of%20emotions%20are,control%20exerted%20by%20a%20stimulus).
        # print(prompt)
        response = self.chat.llm.invoke(prompt)
        result = response.content
        # print(result)
        # return result
        json_result = json.loads(result)
        # return json_result
        # Parse the result to extract valence, arousal, and dominance
        try:
            valence, arousal, dominance = json_result["valence"], json_result["arousal"], json_result["dominance"]                
            self.valence[name] = valence
            self.arousal[name] = arousal
            self.dominance[name] = dominance
        except ValueError:
            raise ValueError("Invalid response format from LLM")
        return (valence, arousal, dominance)
       
    def _generate_emotion(self, name: str, prompt: str) -> str:
        """Generate emotion based on valence, arousal, and dominance."""
        # First, Analyze the valence, arousal, and dominance from the prompt
        (valence, arousal, dominance) = self._analyze_emotion(name, prompt)
        
        # Second, Generate emotion based on valence, arousal, and dominance
        prompt = f'''You are an emotion classification model that maps Valence-Arousal-Dominance values to emotion words.
        Think step by step:
        1. Assess valence for positivity or negativity.
        2. Assess arousal for level of activation.
        3. Assess dominance for control or helplessness.
        
        Each type of value has a meaning as follows:
        Valence: The pleasantness of a stimulus
        Arousal: The intensity of emotion provoked by a stimulus
        Dominance: the degree of control exerted by a stimulus
        
        Based on these, determine the single-word emotion that best fits.

        Output only that one word with no additional text.
        
        Valence: {valence}
        Arousal: {arousal}
        Dominance: {dominance}
        '''
        # Parse the result to extract emotion
        response = self.chat.llm.invoke(prompt)
        result = response.content
        try:
            emotion = result
            if self.emotion.get(name) is None:
                self.emotion[name] = []

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
    
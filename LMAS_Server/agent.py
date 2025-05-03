# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field

from agent_chat import AgentChat
from agent_memory import AgentMemory
from agent_behavior import AgentBehavior
from agent_planner import AgentPlanner
from agent_vad import AgentVAD

class Agent(BaseModel):
    """Agent class"""
    name: str = Field(default="", description="Name of the agent")
    age: int = Field(default=0, description="Age of the agent")
    chat: AgentChat = Field(description="Chat model for the agent")
    memory: AgentMemory = Field(description="Memory model for the agent")
    behavior: AgentBehavior = Field(description="Behavior model for the agent")
    planner: AgentPlanner = Field(description="Planner model for the agent")
    vad: AgentVAD = Field(description="VAD model for the agent")
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
        
        json_encoders = {
            AgentChat:    lambda v: v.dict(),
            AgentMemory:  lambda v: v.dict(),
            AgentBehavior: lambda v: v.dict() if hasattr(v, "dict") else str(v),
            AgentPlanner: lambda v: v.dict(),
            AgentVAD:     lambda v: v.dict(),
        }

    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}

    # def __str__(self):
    #     return f"name={self.name}, age={self.age}, chat={self.chat}, memory={self.memory}, behavior={self.behavior}, planner={self.planner}, vad={self.vad})"
    
    """
    _observe: Observe the environment and update memory.
    """
    def observe(self, observation: str) -> str:
        """Observe the environment and update memory."""
        # self.memory.update(observation)
        return 
    
    def act(self, summary: str) -> str:
        """Perform the action and update memory."""
        # self.behavior.act(summary)
        return
    
    def plan(self, summary: str, goal: str) -> str:
        """Plan the next steps to achieve the goal."""
        # self.planner.plan(summary, goal)
        return
    
    def analyze_emotion(self, prompt: str) -> str:
        """Analyze the emotion of the prompt."""
        result = self.vad._analyze_emotion(self.name, prompt)
        return result
    
    def generate_emotion(self, prompt: str) -> str:
        """Get the emotion of the prompt."""
        # (v, a, d) = self.vad.analyze_emotion(self.name, prompt)
        # return f"Valence: {v}, Arousal: {a}, Dominance: {d}"
        result = self.vad._generate_emotion(self.name, prompt)
        return result
    
    # def _test_chat(self, prompt: str) -> str:
    #     """Test the chat model."""
    #     # result = self.vad.chat.llm.invoke(prompt).content
    #     # return result
    #     # (v, a, d) = self.vad.analyze_emotion(self.name, prompt)
    #     # return f"Valence: {v}, Arousal: {a}, Dominance: {d}"
    #     result = self.vad.analyze_emotion(self.name, prompt)
    #     return result
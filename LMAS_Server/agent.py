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
    def observe(self, observation: str, time: float) -> str:
        """Observe the environment and update memory."""
        emotion = self.generate_emotion(observation)
        return self.memory.update_memory(observation, emotion, time)
    
    def act(self, current_time: float) -> str:
        """Perform the action and update memory."""
        plan = self.planner.plan
        all_memory = self.memory.get_all_memory()
        return self.behavior.act(plan, all_memory, current_time)
    
    def plan(self, current_time: float) -> str:
        """Plan the next steps to achieve the goal."""
        all_summary = self.memory.get_all_summary()
        print(f"Planning with all summary: {all_summary} at time: {current_time}")
        return self.planner.make_plan(all_summary, current_time)
    
    def analyze_emotion(self, prompt: str) -> str:
        """Analyze the emotion of the prompt."""
        result = self.vad.analyze_emotion(self.name, prompt)
        return result
    
    def generate_emotion(self, prompt: str) -> str:
        """Get the emotion of the prompt."""
        # (v, a, d) = self.vad.analyze_emotion(self.name, prompt)
        # return f"Valence: {v}, Arousal: {a}, Dominance: {d}"
        result = self.vad.generate_emotion(self.name, prompt)
        return result
    
    def get_summary(self) -> str:
        """Get the summary of the agent's memory."""
        return self.memory.get_summary()

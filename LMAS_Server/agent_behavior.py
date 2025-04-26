from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Tuple

class AgentBehavior(BaseModel):
    """Agent Behavior class for managing agent's behavior."""
    # name: str = Field(description="Name of the agent")
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}
    
    # def act(self, summary: str) -> str:
    #     """Perform the action and update memory."""
    #     # self.agent.memory.update(summary)
    #     # self.agent.behavior.act(summary)
    #     return f"Acting on the summary: {summary}"
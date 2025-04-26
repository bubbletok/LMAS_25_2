# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field
from typing import Optional, List, Dict, Tuple, Any

class AgentPlanner(BaseModel):
    """Agent Planner class for managing agent's planning tasks."""
    # name: str = Field(description="Name of the agent")
    plan: Dict[str, Any] = Field(default_factory=dict, description="Plan of the agent")
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}

    def _make_plan(self, goal: str) -> str:
        """Make a plan to achieve the goal."""
        return f"Planning to achieve the goal: {goal}"
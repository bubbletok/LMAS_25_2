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

    def _make_plan(self, summary: str, current_time: float) -> str:
        prompt = f"""Your task is to create a step-by-step plan to achieve the following goal, based on the provided summary. 
        Think step by step (CoT: Chain-of-Thought):
        1. Analyze the summary and extract the main objective.
        2. Identify any constraints, resources, or important context from the summary.
        3. Break down the objective into actionable steps, ordered logically.
        4. Present the plan as a numbered list, with each step being clear and concise.

        Summary: {summary}
        Current Time: {current_time}

        Plan:"""
        response = self.chat.llm.invoke(prompt)
        result = response.content
        
        
        return result
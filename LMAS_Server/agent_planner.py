# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field
from typing import Optional, List, Dict, Tuple, Any
from agent_chat import AgentChat

class AgentPlanner(BaseModel):
    """Agent Planner class for managing agent's planning tasks."""
    # name: str = Field(description="Name of the agent")
    plan: str = Field(default="", description="Plan of the agent")
    # Dict[str, Any] = Field(default_factory=dict, description="Plan of the agent")
    chat: AgentChat = Field(description="Chat model for the agent")
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}

    def make_plan(self, experiences: str, current_time: float) -> str:
        prompt = f"""Your task is to create a goal and step-by-step plan based on the provided experience summary and current time.
        
        You are an agent in the world, and you need to create a goal that is relevant to the current time and experiences.
        The experiences are a summary of your past actions and observations, and you should use them to create a plan.
        
        You must make a plan and goal only based on the provided experiences and current time.
        Do not use any external information or reasoning, and not add any other information except for the provided experiences.
        
        Do not overthink, just create a simple and straightforward plan based on the provided experiences.        
        
        If the experiences are not enough to make a goal or empty, you must respond with "Random Action".
        
        Do not output any markdown, backticks, or quotes.

        Experience: {experiences}
        Current Time: {current_time}
        """
        response = self.chat.llm.invoke(prompt)
        result = response.content
        
        self.plan = result
        
        return self.plan
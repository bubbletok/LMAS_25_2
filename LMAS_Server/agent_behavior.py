from langchain_core.pydantic_v1 import BaseModel, Field
from typing import Optional, List, Dict, Tuple
from agent_chat import AgentChat

class AgentBehavior(BaseModel):
    """Agent Behavior class for managing agent's behavior."""
    # name: str = Field(description="Name of the agent")
    recent_action: str = Field(default="", description="Recent action of the agent's actions")
    chat: AgentChat = Field(description="Chat model for the agent")
    
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

    def __str__(self):
        return self.dict()
    
    def act(self, plan: str, all_memory: List[str], current_time: float) -> str:
        """Perform the action and update memory."""
        prompt = f"""You are an agent in a virtual world.
        Your task is to perform an action based on the provided plan and current time.
        You must not output any reasoning or explanation, just the action.
        
        As with behavior types below, you should respond with a single action that the agent should take.
        
        Behavior Types:
        1. None: No specific behavior, just a placeholder. e.g. "None"
        2. Move: Move to a specific location. e.g. "Move x: 10, y: 20"
        When moving, you can only move to a location that is obtained from the agent's memory.
        3. Pickup: Pick up an item at current location. e.g. "Pickup"
        4. Drop: Drop an item that the agent is currently holding at the current location. e.g. "Drop"
        5. Talk: Talk to another agent. e.g. "Talk to agent_name. Message: Hello!" e.g. "Talk"
        6. Interact: Interact with an object. e.g. "Interact with object_name. Action: Open" e.g. "Interact"
        
        if the plan is exactly "Random Action", then you should choose a Move that moves to the random location.
        e.g "Move x: 10, y: 20"
        
        Do not output any markdown, backticks, or quotes.
        Plan: {plan}
        All Memory: {all_memory}
        Current Time: {current_time}
        """
        response = self.chat.llm.invoke(prompt)
        result = response.content
        self.recent_action = result
        return self.recent_action
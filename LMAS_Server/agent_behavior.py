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
        """Perform the action and update memory, returning a JSON-formatted behavior."""
        prompt = """You are an agent in a virtual world.
        Your task is to perform an action based on the provided plan and current time.
        You must respond with a JSON object (no markdown, no backticks, no extra text) matching exactly this format:
        {
        "type": 
        "value": 
        }
        - "type" must be one of: "None", "Move", "Pickup", "Drop", "Talk", "Interact".
        - "value" should be a string describing the action’s parameters:
        
        If there is Item in the world, you can perform actions related to it.
        if there is Interactable object in the world, you can perform actions related to it.
        
        For Move: "x=<number>, y=<number>"
        - coordinates must come from memory, unless plan is "Random Action", then pick a random known location
        - coordinates must not exceed the world bounds.
        
        For Pickup: "x=<number>, y=<number>"
        - coordinates must come from memory.
        - coordinates must not exceed the world bounds.
        
        For Talk: "agent_name: <message>"
        - agent_name must be a known agent in the world.
        - message must be relevant to the agent's context.
        
        For Interact: "x=<number>, y=<number>>"
        - coordinates must come from memory.
        - coordinates must not exceed the world bounds.
        
        Behavior Types:
        None: No action. Example:
        {
        "type": "None",
        "value": ""
        }
        
        Move: Move to a specific location stored in memory. Example:
        {
        "type": "Move",
        "value": "x=10, y=20"
        }
        
        Pickup: Pick up an item at current location. Example:
        {
        "type": "Pickup",
        "value": "x=10, y=20"
        }
        
        Drop: Drop an item currently held. Example:
        {
        "type": "Drop",
        "value": "item_name"
        }
        
        Talk: Talk to another agent. Example:
        {
        "type": "Talk",
        "value": "agent_name: Hello!"
        }
        
        Interact: Interact with an object. Example:
        {
        "type": "Interact",
        "value": "x=15, y=20"
        }

        If the plan is exactly "Random Action", choose a random location from memory and output a Move action accordingly. Example:
        {
        "type": "Move",
        "value": "x=15, y=20"
        }
        
        Fields must be exactly as specified, with no additional text or formatting. Only "type" and "value" fields are allowed.
        

        Plan: """ + plan + """
        All Memory: """ + ",".join(all_memory) + """
        Current Time: """ + str(current_time)

        response = self.chat.llm.invoke(prompt)
        result = response.content.strip()
        if result.startswith("```") and result.endswith("```"):
            result = "\n".join(result.splitlines()[1:-1]).strip()
        self.recent_action = result
        return self.recent_action
    
    def talk(self, message: str) -> str:
        """Make the agent talk to another agent."""
        prompt = f"You are an agent in a virtual world. You need to talk to another agent with the following message: {message}"
        response = self.chat.llm.invoke(prompt)
        result = response.content.strip()
        self.recent_action = result
        return self.recent_action
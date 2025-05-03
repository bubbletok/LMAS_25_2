# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field
from langchain_core.prompts import PromptTemplate
from langchain_core.documents import Document
from typing import Optional, List, Dict, Tuple

class AgentMemory(BaseModel):
    """Agent memory for storing and retrieving information."""
    # name: str = Field(description="Name of the agent")
    working_memory: List[Document] = Field(
        default_factory=list, description="Working memory of the agent"
    )
    
    mid_term_memory: List[Document] = Field(
        default_factory=list, description="Mid-term memory of the agent"
    )
    
    long_term_memory: List[Document] = Field(
        default_factory=list, description="Long-term memory of the agent"
    )
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}
    
    def __str__(self):
        return self.dict()
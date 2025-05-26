from langchain_community.chat_models import ChatOllama
from langchain_core.prompts import PromptTemplate
from langchain_core.output_parsers import StrOutputParser
from langchain_core.runnables import Runnable
# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field
from typing import Optional, List, Dict, Tuple, Any

class AgentChat(BaseModel):
    llm: ChatOllama = Field(description="LLM model for the agent")
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}
    
    def _chain(self, prompt: PromptTemplate) -> Runnable:
        return prompt | self.llm.invoke(prompt) | StrOutputParser()
        
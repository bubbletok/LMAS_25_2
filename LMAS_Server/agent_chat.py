from langchain_community.chat_models import ChatOllama
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser
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
    
    # def __init__(self, model_name: str):
    #     self.model_name = model_name
    #     self.llm = ChatOllama(model=model_name)
        # self.chat_prompt = ChatPromptTemplate.from_messages([
        #     ("system", "You are a helpful assistant."),
        #     ("user", "{input}"),
        #     ("assistant", "{output}")
        # ])
        # self.output_parser = StrOutputParser()
    
    # def chat(self, prompt: str) -> str:
    #     """Chat with the agent using the provided prompt."""
    #     # response = self.llm.invoke(prompt)
    #     # return response
    #     pass
        